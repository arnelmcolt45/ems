import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { CustomersServiceProxy, CreateOrEditCustomerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { CustomerCustomerTypeLookupTableModalComponent } from './customer-customerType-lookup-table-modal.component';
import { CustomerCurrencyLookupTableModalComponent } from './customer-currency-lookup-table-modal.component';
import { CustomerPaymenttermsLookupTableModalComponent } from './customer-paymentterms-lookup-table-modal.component';


@Component({
    selector: 'editCustomerModal',
    templateUrl: './edit-customer-modal.component.html'
})
export class EditCustomerModalComponent extends AppComponentBase {

    @ViewChild('editModal', { static: true }) modal: ModalDirective;
    @ViewChild('customerCustomerTypeLookupTableModal', { static: true }) customerCustomerTypeLookupTableModal: CustomerCustomerTypeLookupTableModalComponent;
    @ViewChild('customerCurrencyLookupTableModal', { static: true }) customerCurrencyLookupTableModal: CustomerCurrencyLookupTableModalComponent;
    @ViewChild('customerCustomerPaymentTermsTypeLookupTableModal', { static: true }) customerCustomerPaymentTermsTypeLookupTableModal: CustomerPaymenttermsLookupTableModalComponent;

    @Output() customerModalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    customer: CreateOrEditCustomerDto = new CreateOrEditCustomerDto();

    customerTypeType = '';
    currencyCode = '';
    isFormValid = false;
    errorMsg = "";
    customerPaymentTermsType = '';

    constructor(
        injector: Injector,
        private _customersServiceProxy: CustomersServiceProxy
    ) {
        super(injector);
    }

    show(customerId?: number): void {
        console.log("xxxxxxxxxxxx CUSTOMER ID IS " + customerId);
        if (!customerId) {
            this.customer = new CreateOrEditCustomerDto();
            this.customer.id = customerId;
            this.customerTypeType = '';
            this.currencyCode = '';
            this.customerPaymentTermsType = '';
            this.active = true;
            this.modal.show();
        } else {
            this._customersServiceProxy.getCustomerForEdit(customerId).subscribe(result => {
                this.customer = result.customer;

                this.customerTypeType = result.customerTypeType;
                this.currencyCode = result.currencyCode;
                this.customerPaymentTermsType = result.customer.paymentTermType;
                this.active = true;
                this.modal.show();
            });
        }
    }

    save(): void {
        if (!this.customer.customerTypeId || !this.customer.reference ||
            !this.customer.name || !this.customer.identifier) {
            this.isFormValid = false;
            this.errorMsg = "Fill all the required fields (*)";
        }
        else
            this.isFormValid = true;

        if (this.isFormValid) {
            this.saving = true;

            this._customersServiceProxy.createOrEdit(this.customer)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                    this.close();
                    this.customerModalSave.emit(null);
                });
        }
        else
            this.message.info(this.errorMsg, this.l('Invalid'));
    }

    openSelectCustomerTypeModal() {
        this.customerCustomerTypeLookupTableModal.id = this.customer.customerTypeId;
        this.customerCustomerTypeLookupTableModal.displayName = this.customerTypeType;
        this.customerCustomerTypeLookupTableModal.show();
    }

    openSelectCustomerPaymentTermsTypeModal() {
        this.customerCustomerPaymentTermsTypeLookupTableModal.displayName = this.customerPaymentTermsType;
        this.customerCustomerPaymentTermsTypeLookupTableModal.show();
    }

    openSelectCurrencyModal() {
        this.customerCurrencyLookupTableModal.id = this.customer.currencyId;
        this.customerCurrencyLookupTableModal.displayName = this.currencyCode;
        this.customerCurrencyLookupTableModal.show();
    }


    setCustomerTypeIdNull() {
        this.customer.customerTypeId = null;
        this.customerTypeType = '';
    }
    setCurrencyIdNull() {
        this.customer.currencyId = null;
        this.currencyCode = '';
    }

    setPaymentTermsTypeNull() {
        this.customer.paymentTermType = this.customerPaymentTermsType = '';
    }

    getNewCustomerTypeId() {
        this.customer.customerTypeId = this.customerCustomerTypeLookupTableModal.id;
        this.customerTypeType = this.customerCustomerTypeLookupTableModal.displayName;
    }
    getNewCurrencyId() {
        this.customer.currencyId = this.customerCurrencyLookupTableModal.id;
        this.currencyCode = this.customerCurrencyLookupTableModal.displayName;
    }

    getNewPaymentTermsType() {
        this.customer.paymentTermType = this.customerPaymentTermsType = this.customerCustomerPaymentTermsTypeLookupTableModal.displayName;
    }


    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
