using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditMaintenancePlanDto : EntityDto<int?>
    {

        [Required]
        public string Subject { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public int? WorkOrderPriorityId { get; set; }

        public int? WorkOrderTypeId { get; set; }

    }
}