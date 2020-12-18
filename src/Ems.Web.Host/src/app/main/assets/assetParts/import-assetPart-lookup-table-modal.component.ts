import { Component, ViewChild, Injector, Output, EventEmitter, ViewEncapsulation} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import {AssetPartsServiceProxy, AssetPartAssetPartLookupTableDto, MoveAssetPartToAssetInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { partial } from 'lodash';
@Component({
    selector: 'importAssetPartLookupTableModal',
    styleUrls: ['./import-assetPart-lookup-table-modal.component.less'],
    encapsulation: ViewEncapsulation.None,
    templateUrl: './import-assetPart-lookup-table-modal.component.html'
})
export class ImportAssetPartLookupTableModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    filterText: string;
    displayName: string;
    assetPartId: number;
    scope: string;
    assetId: number; 
    warehouseId: number;
    assetPartParentId: number; 
    forImportFromWarehouses = true;

    @Output() assetPartImported: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    constructor(
        injector: Injector,
        private _assetPartsServiceProxy: AssetPartsServiceProxy
    ) {
        super(injector);
    }

    show(assetId: number, parentId?: number): void {
        
        this.assetId = assetId;
        this.assetPartParentId = parentId;
        this.active = true;
        this.paginator.rows = 5;
        this.getAll();
        this.modal.show();
    }

    getAll(event?: LazyLoadEvent) {
        if (!this.active) {
            return;
        }

        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._assetPartsServiceProxy.getAllAssetPartForLookupTable(
            this.filterText,
            this.warehouseId,
            this.assetId,
            this.forImportFromWarehouses,
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

    setAndSave(assetPart: AssetPartAssetPartLookupTableDto) {

        this.message.confirm(
            this.l('ImportPartWarningMessage', assetPart.displayName),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    const requestInput = new MoveAssetPartToAssetInput();
                    requestInput.newAssetPartParentId = this.assetPartParentId;
                    requestInput.assetPartId = assetPart.id;
                    requestInput.newAssetId = this.assetId;
                    requestInput.importAssetPart = true;

                        this._assetPartsServiceProxy.moveAssetPartToAsset(requestInput).subscribe(result => {
                            this.notify.info(this.l('ImportedSuccessfully'));
                            this.close();
                        });

                }
                this.active = false;
                this.modal.hide();
            }
        );
    }

    close(): void {
        this.active = false;
        this.modal.hide();
        this.assetPartImported.emit(null);
    }
}
