using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditMaintenanceStepDto : EntityDto<int?>
    {

        public string Comments { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Cost { get; set; }

        public decimal? Price { get; set; }

        public int MaintenancePlanId { get; set; }

        public int? ItemTypeId { get; set; }

        public int? WorkOrderActionId { get; set; }

    }
}