using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Storage.Dtos;
using Ems.Dto;


namespace Ems.Storage
{
    public interface IAzureStorageConfigurationsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAzureStorageConfigurationForViewDto>> GetAll(GetAllAzureStorageConfigurationsInput input);

        Task<GetAzureStorageConfigurationForViewDto> GetAzureStorageConfigurationForView(int id);

		Task<GetAzureStorageConfigurationForEditOutput> GetAzureStorageConfigurationForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAzureStorageConfigurationDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAzureStorageConfigurationsToExcel(GetAllAzureStorageConfigurationsForExcelInput input);

		
    }
}