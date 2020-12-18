import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import * as _ from 'lodash';
import { LazyLoadEvent } from 'primeng/api';
import { CreateOrEditMaintenanceStepModalComponent } from '../maintenanceSteps/create-or-edit-maintenanceStep-modal.component';
import { ViewMaintenanceStepModalComponent } from '../maintenanceSteps/view-maintenanceStep-modal.component';
import { MaintenancePlansServiceProxy, MaintenancePlanDto, MaintenanceStepsServiceProxy, MaintenanceStepDto } from '@shared/service-proxies/service-proxies';
import { CreateOrEditMaintenancePlanModalComponent } from './create-or-edit-maintenancePlan-modal.component';
import * as moment from 'moment';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'viewMaintenancePlan',
    templateUrl: './viewMaintenancePlan.component.html',
    animations: [appModuleAnimation()]
})
export class ViewMaintenancePlanComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditMaintenancePlanModal', { static: true }) createOrEditMaintenancePlanModal: CreateOrEditMaintenancePlanModalComponent;
    @ViewChild('createOrEditMaintenanceStepModal', { static: true }) createOrEditMaintenanceStepModal: CreateOrEditMaintenanceStepModalComponent;
    @ViewChild('viewMaintenanceStepModalComponent', { static: true }) viewMaintenanceStepModal: ViewMaintenanceStepModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table

    primengTableHelper1: PrimengTableHelper;

    // advancedFiltersAreShown = false;
    // filterText = '';
    // maxUpdatedAtFilter: moment.Moment;
    // minUpdatedAtFilter: moment.Moment;
    // updateFilter = '';
    // maxUpdatedByUserIdFilter: number;
    // maxUpdatedByUserIdFilterEmpty: number;
    // minUpdatedByUserIdFilter: number;
    // minUpdatedByUserIdFilterEmpty: number;

    maintenancePlanId : number;
    maintenancePlan: MaintenancePlanDto;
    workOrderPriority = '';
    workOrderType = '';

    subjectFilter = '';
    descriptionFilter = '';
    remarksFilter = '';
    workOrderPriorityPriorityFilter = '';
    workOrderTypeTypeFilter = '';

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
    
    _entityTypeFullName = 'Ems.Support.MaintenancePlan';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _maintenancePlansServiceProxy: MaintenancePlansServiceProxy,
        private _maintenanceStepsServiceProxy: MaintenanceStepsServiceProxy
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
    }

    active = false;
    saving = false;

    ngOnInit(): void {

        this.maintenancePlanId = this._activatedRoute.snapshot.queryParams['maintenancePlanId'];
        this.maintenancePlan = new MaintenancePlanDto();

        this.getMaintenancePlan();

        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }
    
    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }
    
    getMaintenancePlan(event?: LazyLoadEvent){
        this._maintenancePlansServiceProxy.getMaintenancePlanForView(this.maintenancePlanId)
            .subscribe((maintenancePlanResult) => { 

                if (maintenancePlanResult.maintenancePlan == null)
                    {this.close();
                }
                else{
                    this.maintenancePlan = maintenancePlanResult.maintenancePlan;
                    this.maintenancePlanId = maintenancePlanResult.maintenancePlan.id;
                    this.workOrderPriority = maintenancePlanResult.workOrderPriorityPriority;
                    this.workOrderType = maintenancePlanResult.workOrderTypeType;
                    
                    this.getMaintenanceSteps();
                }
            });
    }

    getMaintenanceSteps(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._maintenanceStepsServiceProxy.getAll(
            this.maintenancePlanId,
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
        this.paginator1.changePage(this.paginator1.getPage())
    }

    createMaintenanceStep(): void {
        this.createOrEditMaintenanceStepModal.show(null, this.maintenancePlanId);
    }

    deleteMaintenanceStep(maintenanceStep: MaintenanceStepDto): void {
        this.message.confirm(
            '',
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

    close(): void {
        this._router.navigate(['app/admin/support/maintenancePlans']);
    }
}
