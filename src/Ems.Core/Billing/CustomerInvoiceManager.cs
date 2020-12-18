using Abp.Dependency;
using Abp.Domain.Services;
using System;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Ems.Support;
using Ems.Assets;
using Ems.Quotations;
using Ems.MultiTenancy;
using Abp.Domain.Uow;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using Abp.BackgroundJobs;
using Ems.Support.Dtos;
using Ems.Assets.Dtos;
using Ems.Quotations.Dtos;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ems.Vendors;
using Ems.Customers;
using Abp.Runtime.Caching;
using Ems.Billing.Dtos;
using Ems.Telematics;
using System.Runtime.CompilerServices;
using Ems.Metrics;
using Org.BouncyCastle.Bcpg.Sig;
using System.DirectoryServices.Protocols;
using Ems.Organizations;

namespace Ems.Billing
{
    public class CustomerInvoiceManager : BackgroundJob<CustomerInvoiceManagerArgs>, IDomainService, ITransientDependency

    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<CustomerInvoice> _customerInvoiceRepository;
        private readonly IRepository<CustomerInvoiceDetail> _customerInvoiceDetailRepository;
        private readonly IRepository<Estimate> _estimateRepository;
        private readonly IRepository<EstimateDetail> _estimateDetailRepository;
        private readonly IRepository<LeaseAgreement> _leaseAgreementRepository;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<XeroInvoice> _xeroInvoiceRepository;
        private readonly IRepository<BillingEvent> _billingEventRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<UsageMetricRecord> _usageMetricRecordRepository;
        private readonly IRepository<UsageMetric> _usageMetricRepository;
        private readonly IRepository<BillingRule> _billingRuleRepository;
        private readonly IRepository<BillingRuleType> _billingRuleTypeRepository;
        private readonly IRepository<BillingEventDetail> _billingEventDetailRepository;
        private readonly IRepository<Uom> _uomRepository;
        private readonly IRepository<Contact> _contactRepository;

        public CustomerInvoiceManager(
              IUnitOfWorkManager unitOfWorkManager,
              IRepository<CustomerInvoice> customerInvoiceRepository,
              IRepository<CustomerInvoiceDetail> customerInvoiceDetailRepository,
              IRepository<Estimate> estimateRepository,
              IRepository<EstimateDetail> estimateDetailRepository,
              IRepository<XeroInvoice> xeroInvoiceRepository,
              IRepository<LeaseAgreement> leaseAgreementRepository,
              IRepository<LeaseItem> leaseItemRepository,
              IRepository<BillingEvent> billingEventRepository,
              IRepository<Customer> customerRepository,
              IRepository<Currency> currencyRepository,
              IRepository<UsageMetricRecord> usageMetricRecordRepository,
              IRepository<UsageMetric> usageMetricRepository,
              IRepository<BillingRule> billingRuleRepository,
              IRepository<BillingRuleType> billingRuleTypeRepository,
              IRepository<BillingEventDetail> billingEventDetailRepository,
              IRepository<Uom> uomRepository,
              IRepository<Contact> contactRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _customerInvoiceRepository = customerInvoiceRepository;
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
            _estimateRepository = estimateRepository;
            _estimateDetailRepository = estimateDetailRepository;
            _xeroInvoiceRepository = xeroInvoiceRepository;
            _leaseAgreementRepository = leaseAgreementRepository;
            _leaseItemRepository = leaseItemRepository;
            _billingEventRepository = billingEventRepository;
            _customerRepository = customerRepository;
            _currencyRepository = currencyRepository;
            _usageMetricRecordRepository = usageMetricRecordRepository;
            _usageMetricRepository = usageMetricRepository;
            _billingRuleRepository = billingRuleRepository;
            _billingRuleTypeRepository = billingRuleTypeRepository;
            _billingEventDetailRepository = billingEventDetailRepository;
            _uomRepository = uomRepository;
            _contactRepository = contactRepository;
        }

        [UnitOfWork]
        public override void Execute(CustomerInvoiceManagerArgs args)
        {
            if (args.GenerateMonthlyInvoices)
            {
                GenerateMonthlyInvoices(args.AssetOwnerId, args.FromMonth, args.ToMonth, args.InvoiceGenerationOption);
            }

            if (args.CustomerInvoiceDto != null)
            {
                if (args.CustomerInvoiceDto.Id > 0 && args.CustomerInvoiceDto.EstimateId > 0 && args.Created)
                {
                    CloneAllEstimateDetails(args.CustomerInvoiceDto.Id, (int)args.CustomerInvoiceDto.EstimateId);
                    GenerateXeroInvoice(args.CustomerInvoiceDto);
                }
            }
        }


