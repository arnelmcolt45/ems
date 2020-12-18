using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class MaintenanceStepsExcelExporter : EpPlusExcelExporterBase, IMaintenanceStepsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public MaintenanceStepsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetMaintenanceStepForViewDto> maintenanceSteps)
        {
            return CreateExcelPackage(
                "MaintenanceSteps.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.Workbook.Worksheets.Add(L("MaintenanceSteps"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Comments"),
                        L("Quantity"),
                        L("Cost"),
                        L("Price"),
                        (L("MaintenancePlan")) + L("Subject"),
                        (L("ItemType")) + L("Type"),
                        (L("WorkOrderAction")) + L("Action")
                        );

                    AddObjects(
                        sheet, 2, maintenanceSteps,
                        _ => _.MaintenanceStep.Comments,
                        _ => _.MaintenanceStep.Quantity,
                        _ => _.MaintenanceStep.Cost,
                        _ => _.MaintenanceStep.Price,
                        _ => _.MaintenancePlanSubject,
                        _ => _.ItemTypeType,
                        _ => _.WorkOrderActionAction
                        );

                });
        }
    }
}