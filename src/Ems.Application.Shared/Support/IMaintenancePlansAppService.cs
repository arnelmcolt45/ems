using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IMaintenancePlansAppService : IApplicationService
    {
        Task<PagedResultDto<GetMaintenancePlanForViewDto>> GetAll(GetAllMaintenancePlansInput input);

        Task<GetMaintenancePlanForViewDto> GetMaintenancePlanForView(int id);

        Task<GetMaintenancePlanForEditOutput> GetMaintenancePlanForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditMaintenancePlanDto input);

        Task Delete(EntityDto input);

        Task<PagedResultDto<MaintenancePlanWorkOrderPriorityLookupTableDto>> GetAllWorkOrderPriorityForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<MaintenancePlanWorkOrderTypeLookupTableDto>> GetAllWorkOrderTypeForLookupTable(GetAllForLookupTableInput input);

    }
}