        [UnitOfWork]
        public virtual void GenerateMonthlyInvoices(int assetOwnerId, DateTime? fromMonth, DateTime? toMonth, int customerInvoiceGenerationOption)
        {
            List<LeaseAgreement> leaseAgreements = _leaseAgreementRepository.GetAll().Where(l => l.AssetOwnerId == assetOwnerId && !l.IsDeleted).ToList();
            List<int> leaseAgreementIds = leaseAgreements.Select(l => l.Id).ToList();
            List<LeaseItem> allLeaseItems = _leaseItemRepository.GetAll().Where(l => leaseAgreementIds.Contains((int)l.LeaseAgreementId)).ToList();
            List<BillingEvent> billingEvents = _billingEventRepository.GetAll().Where(b => leaseAgreementIds.Contains((int)b.LeaseAgreementId)).ToList();
            List<int> billingEventIds = billingEvents.Select(e => e.Id).ToList();
            List<BillingEventDetail> billingEventDetails = _billingEventDetailRepository.GetAll().Include(b => b.BillingEventFk).Where(b => billingEventIds.Contains((int)b.BillingEventId)).ToList();
            List<BillingRule> allBillingRules = _billingRuleRepository.GetAll().Where(b => leaseAgreementIds.Contains((int)b.LeaseAgreementId)).ToList();
            
            List<BillingDetail> billingDetails = new List<BillingDetail>();

            if (fromMonth == null && toMonth == null) // Just do the current month
            {
                var month = DateTime.UtcNow.Month;
                var year = DateTime.UtcNow.Year;

                DateTime now = DateTime.UtcNow;
                var firstDayOfThisMonth = new DateTime(now.Year, now.Month, 1);
                var lastDayOfThisMonth = firstDayOfThisMonth.AddMonths(1).AddDays(-1);

                foreach (var leaseAgreement in leaseAgreements)
                {
                    var leaseItems = allLeaseItems.Where(l => l.LeaseAgreementId == leaseAgreement.Id).ToList();

                    foreach (LeaseItem leaseItem in leaseItems)
                    {
                        var assetConsumption = GetAssetConsumption((int)leaseItem.AssetId, firstDayOfThisMonth, lastDayOfThisMonth);
                        List<BillingRule> billingRules = allBillingRules.Where(b => b.LeaseItemId == leaseItem.Id).ToList();

                        foreach(BillingRule billingRule in billingRules)
                        {
                            BillingEventDetail existingBillingEventDetail = billingEventDetails.Where(b => 
                                                                            b.BillingEventFk.LeaseAgreementId == leaseAgreement.Id &&
                                                                            b.BillingRuleId == billingRule.Id && 
                                                                            b.RuleExecutedSuccessfully && 
                                                                            b.BillingEventFk.BillingEventDate.Month == month && 
                                                                            b.BillingEventFk.BillingEventDate.Year == year 
                                                                                ).FirstOrDefault();
                            if (existingBillingEventDetail == null)
                            {
                                if (leaseItem.AssetId != null)
                                {
                                    var thisRuleConsumption = assetConsumption.Where(a => a.UsageMetric.Id == billingRule.UsageMetricId).ToList();

                                    Consumption consolidatedConsumption = new Consumption()
                                    {
                                        FromDate = firstDayOfThisMonth,
                                        ToDate = lastDayOfThisMonth,
                                        Quantity = thisRuleConsumption.Select(c => c.Quantity).Sum(),
                                        Uom = billingRule.UsageMetricFk.UomFk,
                                        UsageMetric = billingRule.UsageMetricFk
                                    };

                                    BillingDetail newBillingDetail = new BillingDetail()
                                    {
                                        Consumption = consolidatedConsumption,
                                        BillingRule = billingRule,
                                        LeaseItem = leaseItem
                                    };

                                    billingDetails.Add(newBillingDetail);
                                }
                            }
                        }

                        if (customerInvoiceGenerationOption == InvoiceGenerationOptions.GenerateInvoiceForEachLeaseItem)
                        {
                            GenerateMonthlyLeaseInvoice(leaseAgreement, billingDetails, year, month);
                            billingDetails.Clear();
                        }
                    }

                    if(customerInvoiceGenerationOption == InvoiceGenerationOptions.GenerateInvoiceForEachLeaseAgreement)
                    {
                        GenerateMonthlyLeaseInvoice(leaseAgreement, billingDetails, year, month);
                        billingDetails.Clear();
                    }
                }

            }
        }

