namespace Ems.Support.Dtos
{
    public class GetMaintenanceStepForViewDto
    {
        public MaintenanceStepDto MaintenanceStep { get; set; }

        public string MaintenancePlanSubject { get; set; }

        public string ItemTypeType { get; set; }

        public string WorkOrderActionAction { get; set; }

    }
}