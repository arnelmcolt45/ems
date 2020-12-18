using Ems.Support;
using Ems.Support;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Administration_MaintenancePlans)]
    public class MaintenancePlansAppService : EmsAppServiceBase, IMaintenancePlansAppService
    {
        private readonly IRepository<MaintenancePlan> _maintenancePlanRepository;
        private readonly IRepository<WorkOrderPriority, int> _lookup_workOrderPriorityRepository;
        private readonly IRepository<WorkOrderType, int> _lookup_workOrderTypeRepository;

        public MaintenancePlansAppService(IRepository<MaintenancePlan> maintenancePlanRepository, IRepository<WorkOrderPriority, int> lookup_workOrderPriorityRepository, IRepository<WorkOrderType, int> lookup_workOrderTypeRepository)
        {
            _maintenancePlanRepository = maintenancePlanRepository;
            _lookup_workOrderPriorityRepository = lookup_workOrderPriorityRepository;
            _lookup_workOrderTypeRepository = lookup_workOrderTypeRepository;

        }

        public async Task<PagedResultDto<GetMaintenancePlanForViewDto>> GetAll(GetAllMaintenancePlansInput input)
        {

            var filteredMaintenancePlans = _maintenancePlanRepository.GetAll()
                        .Include(e => e.WorkOrderPriorityFk)
                        .Include(e => e.WorkOrderTypeFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Subject.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Remarks.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SubjectFilter), e => e.Subject == input.SubjectFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RemarksFilter), e => e.Remarks == input.RemarksFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderPriorityPriorityFilter), e => e.WorkOrderPriorityFk != null && e.WorkOrderPriorityFk.Priority == input.WorkOrderPriorityPriorityFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderTypeTypeFilter), e => e.WorkOrderTypeFk != null && e.WorkOrderTypeFk.Type == input.WorkOrderTypeTypeFilter);

            var pagedAndFilteredMaintenancePlans = filteredMaintenancePlans
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var maintenancePlans = from o in pagedAndFilteredMaintenancePlans
                                   join o1 in _lookup_workOrderPriorityRepository.GetAll() on o.WorkOrderPriorityId equals o1.Id into j1
                                   from s1 in j1.DefaultIfEmpty()

                                   join o2 in _lookup_workOrderTypeRepository.GetAll() on o.WorkOrderTypeId equals o2.Id into j2
                                   from s2 in j2.DefaultIfEmpty()

                                   select new GetMaintenancePlanForViewDto()
                                   {
                                       MaintenancePlan = new MaintenancePlanDto
                                       {
                                           Subject = o.Subject,
                                           Description = o.Description,
                                           Remarks = o.Remarks,
                                           Id = o.Id
                                       },
                                       WorkOrderPriorityPriority = s1 == null || s1.Priority == null ? "" : s1.Priority.ToString(),
                                       WorkOrderTypeType = s2 == null || s2.Type == null ? "" : s2.Type.ToString()
                                   };

            var totalCount = await filteredMaintenancePlans.CountAsync();

            return new PagedResultDto<GetMaintenancePlanForViewDto>(
                totalCount,
                await maintenancePlans.ToListAsync()
            );
        }

        public async Task<GetMaintenancePlanForViewDto> GetMaintenancePlanForView(int id)
        {
            var maintenancePlan = await _maintenancePlanRepository.GetAsync(id);

            var output = new GetMaintenancePlanForViewDto { MaintenancePlan = ObjectMapper.Map<MaintenancePlanDto>(maintenancePlan) };

            if (output.MaintenancePlan.WorkOrderPriorityId != null)
            {
                var _lookupWorkOrderPriority = await _lookup_workOrderPriorityRepository.FirstOrDefaultAsync((int)output.MaintenancePlan.WorkOrderPriorityId);
                output.WorkOrderPriorityPriority = _lookupWorkOrderPriority?.Priority?.ToString();
            }

            if (output.MaintenancePlan.WorkOrderTypeId != null)
            {
                var _lookupWorkOrderType = await _lookup_workOrderTypeRepository.FirstOrDefaultAsync((int)output.MaintenancePlan.WorkOrderTypeId);
                output.WorkOrderTypeType = _lookupWorkOrderType?.Type?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenancePlans_Edit)]
        public async Task<GetMaintenancePlanForEditOutput> GetMaintenancePlanForEdit(EntityDto input)
        {
            var maintenancePlan = await _maintenancePlanRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetMaintenancePlanForEditOutput { MaintenancePlan = ObjectMapper.Map<CreateOrEditMaintenancePlanDto>(maintenancePlan) };

            if (output.MaintenancePlan.WorkOrderPriorityId != null)
            {
                var _lookupWorkOrderPriority = await _lookup_workOrderPriorityRepository.FirstOrDefaultAsync((int)output.MaintenancePlan.WorkOrderPriorityId);
                output.WorkOrderPriorityPriority = _lookupWorkOrderPriority?.Priority?.ToString();
            }

            if (output.MaintenancePlan.WorkOrderTypeId != null)
            {
                var _lookupWorkOrderType = await _lookup_workOrderTypeRepository.FirstOrDefaultAsync((int)output.MaintenancePlan.WorkOrderTypeId);
                output.WorkOrderTypeType = _lookupWorkOrderType?.Type?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditMaintenancePlanDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenancePlans_Create)]
        protected virtual async Task Create(CreateOrEditMaintenancePlanDto input)
        {
            var maintenancePlan = ObjectMapper.Map<MaintenancePlan>(input);

            if (AbpSession.TenantId != null)
            {
                maintenancePlan.TenantId = (int?)AbpSession.TenantId;
            }

            await _maintenancePlanRepository.InsertAsync(maintenancePlan);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenancePlans_Edit)]
        protected virtual async Task Update(CreateOrEditMaintenancePlanDto input)
        {
            var maintenancePlan = await _maintenancePlanRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, maintenancePlan);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenancePlans_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _maintenancePlanRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenancePlans)]
        public async Task<PagedResultDto<MaintenancePlanWorkOrderPriorityLookupTableDto>> GetAllWorkOrderPriorityForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderPriorityRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Priority != null && e.Priority.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderPriorityList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<MaintenancePlanWorkOrderPriorityLookupTableDto>();
            foreach (var workOrderPriority in workOrderPriorityList)
            {
                lookupTableDtoList.Add(new MaintenancePlanWorkOrderPriorityLookupTableDto
                {
                    Id = workOrderPriority.Id,
                    DisplayName = workOrderPriority.Priority?.ToString()
                });
            }

            return new PagedResultDto<MaintenancePlanWorkOrderPriorityLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_MaintenancePlans)]
        public async Task<PagedResultDto<MaintenancePlanWorkOrderTypeLookupTableDto>> GetAllWorkOrderTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type != null && e.Type.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<MaintenancePlanWorkOrderTypeLookupTableDto>();
            foreach (var workOrderType in workOrderTypeList)
            {
                lookupTableDtoList.Add(new MaintenancePlanWorkOrderTypeLookupTableDto
                {
                    Id = workOrderType.Id,
                    DisplayName = workOrderType.Type?.ToString()
                });
            }

            return new PagedResultDto<MaintenancePlanWorkOrderTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}