        private void GenerateMonthlyLeaseInvoice(LeaseAgreement leaseAgreement, List<BillingDetail> billingDetails, int year, int month)
        {
            var monthlyBillingRuleTypeId = _billingRuleTypeRepository.GetAll().Where(b => b.Type == "End on month bill").FirstOrDefault().Id;
            var monthlyBillingRule = _billingRuleRepository.GetAll().Where(b => b.LeaseAgreementId == leaseAgreement.Id).FirstOrDefault();
            if (monthlyBillingRule == null)
            {
                throw new Exception($"Lease Agreement ID {leaseAgreement.Id} does not have an Invoice-level billing rule!");
            }

            Customer customer = _customerRepository.GetAll().Where(c => c.Id == leaseAgreement.CustomerId).FirstOrDefault();
            Contact contact = _contactRepository.GetAll().Where(c => c.CustomerId == customer.Id).FirstOrDefault();
            
            if (contact == null)
            {
                contact = new Contact() { ContactName = "Responsible officer" };
            }

            BillingEvent billingEvent = new BillingEvent()
            {
                LeaseAgreementId = leaseAgreement.Id,
                BillingEventDate = new DateTime(year, month, 1),
                Purpose = string.Format("Monthly Invoice for {0}", leaseAgreement.Description),
                TriggeredBy = "CustomerInvoiceManager.GenerateMonthlyLeaseInvoice()",
                WasInvoiceGenerated = true
            };

            BillingEvent newBillingEvent = _billingEventRepository.Insert(billingEvent);

            var currencyId = customer.CurrencyId;
            if (currencyId == null) {
                currencyId = _currencyRepository.GetAll().FirstOrDefault().Id;
            }

            List<CustomerInvoiceDetail> invoiceDetails = new List<CustomerInvoiceDetail>();

            decimal totalCharge = 0;

            foreach(BillingDetail billingDetail in billingDetails)
            {

                var charge = 0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000; ;
            }

            CustomerInvoice invoice = new CustomerInvoice()
            {
                AssetOwnerId = leaseAgreement.AssetOwnerId,
                BillingEventId = newBillingEvent.Id,
                BillingRuleId = monthlyBillingRule.Id,
                CurrencyId = (int)currencyId,
                CustomerReference = customer.Reference,
                TotalCharge = 00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
                TotalDiscount = 00,
                DateDue = DateTime.UtcNow.AddMonths(1),
                DateIssued = DateTime.UtcNow,
                Description = "Monthly Lease Charges",
                InvoiceRecipient = contact.ContactName,
                InvoiceStatusId = 1
            };
        }

        [UnitOfWork]
        private List<Consumption> GetAssetConsumption(int assetId, DateTime fromDay, DateTime toDay)
        {
            List<UsageMetric> usageMetrics = _usageMetricRepository.GetAll().Include(m => m.UomFk).Where(m => m.AssetId == assetId).ToList();
            List<Consumption> assetConsumption = new List<Consumption>();

            foreach(var usageMetric in usageMetrics)
            {
                List<UsageMetricRecord> records = _usageMetricRecordRepository.GetAll().Where(r => r.UsageMetricId == usageMetric.Id && r.StartTime.Value >= fromDay && r.EndTime.Value <= toDay).ToList();

                if (records.Count > 0)
                {
                    decimal quantity = (decimal)records.Select(r => r.UnitsConsumed).Sum();

                    Consumption newConsumption = new Consumption()
                    {
                        FromDate = fromDay,
                        ToDate = toDay,
                        Quantity = quantity,
                        Uom = usageMetric.UomFk,
                        UsageMetric = usageMetric
                    };

                    assetConsumption.Add(newConsumption);
                }
            }
            return assetConsumption;  
        }

        [UnitOfWork]
        public virtual void GenerateXeroInvoice(CustomerInvoiceDto invoice)
        {

            // Akshay to implement this function

            // Use _xeroInvoiceRepository to store the results of the API call to Xero
        }

