import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { MaintenancePlansServiceProxy, MaintenancePlanDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditMaintenancePlanModalComponent } from './create-or-edit-maintenancePlan-modal.component';

import { ViewMaintenancePlanModalComponent } from './view-maintenancePlan-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';


@Component({
    templateUrl: './maintenancePlans.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class MaintenancePlansComponent extends AppComponentBase {
    
    
    @ViewChild('createOrEditMaintenancePlanModal', { static: true }) createOrEditMaintenancePlanModal: CreateOrEditMaintenancePlanModalComponent;
    @ViewChild('viewMaintenancePlanModalComponent', { static: true }) viewMaintenancePlanModal: ViewMaintenancePlanModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    subjectFilter = '';
    descriptionFilter = '';
    remarksFilter = '';
    workOrderPriorityPriorityFilter = '';
    workOrderTypeTypeFilter = '';

    constructor(
        injector: Injector,
        private _router: Router,
        private _maintenancePlansServiceProxy: MaintenancePlansServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getMaintenancePlans(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._maintenancePlansServiceProxy.getAll(
            this.filterText,
            this.subjectFilter,
            this.descriptionFilter,
            this.remarksFilter,
            this.workOrderPriorityPriorityFilter,
            this.workOrderTypeTypeFilter,
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

    createMaintenancePlan(): void {
        this.createOrEditMaintenancePlanModal.show();        
    }


    deleteMaintenancePlan(maintenancePlan: MaintenancePlanDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._maintenancePlansServiceProxy.delete(maintenancePlan.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    viewMaintenancePlan(maintenancePlanId): void {
        this._router.navigate(['app/admin/support/maintenancePlans/viewMaintenancePlan'], { queryParams: { maintenancePlanId: maintenancePlanId } });
    }
    
    
}
