
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Organizations.Dtos
{
    public class CreateOrEditLocationDto : EntityDto<int?>
    {

		public string LocationName { get; set; }
		
		
		 public long? UserId { get; set; }
		 
		 
    }
}