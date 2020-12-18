using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Organizations.Dtos
{
    public class GetLocationForEditOutput
    {
		public CreateOrEditLocationDto Location { get; set; }

		public string UserName { get; set;}


    }
}