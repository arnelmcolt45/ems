import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetWarehouseForViewDto, WarehouseDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewWarehouseModal',
    templateUrl: './view-warehouse-modal.component.html'
})
export class ViewWarehouseModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetWarehouseForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetWarehouseForViewDto();
        this.item.warehouse = new WarehouseDto();
    }

    show(item: GetWarehouseForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
