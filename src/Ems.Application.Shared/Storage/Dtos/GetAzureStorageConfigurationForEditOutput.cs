using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Storage.Dtos
{
    public class GetAzureStorageConfigurationForEditOutput
    {
		public CreateOrEditAzureStorageConfigurationDto AzureStorageConfiguration { get; set; }


    }
}