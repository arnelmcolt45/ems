namespace Ems.Customers.Dtos
{
    public class GetCustomerForViewDto
    {
		public CustomerDto Customer { get; set; }

		public string CustomerTypeType { get; set;}

		public string CurrencyCode { get; set;}

        public int? PaymentTermNumber { get; set; }

        public string PaymentTermType { get; set; }

    }
}