﻿using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.MultiTenancy.Dto;

namespace Ems.MultiTenancy
{
    public interface ITenantAppService : IApplicationService
    {
        Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input);

        Task CreateTenant(CreateTenantInput input);

        Task CreateTenantForCustomer(CreateTenantInput input);

        Task CreateTenantForVendor(CreateTenantInput input);

        Task<TenantEditDto> GetTenantForEdit(EntityDto input);

        Task<string> GetTenantLogoId(EntityDto input);

        Task UpdateTenant(TenantEditDto input);

        Task DeleteTenant(EntityDto input);

        Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input);

        Task UpdateTenantFeatures(UpdateTenantFeaturesInput input);

        Task ResetTenantSpecificFeatures(EntityDto input);

        Task UnlockTenantAdmin(EntityDto input);
    }
}
