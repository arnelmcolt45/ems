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
import { WarehousesServiceProxy, WarehouseDto, InventoryItemsServiceProxy, InventoryItemDto,  AssetPartsServiceProxy, AssetPartDto  } from '@shared/service-proxies/service-proxies';
import { CreateOrEditWarehouseModalComponent } from './create-or-edit-warehouse-modal.component';
import { CreateOrEditInventoryItemModalComponent } from '../inventoryItems/create-or-edit-inventoryItem-modal.component';
import { CreateOrEditAssetPartModalComponent, IAssetPartOnEdit } from '../assetParts/create-or-edit-assetPart-modal.component';
import { ViewAssetPartModalComponent } from '../assetParts/view-assetPart-modal.component';
import { ViewInventoryItemModalComponent } from '../inventoryItems/view-inventoryItem-modal.component';
import * as moment from 'moment';
import { CreateOrEditAttachmentModalComponent } from '../../storage/attachments/create-or-edit-attachment-modal.component';
import { ViewAttachmentModalComponent } from '../../storage/attachments//view-attachment-modal.component';
import { AttachmentsServiceProxy, AttachmentDto } from '@shared/service-proxies/service-proxies';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'viewWarehouse',
    templateUrl: './viewWarehouse.component.html',
    animations: [appModuleAnimation()]
})
export class ViewWarehouseComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('createOrEditWarehouseModalComponent', { static: true }) createOrEditWarehouseModalComponent: CreateOrEditWarehouseModalComponent;
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('createOrEditInventoryItemModal', { static: true }) createOrEditInventoryItemModal: CreateOrEditInventoryItemModalComponent;
    @ViewChild('viewInventoryItemModalComponent', { static: true }) viewInventoryItemModalComponent: ViewInventoryItemModalComponent;

    @ViewChild('createOrEditAssetPartModal', { static: true }) createOrEditAssetPartModal: CreateOrEditAssetPartModalComponent;
    @ViewChild('viewAssetPartModalComponent', { static: true }) viewAssetPartModal: ViewAssetPartModalComponent;

    @ViewChild('createOrEditAttachmentModal', { static: true }) createOrEditAttachmentModal: CreateOrEditAttachmentModalComponent;
    @ViewChild('viewAttachmentModalComponent', { static: true }) viewAttachmentModal: ViewAttachmentModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For the 2nd table
    @ViewChild('paginator1', { static: true }) paginator1: Paginator; // For the 2nd table

    primengTableHelper1: PrimengTableHelper;

    _entityTypeFullName = 'Ems.Assets.Warehouse';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _warehousesServiceProxy: WarehousesServiceProxy,
        private _inventoryItemsServiceProxy: InventoryItemsServiceProxy,
        private _assetPartsServiceProxy: AssetPartsServiceProxy
    ) {
        super(injector);
        this.primengTableHelper1 = new PrimengTableHelper(); // For the 2nd table
    }

    active = false;
    saving = false;

    assetId: number;
    warehouseId: number;
    warehouse: WarehouseDto;
    addressLine1Filter = '';
    addressLine2Filter = '';
    postalCodeFilter = '';
    cityFilter = '';
    stateFilter = '';
    countryFilter = '';

    advancedFiltersAreShownForInventoryItems = false;
    filterTextForInventoryItems = '';
    nameFilterForInventoryItems = '';
    referenceFilterForInventoryItems = '';
    maxQtyInWarehouseFilter : number;
    maxQtyInWarehouseFilterEmpty : number;
    minQtyInWarehouseFilter : number;
    minQtyInWarehouseFilterEmpty : number;
    maxRestockLimitFilter : number;
    maxRestockLimitFilterEmpty : number;
    minRestockLimitFilter : number;
    minRestockLimitFilterEmpty : number;
    maxQtyOnOrderFilter : number;
    maxQtyOnOrderFilterEmpty : number;
    minQtyOnOrderFilter : number;
    minQtyOnOrderFilterEmpty : number;
    itemTypeTypeFilter = '';
    assetReferenceFilterForInventoryItems = '';
    warehouseNameFilterForInventoryItems = '';

    advancedFiltersAreShownForAssetParts = false;
    filterTextForAssetParts = '';
    nameFilterForAssetParts = '';
    descriptionFilter = '';
    serialNumberFilter = '';
    maxInstallDateFilter : moment.Moment;
    minInstallDateFilter : moment.Moment;
    installedFilter = -1;
    assetPartTypeTypeFilter = '';
    assetPartNameFilter = '';
    assetPartStatusStatusFilter = '';
    usageMetricMetricFilter = '';
    attachmentFilenameFilter = '';
    assetReferenceFilterForAssetParts = '';
    itemTypeTypeFilterForAssetParts = '';
    warehouseNameFilterForAssetParts = '';

    ngOnInit(): void {

        this.warehouseId = this._activatedRoute.snapshot.queryParams['warehouseId'];
        this.warehouse = new WarehouseDto();

        this.getWarehouse();
        
        this.active = true;
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getWarehouse(event?: LazyLoadEvent) {
        this._warehousesServiceProxy.getWarehouseForView(this.warehouseId)
            .subscribe((warehouseResult) => {

                if (warehouseResult.warehouse == null) {
                    this.close();
                }
                else {
                    this.warehouse = warehouseResult.warehouse;
                    //this.getAssetParts();

                    //TODO: Add Warehouse Type to Warehouses

                    //this.warehouseTypeType = warehouseResult.warehouseTypeType;

                }
            });
    }

    createInventoryItem(): void {
        this.createOrEditInventoryItemModal.show(null, this.warehouseId, false);
    }

    getInventoryItems(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._inventoryItemsServiceProxy.getAll(
            this.assetId,
            this.warehouseId,
            this.filterTextForInventoryItems,
            this.nameFilterForInventoryItems,
            this.referenceFilterForInventoryItems,
            this.maxQtyInWarehouseFilter == null ? this.maxQtyInWarehouseFilterEmpty: this.maxQtyInWarehouseFilter,
            this.minQtyInWarehouseFilter == null ? this.minQtyInWarehouseFilterEmpty: this.minQtyInWarehouseFilter,
            this.maxRestockLimitFilter == null ? this.maxRestockLimitFilterEmpty: this.maxRestockLimitFilter,
            this.minRestockLimitFilter == null ? this.minRestockLimitFilterEmpty: this.minRestockLimitFilter,
            this.maxQtyOnOrderFilter == null ? this.maxQtyOnOrderFilterEmpty: this.maxQtyOnOrderFilter,
            this.minQtyOnOrderFilter == null ? this.minQtyOnOrderFilterEmpty: this.minQtyOnOrderFilter,
            this.itemTypeTypeFilter,
            this.assetReferenceFilterForInventoryItems,
            this.warehouseNameFilterForInventoryItems,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }
    
    deleteInventoryItem(inventoryItem: InventoryItemDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._inventoryItemsServiceProxy.delete(inventoryItem.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    getAssetParts(event?: LazyLoadEvent) {
        if (this.primengTableHelper1.shouldResetPaging(event)) {
            this.paginator1.changePage(0);
            return;
        }

        this.primengTableHelper1.showLoadingIndicator();

        this._assetPartsServiceProxy.getAll(
            this.assetId,
            this.warehouseId,
            this.filterTextForAssetParts,
            this.nameFilterForAssetParts,
            this.descriptionFilter,
            this.serialNumberFilter,
            this.maxInstallDateFilter,
            this.minInstallDateFilter,
            this.installedFilter,
            this.assetPartTypeTypeFilter,
            this.assetPartNameFilter,
            this.assetPartStatusStatusFilter,
            this.assetReferenceFilterForAssetParts,
            this.itemTypeTypeFilter,
            this.warehouseNameFilterForAssetParts,
            this.primengTableHelper1.getSorting(this.dataTable1),
            this.primengTableHelper1.getSkipCount(this.paginator1, event),
            this.primengTableHelper1.getMaxResultCount(this.paginator1, event)
        ).subscribe(result => {
            this.primengTableHelper1.totalRecordsCount = result.totalCount;
            this.primengTableHelper1.records = result.items;
            this.primengTableHelper1.hideLoadingIndicator();
        });
    }

    createAssetPart(isItem: boolean, parentId?: number, assetId?: number): void {
        this.createOrEditAssetPartModal.show( {
            isItem: isItem,
            parentId: parentId,
            warehouseId: this.warehouseId,
            assetId: assetId
        });
    }

    deleteAssetPart(assetPart: AssetPartDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._assetPartsServiceProxy.delete(assetPart.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    showHistory(warehouse: WarehouseDto): void {
        this.entityTypeHistoryModal.show({
            entityId: warehouse.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    deleteWarehouse(warehouse: WarehouseDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._warehousesServiceProxy.delete(warehouse.id)
                        .subscribe(() => {
                            this.close();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    reloadPage(): void {
        this.paginator1.changePage(this.paginator1.getPage())
    }

    close(): void {
        this._router.navigate(['app/main/assets/warehouses']);
    }
}
