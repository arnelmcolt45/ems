using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ems.Support.Dtos
{
    public class GetAllCustomersForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int AssetId { get; set; }
    }
}
