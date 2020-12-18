
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetPartDto : EntityDto
    {
		public string Name { get; set; }

		public string Description { get; set; }

		public string SerialNumber { get; set; }

		public DateTime? InstallDate { get; set; }

		public string Code { get; set; }

		public bool Installed { get; set; }

		public virtual Boolean IsItem { get; set; }

		public virtual int? Qty { get; set; }

		public int? AssetPartTypeId { get; set; }

		public int? ParentId { get; set; }

		public int? AssetPartStatusId { get; set; }

		public int? AssetId { get; set; }

		public int? ItemTypeId { get; set; }

		public int? WarehouseId { get; set; }
    }
}