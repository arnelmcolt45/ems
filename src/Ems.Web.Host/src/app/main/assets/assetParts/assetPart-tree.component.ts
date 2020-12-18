import { OnInit, Component, EventEmitter, Injector, Output, ViewChild, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HtmlHelper } from '@shared/helpers/HtmlHelper';
import { ListResultDtoOfAssetPartExtendedDto, MoveAssetPartInput, AssetPartDto, AssetPartsServiceProxy, AssetPartExtendedDto, GetAssetPartForViewDto, AssetOwnershipAssetLookupTableDto, LeaseAgreementAssetOwnerLookupTableDto } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { IBasicAssetPartInfo } from './basic-asset-part-info';
import { CreateOrEditAssetPartModalComponent } from './create-or-edit-assetPart-modal.component';
import { ViewAssetPartModalComponent } from './view-assetPart-modal.component';
import { MoveToAssetLookupTableModalComponent } from './move-to-asset-lookup-table-modal.component';
import { MoveToWarehouseLookupTableModalComponent } from './move-to-warehouse-lookup-table-modal.component'; 
import { ImportAssetPartLookupTableModalComponent } from './import-assetPart-lookup-table-modal.component';

import { TreeNode, MenuItem } from 'primeng/api';

import { ArrayToTreeConverterService } from '@shared/utils/array-to-tree-converter.service';
import { TreeDataHelperService } from '@shared/utils/tree-data-helper.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { stringify } from 'querystring';

export interface IAssetPartOnTree extends IBasicAssetPartInfo {
    id: number;
    parent: string | number;
    code: string;
    name: string;
    //memberCount: number;
    //roleCount: number;
    text: string;
    state: any;
}

@Component({
    selector: 'assetPart-tree',
    templateUrl: './assetPart-tree.component.html'
})
export class AssetPartTreeComponent extends AppComponentBase implements OnInit {

    @Input() isItem: boolean;
    @Input() assetId: number;
    @Input() assetReference: string;

    @Output() apSelected = new EventEmitter<IBasicAssetPartInfo>();
    @Output() apUpdated = new EventEmitter<AssetPartExtendedDto>();

    @ViewChild('createOrEditAssetPartModal', {static: true}) createOrEditAssetPartModal: CreateOrEditAssetPartModalComponent;
    @ViewChild('viewAssetPartModalComponent', { static: true }) viewAssetPartModal: ViewAssetPartModalComponent;
    @ViewChild('entityTypeHistoryModal', {static: true}) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

    @ViewChild('moveToAssetLookupTableModalComponent', {static: true}) moveToAssetLookupTableModal: MoveToAssetLookupTableModalComponent;
    @ViewChild('moveToWarehouseLookupTableModalComponent', {static: true}) moveToWarehouseLookupTableModal: MoveToWarehouseLookupTableModalComponent;
    @ViewChild('importAssetPartLookupTableModalComponent', {static: true}) importAssetPartLookupTableModal: ImportAssetPartLookupTableModalComponent;

    treeData: any;
    selectedAp: TreeNode;
    //selectedMenuItem: TreeNode;
    apContextMenuItems: MenuItem[];
    canManageAssetParts = false;
    //isItem: boolean;
    importAssetPart: boolean;

    _entityTypeFullName = 'Abp.Assets.AssetPart';

    constructor(
        injector: Injector,
        private _assetPartsService: AssetPartsServiceProxy,
        private _arrayToTreeConverterService: ArrayToTreeConverterService,
        private _treeDataHelperService: TreeDataHelperService
    ) {
        super(injector);
    }

    totalPartCount = 0;

    ngOnInit(): void {
        this.canManageAssetParts = this.isGranted('Pages.Main.AssetParts.ManagePartTree');
        //this.apContextMenuItems = this.getContextMenuItems();
        this.getTreeDataFromServer();
    }

    public nodeMenu(event: MouseEvent, node: TreeNode) {

        if (node.data.isItem) {
            this.apContextMenuItems = this.getContextMenuItems(true)
        }
        else{
            this.apContextMenuItems = this.getContextMenuItems(false)
        }
        return true;
      }

