namespace Ems.Support.Dtos
{
    public class GetEstimateForEditOutput
    {
		public CreateOrEditEstimateDto Estimate { get; set; }

		public string WorkOrderSubject { get; set;}

		public string QuotationTitle { get; set; }

        public string EstimateStatusStatus { get; set;}

        public string CustomerName { get; set;}
    }
}