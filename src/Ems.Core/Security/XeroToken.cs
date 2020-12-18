using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Security
{
	[Table("XeroTokens")]
	public class XeroToken : AuditedEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }

		public virtual string Token { get; set; }
	}
}