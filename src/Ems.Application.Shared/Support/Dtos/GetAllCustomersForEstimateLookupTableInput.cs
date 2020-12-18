using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ems.Support.Dtos
{
    public class GetAllCustomersForEstimateLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int WorkOrderId { get; set; }

        public int QuotationId { get; set; }
    }
}
