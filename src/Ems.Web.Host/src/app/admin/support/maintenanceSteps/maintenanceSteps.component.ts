import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { MaintenanceStepsServiceProxy, MaintenanceStepDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditMaintenanceStepModalComponent } from './create-or-edit-maintenanceStep-modal.component';

import { ViewMaintenanceStepModalComponent } from './view-maintenanceStep-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './maintenanceSteps.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class MaintenanceStepsComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditMaintenanceStepModal', { static: true }) createOrEditMaintenanceStepModal: CreateOrEditMaintenanceStepModalComponent;
    @ViewChild('viewMaintenanceStepModalComponent', { static: true }) viewMaintenanceStepModal: ViewMaintenanceStepModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    commentsFilter = '';
    maxQuantityFilter : number;
    maxQuantityFilterEmpty : number;
    minQuantityFilter : number;
    minQuantityFilterEmpty : number;
    maxCostFilter : number;
    maxCostFilterEmpty : number;
    minCostFilter : number;
    minCostFilterEmpty : number;
    maxPriceFilter : number;
    maxPriceFilterEmpty : number;
    minPriceFilter : number;
    minPriceFilterEmpty : number;
    maintenancePlanSubjectFilter = '';
    itemTypeTypeFilter = '';
    workOrderActionActionFilter = '';

    constructor(
        injector: Injector,
        private _maintenanceStepsServiceProxy: MaintenanceStepsServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getMaintenanceSteps(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._maintenanceStepsServiceProxy.getAll(
            0,
            this.filterText,
            this.commentsFilter,
            this.maxQuantityFilter == null ? this.maxQuantityFilterEmpty: this.maxQuantityFilter,
            this.minQuantityFilter == null ? this.minQuantityFilterEmpty: this.minQuantityFilter,
            this.maxCostFilter == null ? this.maxCostFilterEmpty: this.maxCostFilter,
            this.minCostFilter == null ? this.minCostFilterEmpty: this.minCostFilter,
            this.maxPriceFilter == null ? this.maxPriceFilterEmpty: this.maxPriceFilter,
            this.minPriceFilter == null ? this.minPriceFilterEmpty: this.minPriceFilter,
            this.maintenancePlanSubjectFilter,
            this.itemTypeTypeFilter,
            this.workOrderActionActionFilter,
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

    createMaintenanceStep(): void {
        this.createOrEditMaintenanceStepModal.show();        
    }


    deleteMaintenanceStep(maintenanceStep: MaintenanceStepDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._maintenanceStepsServiceProxy.delete(maintenanceStep.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._maintenanceStepsServiceProxy.getMaintenanceStepsToExcel(
        this.filterText,
            this.commentsFilter,
            this.maxQuantityFilter == null ? this.maxQuantityFilterEmpty: this.maxQuantityFilter,
            this.minQuantityFilter == null ? this.minQuantityFilterEmpty: this.minQuantityFilter,
            this.maxCostFilter == null ? this.maxCostFilterEmpty: this.maxCostFilter,
            this.minCostFilter == null ? this.minCostFilterEmpty: this.minCostFilter,
            this.maxPriceFilter == null ? this.maxPriceFilterEmpty: this.maxPriceFilter,
            this.minPriceFilter == null ? this.minPriceFilterEmpty: this.minPriceFilter,
            this.maintenancePlanSubjectFilter,
            this.itemTypeTypeFilter,
            this.workOrderActionActionFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
}
