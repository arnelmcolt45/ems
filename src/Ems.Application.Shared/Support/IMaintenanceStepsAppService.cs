using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IMaintenanceStepsAppService : IApplicationService
    {
        Task<PagedResultDto<GetMaintenanceStepForViewDto>> GetAll(GetAllMaintenanceStepsInput input);

        Task<GetMaintenanceStepForViewDto> GetMaintenanceStepForView(int id);

        Task<GetMaintenanceStepForEditOutput> GetMaintenanceStepForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditMaintenanceStepDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetMaintenanceStepsToExcel(GetAllMaintenanceStepsForExcelInput input);

        Task<PagedResultDto<MaintenanceStepMaintenancePlanLookupTableDto>> GetAllMaintenancePlanForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<MaintenanceStepItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<MaintenanceStepWorkOrderActionLookupTableDto>> GetAllWorkOrderActionForLookupTable(GetAllForLookupTableInput input);

    }
}