using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Assets.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Assets.Exporting
{
    public class WarehousesExcelExporter : EpPlusExcelExporterBase, IWarehousesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public WarehousesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetWarehouseForViewDto> warehouses)
        {
            return CreateExcelPackage(
                "Warehouses.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Warehouses"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("AddressLine1"),
                        L("AddressLine2"),
                        L("PostalCode"),
                        L("City"),
                        L("State"),
                        L("Country")
                        );

                    AddObjects(
                        sheet, 2, warehouses,
                        _ => _.Warehouse.Name,
                        _ => _.Warehouse.AddressLine1,
                        _ => _.Warehouse.AddressLine2,
                        _ => _.Warehouse.PostalCode,
                        _ => _.Warehouse.City,
                        _ => _.Warehouse.State,
                        _ => _.Warehouse.Country
                        );

					
					
                });
        }
    }
}
