
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditWorkOrderUpdateDto : EntityDto<int?>
    {
		public string Comments { get; set; }	
		
		public decimal Number { get; set; }

		public virtual bool Completed { get; set; }

		public int WorkOrderId { get; set; }
		 
		public int? ItemTypeId { get; set; }
		 
		public int WorkOrderActionId { get; set; }
		 
		public int? AssetPartId { get; set; }
    }
}