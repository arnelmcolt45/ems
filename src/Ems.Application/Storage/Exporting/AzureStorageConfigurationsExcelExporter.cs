using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Storage.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Storage.Exporting
{
    public class AzureStorageConfigurationsExcelExporter : EpPlusExcelExporterBase, IAzureStorageConfigurationsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AzureStorageConfigurationsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAzureStorageConfigurationForViewDto> azureStorageConfigurations)
        {
            return CreateExcelPackage(
                "AzureStorageConfigurations.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AzureStorageConfigurations"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Service"),
                        L("AccountName"),
                        L("KeyValue"),
                        L("BlobStorageEndpoint"),
                        L("ContainerName")
                        );

                    AddObjects(
                        sheet, 2, azureStorageConfigurations,
                        _ => _.AzureStorageConfiguration.Service,
                        _ => _.AzureStorageConfiguration.AccountName,
                        _ => _.AzureStorageConfiguration.KeyValue,
                        _ => _.AzureStorageConfiguration.BlobStorageEndpoint,
                        _ => _.AzureStorageConfiguration.ContainerName
                        );

					
					
                });
        }
    }
}
