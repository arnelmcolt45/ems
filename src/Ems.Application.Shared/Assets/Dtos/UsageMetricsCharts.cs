using Ems.Telematics.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ems.Assets.Dtos
{
    public class ChartPeriod
    {
        public string ChartPeriodLabel { get; set; }

        public DateTime ChartPeriodDate { get; set; }
    }

    public class UsageMetricsChartOutput
    {
        public List<UsageMetricsChartInfo> ChartInfo { get; set; } // List of 1 only (to simplify the front end consumption of this data)
        public List<UsageMetricsChartData> ChartData { get; set; } // List of all periods

        public UsageMetricsChartOutput()
        {
            List<UsageMetricsChartInfo> ChartInfo = new List<UsageMetricsChartInfo>();
            List<UsageMetricsChartData> ChartData = new List<UsageMetricsChartData>();
        }
    }

    public class UsageMetricsChartData
    {
        public string Period { get; set; }
        public decimal Value { get; set; }

        public UsageMetricsChartData(string period, decimal value)
        {
            Period = period;
            Value = value;
        }
    }

    public class UsageMetricsChartInfo
    {
        public GetUsageMetricForViewDto UsageMetricDto { get; set; }
        public string ChartName { get; set; }
        public string Total { get; set; }
        public string DailyAvg { get; set; }
        public UsageMetricsChartInfo(GetUsageMetricForViewDto usageMetricDto, string chartName, string total, string dailyAvg)
        {
            UsageMetricDto = usageMetricDto;
            ChartName = chartName;
            Total = total;
            DailyAvg = dailyAvg;
        }
    }
}
