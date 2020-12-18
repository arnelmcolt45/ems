using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Support
{
	[Table("ConsumableTypes")]
    public class ConsumableType : FullAuditedEntity   //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Type { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		

    }
}