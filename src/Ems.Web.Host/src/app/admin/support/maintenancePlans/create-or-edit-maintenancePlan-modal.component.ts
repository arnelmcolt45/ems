import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { MaintenancePlansServiceProxy, CreateOrEditMaintenancePlanDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { MaintenancePlanWorkOrderPriorityLookupTableModalComponent } from './maintenancePlan-workOrderPriority-lookup-table-modal.component';
import { MaintenancePlanWorkOrderTypeLookupTableModalComponent } from './maintenancePlan-workOrderType-lookup-table-modal.component';

@Component({
    selector: 'createOrEditMaintenancePlanModal',
    templateUrl: './create-or-edit-maintenancePlan-modal.component.html'
})
export class CreateOrEditMaintenancePlanModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('maintenancePlanWorkOrderPriorityLookupTableModal', { static: true }) maintenancePlanWorkOrderPriorityLookupTableModal: MaintenancePlanWorkOrderPriorityLookupTableModalComponent;
    @ViewChild('maintenancePlanWorkOrderTypeLookupTableModal', { static: true }) maintenancePlanWorkOrderTypeLookupTableModal: MaintenancePlanWorkOrderTypeLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    maintenancePlan: CreateOrEditMaintenancePlanDto = new CreateOrEditMaintenancePlanDto();

    workOrderPriorityPriority = '';
    workOrderTypeType = '';


    constructor(
        injector: Injector,
        private _maintenancePlansServiceProxy: MaintenancePlansServiceProxy
    ) {
        super(injector);
    }
    
    show(maintenancePlanId?: number): void {
    

        if (!maintenancePlanId) {
            this.maintenancePlan = new CreateOrEditMaintenancePlanDto();
            this.maintenancePlan.id = maintenancePlanId;
            this.workOrderPriorityPriority = '';
            this.workOrderTypeType = '';

            this.active = true;
            this.modal.show();
        } else {
            this._maintenancePlansServiceProxy.getMaintenancePlanForEdit(maintenancePlanId).subscribe(result => {
                this.maintenancePlan = result.maintenancePlan;

                this.workOrderPriorityPriority = result.workOrderPriorityPriority;
                this.workOrderTypeType = result.workOrderTypeType;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._maintenancePlansServiceProxy.createOrEdit(this.maintenancePlan)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
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


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
