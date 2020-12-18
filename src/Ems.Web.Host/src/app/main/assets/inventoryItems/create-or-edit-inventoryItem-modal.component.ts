import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { InventoryItemsServiceProxy, CreateOrEditInventoryItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { InventoryItemItemTypeLookupTableModalComponent } from './inventoryItem-itemType-lookup-table-modal.component';
import { InventoryItemAssetLookupTableModalComponent } from './inventoryItem-asset-lookup-table-modal.component';
import { InventoryItemWarehouseLookupTableModalComponent } from './inventoryItem-warehouse-lookup-table-modal.component';

@Component({
    selector: 'createOrEditInventoryItemModal',
    templateUrl: './create-or-edit-inventoryItem-modal.component.html'
})
export class CreateOrEditInventoryItemModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('inventoryItemItemTypeLookupTableModal', { static: true }) inventoryItemItemTypeLookupTableModal: InventoryItemItemTypeLookupTableModalComponent;
    @ViewChild('inventoryItemAssetLookupTableModal', { static: true }) inventoryItemAssetLookupTableModal: InventoryItemAssetLookupTableModalComponent;
    @ViewChild('inventoryItemWarehouseLookupTableModal', { static: true }) inventoryItemWarehouseLookupTableModal: InventoryItemWarehouseLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    inventoryItem: CreateOrEditInventoryItemDto = new CreateOrEditInventoryItemDto();

    itemTypeType = '';
    assetReference = '';
    warehouseName = '';
    warehouseId = 0;

    showAssetField: boolean;

    constructor(
        injector: Injector,
        private _inventoryItemsServiceProxy: InventoryItemsServiceProxy
    ) {
        super(injector);
    }
    
    show(inventoryItemId?: number, warehouseId?: number, showAssetField?: boolean): void {
        
        if (showAssetField){
            this.showAssetField = true;
        }

        if (!inventoryItemId) {
            this.inventoryItem = new CreateOrEditInventoryItemDto();
            this.inventoryItem.id = inventoryItemId;
            this.inventoryItem.warehouseId = warehouseId;
            this.itemTypeType = '';
            this.assetReference = '';
            this.warehouseName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._inventoryItemsServiceProxy.getInventoryItemForEdit(inventoryItemId).subscribe(result => {
                this.inventoryItem = result.inventoryItem;

                this.itemTypeType = result.itemTypeType;
                this.assetReference = result.assetReference;
                this.warehouseId = result.inventoryItem.warehouseId;
                this.warehouseName = result.warehouseName;
                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

            this._inventoryItemsServiceProxy.createOrEdit(this.inventoryItem)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }

    openSelectItemTypeModal() {
        this.inventoryItemItemTypeLookupTableModal.id = this.inventoryItem.itemTypeId;
        this.inventoryItemItemTypeLookupTableModal.displayName = this.itemTypeType;
        this.inventoryItemItemTypeLookupTableModal.show();
    }
    openSelectAssetModal() {
        this.inventoryItemAssetLookupTableModal.id = this.inventoryItem.assetId;
        this.inventoryItemAssetLookupTableModal.displayName = this.assetReference;
        this.inventoryItemAssetLookupTableModal.show();
    }
    openSelectWarehouseModal() {
        this.inventoryItemWarehouseLookupTableModal.id = this.inventoryItem.warehouseId;
        this.inventoryItemWarehouseLookupTableModal.displayName = this.warehouseName;
        this.inventoryItemWarehouseLookupTableModal.show();
    }


    setItemTypeIdNull() {
        this.inventoryItem.itemTypeId = null;
        this.itemTypeType = '';
    }
    setAssetIdNull() {
        this.inventoryItem.assetId = null;
        this.assetReference = '';
    }
    setWarehouseIdNull() {
        this.inventoryItem.warehouseId = null;
        this.warehouseName = '';
    }


    getNewItemTypeId() {
        this.inventoryItem.itemTypeId = this.inventoryItemItemTypeLookupTableModal.id;
        this.itemTypeType = this.inventoryItemItemTypeLookupTableModal.displayName;
    }
    getNewAssetId() {
        this.inventoryItem.assetId = this.inventoryItemAssetLookupTableModal.id;
        this.assetReference = this.inventoryItemAssetLookupTableModal.displayName;
    }
    getNewWarehouseId() {
        this.inventoryItem.warehouseId = this.inventoryItemWarehouseLookupTableModal.id;
        this.warehouseName = this.inventoryItemWarehouseLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
