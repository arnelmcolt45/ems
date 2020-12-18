using Ems.Support;
using Ems.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Support
{
    [Table("MaintenancePlans")]
    public class MaintenancePlan : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public virtual string Subject { get; set; }

        public virtual string Description { get; set; }

        public virtual string Remarks { get; set; }

        public virtual int? WorkOrderPriorityId { get; set; }

        [ForeignKey("WorkOrderPriorityId")]
        public WorkOrderPriority WorkOrderPriorityFk { get; set; }

        public virtual int? WorkOrderTypeId { get; set; }

        [ForeignKey("WorkOrderTypeId")]
        public WorkOrderType WorkOrderTypeFk { get; set; }

    }
}