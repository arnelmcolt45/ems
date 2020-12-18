
using System;
using Abp.Application.Services.Dto;

namespace Ems.Storage.Dtos
{
    public class AzureStorageConfigurationDto : EntityDto
    {
		public string Service { get; set; }

		public string AccountName { get; set; }

		public string KeyValue { get; set; }

		public string BlobStorageEndpoint { get; set; }

		public string ContainerName { get; set; }



    }
}