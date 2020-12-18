﻿
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditAssetPartDto : EntityDto<int?>
    {

		[Required]
		public string Name { get; set; }
		
		
		public string Description { get; set; }
		
		
		public string SerialNumber { get; set; }
		
		
		public DateTime? InstallDate { get; set; }

		public int Qty { get; set; }

		public bool IsItem { get; set; }

		[Required]
		public string Code { get; set; }
		
		public bool Installed { get; set; }
		
		public int? AssetPartTypeId { get; set; }
		 
		public int? ParentId { get; set; }
		 
		public int? AssetPartStatusId { get; set; }
		 
		public int? AssetId { get; set; }
		 
		public int? ItemTypeId { get; set; }
		 
		public int? WarehouseId { get; set; }
		 
		 
    }
}