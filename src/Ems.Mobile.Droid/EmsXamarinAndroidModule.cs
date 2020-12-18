using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Ems
{
    [DependsOn(typeof(EmsXamarinSharedModule))]
    public class EmsXamarinAndroidModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EmsXamarinAndroidModule).GetAssembly());
        }
    }
}