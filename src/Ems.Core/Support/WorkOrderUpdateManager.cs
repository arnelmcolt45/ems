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
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Ems.Support
{
    public class WorkOrderUpdateManager : BackgroundJob<WorkOrderUpdateManagerArgs>, IDomainService, ITransientDependency

    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<WorkOrderUpdate> _workOrderUpdateRepository;
        private readonly IRepository<AssetPart> _assetPartRepository;
        private readonly IRepository<Asset> _assetRepository;
        private readonly IRepository<ItemType> _itemTypeRepository;
        public WorkOrderUpdateManager
            (
              IUnitOfWorkManager unitOfWorkManager,
              IRepository<WorkOrderUpdate> workOrderUpdateRepository,
              IRepository<AssetPart> assetPartRepository,
              IRepository<Asset> assetRepository,
              IRepository<ItemType> itemTypeRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _workOrderUpdateRepository = workOrderUpdateRepository;
            _assetPartRepository = assetPartRepository;
            _assetRepository = assetRepository;
            _itemTypeRepository = itemTypeRepository;
        }

        [UnitOfWork]
        public override void Execute(WorkOrderUpdateManagerArgs args)
        {
            // if needed, move the AssetPart from the Warehouse to the Asset


                //TODO:

            // If needed, create an AssetPart for the new Inventory Items that were installed
            if(args.WorkOrderUpdateDto.ItemTypeId != null && args.WorkOrderUpdateDto.AssetPartId != null && args.WorkOrderUpdateDto.Number > 0)
            {
                var parentAssetPart = _assetPartRepository.Get((int)args.WorkOrderUpdateDto.AssetPartId);
                var asset = _assetRepository.Get((int)parentAssetPart.AssetId);
                var itemType = _itemTypeRepository.Get((int)args.WorkOrderUpdateDto.ItemTypeId);

                var assetPart = new AssetPart()
                {
                    AssetId = parentAssetPart.AssetId,
                    AssetPartStatusId = null,
                    AssetPartTypeId = null,
                    Code = "0000",
                    Description = string.Format("{0} items for {1}", itemType.Type, asset.Reference),
                    Name = string.Format("Items: {0}", itemType.Type),
                    InstallDate = DateTime.UtcNow,
                    Installed = true,
                    IsItem = true,
                    ParentId = parentAssetPart.Id,
                    Qty = (int)args.WorkOrderUpdateDto.Number,
                    TenantId = args.TenantInfo.Tenant.Id,
                    WarehouseId = null,
                    SerialNumber = null,
                    IsDeleted = false,
                    ItemTypeId = args.WorkOrderUpdateDto.ItemTypeId
                };

                _assetPartRepository.InsertAsync(assetPart);
                CurrentUnitOfWork.SaveChanges();
            }
        }
    }

    public class WorkOrderUpdateManagerArgs
    {
        public WorkOrderUpdateDto WorkOrderUpdateDto { get; set; }

        public TenantInfo TenantInfo { get; set; }
    }
}

