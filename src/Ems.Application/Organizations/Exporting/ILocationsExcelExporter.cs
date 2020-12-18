using System.Collections.Generic;
using Ems.Organizations.Dtos;
using Ems.Dto;

namespace Ems.Organizations.Exporting
{
    public interface ILocationsExcelExporter
    {
        FileDto ExportToFile(List<GetLocationForViewDto> locations);
    }
}