import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetMaintenancePlanForViewDto, MaintenancePlanDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewMaintenancePlanModal',
    templateUrl: './view-maintenancePlan-modal.component.html'
})
export class ViewMaintenancePlanModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetMaintenancePlanForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetMaintenancePlanForViewDto();
        this.item.maintenancePlan = new MaintenancePlanDto();
    }

    show(item: GetMaintenancePlanForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
