import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { WorkOrderUpdatesServiceProxy, WorkOrderUpdateDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditWorkOrderUpdateModalComponent } from './create-or-edit-workOrderUpdate-modal.component';

import { ViewWorkOrderUpdateModalComponent } from './view-workOrderUpdate-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './workOrderUpdates.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class WorkOrderUpdatesComponent extends AppComponentBase {
    
    
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('createOrEditWorkOrderUpdateModal', { static: true }) createOrEditWorkOrderUpdateModal: CreateOrEditWorkOrderUpdateModalComponent;
    @ViewChild('viewWorkOrderUpdateModalComponent', { static: true }) viewWorkOrderUpdateModal: ViewWorkOrderUpdateModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    workOrderId: number;
    filterText = '';
    commentsFilter = '';
    maxNumberFilter : number;
    maxNumberFilterEmpty : number;
    minNumberFilter : number;
    minNumberFilterEmpty : number;
    workOrderSubjectFilter = '';
    itemTypeTypeFilter = '';
    workOrderActionActionFilter = '';
    assetPartNameFilter = '';


    _entityTypeFullName = 'Ems.Support.WorkOrderUpdate';
    entityHistoryEnabled = false;



    constructor(
        injector: Injector,
        private _workOrderUpdatesServiceProxy: WorkOrderUpdatesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return this.isGrantedAny('Pages.Administration.AuditLogs') && customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getWorkOrderUpdates(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._workOrderUpdatesServiceProxy.getAll(
            this.workOrderId,
            this.filterText,
            //this.commentsFilter,
            //this.maxNumberFilter == null ? this.maxNumberFilterEmpty: this.maxNumberFilter,
            //this.minNumberFilter == null ? this.minNumberFilterEmpty: this.minNumberFilter,
            this.workOrderSubjectFilter,
            this.itemTypeTypeFilter,
            this.workOrderActionActionFilter,
            this.assetPartNameFilter,
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

    createWorkOrderUpdate(): void {
        this.createOrEditWorkOrderUpdateModal.show();        
    }


    showHistory(workOrderUpdate: WorkOrderUpdateDto): void {
        this.entityTypeHistoryModal.show({
            entityId: workOrderUpdate.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteWorkOrderUpdate(workOrderUpdate: WorkOrderUpdateDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._workOrderUpdatesServiceProxy.delete(workOrderUpdate.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._workOrderUpdatesServiceProxy.getWorkOrderUpdatesToExcel(
        this.filterText,
            this.commentsFilter,
            this.maxNumberFilter == null ? this.maxNumberFilterEmpty: this.maxNumberFilter,
            this.minNumberFilter == null ? this.minNumberFilterEmpty: this.minNumberFilter,
            this.workOrderSubjectFilter,
            this.itemTypeTypeFilter,
            this.workOrderActionActionFilter,
            this.assetPartNameFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
    
    
    
}
