using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class GetAllUsingIdForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int FilterId { get; set; }
    }
}