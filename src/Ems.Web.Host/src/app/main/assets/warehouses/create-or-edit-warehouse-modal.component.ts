import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { WarehousesServiceProxy, CreateOrEditWarehouseDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditWarehouseModal',
    templateUrl: './create-or-edit-warehouse-modal.component.html'
})
export class CreateOrEditWarehouseModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    warehouse: CreateOrEditWarehouseDto = new CreateOrEditWarehouseDto();



    constructor(
        injector: Injector,
        private _warehousesServiceProxy: WarehousesServiceProxy
    ) {
        super(injector);
    }

    show(warehouseId?: number): void {

        if (!warehouseId) {
            this.warehouse = new CreateOrEditWarehouseDto();
            this.warehouse.id = warehouseId;

            this.active = true;
            this.modal.show();
        } else {
            this._warehousesServiceProxy.getWarehouseForEdit(warehouseId).subscribe(result => {
                this.warehouse = result.warehouse;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._warehousesServiceProxy.createOrEdit(this.warehouse)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
