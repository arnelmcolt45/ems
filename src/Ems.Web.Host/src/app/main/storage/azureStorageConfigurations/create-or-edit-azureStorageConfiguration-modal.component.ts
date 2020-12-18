import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { AzureStorageConfigurationsServiceProxy, CreateOrEditAzureStorageConfigurationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditAzureStorageConfigurationModal',
    templateUrl: './create-or-edit-azureStorageConfiguration-modal.component.html'
})
export class CreateOrEditAzureStorageConfigurationModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    azureStorageConfiguration: CreateOrEditAzureStorageConfigurationDto = new CreateOrEditAzureStorageConfigurationDto();



    constructor(
        injector: Injector,
        private _azureStorageConfigurationsServiceProxy: AzureStorageConfigurationsServiceProxy
    ) {
        super(injector);
    }
    
    show(azureStorageConfigurationId?: number): void {
    

        if (!azureStorageConfigurationId) {
            this.azureStorageConfiguration = new CreateOrEditAzureStorageConfigurationDto();
            this.azureStorageConfiguration.id = azureStorageConfigurationId;

            this.active = true;
            this.modal.show();
        } else {
            this._azureStorageConfigurationsServiceProxy.getAzureStorageConfigurationForEdit(azureStorageConfigurationId).subscribe(result => {
                this.azureStorageConfiguration = result.azureStorageConfiguration;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._azureStorageConfigurationsServiceProxy.createOrEdit(this.azureStorageConfiguration)
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
