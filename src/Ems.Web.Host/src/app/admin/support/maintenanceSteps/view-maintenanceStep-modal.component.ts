import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetMaintenanceStepForViewDto, MaintenanceStepDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewMaintenanceStepModal',
    templateUrl: './view-maintenanceStep-modal.component.html'
})
export class ViewMaintenanceStepModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetMaintenanceStepForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetMaintenanceStepForViewDto();
        this.item.maintenanceStep = new MaintenanceStepDto();
    }

    show(item: GetMaintenanceStepForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
