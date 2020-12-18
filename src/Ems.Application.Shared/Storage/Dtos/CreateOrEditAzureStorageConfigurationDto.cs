
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Storage.Dtos
{
    public class CreateOrEditAzureStorageConfigurationDto : EntityDto<int?>
    {

		[Required]
		public string Service { get; set; }
		
		
		[Required]
		public string AccountName { get; set; }
		
		
		[Required]
		public string KeyValue { get; set; }
		
		
		[Required]
		public string BlobStorageEndpoint { get; set; }
		
		
		[Required]
		public string ContainerName { get; set; }
		
		

    }
}