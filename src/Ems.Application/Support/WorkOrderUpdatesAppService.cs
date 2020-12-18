using Ems.Support;
using Ems.Quotations;
using Ems.Assets;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Support.Exporting;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

using Abp.Web.Mvc.Models;
using Ems.Storage;
using Abp.UI;
using Ems.Metrics;
using System;
using Ems.Assets.Dtos;

namespace Ems.Support
{
	[AbpAuthorize(AppPermissions.Pages_WorkOrderUpdates)]
    public class WorkOrderUpdatesAppService : EmsAppServiceBase, IWorkOrderUpdatesAppService
    {
        private readonly string _entityType = "WorkOrderUpdate";
        private readonly IRepository<WorkOrderUpdate> _workOrderUpdateRepository;
		private readonly IWorkOrderUpdatesExcelExporter _workOrderUpdatesExcelExporter;
		private readonly IRepository<WorkOrder,int> _lookup_workOrderRepository;
		private readonly IRepository<ItemType,int> _lookup_itemTypeRepository;
		private readonly IRepository<WorkOrderAction,int> _lookup_workOrderActionRepository;
		private readonly IRepository<AssetPart,int> _lookup_assetPartRepository;
        private readonly IRepository<Attachment, int> _lookup_attachmentRepository;


        public WorkOrderUpdatesAppService(IRepository<WorkOrderUpdate> workOrderUpdateRepository, IWorkOrderUpdatesExcelExporter workOrderUpdatesExcelExporter , IRepository<WorkOrder, int> lookup_workOrderRepository, IRepository<ItemType, int> lookup_itemTypeRepository, IRepository<WorkOrderAction, int> lookup_workOrderActionRepository, IRepository<AssetPart, int> lookup_assetPartRepository, IRepository<Attachment, int> lookup_attachmentRepository) 
		  {
			_workOrderUpdateRepository = workOrderUpdateRepository;
			_workOrderUpdatesExcelExporter = workOrderUpdatesExcelExporter;
			_lookup_workOrderRepository = lookup_workOrderRepository;
		    _lookup_itemTypeRepository = lookup_itemTypeRepository;
		    _lookup_workOrderActionRepository = lookup_workOrderActionRepository;
		    _lookup_assetPartRepository = lookup_assetPartRepository;
            _lookup_attachmentRepository = lookup_attachmentRepository;

        }

		 public async Task<PagedResultDto<GetWorkOrderUpdateForViewDto>> GetAll(GetAllWorkOrderUpdatesInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo(); //returns asset owe
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var filteredWorkOrderUpdates = _workOrderUpdateRepository.GetAll()
						.Include( e => e.WorkOrderFk)
						.Include( e => e.ItemTypeFk)
						.Include( e => e.WorkOrderActionFk)
						.Include( e => e.AssetPartFk)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(input.WorkOrderId > 0, e => e.WorkOrderId == input.WorkOrderId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Comments.Contains(input.Filter))
						//.WhereIf(!string.IsNullOrWhiteSpace(input.CommentsFilter),  e => e.Comments == input.CommentsFilter)
						//.WhereIf(input.MinNumberFilter != null, e => e.Number >= input.MinNumberFilter)
						//.WhereIf(input.MaxNumberFilter != null, e => e.Number <= input.MaxNumberFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject == input.WorkOrderSubjectFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderActionActionFilter), e => e.WorkOrderActionFk != null && e.WorkOrderActionFk.Action == input.WorkOrderActionActionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartNameFilter), e => e.AssetPartFk != null && e.AssetPartFk.Name == input.AssetPartNameFilter);

			var pagedAndFilteredWorkOrderUpdates = filteredWorkOrderUpdates
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var workOrderUpdates = from o in pagedAndFilteredWorkOrderUpdates
                         join o1 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_assetPartRepository.GetAll() on o.AssetPartId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         select new GetWorkOrderUpdateForViewDto() {
							WorkOrderUpdate = new WorkOrderUpdateDto
							{
                                Comments = o.Comments,
                                Number = o.Number,
                                Completed = o.Completed,
                                Id = o.Id,
                                ItemTypeId = o.ItemTypeId,
                                WorkOrderActionId = o.WorkOrderActionId,
                                WorkOrderId = o.WorkOrderId,
                                AssetPartId = o.AssetPartId
                            },
                         	WorkOrderSubject = s1 == null || s1.Subject == null ? "" : s1.Subject.ToString(),
                         	ItemTypeType = s2 == null || s2.Type == null ? "" : s2.Type.ToString(),
                         	WorkOrderActionAction = s3 == null || s3.Action == null ? "" : s3.Action.ToString(),
                         	AssetPartName = s4 == null || s4.Name == null ? "" : s4.Name.ToString()
						};

            var totalCount = await filteredWorkOrderUpdates.CountAsync();

            return new PagedResultDto<GetWorkOrderUpdateForViewDto>(
                totalCount,
                await workOrderUpdates.ToListAsync()
            );
         }

