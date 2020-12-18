using Ems.Support;
using Ems.Quotations;
using Ems.Support;

using System;
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

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Administration_MaintenanceSteps)]
    public class MaintenanceStepsAppService : EmsAppServiceBase, IMaintenanceStepsAppService
    {
        private readonly IRepository<MaintenanceStep> _maintenanceStepRepository;
        private readonly IMaintenanceStepsExcelExporter _maintenanceStepsExcelExporter;
        private readonly IRepository<MaintenancePlan, int> _lookup_maintenancePlanRepository;
        private readonly IRepository<ItemType, int> _lookup_itemTypeRepository;
        private readonly IRepository<WorkOrderAction, int> _lookup_workOrderActionRepository;

        public MaintenanceStepsAppService(IRepository<MaintenanceStep> maintenanceStepRepository, IMaintenanceStepsExcelExporter maintenanceStepsExcelExporter, IRepository<MaintenancePlan, int> lookup_maintenancePlanRepository, IRepository<ItemType, int> lookup_itemTypeRepository, IRepository<WorkOrderAction, int> lookup_workOrderActionRepository)
        {
            _maintenanceStepRepository = maintenanceStepRepository;
            _maintenanceStepsExcelExporter = maintenanceStepsExcelExporter;
            _lookup_maintenancePlanRepository = lookup_maintenancePlanRepository;
            _lookup_itemTypeRepository = lookup_itemTypeRepository;
            _lookup_workOrderActionRepository = lookup_workOrderActionRepository;

        }

        public async Task<PagedResultDto<GetMaintenanceStepForViewDto>> GetAll(GetAllMaintenanceStepsInput input)
        {

            var filteredMaintenanceSteps = _maintenanceStepRepository.GetAll()
                        .Include(e => e.MaintenancePlanFk)
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.WorkOrderActionFk)
                        .WhereIf(input.MaintenancePlanId > 0, e => e.MaintenancePlanId == input.MaintenancePlanId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Comments.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CommentsFilter), e => e.Comments == input.CommentsFilter)
                        .WhereIf(input.MinQuantityFilter != null, e => e.Quantity >= input.MinQuantityFilter)
                        .WhereIf(input.MaxQuantityFilter != null, e => e.Quantity <= input.MaxQuantityFilter)
                        .WhereIf(input.MinCostFilter != null, e => e.Cost >= input.MinCostFilter)
                        .WhereIf(input.MaxCostFilter != null, e => e.Cost <= input.MaxCostFilter)
                        .WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
                        .WhereIf(input.MaxPriceFilter != null, e => e.Price <= input.MaxPriceFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MaintenancePlanSubjectFilter), e => e.MaintenancePlanFk != null && e.MaintenancePlanFk.Subject == input.MaintenancePlanSubjectFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderActionActionFilter), e => e.WorkOrderActionFk != null && e.WorkOrderActionFk.Action == input.WorkOrderActionActionFilter);

            var pagedAndFilteredMaintenanceSteps = filteredMaintenanceSteps
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var maintenanceSteps = from o in pagedAndFilteredMaintenanceSteps
                                   join o1 in _lookup_maintenancePlanRepository.GetAll() on o.MaintenancePlanId equals o1.Id into j1
                                   from s1 in j1.DefaultIfEmpty()

                                   join o2 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o2.Id into j2
                                   from s2 in j2.DefaultIfEmpty()

                                   join o3 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o3.Id into j3
                                   from s3 in j3.DefaultIfEmpty()

                                   select new GetMaintenanceStepForViewDto()
                                   {
                                       MaintenanceStep = new MaintenanceStepDto
                                       {
                                           Comments = o.Comments,
                                           Quantity = o.Quantity,
                                           Cost = o.Cost,
                                           Price = o.Price,
                                           Id = o.Id
                                       },
                                       MaintenancePlanSubject = s1 == null || s1.Subject == null ? "" : s1.Subject.ToString(),
                                       ItemTypeType = s2 == null || s2.Type == null ? "" : s2.Type.ToString(),
                                       WorkOrderActionAction = s3 == null || s3.Action == null ? "" : s3.Action.ToString()
                                   };

            var totalCount = await filteredMaintenanceSteps.CountAsync();

            return new PagedResultDto<GetMaintenanceStepForViewDto>(
                totalCount,
                await maintenanceSteps.ToListAsync()
            );
        }

        public async Task<GetMaintenanceStepForViewDto> GetMaintenanceStepForView(int id)
        {
            var maintenanceStep = await _maintenanceStepRepository.GetAsync(id);

            var output = new GetMaintenanceStepForViewDto { MaintenanceStep = ObjectMapper.Map<MaintenanceStepDto>(maintenanceStep) };

            if (output.MaintenanceStep.MaintenancePlanId != null)
            {
                var _lookupMaintenancePlan = await _lookup_maintenancePlanRepository.FirstOrDefaultAsync((int)output.MaintenanceStep.MaintenancePlanId);
                output.MaintenancePlanSubject = _lookupMaintenancePlan?.Subject?.ToString();
            }

            if (output.MaintenanceStep.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.MaintenanceStep.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

            if (output.MaintenanceStep.WorkOrderActionId != null)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.MaintenanceStep.WorkOrderActionId);
                output.WorkOrderActionAction = _lookupWorkOrderAction?.Action?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenanceSteps_Edit)]
        public async Task<GetMaintenanceStepForEditOutput> GetMaintenanceStepForEdit(EntityDto input)
        {
            var maintenanceStep = await _maintenanceStepRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetMaintenanceStepForEditOutput { MaintenanceStep = ObjectMapper.Map<CreateOrEditMaintenanceStepDto>(maintenanceStep) };

            if (output.MaintenanceStep.MaintenancePlanId != null)
            {
                var _lookupMaintenancePlan = await _lookup_maintenancePlanRepository.FirstOrDefaultAsync((int)output.MaintenanceStep.MaintenancePlanId);
                output.MaintenancePlanSubject = _lookupMaintenancePlan?.Subject?.ToString();
            }

            if (output.MaintenanceStep.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.MaintenanceStep.ItemTypeId);
                output.ItemTypeType = _lookupItemType?.Type?.ToString();
            }

            if (output.MaintenanceStep.WorkOrderActionId != null)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.MaintenanceStep.WorkOrderActionId);
                output.WorkOrderActionAction = _lookupWorkOrderAction?.Action?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditMaintenanceStepDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenanceSteps_Create)]
        protected virtual async Task Create(CreateOrEditMaintenanceStepDto input)
        {
            var maintenanceStep = ObjectMapper.Map<MaintenanceStep>(input);

            if (AbpSession.TenantId != null)
            {
                maintenanceStep.TenantId = (int?)AbpSession.TenantId;
            }

            await _maintenanceStepRepository.InsertAsync(maintenanceStep);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenanceSteps_Edit)]
        protected virtual async Task Update(CreateOrEditMaintenanceStepDto input)
        {
            var maintenanceStep = await _maintenanceStepRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, maintenanceStep);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenanceSteps_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _maintenanceStepRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetMaintenanceStepsToExcel(GetAllMaintenanceStepsForExcelInput input)
        {

            var filteredMaintenanceSteps = _maintenanceStepRepository.GetAll()
                        .Include(e => e.MaintenancePlanFk)
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.WorkOrderActionFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Comments.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CommentsFilter), e => e.Comments == input.CommentsFilter)
                        .WhereIf(input.MinQuantityFilter != null, e => e.Quantity >= input.MinQuantityFilter)
                        .WhereIf(input.MaxQuantityFilter != null, e => e.Quantity <= input.MaxQuantityFilter)
                        .WhereIf(input.MinCostFilter != null, e => e.Cost >= input.MinCostFilter)
                        .WhereIf(input.MaxCostFilter != null, e => e.Cost <= input.MaxCostFilter)
                        .WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
                        .WhereIf(input.MaxPriceFilter != null, e => e.Price <= input.MaxPriceFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MaintenancePlanSubjectFilter), e => e.MaintenancePlanFk != null && e.MaintenancePlanFk.Subject == input.MaintenancePlanSubjectFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderActionActionFilter), e => e.WorkOrderActionFk != null && e.WorkOrderActionFk.Action == input.WorkOrderActionActionFilter);

            var query = (from o in filteredMaintenanceSteps
                         join o1 in _lookup_maintenancePlanRepository.GetAll() on o.MaintenancePlanId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         select new GetMaintenanceStepForViewDto()
                         {
                             MaintenanceStep = new MaintenanceStepDto
                             {
                                 Comments = o.Comments,
                                 Quantity = o.Quantity,
                                 Cost = o.Cost,
                                 Price = o.Price,
                                 Id = o.Id
                             },
                             MaintenancePlanSubject = s1 == null || s1.Subject == null ? "" : s1.Subject.ToString(),
                             ItemTypeType = s2 == null || s2.Type == null ? "" : s2.Type.ToString(),
                             WorkOrderActionAction = s3 == null || s3.Action == null ? "" : s3.Action.ToString()
                         });

            var maintenanceStepListDtos = await query.ToListAsync();

            return _maintenanceStepsExcelExporter.ExportToFile(maintenanceStepListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenanceSteps)]
        public async Task<PagedResultDto<MaintenanceStepMaintenancePlanLookupTableDto>> GetAllMaintenancePlanForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_maintenancePlanRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Subject != null && e.Subject.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var maintenancePlanList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<MaintenanceStepMaintenancePlanLookupTableDto>();
            foreach (var maintenancePlan in maintenancePlanList)
            {
                lookupTableDtoList.Add(new MaintenanceStepMaintenancePlanLookupTableDto
                {
                    Id = maintenancePlan.Id,
                    DisplayName = maintenancePlan.Subject?.ToString()
                });
            }

            return new PagedResultDto<MaintenanceStepMaintenancePlanLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenanceSteps)]
        public async Task<PagedResultDto<MaintenanceStepItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_itemTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type != null && e.Type.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var itemTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<MaintenanceStepItemTypeLookupTableDto>();
            foreach (var itemType in itemTypeList)
            {
                lookupTableDtoList.Add(new MaintenanceStepItemTypeLookupTableDto
                {
                    Id = itemType.Id,
                    DisplayName = itemType.Type?.ToString()
                });
            }

            return new PagedResultDto<MaintenanceStepItemTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenanceSteps)]
        public async Task<PagedResultDto<MaintenanceStepWorkOrderActionLookupTableDto>> GetAllWorkOrderActionForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderActionRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Action != null && e.Action.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderActionList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<MaintenanceStepWorkOrderActionLookupTableDto>();
            foreach (var workOrderAction in workOrderActionList)
            {
                lookupTableDtoList.Add(new MaintenanceStepWorkOrderActionLookupTableDto
                {
                    Id = workOrderAction.Id,
                    DisplayName = workOrderAction.Action?.ToString()
                });
            }

            return new PagedResultDto<MaintenanceStepWorkOrderActionLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}