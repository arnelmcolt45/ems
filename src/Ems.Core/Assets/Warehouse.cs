using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Assets
{
	[Table("Warehouses")]
    public class Warehouse : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Name { get; set; }
		
		public virtual string AddressLine1 { get; set; }
		
		public virtual string AddressLine2 { get; set; }
		
		public virtual string PostalCode { get; set; }
		
		public virtual string City { get; set; }
		
		public virtual string State { get; set; }
		
		public virtual string Country { get; set; }
		

    }
}