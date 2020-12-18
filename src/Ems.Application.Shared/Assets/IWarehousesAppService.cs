using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;


namespace Ems.Assets
{
    public interface IWarehousesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetWarehouseForViewDto>> GetAll(GetAllWarehousesInput input);

        Task<GetWarehouseForViewDto> GetWarehouseForView(int id);

		Task<GetWarehouseForEditOutput> GetWarehouseForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditWarehouseDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetWarehousesToExcel(GetAllWarehousesForExcelInput input);

		
    }
}