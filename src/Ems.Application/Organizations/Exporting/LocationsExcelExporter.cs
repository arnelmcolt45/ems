using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Organizations.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Organizations.Exporting
{
    public class LocationsExcelExporter : EpPlusExcelExporterBase, ILocationsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public LocationsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetLocationForViewDto> locations)
        {
            return CreateExcelPackage(
                "Locations.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Locations"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("LocationName"),
                        (L("User")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, locations,
                        _ => _.Location.LocationName,
                        _ => _.UserName
                        );

					
					
                });
        }
    }
}
