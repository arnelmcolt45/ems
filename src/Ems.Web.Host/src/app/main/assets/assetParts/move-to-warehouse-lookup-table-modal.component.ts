import { Component, ViewChild, Injector, Output, EventEmitter, ViewEncapsulation} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AssetPartsServiceProxy, AssetPartWarehouseLookupTableDto, MoveAssetPartToWarehouseInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
@Component({
    selector: 'moveToWarehouseLookupTableModal',
    styleUrls: ['./move-to-warehouse-lookup-table-modal.component.less'],
    encapsulation: ViewEncapsulation.None,
    templateUrl: './move-to-warehouse-lookup-table-modal.component.html'
})
export class MoveToWarehouseLookupTableModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    filterText = '';
    displayName: string;
    assetPartId: number;
    warehouseId: number;
    scope: string;

    @Output() assetPartMovedToWarehouse: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    constructor(
        injector: Injector,
        private _assetPartsServiceProxy: AssetPartsServiceProxy
    ) {
        super(injector);
    }

    show(assetPartId: number, scope: string): void {
        this.scope = scope;
        this.assetPartId = assetPartId;
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

        this._assetPartsServiceProxy.getAllWarehouseForLookupTable(
            this.filterText,
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

    setAndSave(warehouse: AssetPartWarehouseLookupTableDto) {
        this.message.confirm(
            this.l('MovePartDeleteWarningMessage', warehouse.displayName),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    const requestInput = new MoveAssetPartToWarehouseInput();
                    requestInput.assetPartId = this.assetPartId;
                    requestInput.newWarehouseId = warehouse.id;
                    
                    if(this.scope == 'component'){
                        this._assetPartsServiceProxy.moveAssetPartToWarehouse(requestInput).subscribe(result => {
                            this.notify.info(this.l('MovedSuccessfully'));
                            this.close();
                        });
                    }
                    else if(this.scope == 'branch'){
                        this._assetPartsServiceProxy.moveBranchToWarehouse(requestInput).subscribe(result => {
                            this.notify.info(this.l('MovedSuccessfully'));
                            this.close();
                        });
                    }
                }
                this.active = false;
                this.modal.hide();
            }
        );
    }

    close(): void {
        this.active = false;
        this.modal.hide();
        this.assetPartMovedToWarehouse.emit(null);
    }
}
