using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class GetBillingEventTypeForEditOutput
    {
		public CreateOrEditBillingEventTypeDto BillingEventType { get; set; }


    }
}