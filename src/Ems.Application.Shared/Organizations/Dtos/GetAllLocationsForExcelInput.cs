using Abp.Application.Services.Dto;
using System;

namespace Ems.Organizations.Dtos
{
    public class GetAllLocationsForExcelInput
    {
		public string Filter { get; set; }

		public string LocationNameFilter { get; set; }


		 public string UserNameFilter { get; set; }

		 
    }
}