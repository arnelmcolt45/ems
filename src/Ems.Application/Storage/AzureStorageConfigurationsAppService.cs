

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Storage.Exporting;
using Ems.Storage.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Storage
{
	[AbpAuthorize(AppPermissions.Pages_AzureStorageConfigurations)]
    public class AzureStorageConfigurationsAppService : EmsAppServiceBase, IAzureStorageConfigurationsAppService
    {
		 private readonly IRepository<AzureStorageConfiguration> _azureStorageConfigurationRepository;
		 private readonly IAzureStorageConfigurationsExcelExporter _azureStorageConfigurationsExcelExporter;
		 

		  public AzureStorageConfigurationsAppService(IRepository<AzureStorageConfiguration> azureStorageConfigurationRepository, IAzureStorageConfigurationsExcelExporter azureStorageConfigurationsExcelExporter ) 
		  {
			_azureStorageConfigurationRepository = azureStorageConfigurationRepository;
			_azureStorageConfigurationsExcelExporter = azureStorageConfigurationsExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetAzureStorageConfigurationForViewDto>> GetAll(GetAllAzureStorageConfigurationsInput input)
         {
			
			var filteredAzureStorageConfigurations = _azureStorageConfigurationRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Service.Contains(input.Filter) || e.AccountName.Contains(input.Filter) || e.KeyValue.Contains(input.Filter) || e.BlobStorageEndpoint.Contains(input.Filter) || e.ContainerName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ServiceFilter),  e => e.Service == input.ServiceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AccountNameFilter),  e => e.AccountName == input.AccountNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.KeyValueFilter),  e => e.KeyValue == input.KeyValueFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.BlobStorageEndpointFilter),  e => e.BlobStorageEndpoint == input.BlobStorageEndpointFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ContainerNameFilter),  e => e.ContainerName == input.ContainerNameFilter);

			var pagedAndFilteredAzureStorageConfigurations = filteredAzureStorageConfigurations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var azureStorageConfigurations = from o in pagedAndFilteredAzureStorageConfigurations
                         select new GetAzureStorageConfigurationForViewDto() {
							AzureStorageConfiguration = new AzureStorageConfigurationDto
							{
                                Service = o.Service,
                                AccountName = o.AccountName,
                                KeyValue = o.KeyValue,
                                BlobStorageEndpoint = o.BlobStorageEndpoint,
                                ContainerName = o.ContainerName,
                                Id = o.Id
							}
						};

            var totalCount = await filteredAzureStorageConfigurations.CountAsync();

            return new PagedResultDto<GetAzureStorageConfigurationForViewDto>(
                totalCount,
                await azureStorageConfigurations.ToListAsync()
            );
         }
		 
		 public async Task<GetAzureStorageConfigurationForViewDto> GetAzureStorageConfigurationForView(int id)
         {
            var azureStorageConfiguration = await _azureStorageConfigurationRepository.GetAsync(id);

            var output = new GetAzureStorageConfigurationForViewDto { AzureStorageConfiguration = ObjectMapper.Map<AzureStorageConfigurationDto>(azureStorageConfiguration) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_AzureStorageConfigurations_Edit)]
		 public async Task<GetAzureStorageConfigurationForEditOutput> GetAzureStorageConfigurationForEdit(EntityDto input)
         {
            var azureStorageConfiguration = await _azureStorageConfigurationRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAzureStorageConfigurationForEditOutput {AzureStorageConfiguration = ObjectMapper.Map<CreateOrEditAzureStorageConfigurationDto>(azureStorageConfiguration)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAzureStorageConfigurationDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_AzureStorageConfigurations_Create)]
		 protected virtual async Task Create(CreateOrEditAzureStorageConfigurationDto input)
         {
            var azureStorageConfiguration = ObjectMapper.Map<AzureStorageConfiguration>(input);

			
			if (AbpSession.TenantId != null)
			{
				azureStorageConfiguration.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _azureStorageConfigurationRepository.InsertAsync(azureStorageConfiguration);
         }

		 [AbpAuthorize(AppPermissions.Pages_AzureStorageConfigurations_Edit)]
		 protected virtual async Task Update(CreateOrEditAzureStorageConfigurationDto input)
         {
            var azureStorageConfiguration = await _azureStorageConfigurationRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, azureStorageConfiguration);
         }

		 [AbpAuthorize(AppPermissions.Pages_AzureStorageConfigurations_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _azureStorageConfigurationRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetAzureStorageConfigurationsToExcel(GetAllAzureStorageConfigurationsForExcelInput input)
         {
			
			var filteredAzureStorageConfigurations = _azureStorageConfigurationRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Service.Contains(input.Filter) || e.AccountName.Contains(input.Filter) || e.KeyValue.Contains(input.Filter) || e.BlobStorageEndpoint.Contains(input.Filter) || e.ContainerName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ServiceFilter),  e => e.Service == input.ServiceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AccountNameFilter),  e => e.AccountName == input.AccountNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.KeyValueFilter),  e => e.KeyValue == input.KeyValueFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.BlobStorageEndpointFilter),  e => e.BlobStorageEndpoint == input.BlobStorageEndpointFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ContainerNameFilter),  e => e.ContainerName == input.ContainerNameFilter);

			var query = (from o in filteredAzureStorageConfigurations
                         select new GetAzureStorageConfigurationForViewDto() { 
							AzureStorageConfiguration = new AzureStorageConfigurationDto
							{
                                Service = o.Service,
                                AccountName = o.AccountName,
                                KeyValue = o.KeyValue,
                                BlobStorageEndpoint = o.BlobStorageEndpoint,
                                ContainerName = o.ContainerName,
                                Id = o.Id
							}
						 });


            var azureStorageConfigurationListDtos = await query.ToListAsync();

            return _azureStorageConfigurationsExcelExporter.ExportToFile(azureStorageConfigurationListDtos);
         }


    }
}