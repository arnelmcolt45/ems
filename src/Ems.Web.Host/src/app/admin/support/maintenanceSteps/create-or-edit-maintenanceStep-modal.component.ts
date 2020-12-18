import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { MaintenanceStepsServiceProxy, CreateOrEditMaintenanceStepDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { MaintenanceStepMaintenancePlanLookupTableModalComponent } from './maintenanceStep-maintenancePlan-lookup-table-modal.component';
import { MaintenanceStepItemTypeLookupTableModalComponent } from './maintenanceStep-itemType-lookup-table-modal.component';
import { MaintenanceStepWorkOrderActionLookupTableModalComponent } from './maintenanceStep-workOrderAction-lookup-table-modal.component';

@Component({
    selector: 'createOrEditMaintenanceStepModal',
    templateUrl: './create-or-edit-maintenanceStep-modal.component.html'
})
export class CreateOrEditMaintenanceStepModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('maintenanceStepMaintenancePlanLookupTableModal', { static: true }) maintenanceStepMaintenancePlanLookupTableModal: MaintenanceStepMaintenancePlanLookupTableModalComponent;
    @ViewChild('maintenanceStepItemTypeLookupTableModal', { static: true }) maintenanceStepItemTypeLookupTableModal: MaintenanceStepItemTypeLookupTableModalComponent;
    @ViewChild('maintenanceStepWorkOrderActionLookupTableModal', { static: true }) maintenanceStepWorkOrderActionLookupTableModal: MaintenanceStepWorkOrderActionLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    maintenanceStep: CreateOrEditMaintenanceStepDto = new CreateOrEditMaintenanceStepDto();

    maintenancePlanId = 0;
    maintenancePlanSubject = '';
    itemTypeType = '';
    workOrderActionAction = '';


    constructor(
        injector: Injector,
        private _maintenanceStepsServiceProxy: MaintenanceStepsServiceProxy
    ) {
        super(injector);
    }
    
    show(maintenanceStepId?: number, maintenancePlanId?: number): void {

        if (!maintenanceStepId) {
            this.maintenancePlanId = maintenancePlanId;
            this.maintenanceStep = new CreateOrEditMaintenanceStepDto();
            this.maintenanceStep.maintenancePlanId = maintenancePlanId;
            this.maintenanceStep.id = maintenanceStepId;
            this.maintenancePlanSubject = '';
            this.itemTypeType = '';
            this.workOrderActionAction = '';
            this.active = true;
            this.modal.show();
        } else {
            this._maintenanceStepsServiceProxy.getMaintenanceStepForEdit(maintenanceStepId).subscribe(result => {
                this.maintenanceStep = result.maintenanceStep;

                this.maintenancePlanSubject = result.maintenancePlanSubject;
                this.itemTypeType = result.itemTypeType;
                this.workOrderActionAction = result.workOrderActionAction;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._maintenanceStepsServiceProxy.createOrEdit(this.maintenanceStep)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectMaintenancePlanModal() {
        this.maintenanceStepMaintenancePlanLookupTableModal.id = this.maintenanceStep.maintenancePlanId;
        this.maintenanceStepMaintenancePlanLookupTableModal.displayName = this.maintenancePlanSubject;
        this.maintenanceStepMaintenancePlanLookupTableModal.show();
    }
    openSelectItemTypeModal() {
        this.maintenanceStepItemTypeLookupTableModal.id = this.maintenanceStep.itemTypeId;
        this.maintenanceStepItemTypeLookupTableModal.displayName = this.itemTypeType;
        this.maintenanceStepItemTypeLookupTableModal.show();
    }
    openSelectWorkOrderActionModal() {
        this.maintenanceStepWorkOrderActionLookupTableModal.id = this.maintenanceStep.workOrderActionId;
        this.maintenanceStepWorkOrderActionLookupTableModal.displayName = this.workOrderActionAction;
        this.maintenanceStepWorkOrderActionLookupTableModal.show();
    }


    setMaintenancePlanIdNull() {
        this.maintenanceStep.maintenancePlanId = null;
        this.maintenancePlanSubject = '';
    }
    setItemTypeIdNull() {
        this.maintenanceStep.itemTypeId = null;
        this.itemTypeType = '';
    }
    setWorkOrderActionIdNull() {
        this.maintenanceStep.workOrderActionId = null;
        this.workOrderActionAction = '';
    }


    getNewMaintenancePlanId() {
        this.maintenanceStep.maintenancePlanId = this.maintenanceStepMaintenancePlanLookupTableModal.id;
        this.maintenancePlanSubject = this.maintenanceStepMaintenancePlanLookupTableModal.displayName;
    }
    getNewItemTypeId() {
        this.maintenanceStep.itemTypeId = this.maintenanceStepItemTypeLookupTableModal.id;
        this.itemTypeType = this.maintenanceStepItemTypeLookupTableModal.displayName;
    }
    getNewWorkOrderActionId() {
        this.maintenanceStep.workOrderActionId = this.maintenanceStepWorkOrderActionLookupTableModal.id;
        this.workOrderActionAction = this.maintenanceStepWorkOrderActionLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
