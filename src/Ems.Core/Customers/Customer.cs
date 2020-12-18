using Ems.Billing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Auditing;

namespace Ems.Customers
{
	[Table("Customers")]
	[Audited]
	public class Customer : FullAuditedEntity //, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		[Required]
		public virtual string Reference { get; set; }

		[Required]
		public virtual string Name { get; set; }

		[Required]
		public virtual string Identifier { get; set; }

		public virtual string LogoUrl { get; set; }

		public virtual string Website { get; set; }

		public virtual string CustomerLoc8UUID { get; set; }

		public virtual string EmailAddress { get; set; } 

		public virtual string XeroContactPersons  { get; set; }

		public virtual string XeroAccountsReceivableTaxType  { get; set; }

		public virtual string XeroAccountsPayableTaxType  { get; set; }

		public virtual string XeroAddresses { get; set; }

		public virtual string XeroPhones { get; set; }

		public virtual string XeroDefaultCurrency { get; set; }

		public virtual string XeroPaymentTerms { get; set; }

		public virtual string XeroContactId { get; set; }

		public virtual int CustomerTypeId { get; set; }

		[ForeignKey("CustomerTypeId")]
		public CustomerType CustomerTypeFk { get; set; }

		public virtual int CurrencyId { get; set; }

		[ForeignKey("CurrencyId")]
		public Currency CurrencyFk { get; set; }

		public virtual int? PaymentTermNumber { get; set; }
		public virtual string PaymentTermType { get; set; }
	}
}