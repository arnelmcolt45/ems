import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAzureStorageConfigurationForViewDto, AzureStorageConfigurationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAzureStorageConfigurationModal',
    templateUrl: './view-azureStorageConfiguration-modal.component.html'
})
export class ViewAzureStorageConfigurationModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAzureStorageConfigurationForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAzureStorageConfigurationForViewDto();
        this.item.azureStorageConfiguration = new AzureStorageConfigurationDto();
    }

    show(item: GetAzureStorageConfigurationForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
