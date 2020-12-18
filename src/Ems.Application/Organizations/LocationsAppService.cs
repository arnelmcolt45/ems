using Ems.Authorization.Users;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Organizations.Exporting;
using Ems.Organizations.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Organizations
{
	[AbpAuthorize(AppPermissions.Pages_Administration_Locations)]
    public class LocationsAppService : EmsAppServiceBase, ILocationsAppService
    {
        private readonly string _entityType = "Location";

        private readonly IRepository<Location> _locationRepository;
		private readonly ILocationsExcelExporter _locationsExcelExporter;
		private readonly IRepository<User,long> _lookup_userRepository;
		 

		  public LocationsAppService(IRepository<Location> locationRepository, ILocationsExcelExporter locationsExcelExporter , IRepository<User, long> lookup_userRepository) 
		  {
			_locationRepository = locationRepository;
			_locationsExcelExporter = locationsExcelExporter;
			_lookup_userRepository = lookup_userRepository;
		
		  }

		 public async Task<PagedResultDto<GetLocationForViewDto>> GetAll(GetAllLocationsInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var filteredLocations = _locationRepository.GetAll()
						.Include( e => e.UserFk)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.LocationName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.LocationNameFilter),  e => e.LocationName == input.LocationNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

			var pagedAndFilteredLocations = filteredLocations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var locations = from o in pagedAndFilteredLocations
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetLocationForViewDto() {
							Location = new LocationDto
							{
                                LocationName = o.LocationName,
                                Id = o.Id
							},
                         	UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
						};

            var totalCount = await filteredLocations.CountAsync();

            return new PagedResultDto<GetLocationForViewDto>(
                totalCount,
                await locations.ToListAsync()
            );
         }
		 
		 public async Task<GetLocationForViewDto> GetLocationForView(int id)
         {
            var location = await _locationRepository.GetAsync(id);

            var output = new GetLocationForViewDto { Location = ObjectMapper.Map<LocationDto>(location) };

		    if (output.Location.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Location.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Administration_Locations_Edit)]
		 public async Task<GetLocationForEditOutput> GetLocationForEdit(EntityDto input)
         {
            var location = await _locationRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetLocationForEditOutput {Location = ObjectMapper.Map<CreateOrEditLocationDto>(location)};

		    if (output.Location.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Location.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditLocationDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_Locations_Create)]
		 protected virtual async Task Create(CreateOrEditLocationDto input)
         {
            var location = ObjectMapper.Map<Location>(input);

			
			if (AbpSession.TenantId != null)
			{
				location.TenantId = (int?) AbpSession.TenantId;
			}
            if (AbpSession.UserId != null)
            {
                location.UserId = (int)AbpSession.UserId;
            }

            await _locationRepository.InsertAsync(location);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_Locations_Edit)]
		 protected virtual async Task Update(CreateOrEditLocationDto input)
         {
            var location = await _locationRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, location);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_Locations_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _locationRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetLocationsToExcel(GetAllLocationsForExcelInput input)
         {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var filteredLocations = _locationRepository.GetAll()
						.Include( e => e.UserFk)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.LocationName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.LocationNameFilter),  e => e.LocationName == input.LocationNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

			var query = (from o in filteredLocations
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetLocationForViewDto() { 
							Location = new LocationDto
							{
                                LocationName = o.LocationName,
                                Id = o.Id
							},
                         	UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
						 });


            var locationListDtos = await query.ToListAsync();

            return _locationsExcelExporter.ExportToFile(locationListDtos);
         }
    }
}

/* - this was Akshay's silly home-grown app service without any front end.  Replaced by the above
 
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Ems.Organizations.Dtos;
using System.Threading.Tasks;
using System.Linq;

namespace Ems.Organizations
{
    public class LocationAppService : EmsAppServiceBase, ILocationAppService
    {
        private readonly IRepository<Location> _locationRepository;

        public LocationAppService(IRepository<Location> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task CreateOrEdit(CreateOrEditLocationDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        protected virtual async Task Create(CreateOrEditLocationDto input)
        {
            var location = ObjectMapper.Map<Location>(input);

            if (AbpSession.TenantId != null)
            {
                location.TenantId = (int?)AbpSession.TenantId;
            }

            await _locationRepository.InsertAsync(location);
        }

        protected virtual async Task Update(CreateOrEditLocationDto input)
        {
            var location = await _locationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, location);
        }

        public async Task Delete(EntityDto input)
        {
            await _locationRepository.DeleteAsync(input.Id);
        }
    }
}


*/