using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class GetMaintenanceStepForEditOutput
    {
        public CreateOrEditMaintenanceStepDto MaintenanceStep { get; set; }

        public string MaintenancePlanSubject { get; set; }

        public string ItemTypeType { get; set; }

        public string WorkOrderActionAction { get; set; }

    }
}