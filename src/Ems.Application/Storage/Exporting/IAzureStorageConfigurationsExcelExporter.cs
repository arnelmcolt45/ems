using System.Collections.Generic;
using Ems.Storage.Dtos;
using Ems.Dto;

namespace Ems.Storage.Exporting
{
    public interface IAzureStorageConfigurationsExcelExporter
    {
        FileDto ExportToFile(List<GetAzureStorageConfigurationForViewDto> azureStorageConfigurations);
    }
}