using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Support.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Support.Exporting
{
    public class WorkOrderUpdatesExcelExporter : EpPlusExcelExporterBase, IWorkOrderUpdatesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public WorkOrderUpdatesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetWorkOrderUpdateForViewDto> workOrderUpdates)
        {
            return CreateExcelPackage(
                "WorkOrderUpdates.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("WorkOrderUpdates"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Comments"),
                        L("Number"),
                        (L("WorkOrder")) + L("Subject"),
                        (L("ItemType")) + L("Type"),
                        (L("WorkOrderAction")) + L("Action"),
                        (L("AssetPart")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, workOrderUpdates,
                        _ => _.WorkOrderUpdate.Comments,
                        _ => _.WorkOrderUpdate.Number,
                        _ => _.WorkOrderSubject,
                        _ => _.ItemTypeType,
                        _ => _.WorkOrderActionAction,
                        _ => _.AssetPartName
                        );

					
					
                });
        }
    }
}
