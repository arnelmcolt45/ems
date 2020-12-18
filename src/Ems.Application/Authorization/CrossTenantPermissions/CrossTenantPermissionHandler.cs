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
using System.Threading.Tasks;
using System.Collections.Generic;
using Abp.BackgroundJobs;
using Ems.Support.Dtos;
using System.Linq;
using Ems.Quotations.Dtos;
using Ems.Assets.Dtos;
using Ems.Customers;
using Ems.Customers.Dtos;
using Abp.Extensions;
using Ems.Vendors;

namespace Ems.Authorization
{
    class CrossTenantPermissionHandler : EmsAppServiceBase, IDomainService, ITransientDependency, 
        IAsyncEventHandler<EntityCreatedEventData<SupportContract>>,
        IAsyncEventHandler<EntityCreatedEventData<LeaseAgreement>>,
        IAsyncEventHandler<EntityCreatedEventData<SupportItem>>,
        IAsyncEventHandler<EntityCreatedEventData<LeaseItem>>
        //IAsyncEventHandler<EntityCreatedEventData<Asset>>,
        //IAsyncEventHandler<EntityCreatedEventData<Incident>>,
        //IAsyncEventHandler<EntityCreatedEventData<WorkOrder>>,
        //IAsyncEventHandler<EntityCreatedEventData<Quotation>>,
        //IAsyncEventHandler<EntityCreatedEventData<Estimate>>

    {
        //private readonly CrossTenantPermissionManager _crossTenantPermissionManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<CrossTenantPermission> _crossTenantPermissionRepository;
        private readonly IRepository<AssetOwner> _assetOwnerRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<Customer> _customerRepository;

        public CrossTenantPermissionHandler(
            IBackgroundJobManager backgroundJobManager, 
            IRepository<Tenant> tenantRepository, 
            IRepository<CrossTenantPermission> crossTenantPermissionRepository,
            IRepository<Customer> customerRepository,
            IRepository<Vendor> vendorRepository,
            IRepository<AssetOwner> assetOwnerRepository
            )
        {
            _backgroundJobManager = backgroundJobManager;
            _tenantRepository = tenantRepository;
            _crossTenantPermissionRepository = crossTenantPermissionRepository;
            _vendorRepository = vendorRepository;
            _assetOwnerRepository = assetOwnerRepository;
            _customerRepository = customerRepository;
        }

        [UnitOfWork]
        public async Task HandleEventAsync(EntityCreatedEventData<LeaseAgreement> eventData)
        {
            TenantInfo tenantInfo = await TenantManager.GetTenantInfo();
            GetLeaseAgreementForViewDto entity = new GetLeaseAgreementForViewDto { LeaseAgreement = ObjectMapper.Map<LeaseAgreementDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    LeaseAgreementDto = entity.LeaseAgreement,
                    EntityType = "LeaseAgreement",
                    TenantInfo = tenantInfo
                });
        }

