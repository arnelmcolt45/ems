import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { AzureStorageConfigurationsServiceProxy, AzureStorageConfigurationDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditAzureStorageConfigurationModalComponent } from './create-or-edit-azureStorageConfiguration-modal.component';

import { ViewAzureStorageConfigurationModalComponent } from './view-azureStorageConfiguration-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './azureStorageConfigurations.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class AzureStorageConfigurationsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditAzureStorageConfigurationModal', { static: true }) createOrEditAzureStorageConfigurationModal: CreateOrEditAzureStorageConfigurationModalComponent;
    @ViewChild('viewAzureStorageConfigurationModalComponent', { static: true }) viewAzureStorageConfigurationModal: ViewAzureStorageConfigurationModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    serviceFilter = '';
    accountNameFilter = '';
    keyValueFilter = '';
    blobStorageEndpointFilter = '';
    containerNameFilter = '';






    constructor(
        injector: Injector,
        private _azureStorageConfigurationsServiceProxy: AzureStorageConfigurationsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getAzureStorageConfigurations(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._azureStorageConfigurationsServiceProxy.getAll(
            this.filterText,
            this.serviceFilter,
            this.accountNameFilter,
            this.keyValueFilter,
            this.blobStorageEndpointFilter,
            this.containerNameFilter,
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

    createAzureStorageConfiguration(): void {
        this.createOrEditAzureStorageConfigurationModal.show();        
    }


    deleteAzureStorageConfiguration(azureStorageConfiguration: AzureStorageConfigurationDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._azureStorageConfigurationsServiceProxy.delete(azureStorageConfiguration.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._azureStorageConfigurationsServiceProxy.getAzureStorageConfigurationsToExcel(
        this.filterText,
            this.serviceFilter,
            this.accountNameFilter,
            this.keyValueFilter,
            this.blobStorageEndpointFilter,
            this.containerNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
}
