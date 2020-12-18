
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditWarehouseDto : EntityDto<int?>
    {

		[Required]
		public string Name { get; set; }
		
		
		public string AddressLine1 { get; set; }
		
		
		public string AddressLine2 { get; set; }
		
		
		public string PostalCode { get; set; }
		
		
		public string City { get; set; }
		
		
		public string State { get; set; }
		
		
		public string Country { get; set; }
		
		

    }
}