        [UnitOfWork]
        public async Task HandleEventAsync(EntityCreatedEventData<LeaseItem> eventData)
        {
            TenantInfo tenantInfo = await TenantManager.GetTenantInfo();
            GetLeaseItemForViewDto entity = new GetLeaseItemForViewDto { LeaseItem = ObjectMapper.Map<LeaseItemDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    LeaseItemDto = entity.LeaseItem,
                    EntityType = "LeaseItem",
                    TenantInfo = tenantInfo
                });
        }

        [UnitOfWork]
        public async Task HandleEventAsync(EntityCreatedEventData<SupportContract> eventData)
        {
            if (eventData.Entity.VendorId != null)
            {
                TenantInfo tenantInfo = await TenantManager.GetTenantInfo();
                GetSupportContractForViewDto entity = new GetSupportContractForViewDto { SupportContract = ObjectMapper.Map<SupportContractDto>(eventData.Entity) };
                await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                    new CrossTenantPermissionManagerArgs
                    {
                        SupportContractDto = entity.SupportContract,
                        EntityType = "SupportContract",
                        TenantInfo = tenantInfo
                    });
            }
        }

        [UnitOfWork]
        public async Task HandleEventAsync(EntityCreatedEventData<SupportItem> eventData)
        {

            TenantInfo tenantInfo = await TenantManager.GetTenantInfo();
            GetSupportItemForViewDto entity = new GetSupportItemForViewDto { SupportItem = ObjectMapper.Map<SupportItemDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    SupportItemDto = entity.SupportItem,
                    EntityType = "SupportItem",
                    TenantInfo = tenantInfo
                });
        }

        /*
         public async Task HandleEventAsync(EntityCreatedEventData<Incident> eventData)
         {
             TenantInfo tenantInfo = GetTenantInfo().Result;
             //List<int> crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Incident");
             GetIncidentForViewDto entity = new GetIncidentForViewDto { Incident = ObjectMapper.Map<IncidentDto>(eventData.Entity) };
             await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                 new CrossTenantPermissionManagerArgs
                 {
                     //CrossTenantPermissions = crossTenantPermissions,
                     IncidentDto = entity.Incident,
                     EntityType = "Incident",
                     TenantInfo = tenantInfo
                 });
         }


         public async Task HandleEventAsync(EntityCreatedEventData<Quotation> eventData)
         {
             TenantInfo tenantInfo = GetTenantInfo().Result;
             //List<int> crossTenantPermissions = TenantManager.GetCrossTenantPermissions(tenantInfo, "Quotation").Result;
             GetQuotationForViewDto entity = new GetQuotationForViewDto { Quotation = ObjectMapper.Map<QuotationDto>(eventData.Entity) };
             await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                 new CrossTenantPermissionManagerArgs
                 {
                     //CrossTenantPermissions = crossTenantPermissions,
                     QuotationDto = entity.Quotation,
                     EntityType = "Quotation",
                     TenantInfo = tenantInfo
                 });
         }

         public async Task HandleEventAsync(EntityCreatedEventData<Estimate> eventData)
         {
             TenantInfo tenantInfo = GetTenantInfo().Result;
             //List<int> crossTenantPermissions = TenantManager.GetCrossTenantPermissions(tenantInfo, "Estimate").Result;
             GetEstimateForViewDto entity = new GetEstimateForViewDto { Estimate = ObjectMapper.Map<EstimateDto>(eventData.Entity) };
             await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                 new CrossTenantPermissionManagerArgs
                 {
                     //CrossTenantPermissions = crossTenantPermissions,
                     EstimateDto = entity.Estimate,
                     EntityType = "Estimate",
                     TenantInfo = tenantInfo
                 });
         }


         public async Task HandleEventAsync(EntityCreatedEventData<WorkOrder> eventData)
         {
             TenantInfo tenantInfo = GetTenantInfo().Result;
             //List<int> crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "WorkOrder");
             GetWorkOrderForViewDto entity = new GetWorkOrderForViewDto { WorkOrder = ObjectMapper.Map<WorkOrderDto>(eventData.Entity) };
             await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                 new CrossTenantPermissionManagerArgs
                 {
                     //CrossTenantPermissions = crossTenantPermissions,
                     WorkOrderDto = entity.WorkOrder,
                     EntityType = "WorkOrder",
                     TenantInfo = tenantInfo
                 });
         }

         public async Task HandleEventAsync(EntityCreatedEventData<Asset> eventData)
         {
             TenantInfo tenantInfo = GetTenantInfo().Result;
             //List<int> crossTenantPermissions = TenantManager.GetCrossTenantPermissions(tenantInfo, "Asset").Result;
             GetAssetForViewDto entity = new GetAssetForViewDto { Asset = ObjectMapper.Map<AssetDto>(eventData.Entity) };
             await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                 new CrossTenantPermissionManagerArgs
                 {
                     //CrossTenantPermissions = crossTenantPermissions,
                     AssetDto = entity.Asset,
                     EntityType = "Asset",
                     TenantInfo = tenantInfo
                 });
         }
         */

        /*
        [UnitOfWork]
        public async Task<TenantInfo> GetTenantInfo()
        {
            //TODO: Cache this in the user Session - check before looking up again

            if (AbpSession.TenantId == null)
            {
                return new TenantInfo() { IsHost = true, Tenant = new Tenant() { TenantType = "H", TenancyName = "Host" } };
            }

            Tenant tenant = await _tenantRepository.GetAsync((int)AbpSession.TenantId);

            TenantInfo tenantInfo = new TenantInfo()
            {
                Tenant = tenant
            };

            switch (tenant.TenantType)
            {
                case "A":
                    var allAssetOwners = await _assetOwnerRepository.GetAllListAsync();
                    tenantInfo.AssetOwner = allAssetOwners.Where(a => a.TenantId == tenant.Id).FirstOrDefault();
                    break;
                case "C":
                    var allCustomers = await _customerRepository.GetAllListAsync();
                    tenantInfo.Customer = allCustomers.Where(a => a.TenantId == tenant.Id).FirstOrDefault();
                    break;
                case "V":
                    var allVendors = await _vendorRepository.GetAllListAsync();
                    tenantInfo.Vendor = allVendors.Where(a => a.TenantId == tenant.Id).FirstOrDefault();
                    break;
                default:
                    throw new Exception($"Cannot determine TenantType for {tenant.TenancyName}!");
            }

            return tenantInfo;
        }
        */
    }
}

