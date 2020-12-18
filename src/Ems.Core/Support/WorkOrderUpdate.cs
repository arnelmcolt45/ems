using Ems.Support;
using Ems.Quotations;
using Ems.Support;
using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Support
{
	[Table("WorkOrderUpdates")]
    [Audited]
    public class WorkOrderUpdate : FullAuditedEntity , IMayHaveTenant
    {
		public int? TenantId { get; set; }
			
		public virtual string Comments { get; set; }
		
		public virtual decimal Number { get; set; }
		
		public virtual bool Completed { get; set; }

		public virtual int WorkOrderId { get; set; }
		
        [ForeignKey("WorkOrderId")]
		public WorkOrder WorkOrderFk { get; set; }
		
		public virtual int? ItemTypeId { get; set; }
		
        [ForeignKey("ItemTypeId")]
		public ItemType ItemTypeFk { get; set; }
		
		public virtual int WorkOrderActionId { get; set; }
		
        [ForeignKey("WorkOrderActionId")]
		public WorkOrderAction WorkOrderActionFk { get; set; }
		
		public virtual int? AssetPartId { get; set; }
		
        [ForeignKey("AssetPartId")]
		public AssetPart AssetPartFk { get; set; }
		
    }
}