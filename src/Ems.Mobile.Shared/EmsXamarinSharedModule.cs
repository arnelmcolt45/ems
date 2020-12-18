using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Ems
{
    [DependsOn(typeof(EmsClientModule), typeof(AbpAutoMapperModule))]
    public class EmsXamarinSharedModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EmsXamarinSharedModule).GetAssembly());
        }
    }
}