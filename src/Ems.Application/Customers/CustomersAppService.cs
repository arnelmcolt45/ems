using Ems.Billing;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Customers.Exporting;
using Ems.Customers.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Ems.Assets;
using Ems.MultiTenancy;
using System;
using Abp.Extensions;
using Newtonsoft.Json;
using Ems.Utilities;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Token;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Xero.NetStandard.OAuth2.Api;
using Abp.UI;
using Ems.Organizations;

namespace Ems.Customers
{
    [AbpAuthorize(AppPermissions.Pages_Main_Customers)]
    public class CustomersAppService : EmsAppServiceBase, ICustomersAppService
    {
        private readonly string _entityType = "Customer";

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<LeaseAgreement> _leaseAgreementRepository;
        private readonly ICustomersExcelExporter _customersExcelExporter;
        private readonly IRepository<CustomerType, int> _lookup_customerTypeRepository;
        private readonly IRepository<Currency, int> _lookup_currencyRepository;
        private readonly IOptions<Xero.NetStandard.OAuth2.Config.XeroConfiguration> _xeroConfig;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepository<Ems.Security.XeroToken> _xeroToken;
        private readonly IRepository<XeroConfiguration, int> _xeroConfigurationRepository;

        public CustomersAppService(
            IRepository<Customer> customerRepository,
            IRepository<LeaseAgreement> leaseAgreementRepository,
            ICustomersExcelExporter customersExcelExporter,
            IRepository<CustomerType, int> lookup_customerTypeRepository,
            IRepository<Currency, int> lookup_currencyRepository,
            IOptions<Xero.NetStandard.OAuth2.Config.XeroConfiguration> xeroConfig,
            IRepository<Ems.Security.XeroToken> xeroToken,
            IHttpClientFactory httpClientFactory,
            IRepository<XeroConfiguration, int> xeroConfigurationRepository
            )
        {
            _leaseAgreementRepository = leaseAgreementRepository;
            _customerRepository = customerRepository;
            _customersExcelExporter = customersExcelExporter;
            _lookup_customerTypeRepository = lookup_customerTypeRepository;
            _lookup_currencyRepository = lookup_currencyRepository;
            _xeroConfig = xeroConfig;
            _xeroToken = xeroToken;
            _httpClientFactory = httpClientFactory;
            _xeroConfigurationRepository = xeroConfigurationRepository;
        }

        public async Task<PagedResultDto<GetCustomerForViewDto>> GetAll(GetAllCustomersInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);
            Dictionary<int, string> terms = new Dictionary<int, string>() { { 1, "Days" }, { 2, "Weeks" }, { 3, "Months" } };