        public async Task<List<GetWorkorderUpdateItemsForViewDto>> GetWorkorderItems(GetWorkorderUpdateItemsInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "WorkOrder");

            var workOrders = _workOrderUpdateRepository.GetAll()
                                .Where(w => w.ItemTypeFk != null)
                                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                                .Include(w => w.ItemTypeFk)
                                .Where(w => !w.IsDeleted)
                                .Where(w => w.CreationTime >= DateTime.Now.AddDays(-input.Days))
                                .ToList();

            var itemsConsumed =
                    from w in workOrders
                    group w by w.ItemTypeFk into s
                    select new GetWorkorderUpdateItemsForViewDto()
                    {
                        Item = new Ems.Quotations.Dtos.ItemTypeDto
                        {
                            Type = s.Key.Type,
                            Description = s.Key.Description
                        },
                        Consumed = s.Count()
                    };

            var result = itemsConsumed.OrderByDescending(a => a.Consumed).Take(10).ToList();

            return result;
        }

        public async Task<GetWorkOrderUpdateForViewDto> GetWorkOrderUpdateForView(int id)
         {
            var workOrderUpdate = await _workOrderUpdateRepository.GetAsync(id);

            var output = new GetWorkOrderUpdateForViewDto { WorkOrderUpdate = ObjectMapper.Map<WorkOrderUpdateDto>(workOrderUpdate) };

		    if (output.WorkOrderUpdate.WorkOrderId > 0)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder?.Subject?.ToString();
            }

		    if (output.WorkOrderUpdate.ItemTypeId > 0)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

