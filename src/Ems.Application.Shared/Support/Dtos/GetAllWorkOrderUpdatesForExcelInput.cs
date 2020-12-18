using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllWorkOrderUpdatesForExcelInput
    {
		public string Filter { get; set; }

		public string CommentsFilter { get; set; }

		public decimal? MaxNumberFilter { get; set; }
		public decimal? MinNumberFilter { get; set; }


		 public string WorkOrderSubjectFilter { get; set; }

		 		 public string ItemTypeTypeFilter { get; set; }

		 		 public string WorkOrderActionActionFilter { get; set; }

		 		 public string AssetPartNameFilter { get; set; }

		 
    }
}