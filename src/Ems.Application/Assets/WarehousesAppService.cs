

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Assets.Exporting;
using Ems.Assets.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Assets
{
	[AbpAuthorize(AppPermissions.Pages_Main_Warehouses)]
    public class WarehousesAppService : EmsAppServiceBase, IWarehousesAppService
    {
		 private readonly IRepository<Warehouse> _warehouseRepository;
		 private readonly IWarehousesExcelExporter _warehousesExcelExporter;
		 

		  public WarehousesAppService(IRepository<Warehouse> warehouseRepository, IWarehousesExcelExporter warehousesExcelExporter ) 
		  {
			_warehouseRepository = warehouseRepository;
			_warehousesExcelExporter = warehousesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetWarehouseForViewDto>> GetAll(GetAllWarehousesInput input)
         {
			
			var filteredWarehouses = _warehouseRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.AddressLine1.Contains(input.Filter) || e.AddressLine2.Contains(input.Filter) || e.PostalCode.Contains(input.Filter) || e.City.Contains(input.Filter) || e.State.Contains(input.Filter) || e.Country.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AddressLine1Filter),  e => e.AddressLine1 == input.AddressLine1Filter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AddressLine2Filter),  e => e.AddressLine2 == input.AddressLine2Filter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.PostalCodeFilter),  e => e.PostalCode == input.PostalCodeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CityFilter),  e => e.City == input.CityFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.StateFilter),  e => e.State == input.StateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CountryFilter),  e => e.Country == input.CountryFilter);

			var pagedAndFilteredWarehouses = filteredWarehouses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var warehouses = from o in pagedAndFilteredWarehouses
                         select new GetWarehouseForViewDto() {
							Warehouse = new WarehouseDto
							{
                                Name = o.Name,
                                AddressLine1 = o.AddressLine1,
                                AddressLine2 = o.AddressLine2,
                                PostalCode = o.PostalCode,
                                City = o.City,
                                State = o.State,
                                Country = o.Country,
                                Id = o.Id
							}
						};

            var totalCount = await filteredWarehouses.CountAsync();

            return new PagedResultDto<GetWarehouseForViewDto>(
                totalCount,
                await warehouses.ToListAsync()
            );
         }
		 
		 public async Task<GetWarehouseForViewDto> GetWarehouseForView(int id)
         {
            var warehouse = await _warehouseRepository.GetAsync(id);

            var output = new GetWarehouseForViewDto { Warehouse = ObjectMapper.Map<WarehouseDto>(warehouse) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_Warehouses_Edit)]
		 public async Task<GetWarehouseForEditOutput> GetWarehouseForEdit(EntityDto input)
         {
            var warehouse = await _warehouseRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetWarehouseForEditOutput {Warehouse = ObjectMapper.Map<CreateOrEditWarehouseDto>(warehouse)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditWarehouseDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Warehouses_Create)]
		 protected virtual async Task Create(CreateOrEditWarehouseDto input)
         {
            var warehouse = ObjectMapper.Map<Warehouse>(input);

			
			if (AbpSession.TenantId != null)
			{
				warehouse.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _warehouseRepository.InsertAsync(warehouse);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Warehouses_Edit)]
		 protected virtual async Task Update(CreateOrEditWarehouseDto input)
         {
            var warehouse = await _warehouseRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, warehouse);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Warehouses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _warehouseRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetWarehousesToExcel(GetAllWarehousesForExcelInput input)
         {
			
			var filteredWarehouses = _warehouseRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.AddressLine1.Contains(input.Filter) || e.AddressLine2.Contains(input.Filter) || e.PostalCode.Contains(input.Filter) || e.City.Contains(input.Filter) || e.State.Contains(input.Filter) || e.Country.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AddressLine1Filter),  e => e.AddressLine1 == input.AddressLine1Filter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AddressLine2Filter),  e => e.AddressLine2 == input.AddressLine2Filter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.PostalCodeFilter),  e => e.PostalCode == input.PostalCodeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CityFilter),  e => e.City == input.CityFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.StateFilter),  e => e.State == input.StateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CountryFilter),  e => e.Country == input.CountryFilter);

			var query = (from o in filteredWarehouses
                         select new GetWarehouseForViewDto() { 
							Warehouse = new WarehouseDto
							{
                                Name = o.Name,
                                AddressLine1 = o.AddressLine1,
                                AddressLine2 = o.AddressLine2,
                                PostalCode = o.PostalCode,
                                City = o.City,
                                State = o.State,
                                Country = o.Country,
                                Id = o.Id
							}
						 });


            var warehouseListDtos = await query.ToListAsync();

            return _warehousesExcelExporter.ExportToFile(warehouseListDtos);
         }


    }
}