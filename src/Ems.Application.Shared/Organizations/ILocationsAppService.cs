using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Organizations.Dtos;
using Ems.Dto;


namespace Ems.Organizations
{
    public interface ILocationsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetLocationForViewDto>> GetAll(GetAllLocationsInput input);

        Task<GetLocationForViewDto> GetLocationForView(int id);

		Task<GetLocationForEditOutput> GetLocationForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditLocationDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetLocationsToExcel(GetAllLocationsForExcelInput input);

		
    }
}