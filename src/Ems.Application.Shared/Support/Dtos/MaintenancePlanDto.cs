using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class MaintenancePlanDto : EntityDto
    {
        public string Subject { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public int? WorkOrderPriorityId { get; set; }

        public int? WorkOrderTypeId { get; set; }

    }
}