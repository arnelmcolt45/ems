using Abp.Application.Services.Dto;
using System;

namespace Ems.Storage.Dtos
{
    public class GetAllAzureStorageConfigurationsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string ServiceFilter { get; set; }

		public string AccountNameFilter { get; set; }

		public string KeyValueFilter { get; set; }

		public string BlobStorageEndpointFilter { get; set; }

		public string ContainerNameFilter { get; set; }




    }
}