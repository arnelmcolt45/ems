
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace Ems.Customers.Dtos
{
    public class CustomerDto : EntityDto
    {
		public string Reference { get; set; }

		public string Name { get; set; }

		public string Identifier { get; set; }

		public string LogoUrl { get; set; }

		public string Website { get; set; }

		public string CustomerLoc8UUID { get; set; }

		public string EmailAddress { get; set; }

		public string XeroContactPersons { get; set; }

		public string XeroAccountsReceivableTaxType { get; set; }

		public string XeroAccountsPayableTaxType { get; set; }

		public string XeroAddresses { get; set; }

		public string XeroPhones { get; set; }

		public string XeroDefaultCurrency { get; set; }

		public string XeroPaymentTerms { get; set; }

		public string XeroContactId { get; set; }

		public int CustomerTypeId { get; set; } 

		public int? CurrencyId { get; set; }

		public int? TenantId { get; set; }

		public int? PaymentTermNumber { get; set; }

		public string PaymentTermType { get; set; }

		public bool IsXeroContactSynced { get; set; }
	}
}