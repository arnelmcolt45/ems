namespace Ems.Customers.Dtos
{
    public class CustomerForCustomerInvoiceEditDto
    {
        public string DueDate { get; set; }
        public int? CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
    }
}
