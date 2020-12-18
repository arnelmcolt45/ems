using Abp.Application.Services.Dto;
using System.Security.Policy;

namespace Ems.Assets.Dtos
{
    public class GetAllAssetPartsForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

        public int? WarehouseId { get; set; }
        
        public int? AssetId { get; set; }

        public bool? ForImportFromWarehouses { get; set; }
    }
}