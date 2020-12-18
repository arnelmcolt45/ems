using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class GetMaintenancePlanForEditOutput
    {
        public CreateOrEditMaintenancePlanDto MaintenancePlan { get; set; }

        public string WorkOrderPriorityPriority { get; set; }

        public string WorkOrderTypeType { get; set; }

    }
}