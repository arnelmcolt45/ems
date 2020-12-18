using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllWarehousesForExcelInput
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string AddressLine1Filter { get; set; }

		public string AddressLine2Filter { get; set; }

		public string PostalCodeFilter { get; set; }

		public string CityFilter { get; set; }

		public string StateFilter { get; set; }

		public string CountryFilter { get; set; }



    }
}