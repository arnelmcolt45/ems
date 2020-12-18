import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CurrenciesServiceProxy, CurrencyDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCurrencyModalComponent } from './create-or-edit-currency-modal.component';
import { ViewCurrencyModalComponent } from './view-currency-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './currencies.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class CurrenciesComponent extends AppComponentBase {

    @ViewChild('createOrEditCurrencyModal', { static: true }) createOrEditCurrencyModal: CreateOrEditCurrencyModalComponent;
    @ViewChild('viewCurrencyModalComponent', { static: true }) viewCurrencyModal: ViewCurrencyModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    codeFilter = '';
    nameFilter = '';
    symbolFilter = '';
    countryFilter = '';
    baseCountryFilter = '';




    constructor(
        injector: Injector,
        private _currenciesServiceProxy: CurrenciesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getCurrencies(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._currenciesServiceProxy.getAll(
            this.filterText,
            this.codeFilter,
            this.nameFilter,
            this.symbolFilter,
            this.countryFilter,
            this.baseCountryFilter,
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

    createCurrency(): void {
        this.createOrEditCurrencyModal.show();
    }

    deleteCurrency(currency: CurrencyDto): void {
        this.message.confirm(
            '',
            (isConfirmed) => {
                if (isConfirmed) {
                    this._currenciesServiceProxy.delete(currency.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._currenciesServiceProxy.getCurrenciesToExcel(
        this.filterText,
            this.codeFilter,
            this.nameFilter,
            this.symbolFilter,
            this.countryFilter,
            this.baseCountryFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
