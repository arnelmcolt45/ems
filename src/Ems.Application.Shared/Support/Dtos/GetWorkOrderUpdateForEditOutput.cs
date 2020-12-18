using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class GetWorkOrderUpdateForEditOutput
    {
		public CreateOrEditWorkOrderUpdateDto WorkOrderUpdate { get; set; }

		public string WorkOrderUpdateSubject { get; set;}

		public string ItemTypeType { get; set;}

		public string WorkOrderUpdateActionAction { get; set;}

		public string AssetPartName { get; set;}
    }
}