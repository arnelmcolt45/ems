using Ems.Quotations;
using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Assets
{
	[Table("AssetParts")]
    public class AssetPart : FullAuditedEntity //, IMayHaveTenant
    {
		public int? TenantId { get; set; }

		[Required]
		public virtual string Name { get; set; }
		
		public virtual string Description { get; set; }
		
		public virtual string SerialNumber { get; set; }
		
		public virtual DateTime? InstallDate { get; set; }
		
		[Required]
		public virtual string Code { get; set; }
		
		public virtual bool Installed { get; set; }

		public virtual Boolean IsItem { get; set; }

		public virtual int? Qty { get; set; }

		public virtual int? AssetPartTypeId { get; set; }
		
        [ForeignKey("AssetPartTypeId")]
		public AssetPartType AssetPartTypeFk { get; set; }
		
		public virtual int? ParentId { get; set; }
		
        [ForeignKey("ParentId")]
		public AssetPart ParentFk { get; set; }
		
		public virtual int? AssetPartStatusId { get; set; }
		
        [ForeignKey("AssetPartStatusId")]
		public AssetPartStatus AssetPartStatusFk { get; set; }
		
		public virtual int? AssetId { get; set; }
		
        [ForeignKey("AssetId")]
		public Asset AssetFk { get; set; }
		
		public virtual int? ItemTypeId { get; set; }
		
        [ForeignKey("ItemTypeId")]
		public ItemType ItemTypeFk { get; set; }
		
		public virtual int? WarehouseId { get; set; }
		
        [ForeignKey("WarehouseId")]
		public Warehouse WarehouseFk { get; set; }
		
    }
}