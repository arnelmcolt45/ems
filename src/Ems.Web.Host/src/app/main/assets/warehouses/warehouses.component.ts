import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { WarehousesServiceProxy, WarehouseDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditWarehouseModalComponent } from './create-or-edit-warehouse-modal.component';

import { MoveToAssetLookupTableModalComponent } from '../assetParts/move-to-asset-lookup-table-modal.component';
import { MoveToWarehouseLookupTableModalComponent } from '../assetParts/move-to-warehouse-lookup-table-modal.component'; 

import { ViewWarehouseModalComponent } from './view-warehouse-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './warehouses.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class WarehousesComponent extends AppComponentBase {
    
    @ViewChild('createOrEditWarehouseModal', { static: true }) createOrEditWarehouseModal: CreateOrEditWarehouseModalComponent;
    @ViewChild('viewWarehouseModalComponent', { static: true }) viewWarehouseModal: ViewWarehouseModalComponent;    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @ViewChild('moveToAssetLookupTableModalComponent', {static: true}) moveToAssetLookupTableModal: MoveToAssetLookupTableModalComponent;
    @ViewChild('moveToWarehouseLookupTableModalComponent', {static: true}) moveToWarehouseLookupTableModal: MoveToWarehouseLookupTableModalComponent;

    advancedFiltersAreShown = false;
    filterText = '';
    nameFilter = '';
    addressLine1Filter = '';
    addressLine2Filter = '';
    postalCodeFilter = '';
    cityFilter = '';
    stateFilter = '';
    countryFilter = '';

    constructor(
        injector: Injector,
        private _router: Router,
        private _warehousesServiceProxy: WarehousesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getWarehouses(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._warehousesServiceProxy.getAll(
            this.filterText,
            this.nameFilter,
            this.addressLine1Filter,
            this.addressLine2Filter,
            this.postalCodeFilter,
            this.cityFilter,
            this.stateFilter,
            this.countryFilter,
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

    createWarehouse(): void {
        this.createOrEditWarehouseModal.show();        
    }


    deleteWarehouse(warehouse: WarehouseDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._warehousesServiceProxy.delete(warehouse.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    viewWarehouse(warehouseId): void {
        this._router.navigate(['app/main/assets/warehouses/viewWarehouse'], { queryParams: { warehouseId: warehouseId } });
    }

    exportToExcel(): void {
        this._warehousesServiceProxy.getWarehousesToExcel(
        this.filterText,
            this.nameFilter,
            this.addressLine1Filter,
            this.addressLine2Filter,
            this.postalCodeFilter,
            this.cityFilter,
            this.stateFilter,
            this.countryFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }


}
