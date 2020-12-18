import { Component, ViewChild, Injector, Output, EventEmitter, OnInit} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { MaintenancePlansServiceProxy, CreateOrEditMaintenancePlanDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { MaintenancePlanWorkOrderPriorityLookupTableModalComponent } from './maintenancePlan-workOrderPriority-lookup-table-modal.component';
import { MaintenancePlanWorkOrderTypeLookupTableModalComponent } from './maintenancePlan-workOrderType-lookup-table-modal.component';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {Observable} from "@node_modules/rxjs";



@Component({
    templateUrl: './create-or-edit-maintenancePlan.component.html',
    animations: [appModuleAnimation()]
})
export class CreateOrEditMaintenancePlanComponent extends AppComponentBase implements OnInit {
    active = false;
    saving = false;
        @ViewChild('maintenancePlanWorkOrderPriorityLookupTableModal', { static: true }) maintenancePlanWorkOrderPriorityLookupTableModal: MaintenancePlanWorkOrderPriorityLookupTableModalComponent;
    @ViewChild('maintenancePlanWorkOrderTypeLookupTableModal', { static: true }) maintenancePlanWorkOrderTypeLookupTableModal: MaintenancePlanWorkOrderTypeLookupTableModalComponent;

    maintenancePlan: CreateOrEditMaintenancePlanDto = new CreateOrEditMaintenancePlanDto();

    workOrderPriorityPriority = '';
    workOrderTypeType = '';



    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,        
        private _maintenancePlansServiceProxy: MaintenancePlansServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.show(this._activatedRoute.snapshot.queryParams['id']);
    }

    show(maintenancePlanId?: number): void {

        if (!maintenancePlanId) {
            this.maintenancePlan = new CreateOrEditMaintenancePlanDto();
            this.maintenancePlan.id = maintenancePlanId;
            this.workOrderPriorityPriority = '';
            this.workOrderTypeType = '';

            this.active = true;
        } else {
            this._maintenancePlansServiceProxy.getMaintenancePlanForEdit(maintenancePlanId).subscribe(result => {
                this.maintenancePlan = result.maintenancePlan;

                this.workOrderPriorityPriority = result.workOrderPriorityPriority;
                this.workOrderTypeType = result.workOrderTypeType;

                this.active = true;
            });
        }
        
    }

    private saveInternal(): Observable<void> {
            this.saving = true;
            
        
        return this._maintenancePlansServiceProxy.createOrEdit(this.maintenancePlan)
         .pipe(finalize(() => { 
            this.saving = false;               
            this.notify.info(this.l('SavedSuccessfully'));
         }));
    }
    
    save(): void {
        this.saveInternal().subscribe(x => {
             this._router.navigate( ['/app/admin/support/maintenancePlans']);
        })
    }
    
    saveAndNew(): void {
        this.saveInternal().subscribe(x => {
            this.maintenancePlan = new CreateOrEditMaintenancePlanDto();
        })
    }

    openSelectWorkOrderPriorityModal() {
        this.maintenancePlanWorkOrderPriorityLookupTableModal.id = this.maintenancePlan.workOrderPriorityId;
        this.maintenancePlanWorkOrderPriorityLookupTableModal.displayName = this.workOrderPriorityPriority;
        this.maintenancePlanWorkOrderPriorityLookupTableModal.show();
    }
    openSelectWorkOrderTypeModal() {
        this.maintenancePlanWorkOrderTypeLookupTableModal.id = this.maintenancePlan.workOrderTypeId;
        this.maintenancePlanWorkOrderTypeLookupTableModal.displayName = this.workOrderTypeType;
        this.maintenancePlanWorkOrderTypeLookupTableModal.show();
    }


    setWorkOrderPriorityIdNull() {
        this.maintenancePlan.workOrderPriorityId = null;
        this.workOrderPriorityPriority = '';
    }
    setWorkOrderTypeIdNull() {
        this.maintenancePlan.workOrderTypeId = null;
        this.workOrderTypeType = '';
    }


    getNewWorkOrderPriorityId() {
        this.maintenancePlan.workOrderPriorityId = this.maintenancePlanWorkOrderPriorityLookupTableModal.id;
        this.workOrderPriorityPriority = this.maintenancePlanWorkOrderPriorityLookupTableModal.displayName;
    }
    getNewWorkOrderTypeId() {
        this.maintenancePlan.workOrderTypeId = this.maintenancePlanWorkOrderTypeLookupTableModal.id;
        this.workOrderTypeType = this.maintenancePlanWorkOrderTypeLookupTableModal.displayName;
    }


}
