﻿
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditInventoryItemDto : EntityDto<int?>
    {

		[Required]
		public string Name { get; set; }
		
		public string Reference { get; set; }
		
		public int QtyInWarehouse { get; set; }
		
		public int RestockLimit { get; set; }
		
		public int? QtyOnOrder { get; set; }
		
		public int? ItemTypeId { get; set; }
		 
		public int? AssetId { get; set; }
		 
		public int? WarehouseId { get; set; }
    }
}