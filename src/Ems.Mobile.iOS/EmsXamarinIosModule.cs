using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Ems
{
    [DependsOn(typeof(EmsXamarinSharedModule))]
    public class EmsXamarinIosModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EmsXamarinIosModule).GetAssembly());
        }
    }
}