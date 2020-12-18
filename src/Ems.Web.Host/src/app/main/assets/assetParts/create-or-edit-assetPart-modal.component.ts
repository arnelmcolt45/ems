import { Component, ViewChild, Injector, Output, EventEmitter, Input} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AssetPartsServiceProxy, CreateOrEditAssetPartDto, AssetPartExtendedDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { AssetPartAssetPartTypeLookupTableModalComponent } from './assetPart-assetPartType-lookup-table-modal.component';
import { AssetPartAssetPartLookupTableModalComponent } from './assetPart-assetPart-lookup-table-modal.component';
import { AssetPartAssetPartStatusLookupTableModalComponent } from './assetPart-assetPartStatus-lookup-table-modal.component';
import { AssetPartAssetLookupTableModalComponent } from './assetPart-asset-lookup-table-modal.component';
import { AssetPartItemTypeLookupTableModalComponent } from './assetPart-itemType-lookup-table-modal.component';
import { AssetPartWarehouseLookupTableModalComponent } from './assetPart-warehouse-lookup-table-modal.component';
import { NumberValueAccessor } from '@angular/forms';

export interface IAssetPartOnEdit {
    id?: number;
    parentId?: number;
    name?: string;
    assetId?: number;
    assetReference?: string
    warehouseId?: number;
    showAssetField?: boolean;
    isItem: boolean;
}

