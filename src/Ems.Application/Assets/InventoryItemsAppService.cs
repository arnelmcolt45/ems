using Ems.Quotations;
using Ems.Assets;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Assets.Exporting;
using Ems.Assets.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Assets
{
    [AbpAuthorize(AppPermissions.Pages_InventoryItems)]
    public class InventoryItemsAppService : EmsAppServiceBase, IInventoryItemsAppService
    {
        private readonly string _entityType = "InventoryItem";

        private readonly IRepository<InventoryItem> _inventoryItemRepository;
		private readonly IInventoryItemsExcelExporter _inventoryItemsExcelExporter;
		private readonly IRepository<ItemType,int> _lookup_itemTypeRepository;
		private readonly IRepository<Asset,int> _lookup_assetRepository;
		private readonly IRepository<Warehouse,int> _lookup_warehouseRepository;
        private readonly IRepository<AssetPart> _assetPartRepository;

        public InventoryItemsAppService
            (
              IRepository<InventoryItem> inventoryItemRepository, 
              IInventoryItemsExcelExporter inventoryItemsExcelExporter , 
              IRepository<ItemType, int> lookup_itemTypeRepository, 
              IRepository<Asset, int> lookup_assetRepository, 
              IRepository<Warehouse, int> lookup_warehouseRepository,
              IRepository<AssetPart> assetPartRepository
              ) 
		  {
			_inventoryItemRepository = inventoryItemRepository;
			_inventoryItemsExcelExporter = inventoryItemsExcelExporter;
			_lookup_itemTypeRepository = lookup_itemTypeRepository;
		    _lookup_assetRepository = lookup_assetRepository;
		    _lookup_warehouseRepository = lookup_warehouseRepository;
            _assetPartRepository = assetPartRepository;
        }

		 public async Task<PagedResultDto<GetInventoryItemForViewDto>> GetAll(GetAllInventoryItemsInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var allDeployedInventoryItems = await _assetPartRepository.GetAll()
                .Include(a => a.ItemTypeFk)
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .Where(a => a.IsItem)
                .ToListAsync();

            //var consolidatedInventoryItems = allDeployedInventoryItems.GroupBy(a => a.Id).Sum...............ToList();

            var consolidatedInventoryItems = (from item in allDeployedInventoryItems
                                              group item by item.ItemTypeId into result
                             select new ConsolidatedInventoryItemDto
                             {
                                 Id = result.Key,
                                 Qty = result.Sum(f => f.Qty)
                             }).ToList();

            var filteredInventoryItems = _inventoryItemRepository.GetAll()
						.Include( e => e.ItemTypeFk)
						.Include( e => e.AssetFk)
						.Include( e => e.WarehouseFk)
                        .WhereIf(input.WarehouseId != null, e => e.WarehouseId == input.WarehouseId)
                        .WhereIf(input.AssetId != null, e => e.AssetId == input.AssetId)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Reference.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference == input.ReferenceFilter)
						.WhereIf(input.MinQtyInWarehouseFilter != null, e => e.QtyInWarehouse >= input.MinQtyInWarehouseFilter)
						.WhereIf(input.MaxQtyInWarehouseFilter != null, e => e.QtyInWarehouse <= input.MaxQtyInWarehouseFilter)
						.WhereIf(input.MinRestockLimitFilter != null, e => e.RestockLimit >= input.MinRestockLimitFilter)
						.WhereIf(input.MaxRestockLimitFilter != null, e => e.RestockLimit <= input.MaxRestockLimitFilter)
						.WhereIf(input.MinQtyOnOrderFilter != null, e => e.QtyOnOrder >= input.MinQtyOnOrderFilter)
						.WhereIf(input.MaxQtyOnOrderFilter != null, e => e.QtyOnOrder <= input.MaxQtyOnOrderFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference == input.AssetReferenceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.WarehouseNameFilter), e => e.WarehouseFk != null && e.WarehouseFk.Name == input.WarehouseNameFilter);

			var pagedAndFilteredInventoryItems = filteredInventoryItems
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var inventoryItems = from o in pagedAndFilteredInventoryItems
                                 join o1 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 join o2 in _lookup_assetRepository.GetAll() on o.AssetId equals o2.Id into j2
                                 from s2 in j2.DefaultIfEmpty()

                                 join o3 in _lookup_warehouseRepository.GetAll() on o.WarehouseId equals o3.Id into j3
                                 from s3 in j3.DefaultIfEmpty()

                                 select new GetInventoryItemForViewDto() {
                                     InventoryItem = new InventoryItemDto
                                     {
                                         Name = o.Name,
                                         Reference = o.Reference,
                                         QtyInWarehouse = o.QtyInWarehouse,
                                         RestockLimit = o.RestockLimit,
                                         QtyOnOrder = o.QtyOnOrder,
                                         Id = o.Id,
                                         AssetId = o.AssetId,
                                         ItemTypeId = o.ItemTypeId,
                                         WarehouseId = o.WarehouseId,
                                         QtyInAssets = (int)consolidatedInventoryItems.Where(i => i.Id == o.ItemTypeId).Select(i => i.Qty).FirstOrDefault()
                            },
                         	ItemTypeType = s1 == null || s1.Type == null ? "" : s1.Type.ToString(),
                         	AssetReference = s2 == null || s2.Reference == null ? "" : s2.Reference.ToString(),
                         	WarehouseName = s3 == null || s3.Name == null ? "" : s3.Name.ToString()
						};

            var totalCount = await filteredInventoryItems.CountAsync();

            return new PagedResultDto<GetInventoryItemForViewDto>(
                totalCount,
                await inventoryItems.ToListAsync()
            );
         }
		 
		 public async Task<GetInventoryItemForViewDto> GetInventoryItemForView(int id)
         {
            var inventoryItem = await _inventoryItemRepository.GetAsync(id);

            var output = new GetInventoryItemForViewDto { InventoryItem = ObjectMapper.Map<InventoryItemDto>(inventoryItem) };

		    if (output.InventoryItem.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.InventoryItem.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

		    if (output.InventoryItem.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.InventoryItem.AssetId);
                output.AssetReference = _lookupAsset?.Reference?.ToString();
            }

		    if (output.InventoryItem.WarehouseId != null)
            {
                var _lookupWarehouse = await _lookup_warehouseRepository.FirstOrDefaultAsync((int)output.InventoryItem.WarehouseId);
                output.WarehouseName = _lookupWarehouse?.Name?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_InventoryItems_Edit)]
		 public async Task<GetInventoryItemForEditOutput> GetInventoryItemForEdit(EntityDto input)
         {
            var inventoryItem = await _inventoryItemRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetInventoryItemForEditOutput {InventoryItem = ObjectMapper.Map<CreateOrEditInventoryItemDto>(inventoryItem)};

		    if (output.InventoryItem.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.InventoryItem.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

		    if (output.InventoryItem.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.InventoryItem.AssetId);
                output.AssetReference = _lookupAsset?.Reference?.ToString();
            }

		    if (output.InventoryItem.WarehouseId != null)
            {
                var _lookupWarehouse = await _lookup_warehouseRepository.FirstOrDefaultAsync((int)output.InventoryItem.WarehouseId);
                output.WarehouseName = _lookupWarehouse?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditInventoryItemDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_InventoryItems_Create)]
		 protected virtual async Task Create(CreateOrEditInventoryItemDto input)
         {
            var inventoryItem = ObjectMapper.Map<InventoryItem>(input);

			
			if (AbpSession.TenantId != null)
			{
				inventoryItem.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _inventoryItemRepository.InsertAsync(inventoryItem);
         }

		 [AbpAuthorize(AppPermissions.Pages_InventoryItems_Edit)]
		 protected virtual async Task Update(CreateOrEditInventoryItemDto input)
         {
            var inventoryItem = await _inventoryItemRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, inventoryItem);
         }

		 [AbpAuthorize(AppPermissions.Pages_InventoryItems_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _inventoryItemRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetInventoryItemsToExcel(GetAllInventoryItemsForExcelInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var filteredInventoryItems = _inventoryItemRepository.GetAll()
						.Include( e => e.ItemTypeFk)
						.Include( e => e.AssetFk)
						.Include( e => e.WarehouseFk)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Reference.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference == input.ReferenceFilter)
						.WhereIf(input.MinQtyInWarehouseFilter != null, e => e.QtyInWarehouse >= input.MinQtyInWarehouseFilter)
						.WhereIf(input.MaxQtyInWarehouseFilter != null, e => e.QtyInWarehouse <= input.MaxQtyInWarehouseFilter)
						.WhereIf(input.MinRestockLimitFilter != null, e => e.RestockLimit >= input.MinRestockLimitFilter)
						.WhereIf(input.MaxRestockLimitFilter != null, e => e.RestockLimit <= input.MaxRestockLimitFilter)
						.WhereIf(input.MinQtyOnOrderFilter != null, e => e.QtyOnOrder >= input.MinQtyOnOrderFilter)
						.WhereIf(input.MaxQtyOnOrderFilter != null, e => e.QtyOnOrder <= input.MaxQtyOnOrderFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference == input.AssetReferenceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.WarehouseNameFilter), e => e.WarehouseFk != null && e.WarehouseFk.Name == input.WarehouseNameFilter);

			var query = (from o in filteredInventoryItems
                         join o1 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetRepository.GetAll() on o.AssetId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_warehouseRepository.GetAll() on o.WarehouseId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         select new GetInventoryItemForViewDto() { 
							InventoryItem = new InventoryItemDto
							{
                                Name = o.Name,
                                Reference = o.Reference,
                                QtyInWarehouse = o.QtyInWarehouse,
                                RestockLimit = o.RestockLimit,
                                QtyOnOrder = o.QtyOnOrder,
                                Id = o.Id
							},
                         	ItemTypeType = s1 == null || s1.Type == null ? "" : s1.Type.ToString(),
                         	AssetReference = s2 == null || s2.Reference == null ? "" : s2.Reference.ToString(),
                         	WarehouseName = s3 == null || s3.Name == null ? "" : s3.Name.ToString()
						 });


            var inventoryItemListDtos = await query.ToListAsync();

            return _inventoryItemsExcelExporter.ExportToFile(inventoryItemListDtos);
         }

		[AbpAuthorize(AppPermissions.Pages_InventoryItems)]
         public async Task<PagedResultDto<InventoryItemItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_itemTypeRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type != null && e.Type.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var itemTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<InventoryItemItemTypeLookupTableDto>();
			foreach(var itemType in itemTypeList){
				lookupTableDtoList.Add(new InventoryItemItemTypeLookupTableDto
				{
					Id = itemType.Id,
					DisplayName = itemType.Type?.ToString()
				});
			}

            return new PagedResultDto<InventoryItemItemTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_InventoryItems)]
         public async Task<PagedResultDto<InventoryItemAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_assetRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Reference != null && e.Reference.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<InventoryItemAssetLookupTableDto>();
			foreach(var asset in assetList){
				lookupTableDtoList.Add(new InventoryItemAssetLookupTableDto
				{
					Id = asset.Id,
					DisplayName = asset.Reference?.ToString()
				});
			}

            return new PagedResultDto<InventoryItemAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_InventoryItems)]
         public async Task<PagedResultDto<InventoryItemWarehouseLookupTableDto>> GetAllWarehouseForLookupTable(GetAllForLookupTableInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_warehouseRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var warehouseList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<InventoryItemWarehouseLookupTableDto>();
			foreach(var warehouse in warehouseList){
				lookupTableDtoList.Add(new InventoryItemWarehouseLookupTableDto
				{
					Id = warehouse.Id,
					DisplayName = warehouse.Name?.ToString()
				});
			}

            return new PagedResultDto<InventoryItemWarehouseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}