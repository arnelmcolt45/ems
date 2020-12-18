using Abp.Domain.Entities.Auditing;
using Ems.Assets;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ems.Billing
{
	[Table("XeroConfigurations")]
	public class XeroConfiguration : AuditedEntity
	{
		public int TenantId { get; set; }

		public virtual bool IsHostConfig { get; set; }

		public virtual string ClientId { get; set; }

		public virtual string ClientSecret { get; set; }
		public virtual string XeroTenantId { get; set; }

		public virtual string CallbackUri { get; set; }

		public virtual string Scope { get; set; }

		public virtual string State { get; set; }


		public virtual int AssetOwnerId { get; set; }

		[ForeignKey("AssetOwnerId")]
		public AssetOwner AssetOwnerFk { get; set; }
	}
}
