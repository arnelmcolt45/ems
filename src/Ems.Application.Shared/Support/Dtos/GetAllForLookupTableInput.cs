using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}