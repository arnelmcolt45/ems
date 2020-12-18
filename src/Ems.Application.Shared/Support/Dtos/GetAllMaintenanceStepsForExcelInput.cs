using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllMaintenanceStepsForExcelInput
    {
        public string Filter { get; set; }

        public string CommentsFilter { get; set; }

        public decimal? MaxQuantityFilter { get; set; }
        public decimal? MinQuantityFilter { get; set; }

        public decimal? MaxCostFilter { get; set; }
        public decimal? MinCostFilter { get; set; }

        public decimal? MaxPriceFilter { get; set; }
        public decimal? MinPriceFilter { get; set; }

        public string MaintenancePlanSubjectFilter { get; set; }

        public string ItemTypeTypeFilter { get; set; }

        public string WorkOrderActionActionFilter { get; set; }

    }
}