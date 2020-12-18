namespace Ems.Support.Dtos
{
    public class GetMaintenancePlanForViewDto
    {
        public MaintenancePlanDto MaintenancePlan { get; set; }

        public string WorkOrderPriorityPriority { get; set; }

        public string WorkOrderTypeType { get; set; }

    }
}