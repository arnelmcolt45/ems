using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using System;

namespace Ems.Telematics.Dtos
{
    public class GetSomeUsageMetricRecordsInput : PagedAndSortedResultRequestDto
    {
        public int AssetId { get; set; }

        public string Filter { get; set; }

        public string ReferenceFilter { get; set; }

        public DateTime? MaxStartTimeFilter { get; set; }
        public DateTime? MinStartTimeFilter { get; set; }

        public DateTime? MaxEndTimeFilter { get; set; }
        public DateTime? MinEndTimeFilter { get; set; }

        public decimal? MaxUnitsConsumedFilter { get; set; }
        public decimal? MinUnitsConsumedFilter { get; set; }

        public string UsageMetricMetricFilter { get; set; }
    }

    public class GetAllUsageMetricRecordsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ReferenceFilter { get; set; }

        public DateTime? MaxStartTimeFilter { get; set; }
        public DateTime? MinStartTimeFilter { get; set; }

        public DateTime? MaxEndTimeFilter { get; set; }
        public DateTime? MinEndTimeFilter { get; set; }

        public decimal? MaxUnitsConsumedFilter { get; set; }
        public decimal? MinUnitsConsumedFilter { get; set; }

        public string UsageMetricMetricFilter { get; set; }
    }

    public class GetAllRecordsByUsageMetricInput : PagedAndSortedResultRequestDto
    {
        public int UsageMetricId { get; set; }
        public string Filter { get; set; }

        public string ReferenceFilter { get; set; }

        public DateTime? MaxStartTimeFilter { get; set; }
        public DateTime? MinStartTimeFilter { get; set; }

        public DateTime? MaxEndTimeFilter { get; set; }
        public DateTime? MinEndTimeFilter { get; set; }

        public decimal? MaxUnitsConsumedFilter { get; set; }
        public decimal? MinUnitsConsumedFilter { get; set; }


        public string UsageMetricMetricFilter { get; set; }
    }
}