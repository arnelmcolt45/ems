using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Storage
{
	[Table("AzureStorageConfigurations")]
    public class AzureStorageConfiguration : AuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Service { get; set; }
		
		[Required]
		public virtual string AccountName { get; set; }
		
		[Required]
		public virtual string KeyValue { get; set; }
		
		[Required]
		public virtual string BlobStorageEndpoint { get; set; }
		
		[Required]
		public virtual string ContainerName { get; set; }
		

    }
}