		    if (output.WorkOrderUpdate.WorkOrderActionId > 0)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.WorkOrderActionId);
                output.WorkOrderActionAction = _lookupWorkOrderAction?.Action?.ToString();
            }

		    if (output.WorkOrderUpdate.AssetPartId != null)
            {
                var _lookupAssetPart = await _lookup_assetPartRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.AssetPartId);
                output.AssetPartName = _lookupAssetPart?.Name?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_WorkOrderUpdates_Edit)]
		 public async Task<GetWorkOrderUpdateForEditOutput> GetWorkOrderUpdateForEdit(EntityDto input)
         {
            var workOrderUpdate = await _workOrderUpdateRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetWorkOrderUpdateForEditOutput {WorkOrderUpdate = ObjectMapper.Map<CreateOrEditWorkOrderUpdateDto>(workOrderUpdate)};

		    if (output.WorkOrderUpdate.WorkOrderId > 0)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.WorkOrderId);
                output.WorkOrderUpdateSubject = _lookupWorkOrder?.Subject?.ToString();
            }

		    if (output.WorkOrderUpdate.ItemTypeId > 0)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

		    if (output.WorkOrderUpdate.WorkOrderActionId > 0)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.WorkOrderActionId);
                output.WorkOrderUpdateActionAction = _lookupWorkOrderAction?.Action?.ToString();
            }

		    if (output.WorkOrderUpdate.AssetPartId != null)
            {
                var _lookupAssetPart = await _lookup_assetPartRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.AssetPartId);
                output.AssetPartName = _lookupAssetPart?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditWorkOrderUpdateDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_WorkOrderUpdates_Create)]
		 protected virtual async Task Create(CreateOrEditWorkOrderUpdateDto input)
         {
            ErrorViewModel errorInfo = CheckWOUpdatesValidToSetComplete(input.WorkOrderId, input.Id ?? 0, input.ItemTypeId ?? 0, input.WorkOrderActionId);
            if (errorInfo == null)
            {
                var workOrderUpdate = ObjectMapper.Map<WorkOrderUpdate>(input);

                if (AbpSession.TenantId != null)
                    workOrderUpdate.TenantId = (int?)AbpSession.TenantId;

                await _workOrderUpdateRepository.InsertAsync(workOrderUpdate);
            }
            else
                throw new UserFriendlyException(errorInfo.ErrorInfo.Message, errorInfo.ErrorInfo.Details);
        }

        [AbpAuthorize(AppPermissions.Pages_WorkOrderUpdates_Edit)]
        protected virtual async Task Update(CreateOrEditWorkOrderUpdateDto input)
        {
            ErrorViewModel errorInfo = CheckWOUpdatesValidToSetComplete(input.WorkOrderId, input.Id ?? 0, input.ItemTypeId ?? 0, input.WorkOrderActionId);
            if (errorInfo == null)
            {
                var workOrderUpdate = await _workOrderUpdateRepository.FirstOrDefaultAsync((int)input.Id);
                ObjectMapper.Map(input, workOrderUpdate);
            }
            else
                throw new UserFriendlyException(errorInfo.ErrorInfo.Message, errorInfo.ErrorInfo.Details);
        }

        [AbpAuthorize(AppPermissions.Pages_WorkOrderUpdates_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _workOrderUpdateRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetWorkOrderUpdatesToExcel(GetAllWorkOrderUpdatesForExcelInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo(); //returns asset owe
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);


            var filteredWorkOrderUpdates = _workOrderUpdateRepository.GetAll()
						.Include( e => e.WorkOrderFk)
						.Include( e => e.ItemTypeFk)
						.Include( e => e.WorkOrderActionFk)
						.Include( e => e.AssetPartFk)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Comments.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.CommentsFilter),  e => e.Comments == input.CommentsFilter)
						.WhereIf(input.MinNumberFilter != null, e => e.Number >= input.MinNumberFilter)
						.WhereIf(input.MaxNumberFilter != null, e => e.Number <= input.MaxNumberFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject == input.WorkOrderSubjectFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderActionActionFilter), e => e.WorkOrderActionFk != null && e.WorkOrderActionFk.Action == input.WorkOrderActionActionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartNameFilter), e => e.AssetPartFk != null && e.AssetPartFk.Name == input.AssetPartNameFilter);

			var query = (from o in filteredWorkOrderUpdates
                         join o1 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_assetPartRepository.GetAll() on o.AssetPartId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         select new GetWorkOrderUpdateForViewDto() { 
							WorkOrderUpdate = new WorkOrderUpdateDto
							{
                                Comments = o.Comments,
                                Number = o.Number,
                                Id = o.Id,
                                Completed = o.Completed
							},
                         	WorkOrderSubject = s1 == null || s1.Subject == null ? "" : s1.Subject.ToString(),
                         	ItemTypeType = s2 == null || s2.Type == null ? "" : s2.Type.ToString(),
                         	WorkOrderActionAction = s3 == null || s3.Action == null ? "" : s3.Action.ToString(),
                         	AssetPartName = s4 == null || s4.Name == null ? "" : s4.Name.ToString()
						 });


            var workOrderUpdateListDtos = await query.ToListAsync();

            return _workOrderUpdatesExcelExporter.ExportToFile(workOrderUpdateListDtos);
         }

        protected ErrorViewModel CheckWOUpdatesValidToSetComplete(int woId, int woUpdateId, int woItmTypeId, int woActionId)
        {
            if (woItmTypeId == 0)
                return null;

            ErrorViewModel errorInfo = new ErrorViewModel();
            errorInfo.ErrorInfo = new Abp.Web.Models.ErrorInfo();

            var woItemTypeInfo = _lookup_itemTypeRepository.FirstOrDefault(woItmTypeId);
            if (woItemTypeInfo != null)
            {
                if (woItemTypeInfo.Type.ToLower() != "gpu load test")
                    return null;

                var woActionInfo = _lookup_workOrderActionRepository.FirstOrDefault(woActionId);
                if (woActionInfo != null)
                {
                    if (woActionInfo.Action.ToLower() != "completed")
                        return null;
                    else
                    {
                        int attachmentCount = _lookup_attachmentRepository.GetAll()
                            //.WhereIf(woStartDate != null, w => w.CreationTime > woStartDate || w.LastModificationTime > woStartDate)
                            .Where(w => w.WorkOrderId == woId)
                            .Count();

                        if (attachmentCount > 0)
                            return null;
                        else
                        {
                            errorInfo.ErrorInfo.Message = "Need attention";
                            errorInfo.ErrorInfo.Details = "Please add attachment before completing the case";
                            return errorInfo;
                        }
                    }
                }
                else
                {
                    errorInfo.ErrorInfo.Message = "Error";
                    errorInfo.ErrorInfo.Details = "Workorder Action not found";
                    return errorInfo;
                }
            }
            else
            {
                errorInfo.ErrorInfo.Message = "Error";
                errorInfo.ErrorInfo.Details = "Workorder ItemType not found";
                return errorInfo;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_WorkOrderUpdates)]
        public async void CompleteWorkOrderUpdate(int id)
        {
            var workOrderUpdate = _workOrderUpdateRepository.GetAsync(id).Result;

            if(workOrderUpdate != null)
            {
                workOrderUpdate.Completed = true;

                await _workOrderUpdateRepository.UpdateAsync(workOrderUpdate);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_WorkOrderUpdates)]
         public async Task<PagedResultDto<WorkOrderUpdateAssetPartLookupTableDto>> GetAllAssetPartForLookupTable(GetAllAssetPartsForLookupTableInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_assetPartRepository.GetAll()
                
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                )
                .Where(a => !a.IsItem && !a.Installed && a.WarehouseId != null || !a.IsItem && a.AssetId == input.AssetId);  // only select Components that are in a Warehouse or already installed on this Asset 

            var totalCount = await query.CountAsync();

            var assetPartList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<WorkOrderUpdateAssetPartLookupTableDto>();
			foreach(var assetPart in assetPartList){
				lookupTableDtoList.Add(new WorkOrderUpdateAssetPartLookupTableDto
				{
					Id = assetPart.Id,
					DisplayName = assetPart.Name?.ToString()
				});
			}

            return new PagedResultDto<WorkOrderUpdateAssetPartLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

        public async Task<PagedResultDto<WorkOrderUpdateWorkOrderActionLookupTableDto>> GetAllWorkOrderActionForLookupTable(Assets.Dtos.GetAllForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo(); 
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var query = _lookup_workOrderActionRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Action.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderActionList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderUpdateWorkOrderActionLookupTableDto>();
            foreach (var workOrderAction in workOrderActionList)
            {
                lookupTableDtoList.Add(new WorkOrderUpdateWorkOrderActionLookupTableDto
                {
                    Id = workOrderAction.Id,
                    DisplayName = workOrderAction.Action?.ToString()
                });
            }

            return new PagedResultDto<WorkOrderUpdateWorkOrderActionLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}