        public void CloneAllEstimateDetails(int customerInvoiceId, int estimateId)
        {
            var estimate = _estimateRepository.Get(estimateId);
            
            if (estimate != null)
            {
                var tenantId = estimate.TenantId;
                var estimateDetailList = _estimateDetailRepository.GetAll()
                    .Where(e => e.EstimateId == estimate.Id && !e.IsDeleted);

                if (estimateDetailList != null && estimateDetailList.Count() > 0)
                {
                    foreach (var item in estimateDetailList)
                    {
                        CustomerInvoiceDetail customerInvoiceDetail = new CustomerInvoiceDetail()
                        {
                            Charge = item.Charge,
                            CustomerInvoiceId = customerInvoiceId,
                            Description = item.Description,
                            Discount = item.Discount,
                            Gross = item.Cost,
                            Net = item.MarkUp,
                            Quantity = item.Quantity,
                            ItemTypeId = item.ItemTypeId,
                            UomId = item.UomId,
                            WorkOrderActionId = item.WorkOrderActionId,
                            Tax = item.Tax,
                            UnitPrice = item.UnitPrice
                        };

                        if (tenantId != null)
                            customerInvoiceDetail.TenantId = (int?)tenantId;

                        using (var unitOfWork = _unitOfWorkManager.Begin())
                        {
                            _customerInvoiceDetailRepository.InsertAndGetId(customerInvoiceDetail);
                            unitOfWork.Complete();
                            unitOfWork.Dispose();
                        }
                    }

                    UpdateCustomerInvoicePrices(customerInvoiceId);
                }
            }
        }

        private void UpdateCustomerInvoicePrices(int customerInvoiceId)
        {
            var invoice = _customerInvoiceRepository.Get(customerInvoiceId);

            if (invoice != null)
            {
                var invoiceDetailList = _customerInvoiceDetailRepository.GetAll()
                    .Where(e => e.CustomerInvoiceId == invoice.Id && !e.IsDeleted)
                    .ToList();

                if (invoiceDetailList != null && invoiceDetailList.Count() > 0)
                {
                    decimal totalPrice = 0, totalTax = 0, totalDiscount = 0, totalCharge = 0;

                    foreach (var item in invoiceDetailList)
                    {
                        decimal discountPrice = 0, taxPrice = 0, costPrice = 0;

                        costPrice = item.UnitPrice * item.Quantity;

                        if (item.Net > 0)
                            costPrice += costPrice * (item.Net / 100);

                        if (item.Discount > 0)
                            discountPrice = costPrice * ((decimal)item.Discount / 100);

                        if (item.Tax > 0)
                            taxPrice = (costPrice - discountPrice) * ((decimal)item.Tax / 100);

                        totalDiscount += discountPrice;
                        totalTax += taxPrice;
                        totalPrice += costPrice;
                        totalCharge += (costPrice - discountPrice) + taxPrice;
                    }

                    invoice.TotalPrice = totalPrice;
                    invoice.TotalTax = totalTax;
                    invoice.TotalDiscount = totalDiscount;
                    invoice.TotalCharge = totalCharge;
                }
                else
                {
                    invoice.TotalPrice = 0;
                    invoice.TotalTax = 0;
                    invoice.TotalDiscount = 0;
                    invoice.TotalCharge = 0;
                }

                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    _customerInvoiceRepository.Update(invoice);
                    unitOfWork.Complete();
                    unitOfWork.Dispose();
                }
                
            }
        }
    }

    public class CustomerInvoiceManagerArgs
    {
        public bool GenerateMonthlyInvoices { get; set; }
        public DateTime? FromMonth { get; set; }
        public DateTime? ToMonth { get; set; }
        public int AssetOwnerId { get; set; }
        public bool Created { get; set; }
        public CustomerInvoiceDto CustomerInvoiceDto { get; set; }
        public int InvoiceGenerationOption { get; set; }
    }

    public class Consumption
    {
        public Uom Uom { get; set; }
        public UsageMetric UsageMetric { get; set; }
        public decimal Quantity { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class BillingDetail
    {
        public LeaseItem LeaseItem { get; set; }
        public Consumption Consumption { get; set; }
        public BillingRule BillingRule { get; set; }
    }
}

