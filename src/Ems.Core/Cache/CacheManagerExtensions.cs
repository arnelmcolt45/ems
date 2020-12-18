using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Ems.Authorization;
using System;

namespace Ems
{
    public static class CacheManagerExtensions
    {
        public static ITypedCache<string, CtpCacheItem> GetCrossTenantPermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, CtpCacheItem>(CtpCacheItem.CacheName);
        }

        public static ITypedCache<string, TenantInfoCacheItem> GetTenantInfoCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, TenantInfoCacheItem>(TenantInfoCacheItem.CacheName);
        }
    }

    [Serializable]
    public class CtpCacheItem
    {
        public const string CacheName = "CrossTenantPermissionCache";

        public CrossTenantPermission CrossTenantPermission { get; set; }

        public DateTime LastRefresh { get; set; }
    }

    [Serializable]
    public class TenantInfoCacheItem
    {
        public const string CacheName = "TenantInfoCache";

        public Ems.MultiTenancy.TenantInfo TenantInfo { get; set; }

        public DateTime LastRefresh { get; set; }
    }
}