    nodeSelect(event) {

        this.isItem = event.node.data.isItem;
        var installed = "No";

        if(event.node.data.installed == true){ installed = "Yes" }

        this.apSelected.emit(<IBasicAssetPartInfo>{
            id: event.node.data.id,
            name: event.node.data.name,
            description: event.node.data.description,
            serialNumber: event.node.data.serialNumber,
            installDate: event.node.data.installDate,
            installed: installed,
            assetPartType: event.node.data.assetPartType,
            assetPartStatus: event.node.data.assetPartStatus,
            assetReference: event.node.data.assetReference,
            itemType: event.node.data.itemType,
            code: event.node.data.code,
            qty: event.node.data.qty,
            isItem: event.node.data.isItem
        });
    }

    nodeDrop(event) {
        this.message.confirm(
            this.l('AssetPartMoveConfirmMessage', event.dragNode.data.name, event.dropNode.data.name),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    const input = new MoveAssetPartInput();
                    input.id = event.dragNode.data.id;
                    input.newParentId = event.originalEvent.target.nodeName === 'SPAN' ? event.dropNode.data.id : event.dropNode.parent.data.id;

                    this._assetPartsService.moveAssetPart(input)
                        .pipe(catchError(error => {
                            this.revertDragDrop();
                            return Observable.throw(error);
                        }))
                        .subscribe(() => {
                            this.notify.success(this.l('SuccessfullyMoved'));
                            this.reload();
                        });
                } else {
                    this.revertDragDrop();
                }
            }
        );
    }

    revertDragDrop() {
        this.reload();
    }

    reload(): void {
        this.apSelected.emit(<IBasicAssetPartInfo>{
            assetId: this.assetId,
            isItem: this.isItem,
            id: 0,
            name: "",
            description: "",
            serialNumber: "",
            installDate: null,
            installed: "",
            assetPartType: "",
            assetPartStatus: "",
            assetReference: "",
            itemType: "",
            code: "",
        });
        this.getTreeDataFromServer();
    }

    private getTreeDataFromServer(): void {
        let self = this;
        this._assetPartsService.getAssetParts(this.assetId).subscribe((result: ListResultDtoOfAssetPartExtendedDto) => {
            this.totalPartCount = result.items.length;

            this.treeData = this._arrayToTreeConverterService.createTree(result.items,
                'parentId',
                'id',
                null,
                'children',
                [
                    {
                        target: 'label',
                        targetFunction(item) {
                            return item.name;
                        }
                    }, {
                        target: 'expandedIcon',
                        value: 'fa fa-folder-open m--font-warning'
                    },
                    {
                        target: 'collapsedIcon',
                        value: 'fa fa-folder m--font-warning'
                    },
                    {
                        target: 'selectable',
                        value: true
                    }
                ]);
        });
    }

    private isEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    private getContextMenuItems(isItem: boolean): any[] {

        const canManageAssetPartTree = this.isGranted('Pages.Main.AssetParts.ManagePartTree');
        
        let items = [];

        let itemView = 
            {
                label: this.l('View'),
                disabled: null,
                command: (event) => {

                    var assetPartDto = new GetAssetPartForViewDto();
                    assetPartDto.assetPart = new AssetPartDto();

                    assetPartDto.assetPart.assetId =  this.selectedAp.data.assetId;
                    assetPartDto.assetPart.code =  this.selectedAp.data.code;
                    assetPartDto.assetPart.description =  this.selectedAp.data.description;
                    assetPartDto.assetPart.id =  this.selectedAp.data.id;
                    assetPartDto.assetPart.installDate =  this.selectedAp.data.installDate;
                    assetPartDto.assetPart.installed =  this.selectedAp.data.installed;
                    assetPartDto.assetPart.name =  this.selectedAp.data.name;
                    assetPartDto.assetPart.parentId =  this.selectedAp.data.parentId;
                    assetPartDto.assetPart.serialNumber =  this.selectedAp.data.serialNumber;
                    assetPartDto.assetPart.isItem =  this.selectedAp.data.isItem;
                    assetPartDto.assetPart.qty =  this.selectedAp.data.qty;
                    assetPartDto.assetPartStatusStatus = this.selectedAp.data.assetPartStatus;
                    assetPartDto.assetPartTypeType = this.selectedAp.data.assetPartType;
                    assetPartDto.itemTypeType = this.selectedAp.data.itemType;
                    assetPartDto.assetReference = this.selectedAp.data.assetReference;

                    this.viewAssetPartModal.show(
                        assetPartDto //    <--- this needs to be a GetAssetPartForViewDto, rather than AssetPartExtendedDto 
                    );
                }
            };

        let itemEdit =
            {
                label: this.l('Edit'),
                disabled: !canManageAssetPartTree,
                command: (event) => {
                    this.createOrEditAssetPartModal.show({
                        isItem: this.selectedAp.data.isItem,
                        id: this.selectedAp.data.id,
                        name: this.selectedAp.data.name,
                        assetReference: this.assetReference
                    });

                    /*
                    id?: number;
                    parentId?: number;
                    name?: string;
                    assetId?: number;
                    assetReference?: string
                    warehouseId?: number;
                    showAssetField?: boolean;
                    isItem: boolean;
                    */

                }
            };

        let itemImportComponent =
        {
            label: this.l('Import Component'),
            disabled: !canManageAssetPartTree,
            command: () => {
                this.importPart(this.selectedAp.data.id);
            }
        };
            
        let itemAddInventoryItem =
            {
                label: this.l('Add Inventory Item'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.addPart( true, this.selectedAp.data.id);
                }
            };
        
        let itemMoveToWarehouse =
            {
                label: this.l('Move to Warehouse'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.moveToWarehouseLookupTableModal.show(
                        this.selectedAp.data.id, 'component'
                    );
                }
            };
        
        let itemMoveBranchToWarehouse =
            {
                label: this.l('Move Branch to Warehouse'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.moveToWarehouseLookupTableModal.show(
                        this.selectedAp.data.id, 'branch'
                    );
                }
            };
        
        let itemMoveToAsset =
            {
                label: this.l('Move to Asset'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.moveToAssetLookupTableModal.show(
                        this.selectedAp.data.id, 'component'
                    );
                }
            };

        let itemMoveBranchToAsset =
            {
                label: this.l('Move Branch to Asset'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.moveToAssetLookupTableModal.show(
                        this.selectedAp.data.id, 'branch'
                    );
                }
            };

        let itemDeleteComponent =
            {
                label: this.l('Delete Component'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.message.confirm(
                        this.l('AssetPartDeleteWarningMessage', this.selectedAp.data.name),
                        this.l('AreYouSure'),
                        isConfirmed => {
                            if (isConfirmed) {
                                this._assetPartsService.deleteComponent(this.selectedAp.data.id).subscribe(() => {
                                    this.deletePart(this.selectedAp.data.id);
                                    this.notify.success(this.l('SuccessfullyDeleted'));
                                    this.selectedAp = null;
                                    this.reload();
                                });
                            }
                        }
                    );
                }
            };

            let itemDeleteItems =
            {
                label: this.l('Delete Items'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.message.confirm(
                        this.l('AssetPartDeleteWarningMessage', this.selectedAp.data.name),
                        this.l('AreYouSure'),
                        isConfirmed => {
                            if (isConfirmed) {
                                this._assetPartsService.deleteComponent(this.selectedAp.data.id).subscribe(() => {
                                    this.deletePart(this.selectedAp.data.id);
                                    this.notify.success(this.l('SuccessfullyDeleted'));
                                    this.selectedAp = null;
                                    this.reload();
                                });
                            }
                        }
                    );
                }
            };

        let itemDeleteBranch = 
            {
                label: this.l('Delete Branch'),
                disabled: !canManageAssetPartTree,
                command: () => {
                    this.message.confirm(
                        this.l('AssetPartDeleteWarningMessage', this.selectedAp.data.name),
                        this.l('AreYouSure'),
                        isConfirmed => {
                            if (isConfirmed) {
                                this._assetPartsService.deleteBranch(this.selectedAp.data.id).subscribe(() => {
                                    this.deletePart(this.selectedAp.data.id);
                                    this.notify.success(this.l('SuccessfullyDeleted'));
                                    this.selectedAp = null;
                                    this.reload();
                                });
                            }
                        }
                    );
                }
            };

        if(isItem){
            items.push(itemView);
            items.push(itemEdit);
            items.push(itemMoveToWarehouse);
            items.push(itemDeleteItems);
        }
        else{
            items.push(itemView);
            items.push(itemEdit);
            items.push(itemImportComponent);
            items.push(itemAddInventoryItem);
            items.push(itemMoveToWarehouse);
            items.push(itemMoveBranchToWarehouse);
            items.push(itemMoveToAsset);
            items.push(itemMoveBranchToAsset);
            items.push(itemDeleteComponent);
            items.push(itemDeleteBranch);
        }
        
        if (this.isEntityHistoryEnabled()) {
            items.push({
                label: this.l('History'),
                disabled: false,
                command: (event) => {
                    this.entityTypeHistoryModal.show({
                        entityId: this.selectedAp.data.id.toString(),
                        entityTypeFullName: this._entityTypeFullName,
                        entityTypeDescription: this.selectedAp.data.name
                    });
                }
            });
        }

        return items;
    }

    addPart(isItem: boolean, parentId?: number): void {
        this.createOrEditAssetPartModal.show({
            parentId: parentId,
            assetId: this.assetId,
            assetReference: this.assetReference,
            isItem: isItem
        });
    }

    importPart(parentId?: number): void {
        this.importAssetPartLookupTableModal.show(
            this.assetId,
            parentId
        );
    }
    
    partCreated(ap: AssetPartDto): void {

        if (ap.parentId) {
            let part = this._treeDataHelperService.findNode(this.treeData, { data: { id: ap.parentId } });
            if (!part) {
                return;
            }

            part.children.push({
                label: ap.name,
                expandedIcon: 'fa fa-folder-open m--font-warning',
                collapsedIcon: 'fa fa-folder m--font-warning',
                selected: true,
                children: [],
                data: ap
            });
        } else {
            this.treeData.push({
                label: ap.name,
                expandedIcon: 'fa fa-folder-open m--font-warning',
                collapsedIcon: 'fa fa-folder m--font-warning',
                selected: true,
                children: [],
                data: ap
            });
        }
        this.totalPartCount += 1;
    }

    deletePart(id) {
        let node = this._treeDataHelperService.findNode(this.treeData, { data: { id: id } });
        if (!node) {
            return;
        }

        if (!node.data.parentId) {
            _.remove(this.treeData, {
                data: {
                    id: id
                }
            });
        }

        let parentNode = this._treeDataHelperService.findNode(this.treeData, { data: { id: node.data.parentId } });
        if (!parentNode) {
            return;
        }

        _.remove(parentNode.children, {
            data: {
                id: id
            }
        });
    }

    partUpdated(ap: AssetPartExtendedDto): void {
        let item = this._treeDataHelperService.findNode(this.treeData, { data: { id: ap.id } });
        if (!item) {
            return;
        }

        item.data.name = ap.name;
        item.data.description = ap.description,
        item.data.serialNumber = ap.serialNumber,
        item.data.installDate = ap.installDate,
        item.data.installed = ap.installed,
        item.data.assetPartType = ap.assetPartType,
        item.data.assetPartStatus = ap.assetPartStatus,
        item.data.assetReference = ap.assetReference,
        item.data.itemType = ap.itemType,
        item.data.code = ap.code,
        item.label = ap.name;

        this.apUpdated.emit(ap);
    }

}
