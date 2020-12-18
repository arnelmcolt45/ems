using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Ems
{
    public class EmsClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EmsClientModule).GetAssembly());
        }
    }
}
