import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { AssetPartsServiceProxy, AssetPartDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAssetPartModalComponent } from './create-or-edit-assetPart-modal.component';

import { ViewAssetPartModalComponent } from './view-assetPart-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './assetParts.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class AssetPartsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditAssetPartModal', { static: true }) createOrEditAssetPartModal: CreateOrEditAssetPartModalComponent;
    @ViewChild('viewAssetPartModalComponent', { static: true }) viewAssetPartModal: ViewAssetPartModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '';
    descriptionFilter = '';
    serialNumberFilter = '';
    maxInstallDateFilter : moment.Moment;
    minInstallDateFilter : moment.Moment;
    installedFilter = -1;
    assetPartTypeTypeFilter = '';
    assetPartNameFilter = '';
    assetPartStatusStatusFilter = '';
    assetReferenceFilter = '';
    itemTypeTypeFilter = '';
    warehouseNameFilter = '';

    constructor(
        injector: Injector,
        private _assetPartsServiceProxy: AssetPartsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getAssetParts(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._assetPartsServiceProxy.getAll(
            null,
            null,
            this.filterText,
            this.nameFilter,
            this.descriptionFilter,
            this.serialNumberFilter,
            this.maxInstallDateFilter,
            this.minInstallDateFilter,
            this.installedFilter,
            this.assetPartTypeTypeFilter,
            this.assetPartNameFilter,
            this.assetPartStatusStatusFilter,
            this.assetReferenceFilter,
            this.itemTypeTypeFilter,
            this.warehouseNameFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createAssetPart(isItem: boolean, parentId?: number): void {
        this.createOrEditAssetPartModal.show({
            isItem: isItem, 
            parentId: parentId
        });
    }

    deleteAssetPart(assetPart: AssetPartDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._assetPartsServiceProxy.delete(assetPart.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._assetPartsServiceProxy.getAssetPartsToExcel(
        this.filterText,
            this.nameFilter,
            this.descriptionFilter,
            this.serialNumberFilter,
            this.maxInstallDateFilter,
            this.minInstallDateFilter,
            this.installedFilter,
            this.assetPartTypeTypeFilter,
            this.assetPartNameFilter,
            this.assetPartStatusStatusFilter,
            this.assetReferenceFilter,
            this.itemTypeTypeFilter,
            this.warehouseNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
}
