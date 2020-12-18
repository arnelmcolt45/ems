import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { LocationsServiceProxy, CreateOrEditLocationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditLocationModal',
    templateUrl: './create-or-edit-location-modal.component.html'
})
export class CreateOrEditLocationModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    location: CreateOrEditLocationDto = new CreateOrEditLocationDto();

    userName = '';


    constructor(
        injector: Injector,
        private _locationsServiceProxy: LocationsServiceProxy
    ) {
        super(injector);
    }
    
    show(locationId?: number): void {
    

        if (!locationId) {
            this.location = new CreateOrEditLocationDto();
            this.location.id = locationId;
            this.userName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._locationsServiceProxy.getLocationForEdit(locationId).subscribe(result => {
                this.location = result.location;

                this.userName = result.userName;

                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._locationsServiceProxy.createOrEdit(this.location)
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
