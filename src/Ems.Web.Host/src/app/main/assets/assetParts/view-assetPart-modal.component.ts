﻿import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetAssetPartForViewDto, AssetPartDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewAssetPartModal',
    templateUrl: './view-assetPart-modal.component.html'
})
export class ViewAssetPartModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetAssetPartForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetAssetPartForViewDto();
        this.item.assetPart = new AssetPartDto();
    }

    show(item: GetAssetPartForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
