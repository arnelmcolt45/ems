
using System;
using Abp.Application.Services.Dto;

namespace Ems.Organizations.Dtos
{
    public class LocationDto : EntityDto
    {
		public string LocationName { get; set; }


		 public long? UserId { get; set; }

		 
    }
}