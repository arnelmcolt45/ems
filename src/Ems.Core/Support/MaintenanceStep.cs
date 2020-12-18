using Ems.Support;
using Ems.Quotations;
using Ems.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Support
{
    [Table("MaintenanceSteps")]
    public class MaintenanceStep : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Comments { get; set; }

        public virtual decimal? Quantity { get; set; }

        public virtual decimal? Cost { get; set; }

        public virtual decimal? Price { get; set; }

        public virtual int MaintenancePlanId { get; set; }

        [ForeignKey("MaintenancePlanId")]
        public MaintenancePlan MaintenancePlanFk { get; set; }

        public virtual int? ItemTypeId { get; set; }

        [ForeignKey("ItemTypeId")]
        public ItemType ItemTypeFk { get; set; }

        public virtual int? WorkOrderActionId { get; set; }

        [ForeignKey("WorkOrderActionId")]
        public WorkOrderAction WorkOrderActionFk { get; set; }

    }
}