@Component({
    selector: 'createOrEditAssetPartModal',
    templateUrl: './create-or-edit-assetPart-modal.component.html'
})
export class CreateOrEditAssetPartModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('assetPartAssetPartTypeLookupTableModal', { static: true }) assetPartAssetPartTypeLookupTableModal: AssetPartAssetPartTypeLookupTableModalComponent;
    @ViewChild('assetPartAssetPartLookupTableModal', { static: true }) assetPartAssetPartLookupTableModal: AssetPartAssetPartLookupTableModalComponent;
    @ViewChild('assetPartAssetPartStatusLookupTableModal', { static: true }) assetPartAssetPartStatusLookupTableModal: AssetPartAssetPartStatusLookupTableModalComponent;
    @ViewChild('assetPartAssetLookupTableModal', { static: true }) assetPartAssetLookupTableModal: AssetPartAssetLookupTableModalComponent;
    @ViewChild('assetPartItemTypeLookupTableModal', { static: true }) assetPartItemTypeLookupTableModal: AssetPartItemTypeLookupTableModalComponent;
    @ViewChild('assetPartWarehouseLookupTableModal', { static: true }) assetPartWarehouseLookupTableModal: AssetPartWarehouseLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Output() partUpdated: EventEmitter<any> = new EventEmitter<any>();
    @Output() partCreated: EventEmitter<any> = new EventEmitter<any>();

    @Input() assetId: number;
    @Input() assetReference: string;
    @Input() isItem: boolean;

    active = false;
    saving = false;

    assetPartOnEdit: IAssetPartOnEdit = {isItem: false};

    assetPart: CreateOrEditAssetPartDto = new CreateOrEditAssetPartDto();

    installDate: Date;
    assetPartTypeType = '';
    assetPartName = '';
    assetPartStatusStatus = '';
    usageMetricMetric = '';
    attachmentFilename = '';
    itemTypeType = '';
    warehouseName = '';
    warehouseId: number;
    showAssetField: boolean;
    qty: number;

    //isItem: boolean;

    constructor(
        injector: Injector,
        private _assetPartsServiceProxy: AssetPartsServiceProxy
    ) {
        super(injector);
    }

    show(assetPartOnEdit: IAssetPartOnEdit): void {
        this.assetReference = assetPartOnEdit.assetReference;
        this.isItem = assetPartOnEdit.isItem;

        if (assetPartOnEdit.showAssetField == true){
            this.showAssetField = true;
        }
        this.installDate = null;
        this.assetPart.assetId = this.assetId;
        this.assetPart.code = '00000';
        
        if ( !assetPartOnEdit.id) {
            this.assetPart = new CreateOrEditAssetPartDto();
            this.assetPart.assetId = assetPartOnEdit.assetId;
            this.assetPart.warehouseId = assetPartOnEdit.warehouseId;
            this.assetPart.parentId = assetPartOnEdit.parentId;
            this.assetPart.id = assetPartOnEdit.id;
            this.assetPartTypeType = '';
            this.assetPartName = '';
            this.assetPartStatusStatus = '';
            this.usageMetricMetric = '';
            this.attachmentFilename = '';
            //this.assetReference = '';
            this.itemTypeType = '';
            this.qty = 0;
            this.warehouseName = '';   
            this.assetPart.warehouseId = assetPartOnEdit.warehouseId;

            this.active = true;
            this.modal.show();
        } else {
            this._assetPartsServiceProxy.getAssetPartForEdit(assetPartOnEdit.id).subscribe(result => {
                this.assetPart = result.assetPart;

                if (this.assetPart.installDate) {
					this.installDate = this.assetPart.installDate.toDate();
                }
                this.assetPartTypeType = result.assetPartTypeType;
                this.assetPartName = result.assetPartName;
                this.assetPartStatusStatus = result.assetPartStatusStatus;
                this.assetReference = result.assetReference;
                this.itemTypeType = result.itemTypeType;
                this.warehouseId = result.assetPart.warehouseId;
                this.warehouseName = result.warehouseName;
                
                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
        this.saving = true;
        
        this.assetPart.assetId = this.assetId;
        this.assetPart.isItem = this.isItem;

        if(this.assetPart.name == null ||  this.assetPart.name == "" ){

            this.assetPart.name = "Items: " + this.assetPartItemTypeLookupTableModal.displayName;
            this.assetPart.description = this.assetPartItemTypeLookupTableModal.displayName + " items for " + this.assetReference;
        }

        if (this.installDate) {
            if (!this.assetPart.installDate) {
                this.assetPart.installDate = moment(this.installDate).startOf('day');
            }
            else {
                this.assetPart.installDate = moment(this.installDate);
            }
        }
        else {
            this.assetPart.installDate = null;
        }
        
        this.assetPart.code = '00000';

        this._assetPartsServiceProxy.createOrEdit(this.assetPart)
            .pipe(finalize(() => { this.saving = false;}))
            .subscribe((result: CreateOrEditAssetPartDto) => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close();

        if(!this.assetPart.id) {
            this.assetPart.code = '00000';
            this.assetPart.id = result.id;
            this.partCreated.emit(this.assetPart)
        }
        else{

            var assetPartExtendedDto = new AssetPartExtendedDto();

            assetPartExtendedDto.assetPartStatus = this.assetPartStatusStatus;
            assetPartExtendedDto.assetPartType = this.assetPartTypeType;
            assetPartExtendedDto.assetReference = this.assetReference;
            assetPartExtendedDto.itemType = this.itemTypeType;
            assetPartExtendedDto.warehouseName = this.warehouseName;
            assetPartExtendedDto.name = this.assetPart.name;
            assetPartExtendedDto.description = this.assetPart.description;
            assetPartExtendedDto.serialNumber = this.assetPart.serialNumber;
            assetPartExtendedDto.installDate = this.assetPart.installDate;
            assetPartExtendedDto.code = this.assetPart.code;
            assetPartExtendedDto.installed = this.assetPart.installed;
            assetPartExtendedDto.id = this.assetPart.id;
            
            this.partUpdated.emit(assetPartExtendedDto)
        }
        this.modalSave.emit(null);
        });
    }

    openSelectAssetPartTypeModal() {
        this.assetPartAssetPartTypeLookupTableModal.id = this.assetPart.assetPartTypeId;
        this.assetPartAssetPartTypeLookupTableModal.displayName = this.assetPartTypeType;
        this.assetPartAssetPartTypeLookupTableModal.show();
    }
    openSelectAssetPartModal() {
        this.assetPartAssetPartLookupTableModal.id = this.assetPart.parentId;
        this.assetPartAssetPartLookupTableModal.displayName = this.assetPartName;
        this.assetPartAssetPartLookupTableModal.show();
    }
    openSelectAssetPartStatusModal() {
        this.assetPartAssetPartStatusLookupTableModal.id = this.assetPart.assetPartStatusId;
        this.assetPartAssetPartStatusLookupTableModal.displayName = this.assetPartStatusStatus;
        this.assetPartAssetPartStatusLookupTableModal.show();
    }
    openSelectAssetModal() {
        this.assetPartAssetLookupTableModal.id = this.assetPart.assetId;
        this.assetPartAssetLookupTableModal.displayName = this.assetReference;
        this.assetPartAssetLookupTableModal.show();
    }
    openSelectItemTypeModal() {
        this.assetPartItemTypeLookupTableModal.id = this.assetPart.itemTypeId;
        this.assetPartItemTypeLookupTableModal.displayName = this.itemTypeType;
        this.assetPartItemTypeLookupTableModal.show();
    }
    openSelectWarehouseModal() {
        this.assetPartWarehouseLookupTableModal.id = this.assetPart.warehouseId;
        this.assetPartWarehouseLookupTableModal.displayName = this.warehouseName;
        this.assetPartWarehouseLookupTableModal.show();
    }

    setAssetPartTypeIdNull() {
        this.assetPart.assetPartTypeId = null;
        this.assetPartTypeType = '';
    }
    setParentIdNull() {
        this.assetPart.parentId = null;
        this.assetPartName = '';
    }
    setAssetPartStatusIdNull() {
        this.assetPart.assetPartStatusId = null;
        this.assetPartStatusStatus = '';
    }
    setAssetIdNull() {
        this.assetPart.assetId = null;
        this.assetReference = '';
    }
    setItemTypeIdNull() {
        this.assetPart.itemTypeId = null;
        this.itemTypeType = '';
    }
    setWarehouseIdNull() {
        this.assetPart.warehouseId = null;
        this.warehouseName = '';
    }

    getNewAssetPartTypeId() {
        this.assetPart.assetPartTypeId = this.assetPartAssetPartTypeLookupTableModal.id;
        this.assetPartTypeType = this.assetPartAssetPartTypeLookupTableModal.displayName;
    }
    getNewParentId() {
        this.assetPart.parentId = this.assetPartAssetPartLookupTableModal.id;
        this.assetPartName = this.assetPartAssetPartLookupTableModal.displayName;
    }
    getNewAssetPartStatusId() {
        this.assetPart.assetPartStatusId = this.assetPartAssetPartStatusLookupTableModal.id;
        this.assetPartStatusStatus = this.assetPartAssetPartStatusLookupTableModal.displayName;
    }
    getNewAssetId() {
        this.assetPart.assetId = this.assetPartAssetLookupTableModal.id;
        this.assetReference = this.assetPartAssetLookupTableModal.displayName;
    }
    getNewItemTypeId() {
        this.assetPart.itemTypeId = this.assetPartItemTypeLookupTableModal.id;
        this.itemTypeType = this.assetPartItemTypeLookupTableModal.displayName;
    }
    getNewWarehouseId() {
        this.assetPart.warehouseId = this.assetPartWarehouseLookupTableModal.id;
        this.warehouseName = this.assetPartWarehouseLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
