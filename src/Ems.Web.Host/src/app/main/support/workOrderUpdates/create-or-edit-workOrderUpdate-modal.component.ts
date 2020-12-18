import { Component, ViewChild, Injector, Output, EventEmitter, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WorkOrderUpdatesServiceProxy, CreateOrEditWorkOrderUpdateDto, AssetPartDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { CreateOrEditAssetPartModalComponent, IAssetPartOnEdit } from '../../assets/assetParts/create-or-edit-assetPart-modal.component';
import { ItemTypeLookupTableModalComponent } from '@app/main/assets/itemTypes/itemType-lookup-table-modal.component';
import { WorkOrderUpdateWorkOrderActionLookupTableModalComponent } from './workOrderUpdate-workOrderAction-lookup-table-modal.component';
//import { UomLookupTableModalComponent } from '@app/config/metrics/uoms/uom-lookup-table-modal.component';
import { WorkOrderUpdateAssetPartLookupTableModalComponent } from './workOrderUpdate-assetPart-lookup-table-modal.component';
 
@Component({
    selector: 'createOrEditWorkOrderUpdateModal',
    templateUrl: './create-or-edit-workOrderUpdate-modal.component.html'
})
export class CreateOrEditWorkOrderUpdateModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditAssetPartModal', { static: true }) createOrEditAssetPartModal: CreateOrEditAssetPartModalComponent;
    @ViewChild('workOrderUpdateAssetPartLookupTableModal', { static: true }) workOrderUpdateAssetPartLookupTableModal: WorkOrderUpdateAssetPartLookupTableModalComponent;
    @ViewChild('itemTypeLookupTableModal', { static: true }) itemTypeLookupTableModal: ItemTypeLookupTableModalComponent;
    @ViewChild('workOrderUpdateWorkOrderActionLookupTableModal', { static: true }) workOrderUpdateWorkOrderActionLookupTableModal: WorkOrderUpdateWorkOrderActionLookupTableModalComponent;
    //@ViewChild('uomLookupTableModal', { static: true }) uomLookupTableModal: UomLookupTableModalComponent;

    @Output() workOrderUpdateModalSave: EventEmitter<any> = new EventEmitter<any>();

    @Input() assetId: number;
    @Input() assetReference: string;

    active = false;
    saving = false;

    workOrderUpdate: CreateOrEditWorkOrderUpdateDto = new CreateOrEditWorkOrderUpdateDto();
    parentId: number;
    warehouseId: number;
    isItem = false;
    workOrderId: number;
    workOrderUpdateSubject = '';
    workOrderUpdateActionAction = '';
    itemTypeType = '';
    assetPartName = '';

    constructor(
        injector: Injector,
        private _workOrderUpdatesServiceProxy: WorkOrderUpdatesServiceProxy
    ) {
        super(injector);
    }

    show(workOrderUpdateId?: number, workOrderId?: number): void {

        if (!workOrderUpdateId) {
            this.workOrderUpdate = new CreateOrEditWorkOrderUpdateDto();
            this.workOrderUpdate.id = workOrderUpdateId;
            this.workOrderUpdate.workOrderId = workOrderId;
            //this.workOrderUpdate.updatedAt = moment().startOf('day');
            this.workOrderUpdateSubject = '';
            this.workOrderUpdateActionAction = '';
            this.itemTypeType = '';
            this.assetPartName = '';
            this.active = true;
            this.modal.show();
        } else {
            this._workOrderUpdatesServiceProxy.getWorkOrderUpdateForEdit(workOrderUpdateId).subscribe(result => {

                this.workOrderUpdate = result.workOrderUpdate;
                this.workOrderUpdate.workOrderId = workOrderId;

                this.workOrderUpdateSubject = result.workOrderUpdateSubject;
                this.workOrderUpdateActionAction = result.workOrderUpdateActionAction;
                this.itemTypeType = result.itemTypeType;
                this.assetPartName = result.assetPartName;

                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        this.saving = true;
        this._workOrderUpdatesServiceProxy.createOrEdit(this.workOrderUpdate)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.workOrderUpdateModalSave.emit(null);
            });
    }
    
    createAssetPart(): void {
        this.createOrEditAssetPartModal.show( {
            isItem: false,
            parentId: this.parentId,
            warehouseId: this.warehouseId,
            assetId: this.assetId
        });
    }

    partCreated(ap: AssetPartDto): void {
        this.workOrderUpdate.assetPartId = ap.id;
        this.assetPartName = ap.name;
    }

    setWorkOrderIdNull() {
        this.workOrderUpdate.workOrderId = null;
        this.workOrderUpdateSubject = '';
    }

    openSelectAssetPartModal() {
        this.workOrderUpdateAssetPartLookupTableModal.id = this.workOrderUpdate.assetPartId;
        this.workOrderUpdateAssetPartLookupTableModal.displayName = this.assetPartName;
        this.workOrderUpdateAssetPartLookupTableModal.show();
    }
    
    openSelectItemTypeModal() {
        this.itemTypeLookupTableModal.id = this.workOrderUpdate.itemTypeId;
        this.itemTypeLookupTableModal.displayName = this.itemTypeType;
        this.itemTypeLookupTableModal.show();
    }
    openSelectActionModal() {
        this.workOrderUpdateWorkOrderActionLookupTableModal.id = this.workOrderUpdate.workOrderActionId;
        this.workOrderUpdateWorkOrderActionLookupTableModal.displayName = this.workOrderUpdateActionAction;
        this.workOrderUpdateWorkOrderActionLookupTableModal.show();
    }

    setItemTypeIdNull() {
        this.workOrderUpdate.itemTypeId = null;
        this.itemTypeType = '';
    }
    setActionIdNull() {
        this.workOrderUpdate.workOrderActionId = null;
        this.workOrderUpdateActionAction = '';
    }
    setAssetPartIdNull() {
        this.workOrderUpdate.assetPartId = null;
        this.assetPartName = '';
    }

    getNewAssetPartId() {
        this.workOrderUpdate.assetPartId = this.workOrderUpdateAssetPartLookupTableModal.id;
        this.assetPartName = this.workOrderUpdateAssetPartLookupTableModal.displayName;
    }

    getNewItemTypeId() {
        this.workOrderUpdate.itemTypeId = this.itemTypeLookupTableModal.id;
        this.itemTypeType = this.itemTypeLookupTableModal.displayName;
    }
    getNewActionId() {
        this.workOrderUpdate.workOrderActionId = this.workOrderUpdateWorkOrderActionLookupTableModal.id;
        this.workOrderUpdateActionAction = this.workOrderUpdateWorkOrderActionLookupTableModal.displayName;
    }

    close(): void {

        this.active = false;
        this.modal.hide();
    }
}


/*
import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WorkOrderUpdatesServiceProxy, CreateOrEditWorkOrderUpdateDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ItemTypeLookupTableModalComponent } from '@app/main/assets/itemTypes/itemType-lookup-table-modal.component';
import { WorkOrderUpdateWorkOrderActionLookupTableModalComponent } from '../workOrderUpdates/workOrderUpdate-workOrderAction-lookup-table-modal.component';
import { WorkOrderUpdateAssetPartLookupTableModalComponent } from './workOrderUpdate-assetPart-lookup-table-modal.component';

@Component({
    selector: 'createOrEditWorkOrderUpdateModal',
    templateUrl: './create-or-edit-workOrderUpdate-modal.component.html'
})
export class CreateOrEditWorkOrderUpdateModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('workOrderUpdateAssetPartLookupTableModal', { static: true }) workOrderUpdateAssetPartLookupTableModal: WorkOrderUpdateAssetPartLookupTableModalComponent;
    @ViewChild('itemTypeLookupTableModal', { static: true }) itemTypeLookupTableModal: ItemTypeLookupTableModalComponent;
    @ViewChild('workOrderUpdateWorkOrderActionLookupTableModal', { static: true }) workOrderUpdateWorkOrderActionLookupTableModal: WorkOrderUpdateWorkOrderActionLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    workOrderUpdate: CreateOrEditWorkOrderUpdateDto = new CreateOrEditWorkOrderUpdateDto();
    workOrderId: number;
    workOrderSubject = '';
    itemTypeType = '';
    workOrderActionAction = '';
    assetPartName = '';


    constructor(
        injector: Injector,
        private _workOrderUpdatesServiceProxy: WorkOrderUpdatesServiceProxy
    ) {
        super(injector);
    }
    
    show(workOrderUpdateId?: number, workOrderId?: number): void {

        if (!workOrderUpdateId) {
            this.workOrderUpdate = new CreateOrEditWorkOrderUpdateDto();
            this.workOrderUpdate.id = workOrderUpdateId;
            this.workOrderSubject = '';
            this.itemTypeType = '';
            this.workOrderActionAction = '';
            this.assetPartName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._workOrderUpdatesServiceProxy.getWorkOrderUpdateForEdit(workOrderUpdateId).subscribe(result => {
                this.workOrderUpdate = result.workOrderUpdate;

                this.workOrderSubject = result.workOrderSubject;
                this.itemTypeType = result.itemTypeType;
                this.workOrderActionAction = result.workOrderActionAction;
                this.assetPartName = result.assetPartName;

                this.active = true;
                this.modal.show();


                this.workOrderUpdate = result.workOrderUpdate;
                this.workOrderUpdate.workOrderId = workOrderId;

                this.workOrderSubject = result.workOrderSubject;
                this.workOrderActionAction = result.workOrderActionAction;
                this.itemTypeType = result.itemTypeType;

                this.active = true;
                this.modal.show();

            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._workOrderUpdatesServiceProxy.createOrEdit(this.workOrderUpdate)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectAssetPartModal() {
        this.workOrderUpdateAssetPartLookupTableModal.id = this.workOrderUpdate.assetPartId;
        this.workOrderUpdateAssetPartLookupTableModal.displayName = this.assetPartName;
        this.workOrderUpdateAssetPartLookupTableModal.show();
    }


    setAssetPartIdNull() {
        this.workOrderUpdate.assetPartId = null;
        this.assetPartName = '';
    }


    getNewAssetPartId() {
        this.workOrderUpdate.assetPartId = this.workOrderUpdateAssetPartLookupTableModal.id;
        this.assetPartName = this.workOrderUpdateAssetPartLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
*/