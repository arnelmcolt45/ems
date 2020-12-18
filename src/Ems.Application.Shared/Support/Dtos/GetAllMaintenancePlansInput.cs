using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllMaintenancePlansInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string SubjectFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public string RemarksFilter { get; set; }

        public string WorkOrderPriorityPriorityFilter { get; set; }

        public string WorkOrderTypeTypeFilter { get; set; }

    }
}