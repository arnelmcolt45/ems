using Ems.Assets;
using Ems.Quotations;
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
using Ems.Assets.Dto;
using Org.BouncyCastle.Crypto.Agreement.Srp;

namespace Ems.Assets
{
    [AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
    public class AssetPartsAppService : EmsAppServiceBase, IAssetPartsAppService
    {
        private readonly string _entityType = "AssetPart";

        private readonly IRepository<AssetPart> _assetPartRepository;
        private readonly IAssetPartsExcelExporter _assetPartsExcelExporter;
        private readonly IRepository<AssetPartType, int> _lookup_assetPartTypeRepository;
        private readonly IRepository<AssetPart, int> _lookup_assetPartRepository;
        private readonly IRepository<AssetPartStatus, int> _lookup_assetPartStatusRepository;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<ItemType, int> _lookup_itemTypeRepository;
        private readonly IRepository<Warehouse, int> _lookup_warehouseRepository;
        private readonly IRepository<InventoryItem> _inventoryItemRepository;

        public AssetPartsAppService
            (
            IRepository<AssetPart> assetPartRepository, 
            IAssetPartsExcelExporter assetPartsExcelExporter, 
            IRepository<AssetPartType, int> lookup_assetPartTypeRepository, 
            IRepository<AssetPart, int> lookup_assetPartRepository, 
            IRepository<AssetPartStatus, int> lookup_assetPartStatusRepository, 
            IRepository<Asset, int> lookup_assetRepository, 
            IRepository<ItemType, int> lookup_itemTypeRepository,
            IRepository<InventoryItem> inventoryItemRepository,
            IRepository<Warehouse, int> lookup_warehouseRepository
            )
        {
            _assetPartRepository = assetPartRepository;
            _assetPartsExcelExporter = assetPartsExcelExporter;
            _lookup_assetPartTypeRepository = lookup_assetPartTypeRepository;
            _lookup_assetPartRepository = lookup_assetPartRepository;
            _lookup_assetPartStatusRepository = lookup_assetPartStatusRepository;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_itemTypeRepository = lookup_itemTypeRepository;
            _lookup_warehouseRepository = lookup_warehouseRepository;
            _inventoryItemRepository = inventoryItemRepository;
        }


        /*
        public async Task<ListResultDto<AssetPartDto>> GetAssetParts()
        {
            var query = from ou in _assetPartRepository.GetAll()
                        select new
                        {
                            ou
                        };

            var items = await query.ToListAsync();

            return new ListResultDto<AssetPartDto>(
                items.Select(item =>
                {
                    var assetPartDto = ObjectMapper.Map<AssetPartDto>(item.ou);
                    return assetPartDto;
                }).ToList());
        }
        */

        public async Task<ListResultDto<AssetPartExtendedDto>> GetAssetParts(int assetId)
        {
            var query = from o in _assetPartRepository.GetAll().Where(a => a.AssetId == assetId)

                        join o1 in _lookup_assetPartTypeRepository.GetAll() on o.AssetPartTypeId equals o1.Id into j1
                        from s1 in j1.DefaultIfEmpty()

                        join o2 in _lookup_assetPartRepository.GetAll() on o.ParentId equals o2.Id into j2
                        from s2 in j2.DefaultIfEmpty()

                        join o3 in _lookup_assetPartStatusRepository.GetAll() on o.AssetPartStatusId equals o3.Id into j3
                        from s3 in j3.DefaultIfEmpty()

                        join o4 in _lookup_assetRepository.GetAll() on o.AssetId equals o4.Id into j4
                        from s4 in j4.DefaultIfEmpty()

                        join o5 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o5.Id into j5
                        from s5 in j5.DefaultIfEmpty()

                        join o6 in _lookup_warehouseRepository.GetAll() on o.WarehouseId equals o6.Id into j6
                        from s6 in j6.DefaultIfEmpty()

                        select new AssetPartExtendedDto()
                        {
                            Name = o.Name,
                            Description = o.Description,
                            SerialNumber = o.SerialNumber,
                            InstallDate = o.InstallDate,
                            Code = o.Code,
                            Installed = o.Installed,
                            IsItem = o.IsItem,
                            Qty = o.Qty,
                            Id = o.Id,
                            ParentId = o.ParentId,
                            AssetId = o.AssetId,
                            WarehouseId = o.WarehouseId,
                            AssetPartType = s1 == null ? "" : s1.Type.ToString(),
                            ParentName = s2 == null ? "" : s2.Name.ToString(),
                            AssetPartStatus = s3 == null ? "" : s3.Status.ToString(),
                            AssetReference = s4 == null ? "" : s4.Reference.ToString(),
                            ItemType = s5 == null ? "" : s5.Type.ToString(),
                            WarehouseName = s6 == null ? "" : s6.Name.ToString()
                        };

            var items = await query.ToListAsync();
            //items = items.Where(a => a.AssetId == assetId).ToList();

            return new ListResultDto<AssetPartExtendedDto>(
                items.Select(item =>
                {
                    var extendedAssetPartDto = ObjectMapper.Map<AssetPartExtendedDto>(item);
                    return extendedAssetPartDto;
                }).ToList());
        }

        /*
        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> CreateAssetPart(CreateOrEditAssetPartDto input)
        {
            var assetPart = new AssetPart(
                AbpSession.TenantId,
                input., input.ParentId);
            input.
            await CreateAsync(assetPart);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<AssetPartDto>(assetPart);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> UpdateAssetPart(UpdateAssetPartInput input)
        {
            var assetPart = await _assetPartRepository.GetAsync(input.Id);

            assetPart.DisplayName = input.DisplayName;

            await UpdateAsync(assetPart);

            return await CreateAssetPartDto(assetPart);
        }
        */




        /*
        private async Task<AssetPartDto> CreateAssetPartDto(AssetPart assetPart)
        {
            var dto = ObjectMapper.Map<AssetPartDto>(assetPart);
            dto.MemberCount = await _userAssetPartRepository.CountAsync(uou => uou.AssetPartId == assetPart.Id);
            return dto;
        }
        */

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> MoveAssetPart(MoveAssetPartInput input)
        {
            Move(input.Id, input.NewParentId);

            var dto = ObjectMapper.Map<AssetPartDto>(await _assetPartRepository.GetAsync(input.Id));

            return dto;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> MoveAssetPartToAsset(MoveAssetPartToAssetInput input)
        {
            bool importAssetPart = (input.ImportAssetPart == true) ? true : false;

            // Get any/all first-level children of this AssetPart
            var children = await FindChildrenOrNull(input.AssetPartId, false);

            List<AssetPart> childItems = new List<AssetPart>();
            List<AssetPart> childComponents = new List<AssetPart>();

            if (children != null)
            {
                childItems = children.Where(a => a.IsItem).ToList();
                childComponents = children.Where(a => !a.IsItem).ToList();
            }

            if (children?.Count() > 0)
            {
                foreach (var child in children)
                {
                    if(child.ParentId == input.AssetPartId)
                    {
                        AssetPart childToUpdate = _assetPartRepository.Get(child.Id);

                        switch (child.IsItem)
                        { 
                            // Set the first level child parentId to NULL (if it is not an Item)
                            case false:
                                childToUpdate.ParentId = null;
                                childToUpdate.Installed = true;
                                childToUpdate.InstallDate = DateTime.UtcNow;
                                await _assetPartRepository.UpdateAsync(childToUpdate);
                                break;

                            // or, move any inventory items along with the component (assetPart)
                            case true:
                                childToUpdate.AssetId = input.NewAssetId;
                                childToUpdate.Installed = true;
                                childToUpdate.InstallDate = DateTime.UtcNow;
                                await _assetPartRepository.UpdateAsync(childToUpdate);
                                break;
                        }
                    }
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Set the AssetPart's  AssetId to the new AssetId and the ParentId to NULL or new ParentId if Imported
            var assetPart = _assetPartRepository.Get(input.AssetPartId);
            assetPart.ParentId = (importAssetPart == true) ? input.NewAssetPartParentId : null;
            assetPart.AssetId = input.NewAssetId;
            assetPart.WarehouseId = null;
            await _assetPartRepository.UpdateAsync(assetPart);

            var dto = ObjectMapper.Map<AssetPartDto>(assetPart);

            return dto;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> MoveBranchToAsset(MoveAssetPartToAssetInput input)
        {
            // Get any/all children of this AssetPart

            var children = await FindChildrenOrNull(input.AssetPartId, true);

            // Set all the children AssetId's to the new AssetId
            if (children?.Count() > 0)
            {
                foreach (var child in children)
                {
                    if (child.ParentId == input.AssetPartId)
                    {
                        AssetPart childToUpdate = _assetPartRepository.Get(child.Id);

                        switch (child.IsItem)
                        {
                            // Set the first level child parentId to NULL (if it is not an Item)
                            case false:
                                childToUpdate.ParentId = null;
                                childToUpdate.Installed = true;
                                childToUpdate.InstallDate = DateTime.UtcNow;
                                await _assetPartRepository.UpdateAsync(childToUpdate);
                                break;

                            // or, move any inventory items along with the component (assetPart)
                            case true:
                                childToUpdate.AssetId = input.NewAssetId;
                                childToUpdate.Installed = true;
                                childToUpdate.InstallDate = DateTime.UtcNow;
                                await _assetPartRepository.UpdateAsync(childToUpdate);
                                break;
                        }
                    }
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Set the AssetPart's ParentId to NULL and AssetId to the new AssetId
            var assetPart = _assetPartRepository.Get(input.AssetPartId);
            assetPart.ParentId = null;
            assetPart.AssetId = input.NewAssetId;
            assetPart.Installed = true;
            assetPart.InstallDate = DateTime.UtcNow;
            assetPart.WarehouseId = null;
            await _assetPartRepository.UpdateAsync(assetPart);

            var dto = ObjectMapper.Map<AssetPartDto>(assetPart);

            return dto;
        }

        private async Task<AssetPartDto> MoveToWareHouse(MoveAssetPartToWarehouseInput input, List<AssetPart> children)
        {
            //Get the AssetPart
            var assetPart = await _assetPartRepository.GetAll().Where(a => a.Id == input.AssetPartId).Include(a => a.ItemTypeFk).FirstOrDefaultAsync();

            Boolean IsItem = (assetPart.IsItem) ? true : false;

            List<AssetPart> childItems = new List<AssetPart>();
            List<AssetPart> childComponents = new List<AssetPart>();

            if (children != null)
            {
                childItems = children.Where(a => a.IsItem).ToList();
                childComponents = children.Where(a => !a.IsItem).ToList();
            }

            // Set any first level childComponent's parentIds to NULL
            if (childComponents?.Count() > 0)
            {
                foreach (var child in childComponents)
                {
                    var childToUpdate = _assetPartRepository.Get(child.Id);
                    childToUpdate.ParentId = null;
                    childToUpdate.InstallDate = null;
                    childToUpdate.Installed = false;
                    await _assetPartRepository.UpdateAsync(childToUpdate);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Convert childItems into Inventory Items

            if (assetPart.IsItem)
            {
                childItems.Add(assetPart);
            }

            if (childItems?.Count() > 0)
            {
                var warehouseItems = await _inventoryItemRepository.GetAll().Where(i => i.WarehouseId == input.NewWarehouseId).ToListAsync();
                var warehouseItemTypeIds = warehouseItems.Select(i => i.ItemTypeId).ToList();

                foreach (var child in childItems)
                {
                    if (child.ItemTypeId != null)
                    {
                        if (warehouseItemTypeIds.Contains((int)child.ItemTypeId))
                        {
                            // update InventoryItem

                            var query = _inventoryItemRepository.GetAll().Where(i => i.WarehouseId == input.NewWarehouseId && i.ItemTypeId == ((int)child.ItemTypeId));
                            var existingInvItem = await query.FirstOrDefaultAsync();

                            var qtyToAdd = (child.Qty != null) ? child.Qty : 0;

                            existingInvItem.QtyInWarehouse = existingInvItem.QtyInWarehouse + (int)qtyToAdd;
                            await _inventoryItemRepository.UpdateAsync(existingInvItem);
                            //await CurrentUnitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            // create InventoryItem

                            var qtyToAdd = (child.Qty != null) ? child.Qty : 0;

                            InventoryItem newInventoryItem = new InventoryItem()
                            {
                                Name = string.Format("{0}", child.ItemTypeFk.Type),
                                ItemTypeId = child.ItemTypeId,
                                QtyInWarehouse = (int)qtyToAdd,
                                WarehouseId = input.NewWarehouseId,
                                TenantId = AbpSession.TenantId,
                                RestockLimit = 0,
                                QtyOnOrder = 0,
                                Reference = "auto generated"
                            };

                            await _inventoryItemRepository.InsertAsync(newInventoryItem);
                            await CurrentUnitOfWork.SaveChangesAsync();
                            warehouseItemTypeIds.Add((int)newInventoryItem.ItemTypeId);
                        }
                    }
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Delete childItems
            foreach (var child in childItems)
            {
                await _assetPartRepository.DeleteAsync(child.Id);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            // Set the AssetPart's ParentId and AssetId to NULL and WarehouseId to the New WarehouseId

            AssetPartDto dto = new AssetPartDto();

            if (!IsItem && assetPart != null)
            {
                assetPart.ParentId = null;
                assetPart.AssetId = null;
                assetPart.InstallDate = null;
                assetPart.Installed = false;
                assetPart.WarehouseId = input.NewWarehouseId;
                await _assetPartRepository.UpdateAsync(assetPart);

                dto = ObjectMapper.Map<AssetPartDto>(assetPart);
            }

            return dto;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> MoveAssetPartToWarehouse(MoveAssetPartToWarehouseInput input)
        {
            //Get all first - level children of this AssetPart
            List<AssetPart> children = await FindChildrenOrNull(input.AssetPartId, false);

            var dto = await MoveToWareHouse(input, children);

            /*
            //Get the AssetPart
            var assetPart = await _assetPartRepository.GetAll().Where(a => a.Id == input.AssetPartId).Include(a => a.ItemTypeFk).FirstOrDefaultAsync();

            Boolean IsItem = (assetPart.IsItem) ? true : false;

            List<AssetPart> childItems = new List<AssetPart>();
            List<AssetPart> childComponents = new List<AssetPart>();

            if (children != null)
            {
                childItems = children.Where(a => a.IsItem).ToList();
                childComponents = children.Where(a => !a.IsItem).ToList();
            }

            // Set any first level childComponent's parentIds to NULL
            if (childComponents?.Count() > 0)
            {
                foreach (var child in childComponents)
                {
                    var childToUpdate = _assetPartRepository.Get(child.Id);
                    childToUpdate.ParentId = null;
                    childToUpdate.InstallDate = null;
                    childToUpdate.Installed = false;
                    await _assetPartRepository.UpdateAsync(childToUpdate);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Convert childItems into Inventory Items

            if (assetPart.IsItem)
            {
                childItems.Add(assetPart);
            }

            if (childItems?.Count() > 0)
            {
                var warehouseItems = await _inventoryItemRepository.GetAll().Where(i => i.WarehouseId == input.NewWarehouseId).ToListAsync();
                var warehouseItemTypeIds = warehouseItems.Select(i => i.ItemTypeId).ToList();

                foreach (var child in childItems)
                {
                    if(child.ItemTypeId != null)
                    {
                        if (warehouseItemTypeIds.Contains((int)child.ItemTypeId))
                        {
                            // update InventoryItem

                            var query = _inventoryItemRepository.GetAll().Where(i => i.WarehouseId == input.NewWarehouseId && i.ItemTypeId == ((int)child.ItemTypeId));
                            var existingInvItem = await query.FirstOrDefaultAsync();

                            var qtyToAdd = (child.Qty != null) ? child.Qty : 0;

                            existingInvItem.QtyInWarehouse = existingInvItem.QtyInWarehouse + (int)qtyToAdd;
                            await _inventoryItemRepository.UpdateAsync(existingInvItem);
                            //await CurrentUnitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            // create InventoryItem

                            var qtyToAdd = (child.Qty != null) ? child.Qty : 0;

                            InventoryItem newInventoryItem = new InventoryItem()
                            {
                                Name = string.Format("{0}", child.ItemTypeFk.Type),
                                ItemTypeId = child.ItemTypeId,
                                QtyInWarehouse = (int)qtyToAdd,
                                WarehouseId = input.NewWarehouseId,
                                TenantId = AbpSession.TenantId,
                                RestockLimit = 0,
                                QtyOnOrder = 0,
                                Reference = "auto generated"
                            };

                            await _inventoryItemRepository.InsertAsync(newInventoryItem);
                            await CurrentUnitOfWork.SaveChangesAsync();
                            warehouseItemTypeIds.Add((int)newInventoryItem.ItemTypeId);
                        }
                    }
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Delete childItems
            foreach (var child in childItems)
            {
                await _assetPartRepository.DeleteAsync(child.Id);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            // Set the AssetPart's ParentId and AssetId to NULL and WarehouseId to the New WarehouseId

            AssetPartDto dto = new AssetPartDto();

            if (!IsItem && assetPart != null)
            {
                assetPart.ParentId = null;
                assetPart.AssetId = null;
                assetPart.InstallDate = null;
                assetPart.Installed = false;
                assetPart.WarehouseId = input.NewWarehouseId;
                await _assetPartRepository.UpdateAsync(assetPart);

                dto = ObjectMapper.Map<AssetPartDto>(assetPart);
            }
            */

            return dto;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> MoveBranchToWarehouse(MoveAssetPartToWarehouseInput input)
        {
            // Get any/all children of this AssetPart
            var children = await FindChildrenOrNull(input.AssetPartId, true);

            var dto = await MoveToWareHouse(input, children);

            /*
            // Set all the children AssetId and ParentId to NULL and WarehouseId to the New WarehouseId

            if (children?.Count() > 0)
            {
                foreach (var child in children)
                {
                    var childToUpdate = _assetPartRepository.Get(child.Id);
                    childToUpdate.AssetId = null;
                    childToUpdate.ParentId = null;
                    childToUpdate.InstallDate = null;
                    childToUpdate.Installed = false;
                    childToUpdate.WarehouseId = input.NewWarehouseId;
                    await _assetPartRepository.UpdateAsync(childToUpdate);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Set the AssetPart's ParentId and AssetId to NULL and WarehouseId to the New WarehouseId
            var assetPart = _assetPartRepository.Get(input.AssetPartId);
            assetPart.ParentId = null;
            assetPart.AssetId = null;
            assetPart.WarehouseId = input.NewWarehouseId;
            await _assetPartRepository.UpdateAsync(assetPart);

            var dto = ObjectMapper.Map<AssetPartDto>(assetPart);
            */
            return dto;
        }

        private void Move(int id, int? parentId)
        {
            // NOTE: Originally copied the pattern found in Organization Unit that used the field "Code"
            //          to represent the tree structure, but then discovered that it is not needed, so
            //          commented out the relevant lines of code.  Might be needed in future to enable 
            //          functions such as recursive operations.

            // Get the Part that needs to be moved
            var partToMove = _assetPartRepository.Get(id);
            //var partToMoveOldCode = partToMove.Code;

            // If it has been droped into the root
            if (parentId == null)
            {
                var allParts = _assetPartRepository.GetAll().OrderBy(p => p.Code).ToList();
                //var lastRootCode = allParts.Last().Code.Substring(0, 5);
                //var lastCodeNumber = Convert.ToInt32(lastRootCode);
                //var nextRootCode = lastCodeNumber + 1.ToString().PadLeft(5, '0');

                //partToMove.Code = nextRootCode;
                partToMove.ParentId = null;
                _assetPartRepository.Update(partToMove);
                return;
            }

            // Store the current Parent ID of the part to be moved
            var sourceParentId = partToMove.ParentId;
            var destinationParentId = parentId;

            // Store the original parent code and ID
            //var originalParentId = partToMove.Code;
            //var originalParentCode = (partToMove.Code.Length > 6) ?  partToMove.Code.Substring(0, partToMove.Code.Length - 6) : "";

            // Get the new Parent Part
            var destinationParent = GetDestinationParentOrNull(parentId);

            // find the next code under the new parent... 
            //var nextChildCodeOfDestinationParent = GetNextChildCode(destinationParentId);

            // Update the ParentId, and Code of the moved Part to be the next available code
            //partToMove.Code = string.Format("{0}.{1}", destinationParent.Code, nextChildCodeOfDestinationParent);
            partToMove.ParentId = destinationParent.Id;
            _assetPartRepository.Update(partToMove);

            /*
            // Update the code of all children by replacing the old code of the moved part with the new code of the moved part.
            var allMovedPartChildren = FindChildrenOrNull(partToMove.Id);
            if (allMovedPartChildren != null)
            {
                foreach (var child in allMovedPartChildren)
                {
                    child.Code = string.Format("{0}.{1}", partToMove.Code, child.Code.Substring(partToMoveOldCode.Length));
                    _assetPartRepository.Update(child);
                }
            }

            // Update all the children of the old Parent to ensure sequential codes
            var allChildrenOfOldParent = FindChildrenOrNull(sourceParentId);
            var index = 1;

            if (allChildrenOfOldParent != null)
            {
                foreach (var child in allChildrenOfOldParent)
                {
                    child.Code = string.Format("{0}.{1}", partToMoveOldCode, index.ToString().PadLeft(5, '0'));
                    _assetPartRepository.Update(child);
                    index = index + 1;
                }

                // Note: Need to extend this code to walk down every branch of the tree structure
            }
            */
        }

        private async Task<List<AssetPart>> FindChildrenOrNull(int? parentId, bool recursive = false)
        {
            if (parentId == null) return null;

            List<AssetPart> children = new List<AssetPart>();

            if (!recursive)
            {
                var query = _assetPartRepository.GetAll()
                    .Include(a => a.ItemTypeFk)
                    .Where(p => p.ParentId == parentId).OrderBy(p => p.Code); 
                
                children = await query.ToListAsync();

                if (children != null)
                {
                    if (children.Count != 0)
                    {
                        return children;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            else
            {
                //var parentCode = string.Format("{0}.", _assetPartRepository.Get((int)parentId).Code);
                var parent = _assetPartRepository.GetAll().Where(p => p.Id == parentId).FirstOrDefault();
                var assetId = parent.AssetId;
                var allParts = _assetPartRepository.GetAll().Where(p => p.AssetId == assetId).ToList();
                List<AssetPart> allPartsInBranch = new List<AssetPart>();

                foreach(var part in allParts)
                {
                    bool stillSearching = true;
                    int? parentToTest = part.ParentId;
                    do 
                    {
                        if(parentToTest == null)
                        {
                            stillSearching = false;
                        }
                        if(parentToTest == parentId)
                        {
                            allPartsInBranch.Add(part);
                            stillSearching = false;
                        }
                        if(parentToTest != null)
                        {
                            parentToTest = allParts.Where(p => p.Id == parentToTest).FirstOrDefault().ParentId;
                        }
                    }
                    while (stillSearching);
                }

                if(allPartsInBranch.Count > 0)
                {
                    return allPartsInBranch;
                }

                return null;
                /*
                 * children = _assetPartRepository.GetAll().Where(p => p.Code.Contains(parentCode)).OrderBy(p => p.Code).ToList();
                
                if (children != null)
                {
                    if (children.Count != 0)
                    {
                        return children;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
                */
            }

        }

        private AssetPart GetDestinationParentOrNull(int? parentId)
        {
            if (parentId == null)
            {
                return null;
            }
            return _assetPartRepository.Get((int)parentId);
        }

        private async Task<string> GetNextChildCode(int? parentId)
        {
            var children = await FindChildrenOrNull(parentId);

            if (children != null)
            {
                if (children.Count != 0)
                {
                    var lastChild = children.Last();

                    var codeNumberElements = lastChild.Code.Split('.').ToList();
                    var lastCodeNumber = Convert.ToInt32(codeNumberElements.Last());

                    var nextCodeNumber = lastCodeNumber + 1;
                    var nextCode = nextCodeNumber.ToString().PadLeft(5, '0');

                    return nextCode;
                }
                else
                {
                    return "00001";
                }
            }
            else
            {
                return "00001";
            }
        }

        public async Task<PagedResultDto<GetAssetPartForViewDto>> GetAll(GetAllAssetPartsInput input) 
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var filteredAssetParts = _assetPartRepository.GetAll()
                        .Include(e => e.AssetPartTypeFk)
                        .Include(e => e.ParentFk)
                        .Include(e => e.AssetPartStatusFk)
                        .Include(e => e.AssetFk)
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.WarehouseFk)
                        .WhereIf(input.WarehouseId != null, e => e.WarehouseId == input.WarehouseId)
                        .WhereIf(input.AssetId != null, e => e.AssetId == input.AssetId)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.SerialNumber.Contains(input.Filter) || e.Code.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SerialNumberFilter), e => e.SerialNumber == input.SerialNumberFilter)
                        .WhereIf(input.MinInstallDateFilter != null, e => e.InstallDate >= input.MinInstallDateFilter)
                        .WhereIf(input.MaxInstallDateFilter != null, e => e.InstallDate <= input.MaxInstallDateFilter)
                        .WhereIf(input.InstalledFilter > -1, e => (input.InstalledFilter == 1 && e.Installed) || (input.InstalledFilter == 0 && !e.Installed))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartTypeTypeFilter), e => e.AssetPartTypeFk != null && e.AssetPartTypeFk.Type == input.AssetPartTypeTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartNameFilter), e => e.ParentFk != null && e.ParentFk.Name == input.AssetPartNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartStatusStatusFilter), e => e.AssetPartStatusFk != null && e.AssetPartStatusFk.Status == input.AssetPartStatusStatusFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference == input.AssetReferenceFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter);

            var pagedAndFilteredAssetParts = filteredAssetParts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var assetParts = from o in pagedAndFilteredAssetParts
                             join o1 in _lookup_assetPartTypeRepository.GetAll() on o.AssetPartTypeId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()

                             join o2 in _lookup_assetPartRepository.GetAll() on o.ParentId equals o2.Id into j2
                             from s2 in j2.DefaultIfEmpty()

                             join o3 in _lookup_assetPartStatusRepository.GetAll() on o.AssetPartStatusId equals o3.Id into j3
                             from s3 in j3.DefaultIfEmpty()

                             join o4 in _lookup_assetRepository.GetAll() on o.AssetId equals o4.Id into j4
                             from s4 in j4.DefaultIfEmpty()

                             join o5 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o5.Id into j5
                             from s5 in j5.DefaultIfEmpty()

                             join o6 in _lookup_warehouseRepository.GetAll() on o.WarehouseId equals o6.Id into j6
                             from s6 in j6.DefaultIfEmpty()

                             select new GetAssetPartForViewDto()
                             {
                                 AssetPart = new AssetPartDto
                                 {
                                     Name = o.Name,
                                     Description = o.Description,
                                     SerialNumber = o.SerialNumber,
                                     InstallDate = o.InstallDate,
                                     Code = o.Code,
                                     Installed = o.Installed,
                                     IsItem = o.IsItem,
                                     Qty = o.Qty,
                                     Id = o.Id,
                                     WarehouseId = o.WarehouseId,
                                     AssetId = o.AssetId,
                                     AssetPartStatusId = o.AssetPartStatusId,
                                     AssetPartTypeId = o.AssetPartTypeId,
                                     ItemTypeId = o.ItemTypeId,
                                     ParentId = o.ParentId
                                 },
                                 AssetPartTypeType = s1 == null || s1.Type == null ? "" : s1.Type.ToString(),
                                 AssetPartName = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                                 AssetPartStatusStatus = s3 == null || s3.Status == null ? "" : s3.Status.ToString(),
                                 AssetReference = s4 == null || s4.Reference == null ? "" : s4.Reference.ToString(),
                                 ItemTypeType = s5 == null || s5.Type == null ? "" : s5.Type.ToString(),
                                 WarehouseName = s6 == null || s6.Name == null ? "" : s6.Name.ToString()                             };

            var totalCount = await filteredAssetParts.CountAsync();

            return new PagedResultDto<GetAssetPartForViewDto>(
                totalCount,
                await assetParts.ToListAsync()
            );
        }

        public async Task<GetAssetPartForViewDto> GetAssetPartForView(int id)
        {
            var assetPart = await _assetPartRepository.GetAsync(id);

            var output = new GetAssetPartForViewDto { AssetPart = ObjectMapper.Map<AssetPartDto>(assetPart) };

            if (output.AssetPart.AssetPartTypeId != null)
            {
                var _lookupAssetPartType = await _lookup_assetPartTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartTypeId);
                output.AssetPartTypeType = _lookupAssetPartType?.Type?.ToString();
            }

            if (output.AssetPart.ParentId != null)
            {
                var _lookupAssetPart = await _lookup_assetPartRepository.FirstOrDefaultAsync((int)output.AssetPart.ParentId);
                output.AssetPartName = _lookupAssetPart?.Name?.ToString();
            }

            if (output.AssetPart.AssetPartStatusId != null)
            {
                var _lookupAssetPartStatus = await _lookup_assetPartStatusRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartStatusId);
                output.AssetPartStatusStatus = _lookupAssetPartStatus?.Status?.ToString();
            }

            if (output.AssetPart.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetId);
                output.AssetReference = _lookupAsset?.Reference?.ToString();
            }

            if (output.AssetPart.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Edit)]
        public async Task<GetAssetPartForEditOutput> GetAssetPartForEdit(EntityDto input)
        {
            var assetPart = await _assetPartRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetAssetPartForEditOutput { AssetPart = ObjectMapper.Map<CreateOrEditAssetPartDto>(assetPart) };

            if (output.AssetPart.AssetPartTypeId != null)
            {
                var _lookupAssetPartType = await _lookup_assetPartTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartTypeId);
                output.AssetPartTypeType = _lookupAssetPartType?.Type?.ToString();
            }

            if (output.AssetPart.ParentId != null)
            {
                var _lookupAssetPart = await _lookup_assetPartRepository.FirstOrDefaultAsync((int)output.AssetPart.ParentId);
                output.AssetPartName = _lookupAssetPart?.Name?.ToString();
            }

            if (output.AssetPart.AssetPartStatusId != null)
            {
                var _lookupAssetPartStatus = await _lookup_assetPartStatusRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartStatusId);
                output.AssetPartStatusStatus = _lookupAssetPartStatus?.Status?.ToString();
            }

            if (output.AssetPart.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetId);
                output.AssetReference = _lookupAsset?.Reference?.ToString();
            }

            if (output.AssetPart.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

            return output;
        }

        public async Task<CreateOrEditAssetPartDto> CreateOrEdit(CreateOrEditAssetPartDto input)
        {
            input.Code = string.Format("{0}{1}", input.AssetId.ToString(), (input.Id != null) ? input.Id.ToString() : "000");
            if (input.Id == null)
            {
                input.Id = await Create(input);
            }
            else
            {
                await Update(input);
            }
            return ObjectMapper.Map<CreateOrEditAssetPartDto>(input);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Create)]
        protected virtual async Task<int> Create(CreateOrEditAssetPartDto input)
        {
            var assetPart = ObjectMapper.Map<AssetPart>(input);

            if (AbpSession.TenantId != null)
            {
                assetPart.TenantId = (int?)AbpSession.TenantId;
            }

            await _assetPartRepository.InsertAsync(assetPart);
            await CurrentUnitOfWork.SaveChangesAsync();

            return assetPart.Id;
        }


        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Edit)]
        protected virtual async Task Update(CreateOrEditAssetPartDto input)
        {
            var assetPart = await _assetPartRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, assetPart);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Delete)]
        public async Task Delete(EntityDto input)
        {
            await DeleteComponent(input);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Delete)]
        public async Task DeleteComponent(EntityDto input)
        {
            var children = await FindChildrenOrNull(input.Id, false);
            
            if (children !=null)
            {
                foreach(var child in children)
                {
                    child.ParentId = null;
                    await _assetPartRepository.UpdateAsync(child);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            await _assetPartRepository.DeleteAsync(input.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Delete)]
        public async Task DeleteBranch(EntityDto input)
        {
            var children = await FindChildrenOrNull(input.Id, true);

            if (children != null)
            {
                foreach (var child in children)
                {
                    child.ParentId = null;
                    await _assetPartRepository.DeleteAsync(child.Id);
                }
            }
            await _assetPartRepository.DeleteAsync(input.Id); 
        }

        public async Task<FileDto> GetAssetPartsToExcel(GetAllAssetPartsForExcelInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var filteredAssetParts = _assetPartRepository.GetAll()
                        .Include(e => e.AssetPartTypeFk)
                        .Include(e => e.ParentFk)
                        .Include(e => e.AssetPartStatusFk)
                        .Include(e => e.AssetFk)
                        .Include(e => e.ItemTypeFk)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.SerialNumber.Contains(input.Filter) || e.Code.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SerialNumberFilter), e => e.SerialNumber == input.SerialNumberFilter)
                        .WhereIf(input.MinInstallDateFilter != null, e => e.InstallDate >= input.MinInstallDateFilter)
                        .WhereIf(input.MaxInstallDateFilter != null, e => e.InstallDate <= input.MaxInstallDateFilter)
                        .WhereIf(input.InstalledFilter > -1, e => (input.InstalledFilter == 1 && e.Installed) || (input.InstalledFilter == 0 && !e.Installed))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartTypeTypeFilter), e => e.AssetPartTypeFk != null && e.AssetPartTypeFk.Type == input.AssetPartTypeTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartNameFilter), e => e.ParentFk != null && e.ParentFk.Name == input.AssetPartNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartStatusStatusFilter), e => e.AssetPartStatusFk != null && e.AssetPartStatusFk.Status == input.AssetPartStatusStatusFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference == input.AssetReferenceFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter);

            var query = (from o in filteredAssetParts
                         join o1 in _lookup_assetPartTypeRepository.GetAll() on o.AssetPartTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_assetPartRepository.GetAll() on o.ParentId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_assetPartStatusRepository.GetAll() on o.AssetPartStatusId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_assetRepository.GetAll() on o.AssetId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         join o5 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()

                         select new GetAssetPartForViewDto()
                         {
                             AssetPart = new AssetPartDto
                             {
                                 Name = o.Name,
                                 Description = o.Description,
                                 SerialNumber = o.SerialNumber,
                                 InstallDate = o.InstallDate,
                                 Code = o.Code,
                                 Installed = o.Installed,
                                 IsItem = o.IsItem,
                                 Qty = o.Qty,
                                 Id = o.Id
                             },
                             AssetPartTypeType = s1 == null || s1.Type == null ? "" : s1.Type.ToString(),
                             AssetPartName = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                             AssetPartStatusStatus = s3 == null || s3.Status == null ? "" : s3.Status.ToString(),
                             AssetReference = s4 == null || s4.Reference == null ? "" : s4.Reference.ToString(),
                             ItemTypeType = s5 == null || s5.Type == null ? "" : s5.Type.ToString()
                         });


            var assetPartListDtos = await query.ToListAsync();

            return _assetPartsExcelExporter.ExportToFile(assetPartListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
        public async Task<PagedResultDto<AssetPartAssetPartTypeLookupTableDto>> GetAllAssetPartTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_assetPartTypeRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type != null && e.Type.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetPartTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AssetPartAssetPartTypeLookupTableDto>();
            foreach (var assetPartType in assetPartTypeList)
            {
                lookupTableDtoList.Add(new AssetPartAssetPartTypeLookupTableDto
                {
                    Id = assetPartType.Id,
                    DisplayName = assetPartType.Type?.ToString()
                });
            }

            return new PagedResultDto<AssetPartAssetPartTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
        public async Task<PagedResultDto<AssetPartAssetPartLookupTableDto>> GetAllAssetPartForLookupTable(GetAllAssetPartsForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_assetPartRepository.GetAll()
                .WhereIf(input.ForImportFromWarehouses == true, e => !e.Installed && e.WarehouseId != null)
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetPartList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AssetPartAssetPartLookupTableDto>();
            foreach (var assetPart in assetPartList)
            {
                lookupTableDtoList.Add(new AssetPartAssetPartLookupTableDto
                {
                    Id = assetPart.Id,
                    DisplayName = assetPart.Name?.ToString()
                });
            }

            return new PagedResultDto<AssetPartAssetPartLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
        public async Task<PagedResultDto<AssetPartAssetPartStatusLookupTableDto>> GetAllAssetPartStatusForLookupTable(GetAllForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_assetPartStatusRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Status != null && e.Status.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetPartStatusList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AssetPartAssetPartStatusLookupTableDto>();
            foreach (var assetPartStatus in assetPartStatusList)
            {
                lookupTableDtoList.Add(new AssetPartAssetPartStatusLookupTableDto
                {
                    Id = assetPartStatus.Id,
                    DisplayName = assetPartStatus.Status?.ToString()
                });
            }

            return new PagedResultDto<AssetPartAssetPartStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
        public async Task<PagedResultDto<AssetPartAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_assetRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Reference != null && e.Reference.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AssetPartAssetLookupTableDto>();
            foreach (var asset in assetList)
            {
                lookupTableDtoList.Add(new AssetPartAssetLookupTableDto
                {
                    Id = asset.Id,
                    DisplayName = asset.Reference?.ToString()
                });
            }

            return new PagedResultDto<AssetPartAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
        public async Task<PagedResultDto<AssetPartItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_itemTypeRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type != null && e.Type.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var itemTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AssetPartItemTypeLookupTableDto>();
            foreach (var itemType in itemTypeList)
            {
                lookupTableDtoList.Add(new AssetPartItemTypeLookupTableDto
                {
                    Id = itemType.Id,
                    DisplayName = itemType.Type?.ToString()
                });
            }

            return new PagedResultDto<AssetPartItemTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }


        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
        public async Task<PagedResultDto<AssetPartWarehouseLookupTableDto>> GetAllWarehouseForLookupTable(GetAllForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_warehouseRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var warehouseList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AssetPartWarehouseLookupTableDto>();
            foreach (var warehouse in warehouseList)
            {
                lookupTableDtoList.Add(new AssetPartWarehouseLookupTableDto
                {
                    Id = warehouse.Id,
                    DisplayName = warehouse.Name?.ToString()
                });
            }

            return new PagedResultDto<AssetPartWarehouseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}


/*
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
	[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
    public class AssetPartsAppService : EmsAppServiceBase, IAssetPartsAppService
    {
		 private readonly IRepository<AssetPart> _assetPartRepository;
		 private readonly IAssetPartsExcelExporter _assetPartsExcelExporter;
		 private readonly IRepository<AssetPartType,int> _lookup_assetPartTypeRepository;
		 private readonly IRepository<AssetPart,int> _lookup_assetPartRepository;
		 private readonly IRepository<AssetPartStatus,int> _lookup_assetPartStatusRepository;
		 private readonly IRepository<Asset,int> _lookup_assetRepository;
		 private readonly IRepository<ItemType,int> _lookup_itemTypeRepository;
		 private readonly IRepository<Warehouse,int> _lookup_warehouseRepository;
		 

		  public AssetPartsAppService(IRepository<AssetPart> assetPartRepository, IAssetPartsExcelExporter assetPartsExcelExporter , IRepository<AssetPartType, int> lookup_assetPartTypeRepository, IRepository<AssetPart, int> lookup_assetPartRepository, IRepository<AssetPartStatus, int> lookup_assetPartStatusRepository, IRepository<Asset, int> lookup_assetRepository, IRepository<ItemType, int> lookup_itemTypeRepository, IRepository<Warehouse, int> lookup_warehouseRepository) 
		  {
			_assetPartRepository = assetPartRepository;
			_assetPartsExcelExporter = assetPartsExcelExporter;
			_lookup_assetPartTypeRepository = lookup_assetPartTypeRepository;
		    _lookup_assetPartRepository = lookup_assetPartRepository;
		    _lookup_assetPartStatusRepository = lookup_assetPartStatusRepository;
		    _lookup_assetRepository = lookup_assetRepository;
		    _lookup_itemTypeRepository = lookup_itemTypeRepository;
		    _lookup_warehouseRepository = lookup_warehouseRepository;
		
		  }

		 public async Task<PagedResultDto<GetAssetPartForViewDto>> GetAll(GetAllAssetPartsInput input)
         {
			
			var filteredAssetParts = _assetPartRepository.GetAll()
						.Include( e => e.AssetPartTypeFk)
						.Include( e => e.ParentFk)
						.Include( e => e.AssetPartStatusFk)
						.Include( e => e.AssetFk)
						.Include( e => e.ItemTypeFk)
						.Include( e => e.WarehouseFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.SerialNumber.Contains(input.Filter) || e.Code.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description == input.DescriptionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.SerialNumberFilter),  e => e.SerialNumber == input.SerialNumberFilter)
						.WhereIf(input.MinInstallDateFilter != null, e => e.InstallDate >= input.MinInstallDateFilter)
						.WhereIf(input.MaxInstallDateFilter != null, e => e.InstallDate <= input.MaxInstallDateFilter)
						.WhereIf(input.InstalledFilter.HasValue && input.InstalledFilter > -1,  e => (input.InstalledFilter == 1 && e.Installed) || (input.InstalledFilter == 0 && !e.Installed) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartTypeTypeFilter), e => e.AssetPartTypeFk != null && e.AssetPartTypeFk.Type == input.AssetPartTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartNameFilter), e => e.ParentFk != null && e.ParentFk.Name == input.AssetPartNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartStatusStatusFilter), e => e.AssetPartStatusFk != null && e.AssetPartStatusFk.Status == input.AssetPartStatusStatusFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference == input.AssetReferenceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.WarehouseNameFilter), e => e.WarehouseFk != null && e.WarehouseFk.Name == input.WarehouseNameFilter);

			var pagedAndFilteredAssetParts = filteredAssetParts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var assetParts = from o in pagedAndFilteredAssetParts
                         join o1 in _lookup_assetPartTypeRepository.GetAll() on o.AssetPartTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetPartRepository.GetAll() on o.ParentId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_assetPartStatusRepository.GetAll() on o.AssetPartStatusId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_assetRepository.GetAll() on o.AssetId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         join o5 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()
                         
                         join o6 in _lookup_warehouseRepository.GetAll() on o.WarehouseId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()
                         
                         select new GetAssetPartForViewDto() {
							AssetPart = new AssetPartDto
							{
                                Name = o.Name,
                                Description = o.Description,
                                SerialNumber = o.SerialNumber,
                                InstallDate = o.InstallDate,
                                Code = o.Code,
                                Installed = o.Installed,
                                Id = o.Id
							},
                         	AssetPartTypeType = s1 == null || s1.Type == null ? "" : s1.Type.ToString(),
                         	AssetPartName = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                         	AssetPartStatusStatus = s3 == null || s3.Status == null ? "" : s3.Status.ToString(),
                         	AssetReference = s4 == null || s4.Reference == null ? "" : s4.Reference.ToString(),
                         	ItemTypeType = s5 == null || s5.Type == null ? "" : s5.Type.ToString(),
                         	WarehouseName = s6 == null || s6.Name == null ? "" : s6.Name.ToString()
						};

            var totalCount = await filteredAssetParts.CountAsync();

            return new PagedResultDto<GetAssetPartForViewDto>(
                totalCount,
                await assetParts.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetPartForViewDto> GetAssetPartForView(int id)
         {
            var assetPart = await _assetPartRepository.GetAsync(id);

            var output = new GetAssetPartForViewDto { AssetPart = ObjectMapper.Map<AssetPartDto>(assetPart) };

		    if (output.AssetPart.AssetPartTypeId != null)
            {
                var _lookupAssetPartType = await _lookup_assetPartTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartTypeId);
                output.AssetPartTypeType = _lookupAssetPartType?.Type?.ToString();
            }

		    if (output.AssetPart.ParentId != null)
            {
                var _lookupAssetPart = await _lookup_assetPartRepository.FirstOrDefaultAsync((int)output.AssetPart.ParentId);
                output.AssetPartName = _lookupAssetPart?.Name?.ToString();
            }

		    if (output.AssetPart.AssetPartStatusId != null)
            {
                var _lookupAssetPartStatus = await _lookup_assetPartStatusRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartStatusId);
                output.AssetPartStatusStatus = _lookupAssetPartStatus?.Status?.ToString();
            }

		    if (output.AssetPart.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetId);
                output.AssetReference = _lookupAsset?.Reference?.ToString();
            }

		    if (output.AssetPart.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

		    if (output.AssetPart.WarehouseId != null)
            {
                var _lookupWarehouse = await _lookup_warehouseRepository.FirstOrDefaultAsync((int)output.AssetPart.WarehouseId);
                output.WarehouseName = _lookupWarehouse?.Name?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Edit)]
		 public async Task<GetAssetPartForEditOutput> GetAssetPartForEdit(EntityDto input)
         {
            var assetPart = await _assetPartRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetPartForEditOutput {AssetPart = ObjectMapper.Map<CreateOrEditAssetPartDto>(assetPart)};

		    if (output.AssetPart.AssetPartTypeId != null)
            {
                var _lookupAssetPartType = await _lookup_assetPartTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartTypeId);
                output.AssetPartTypeType = _lookupAssetPartType?.Type?.ToString();
            }

		    if (output.AssetPart.ParentId != null)
            {
                var _lookupAssetPart = await _lookup_assetPartRepository.FirstOrDefaultAsync((int)output.AssetPart.ParentId);
                output.AssetPartName = _lookupAssetPart?.Name?.ToString();
            }

		    if (output.AssetPart.AssetPartStatusId != null)
            {
                var _lookupAssetPartStatus = await _lookup_assetPartStatusRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartStatusId);
                output.AssetPartStatusStatus = _lookupAssetPartStatus?.Status?.ToString();
            }

		    if (output.AssetPart.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetId);
                output.AssetReference = _lookupAsset?.Reference?.ToString();
            }

		    if (output.AssetPart.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

		    if (output.AssetPart.WarehouseId != null)
            {
                var _lookupWarehouse = await _lookup_warehouseRepository.FirstOrDefaultAsync((int)output.AssetPart.WarehouseId);
                output.WarehouseName = _lookupWarehouse?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAssetPartDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Create)]
		 protected virtual async Task Create(CreateOrEditAssetPartDto input)
         {
            var assetPart = ObjectMapper.Map<AssetPart>(input);

			
			if (AbpSession.TenantId != null)
			{
				assetPart.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _assetPartRepository.InsertAsync(assetPart);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetPartDto input)
         {
            var assetPart = await _assetPartRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetPart);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetPartRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetAssetPartsToExcel(GetAllAssetPartsForExcelInput input)
         {
			
			var filteredAssetParts = _assetPartRepository.GetAll()
						.Include( e => e.AssetPartTypeFk)
						.Include( e => e.ParentFk)
						.Include( e => e.AssetPartStatusFk)
						.Include( e => e.AssetFk)
						.Include( e => e.ItemTypeFk)
						.Include( e => e.WarehouseFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.SerialNumber.Contains(input.Filter) || e.Code.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description == input.DescriptionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.SerialNumberFilter),  e => e.SerialNumber == input.SerialNumberFilter)
						.WhereIf(input.MinInstallDateFilter != null, e => e.InstallDate >= input.MinInstallDateFilter)
						.WhereIf(input.MaxInstallDateFilter != null, e => e.InstallDate <= input.MaxInstallDateFilter)
						.WhereIf(input.InstalledFilter.HasValue && input.InstalledFilter > -1,  e => (input.InstalledFilter == 1 && e.Installed) || (input.InstalledFilter == 0 && !e.Installed) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartTypeTypeFilter), e => e.AssetPartTypeFk != null && e.AssetPartTypeFk.Type == input.AssetPartTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartNameFilter), e => e.ParentFk != null && e.ParentFk.Name == input.AssetPartNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartStatusStatusFilter), e => e.AssetPartStatusFk != null && e.AssetPartStatusFk.Status == input.AssetPartStatusStatusFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference == input.AssetReferenceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.WarehouseNameFilter), e => e.WarehouseFk != null && e.WarehouseFk.Name == input.WarehouseNameFilter);

			var query = (from o in filteredAssetParts
                         join o1 in _lookup_assetPartTypeRepository.GetAll() on o.AssetPartTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetPartRepository.GetAll() on o.ParentId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_assetPartStatusRepository.GetAll() on o.AssetPartStatusId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_assetRepository.GetAll() on o.AssetId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         join o5 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()
                         
                         join o6 in _lookup_warehouseRepository.GetAll() on o.WarehouseId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()
                         
                         select new GetAssetPartForViewDto() { 
							AssetPart = new AssetPartDto
							{
                                Name = o.Name,
                                Description = o.Description,
                                SerialNumber = o.SerialNumber,
                                InstallDate = o.InstallDate,
                                Code = o.Code,
                                Installed = o.Installed,
                                Id = o.Id
							},
                         	AssetPartTypeType = s1 == null || s1.Type == null ? "" : s1.Type.ToString(),
                         	AssetPartName = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                         	AssetPartStatusStatus = s3 == null || s3.Status == null ? "" : s3.Status.ToString(),
                         	AssetReference = s4 == null || s4.Reference == null ? "" : s4.Reference.ToString(),
                         	ItemTypeType = s5 == null || s5.Type == null ? "" : s5.Type.ToString(),
                         	WarehouseName = s6 == null || s6.Name == null ? "" : s6.Name.ToString()
						 });


            var assetPartListDtos = await query.ToListAsync();

            return _assetPartsExcelExporter.ExportToFile(assetPartListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAssetPartTypeLookupTableDto>> GetAllAssetPartTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetPartTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type != null && e.Type.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetPartTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAssetPartTypeLookupTableDto>();
			foreach(var assetPartType in assetPartTypeList){
				lookupTableDtoList.Add(new AssetPartAssetPartTypeLookupTableDto
				{
					Id = assetPartType.Id,
					DisplayName = assetPartType.Type?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAssetPartTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAssetPartLookupTableDto>> GetAllAssetPartForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetPartRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetPartList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAssetPartLookupTableDto>();
			foreach(var assetPart in assetPartList){
				lookupTableDtoList.Add(new AssetPartAssetPartLookupTableDto
				{
					Id = assetPart.Id,
					DisplayName = assetPart.Name?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAssetPartLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAssetPartStatusLookupTableDto>> GetAllAssetPartStatusForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetPartStatusRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Status != null && e.Status.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetPartStatusList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAssetPartStatusLookupTableDto>();
			foreach(var assetPartStatus in assetPartStatusList){
				lookupTableDtoList.Add(new AssetPartAssetPartStatusLookupTableDto
				{
					Id = assetPartStatus.Id,
					DisplayName = assetPartStatus.Status?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAssetPartStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Reference != null && e.Reference.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAssetLookupTableDto>();
			foreach(var asset in assetList){
				lookupTableDtoList.Add(new AssetPartAssetLookupTableDto
				{
					Id = asset.Id,
					DisplayName = asset.Reference?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_itemTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type != null && e.Type.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var itemTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartItemTypeLookupTableDto>();
			foreach(var itemType in itemTypeList){
				lookupTableDtoList.Add(new AssetPartItemTypeLookupTableDto
				{
					Id = itemType.Id,
					DisplayName = itemType.Type?.ToString()
				});
			}

            return new PagedResultDto<AssetPartItemTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartWarehouseLookupTableDto>> GetAllWarehouseForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_warehouseRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var warehouseList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartWarehouseLookupTableDto>();
			foreach(var warehouse in warehouseList){
				lookupTableDtoList.Add(new AssetPartWarehouseLookupTableDto
				{
					Id = warehouse.Id,
					DisplayName = warehouse.Name?.ToString()
				});
			}

            return new PagedResultDto<AssetPartWarehouseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}
*/