            /*
            List<int?> myCustomerIds = new List<int?>();

            if (tenantInfo.Tenant.TenantType == "A")
            {
                myCustomerIds = _leaseAgreementRepository.GetAll()
                        .Where(e => e.AssetOwnerId == tenantInfo.AssetOwner.Id)
                        .Select(e => e.CustomerId)
                        .ToList();
            }
            */
            var filteredCustomers = _customerRepository.GetAll()
                        .Include(e => e.CustomerTypeFk)
                        .Include(e => e.CurrencyFk)
                        //.WhereIf(tenantInfo.Tenant.TenantType == "A", e => myCustomerIds.Contains(e.Id)) // Get all my Customers
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Identifier.Contains(input.Filter) || e.LogoUrl.Contains(input.Filter) || e.Website.Contains(input.Filter) || e.CustomerLoc8UUID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IdentifierFilter), e => e.Identifier.ToLower() == input.IdentifierFilter.ToLower().Trim())
                        //.WhereIf(input.PaymentTermNumberFilter != null, e => e.PaymentTermNumber == input.PaymentTermNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PaymentTermTypeFilter), e => e.PaymentTermType.ToLower() == input.PaymentTermTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerLoc8UUIDFilter), e => e.CustomerLoc8UUID.ToLower() == input.CustomerLoc8UUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerTypeTypeFilter), e => e.CustomerTypeFk != null && e.CustomerTypeFk.Type.ToLower() == input.CustomerTypeTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim());
            
            // Akshay - why, oh why did you do this???

            //Xero.NetStandard.OAuth2.Token.XeroOAuth2Token xeroToken = await GetToken();
            //if (xeroToken != null && !string.IsNullOrEmpty(xeroToken.AccessToken) && filteredCustomers.Count() > 0)
            //{
            //    //filteredCustomers.WhereIf(input.XeroCustomerFilter > -1, e => Convert.ToInt32(GetXeroContacts().Result.ToList().Where(item => item.Name.Trim().ToLower() == e.Name.ToLower()).FirstOrDefault() != null ? true : false) == input.XeroCustomerFilter);
            //}

            var pagedAndFilteredCustomers = filteredCustomers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var customers = from o in pagedAndFilteredCustomers
                            join o1 in _lookup_customerTypeRepository.GetAll() on o.CustomerTypeId equals o1.Id into j1
                            from s1 in j1.DefaultIfEmpty()

                            join o2 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o2.Id into j2
                            from s2 in j2.DefaultIfEmpty()

                            select new GetCustomerForViewDto()
                            {
                                Customer = new CustomerDto
                                {
                                    Reference = o.Reference,
                                    Name = o.Name,
                                    Identifier = o.Identifier,
                                    LogoUrl = o.LogoUrl,
                                    Website = o.Website,
                                    CustomerLoc8UUID = o.CustomerLoc8UUID,
                                    PaymentTermType = o.PaymentTermType,
                                    PaymentTermNumber = o.PaymentTermNumber,
                                    Id = o.Id,
                                    XeroContactId = o.XeroContactId

                                    // Akshay - why, why, why?
                                    //IsXeroContactSynced = false // xeroToken != null && !string.IsNullOrEmpty(xeroToken.AccessToken) ? (GetXeroContacts().Result.ToList().Where(item => item.Name.Trim().ToLower() == o.Name.ToLower()).FirstOrDefault() != null ? true : false) : false
                                    //IsXeroContactSynced = xeroToken != null && !string.IsNullOrEmpty(xeroToken.AccessToken) ? (GetXeroContacts().Result.ToList().Where(item => item.Name.Trim().ToLower() == o.Name.ToLower()).FirstOrDefault() != null ? true : false) : false
                                },
                                CustomerTypeType = s1 == null ? "" : s1.Type.ToString(),
                                CurrencyCode = s2 == null ? "" : s2.Code.ToString()
                            };

            var totalCount = await filteredCustomers.CountAsync();

            return new PagedResultDto<GetCustomerForViewDto>(
                totalCount,
                await customers.ToListAsync()
            );
        }

        public async Task<GetCustomerForViewDto> GetCustomerForView(int? id)
        {
            // START UPDATE // Add this code to enable the Customer Profile to work -- NB: made 'id' nullable and updated the Interface accordingly

            Customer currentCustomer;
            int customerId;

            if (id == null)
            {
                currentCustomer = _customerRepository.GetAll().Where(e => e.TenantId == AbpSession.TenantId).FirstOrDefault();
                if (currentCustomer == null)
                {
                    return new GetCustomerForViewDto();
                }
                customerId = currentCustomer.Id;
            }
            else
            {
                customerId = (int)id;
            }

            var customer = await _customerRepository.GetAsync(customerId);

            // END UPDATE //

            var output = new GetCustomerForViewDto { Customer = ObjectMapper.Map<CustomerDto>(customer) };

            if (output.Customer != null)
            {
                var _lookupCustomerType = await _lookup_customerTypeRepository.FirstOrDefaultAsync((int)output.Customer.CustomerTypeId);
                output.CustomerTypeType = _lookupCustomerType.Type.ToString();
            }

            if (output.Customer.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.Customer.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }

            if (!string.IsNullOrWhiteSpace(output.Customer.LogoUrl))
            {
                int length = (output.Customer.LogoUrl.Length >= 36) ? 36 : output.Customer.LogoUrl.Length;
                output.Customer.LogoUrl = string.Format("{0}...", output.Customer.LogoUrl.Substring(0, 36));
            }
            if (!string.IsNullOrWhiteSpace(output.Customer.Website))
            {
                int length = (output.Customer.Website.Length >= 36) ? 36 : output.Customer.Website.Length;
                output.Customer.Website = string.Format("{0}...", output.Customer.Website.Substring(0, 36));
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers_Edit)]
        public async Task<GetCustomerForEditOutput> GetCustomerForEdit(EntityDto input)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCustomerForEditOutput { Customer = ObjectMapper.Map<CreateOrEditCustomerDto>(customer) };

            if (output.Customer != null)
            {
                var _lookupCustomerType = await _lookup_customerTypeRepository.FirstOrDefaultAsync((int)output.Customer.CustomerTypeId);
                output.CustomerTypeType = _lookupCustomerType.Type.ToString();
            }

            if (output.Customer.CurrencyId > 0)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.Customer.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCustomerDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers_Create)]
        protected virtual async Task Create(CreateOrEditCustomerDto input)
        {
            var customer = ObjectMapper.Map<Customer>(input);


            if (AbpSession.TenantId != null)
            {
                customer.TenantId = (int?)AbpSession.TenantId;
            }


            await _customerRepository.InsertAsync(customer);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers_Edit)]
        protected virtual async Task Update(CreateOrEditCustomerDto input)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, customer);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers_Delete)]
        public async Task Delete(EntityDto input)
        {
            Customer customer = await _customerRepository.GetAsync(input.Id);
            if (customer != null && customer.TenantId.HasValue)
            {
                Tenant tenant = await TenantManager.GetByIdAsync(customer.TenantId.Value);
                tenant.IsActive = false;
                await TenantManager.UpdateAsync(tenant);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            await _customerRepository.DeleteAsync(customer);
        }

        public async Task<FileDto> GetCustomersToExcel(GetAllCustomersForExcelInput input)
        {

            var filteredCustomers = _customerRepository.GetAll()
                        .Include(e => e.CustomerTypeFk)
                        .Include(e => e.CurrencyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Identifier.Contains(input.Filter) || e.LogoUrl.Contains(input.Filter) || e.Website.Contains(input.Filter) || e.CustomerLoc8UUID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IdentifierFilter), e => e.Identifier.ToLower() == input.IdentifierFilter.ToLower().Trim())
                        .WhereIf(input.PaymentTermNumberFilter != null, e => e.PaymentTermNumber == input.PaymentTermNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PaymentTermTypeFilter), e => e.PaymentTermType.ToLower() == input.PaymentTermTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerLoc8UUIDFilter), e => e.CustomerLoc8UUID.ToLower() == input.CustomerLoc8UUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerTypeTypeFilter), e => e.CustomerTypeFk != null && e.CustomerTypeFk.Type.ToLower() == input.CustomerTypeTypeFilter.ToLower().Trim())
                        //.WhereIf(input.XeroCustomerFilter > -1, e => Convert.ToInt32(GetXeroContacts().Result.ToList().Where(item => item.Name.Trim().ToLower() == e.Name.ToLower()).FirstOrDefault() != null ? true : false) == input.XeroCustomerFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim());
            
            var query = (from o in filteredCustomers
                         join o1 in _lookup_customerTypeRepository.GetAll() on o.CustomerTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         select new GetCustomerForViewDto()
                         {
                             Customer = new CustomerDto
                             {
                                 Reference = o.Reference,
                                 Name = o.Name,
                                 Identifier = o.Identifier,
                                 LogoUrl = o.LogoUrl,
                                 Website = o.Website,
                                 PaymentTermNumber = o.PaymentTermNumber,
                                 PaymentTermType = o.PaymentTermType,
                                 CustomerLoc8UUID = o.CustomerLoc8UUID,
                                 Id = o.Id,
                                 XeroContactId = o.XeroContactId
                                 //FIX THIS CRAP :::::::::: IsXeroContactSynced = GetXeroContacts().Result.ToList().Where(item => item.Name.Trim().ToLower() == o.Name.ToLower()).FirstOrDefault() != null ? true : false
                             },
                             CustomerTypeType = s1 == null ? "" : s1.Type.ToString(),
                             CurrencyCode = s2 == null ? "" : s2.Code.ToString()
                         });


            var customerListDtos = await query.ToListAsync();

            return _customersExcelExporter.ExportToFile(customerListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers)]
        public async Task<string> GetDefaultCurrency()
        {
            string currencyDtoJSON = string.Empty;
            List<Currency> currencies = await _lookup_currencyRepository.GetAll().ToListAsync();
            if (currencies != null && currencies.Count > 0)
            {
                Currency currency = currencies.Where(x => x.Code.ToLower() == AppConsts.DefaultCurrency).FirstOrDefault();
                currencyDtoJSON = JsonConvert.SerializeObject(currency);
            }
            return currencyDtoJSON;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers)]
        public async Task<PagedResultDto<CustomerCustomerTypeLookupTableDto>> GetAllCustomerTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_customerTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var customerTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<CustomerCustomerTypeLookupTableDto>();
            foreach (var customerType in customerTypeList)
            {
                lookupTableDtoList.Add(new CustomerCustomerTypeLookupTableDto
                {
                    Id = customerType.Id,
                    DisplayName = customerType.Type?.ToString()
                });
            }

            return new PagedResultDto<CustomerCustomerTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers)]
        public async Task<PagedResultDto<CustomerCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_currencyRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Code.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var currencyList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<CustomerCurrencyLookupTableDto>();
            foreach (var currency in currencyList)
            {
                lookupTableDtoList.Add(new CustomerCurrencyLookupTableDto
                {
                    Id = currency.Id,
                    DisplayName = currency.Code?.ToString()
                });
            }

            return new PagedResultDto<CustomerCurrencyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers)]
        public PagedResultDto<CustomerPaymentTermsTypeLookupTableDto> GetAllPaymentTermsTypeForLookupTable(GetAllForLookupTableInput input)
        {

            Dictionary<int, string> terms = new Dictionary<int, string>() { { 1, AppConsts.PaymentTermTypeDay }, { 2, AppConsts.PaymentTermTypeWeek }, { 3, AppConsts.PaymentTermTypeMonth } };

            var query = terms.AsQueryable().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Value.ToString().Contains(input.Filter)
               );

            var totalCount = query.Count();

            var termsList = query
                .PageBy(input)
                .ToList();

            var lookupTableDtoList = new List<CustomerPaymentTermsTypeLookupTableDto>();
            foreach (var term in termsList)
            {
                lookupTableDtoList.Add(new CustomerPaymentTermsTypeLookupTableDto
                {
                    Id = term.Key,
                    DisplayName = term.Value?.ToString()
                });
            }

            return new PagedResultDto<CustomerPaymentTermsTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );

        }

        #region Get Customer Payment Due's 
        
        // TODO: This is rubbish - needs to take an "effective date" parameter instead of just using DateTime.Now()
        
        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_Edit)]
        public async Task<CustomerForCustomerInvoiceEditDto> GetCustomerPaymentDue(int id)
        {
            Customer customer = await _customerRepository.GetAsync(id);

            CustomerForCustomerInvoiceEditDto customerForCustomerInvoiceEditDto = new CustomerForCustomerInvoiceEditDto();

            if (customer != null)
            {
                if (customer.CurrencyId > 0)
                {
                    customerForCustomerInvoiceEditDto.CurrencyId = customer.CurrencyId;

                    customerForCustomerInvoiceEditDto.CurrencyCode = _lookup_currencyRepository.FirstOrDefault(customer.CurrencyId).Code;
                }
                if (!string.IsNullOrEmpty(customer.PaymentTermType))
                {
                    if (customer.PaymentTermType.ToLower() == AppConsts.PaymentTermTypeDay.ToLower())
                    {
                        customer.PaymentTermNumber = customer.PaymentTermNumber;
                        customerForCustomerInvoiceEditDto.DueDate = DateTime.Now.AddDays(Convert.ToInt32(customer.PaymentTermNumber)).ToString();
                    }
                    else if (customer.PaymentTermType.ToLower() == AppConsts.PaymentTermTypeMonth.ToLower())
                    {
                        customer.PaymentTermNumber = customer.PaymentTermNumber;
                        customerForCustomerInvoiceEditDto.DueDate = DateTime.Now.AddMonths(Convert.ToInt32(customer.PaymentTermNumber)).ToString();
                    }
                    else if (customer.PaymentTermType.ToLower() == AppConsts.PaymentTermTypeWeek.ToLower())
                    {
                        customer.PaymentTermNumber = customer.PaymentTermNumber;
                        customerForCustomerInvoiceEditDto.DueDate = DateTime.Now.AddDays(Convert.ToInt32(customer.PaymentTermNumber) * 7).ToString();
                    }
                }
                else
                {
                    customerForCustomerInvoiceEditDto.DueDate = "";
                }
            }
            return customerForCustomerInvoiceEditDto;
        }
        #endregion


        #region Xero Integration

        private async Task<Xero.NetStandard.OAuth2.Token.XeroOAuth2Token> GetToken()
        {
            var token = _xeroToken.GetAll().Where(t => t.TenantId == AbpSession.TenantId).FirstOrDefault();
            if (token != null)
            {
                XeroOAuth2Token xeroToken = JsonConvert.DeserializeObject<XeroOAuth2Token>(token.Token);
                var utcTimeNow = DateTime.UtcNow;

                if (utcTimeNow > xeroToken.ExpiresAtUtc)
                {
                    var client = new XeroClient(_xeroConfig.Value, _httpClientFactory);
                    xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);

                    _xeroToken.Delete(token);
                    _xeroToken.Insert(new Security.XeroToken { Token = JsonConvert.SerializeObject(xeroToken), TenantId = AbpSession.TenantId });
                    
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                return xeroToken;
            }
            else
            {
                return null;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers)]
        public async Task<string> CheckIfUserXeroLoggedIn()
        {
            Xero.NetStandard.OAuth2.Token.XeroOAuth2Token xeroToken = await GetToken();

            if (xeroToken != null && !string.IsNullOrEmpty(xeroToken.AccessToken))
            {
                //var client = new XeroClient(_xeroConfig.Value, _httpClientFactory);
                var client = GetXeroClient(GetXeroConfig());
                return client.BuildLoginUri().ToString();
            }
            return string.Empty;
        }

        private XeroClient GetXeroClient(Ems.Billing.XeroConfiguration xeroConfiguration)
        {

            //var xeroConfiguration = GetXeroConfig();
            Xero.NetStandard.OAuth2.Config.XeroConfiguration xeroConfig = new Xero.NetStandard.OAuth2.Config.XeroConfiguration()
            {
                CallbackUri = new Uri(xeroConfiguration.CallbackUri),
                ClientId = xeroConfiguration.ClientId,
                ClientSecret = xeroConfiguration.ClientSecret,
                State = xeroConfiguration.State,
                Scope = xeroConfiguration.Scope
            };

            XeroClient client = new XeroClient(xeroConfig, _httpClientFactory);
            return client;
        }

        private Ems.Billing.XeroConfiguration GetXeroConfig()
        {
            XeroConfiguration xeroConfiguration = _xeroConfigurationRepository.GetAll().Where(X => X.TenantId == AbpSession.TenantId).FirstOrDefault();
            return xeroConfiguration;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Customers)]
        public async Task<string> SyncXeroCustomers()
        {
            string loginUri = await CheckIfUserXeroLoggedIn();

            var xeroContacts = await GetXeroContacts();
            var xeroContactNames = xeroContacts.Select(x => x.Name).ToList();

            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            List<Customer> customers = await _customerRepository.GetAll()
                        .Include(e => e.CustomerTypeFk)
                        .Include(e => e.CurrencyFk)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .ToListAsync();

            // Check that all customer names are unique - if not, show the user the problem!
            var duplicateCheck = customers.GroupBy(x => x.Name)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if(duplicateCheck.Count > 0)
            {
                string duplicates = string.Join(",", duplicateCheck);
                throw new UserFriendlyException("We have a problem!", string.Format( "The following Customer(s) have duplicate Names in the database: {0}", duplicates));
            }

            foreach (var customer in customers)
            {
                if(customer.XeroContactId == null || customer.XeroContactId == "")
                {
                    Xero.NetStandard.OAuth2.Model.Contact contact = xeroContacts.Where(x => x.Name == customer.Name).FirstOrDefault();
                    
                    // Is there a XeroContact with the same name as the Customer?
                        
                    if(contact != null)
                    {
                        // YES: Update this Customer with the corresponding XeroContactId and other details
                        UpdateCustomerWithXeroContactDetails(customer, contact, true);  // TODO: Add th Nulls Overight option to the UI
                    }
                    else
                    {
                        //  NO: Create a XeroContact for this Customer and add it's XeroContactId to the Customer
                        Xero.NetStandard.OAuth2.Model.Contact newContact = new Xero.NetStandard.OAuth2.Model.Contact() { Name = customer.Name };

                        var newContactId = await CreateXeroContact(newContact);

                        var customerToUpdate = _customerRepository.GetAll().Where(c => c.Id == customer.Id).FirstOrDefault();

                        customerToUpdate.XeroContactId = newContactId;
                        _customerRepository.Update(customerToUpdate);
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }

                }
                else
                {
                    // Get the latest data from Xero and update the Customer

                    Xero.NetStandard.OAuth2.Model.Contact contact = xeroContacts.Where(x => x.ContactID.ToString() == customer.XeroContactId).FirstOrDefault();
                    if(contact != null)
                    {
                        var result = await UpdateCustomerWithXeroContactDetails(customer, contact, true); // TODO: Add th Nulls Overight option to the UI
                    }
                    else
                    {
                        throw new UserFriendlyException("We have a problem!", string.Format("Cannot find a Contact with ID '{0}' in Xero for Customer {1}", customer.XeroContactId, customer.Name));
                    }
                }

            }

            // Don't do this yet.  However, there's always a chance that David will change his mind at some stage :-)
            /*
            foreach (var xeroContact in xeroContacts)
            {
                // Is this XeroContact represented by a Customer in our Customers DB?
                    // YES: Great!
                    //  NO: What should we do?  Maybe we don't want them all imported!  Do we want to see a list of these additional XeroContacts here?
            }
            */

            return loginUri;
        }

        private async Task<Customer> UpdateCustomerWithXeroContactDetails(Customer customerDto, Xero.NetStandard.OAuth2.Model.Contact contact, bool OverwriteWithNulls)
        {
            var customer = _customerRepository.GetAll().Where(c => c.Id == customerDto.Id).FirstOrDefault();

            customer.XeroContactId = contact.ContactID.ToString();
            customer.Name = contact.Name;
            customer.XeroDefaultCurrency = contact.DefaultCurrency.ToString();

            if (OverwriteWithNulls)
            {
                customer.Website = contact.Website;
                customer.EmailAddress = contact.EmailAddress;
                customer.XeroAccountsPayableTaxType = contact.AccountsPayableTaxType;
                customer.XeroAccountsReceivableTaxType = contact.AccountsReceivableTaxType;
                customer.XeroAddresses = (contact.Addresses != null && contact.Addresses.Count() > 0) ? contact.Addresses[0].ToString() : null;  //TODO: concatonate the collection
                customer.XeroContactPersons = customer.XeroContactPersons = (contact.ContactPersons != null && contact.ContactPersons.Count() > 0) ? contact.ContactPersons[0].ToString() : null; //TODO: concatonate the collection
                customer.XeroPaymentTerms = (contact.PaymentTerms != null) ? contact.PaymentTerms.ToString() : null; 
                customer.XeroPhones = (contact.Phones != null && contact.Phones.Count() > 0) ? contact.Phones[0].ToString() : null; //TODO: concatonate the collection
            }
            else
            {
                customer.Website = (contact.Website != null || contact.Website != "") ? contact.Website : customer.Website;
                customer.EmailAddress = (contact.EmailAddress != null || contact.EmailAddress != "") ? contact.EmailAddress : customer.EmailAddress;
                customer.XeroAccountsPayableTaxType = (contact.AccountsPayableTaxType != null || contact.AccountsPayableTaxType != "") ? contact.AccountsPayableTaxType : customer.XeroAccountsPayableTaxType;
                customer.XeroAccountsReceivableTaxType = (contact.AccountsReceivableTaxType != null || contact.AccountsReceivableTaxType != "") ? contact.AccountsReceivableTaxType : customer.XeroAccountsReceivableTaxType;
                customer.XeroAddresses = (contact.Addresses != null || contact.Addresses.Count() == 0) ? contact.Addresses.ToString() : customer.XeroAddresses;
                customer.XeroContactPersons = (contact.ContactPersons != null || contact.ContactPersons.Count() == 0) ? contact.ContactPersons.ToString() : customer.XeroContactPersons;
                customer.XeroPaymentTerms = (contact.PaymentTerms != null ) ? contact.PaymentTerms.ToString() : customer.XeroPaymentTerms;
                customer.XeroPhones = (contact.Phones != null || contact.Phones.Count() == 0) ? contact.Phones.ToString() : customer.XeroPhones;
            }

            await _customerRepository.UpdateAsync(customer);
            await CurrentUnitOfWork.SaveChangesAsync();

            return customer;
        }

        private async Task<List<Xero.NetStandard.OAuth2.Model.Contact>> GetXeroContacts()
        {
            List<Xero.NetStandard.OAuth2.Model.Contact> contacts = new List<Xero.NetStandard.OAuth2.Model.Contact>();
            Xero.NetStandard.OAuth2.Token.XeroOAuth2Token xeroToken = await GetToken();
            if (xeroToken != null && !string.IsNullOrEmpty(xeroToken.AccessToken))
            {
                string accessToken = xeroToken.AccessToken;

                Ems.Billing.XeroConfiguration xeroConfiguration = _xeroConfigurationRepository.GetAll().Where(X => X.TenantId == AbpSession.TenantId).FirstOrDefault();
                string xeroTenantId = xeroConfiguration.XeroTenantId;
                
                var accountingApi = new AccountingApi();
                var contactsResponse = await accountingApi.GetContactsAsync(accessToken, xeroTenantId);
                if (contactsResponse._Contacts != null && contactsResponse._Contacts.Count > 0)
                {
                    contacts = contactsResponse._Contacts;
                }
            }
            return contacts;
        }

        private async Task<string> CreateXeroContact(Xero.NetStandard.OAuth2.Model.Contact contact)
        {
            Xero.NetStandard.OAuth2.Model.Contact newContact = new Xero.NetStandard.OAuth2.Model.Contact();
            Xero.NetStandard.OAuth2.Token.XeroOAuth2Token xeroToken = await GetToken();
            if (xeroToken != null && !string.IsNullOrEmpty(xeroToken.AccessToken))
            {
                string accessToken = xeroToken.AccessToken;

                Ems.Billing.XeroConfiguration xeroConfiguration = _xeroConfigurationRepository.GetAll().Where(X => X.TenantId == AbpSession.TenantId).FirstOrDefault();
                string xeroTenantId = xeroConfiguration.XeroTenantId;

                var accountingApi = new AccountingApi();
                var response = await accountingApi.CreateContactAsync(accessToken, xeroTenantId, contact);
                if (response._Contacts != null && response._Contacts.Count > 0)
                {
                    newContact = response._Contacts.FirstOrDefault();
                }
            }
            return newContact.ContactID.ToString();
        }

        #endregion
    }
}