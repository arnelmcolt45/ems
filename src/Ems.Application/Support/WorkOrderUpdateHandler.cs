
using Abp.Dependency;
using Abp.Domain.Services;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Ems.MultiTenancy;
using Abp.Domain.Uow;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Ems.Support.Dtos;

namespace Ems.Support
{
    class WorkOrderUpdateHandler : EmsAppServiceBase, IDomainService, ITransientDependency,
        IAsyncEventHandler<EntityCreatedEventData<WorkOrderUpdate>>
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<WorkOrderUpdate> _workOrderUpdateRepository;

        public WorkOrderUpdateHandler(
            IBackgroundJobManager backgroundJobManager,
            IRepository<WorkOrderUpdate> workOrderUpdateRepository            )
        {
            _backgroundJobManager = backgroundJobManager;
            _workOrderUpdateRepository = workOrderUpdateRepository;
        }

        [UnitOfWork]
        public async Task HandleEventAsync(EntityCreatedEventData<WorkOrderUpdate> eventData)
        {
            TenantInfo tenantInfo = await TenantManager.GetTenantInfo();
            GetWorkOrderUpdateForViewDto entity = new GetWorkOrderUpdateForViewDto { WorkOrderUpdate = ObjectMapper.Map<WorkOrderUpdateDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<WorkOrderUpdateManager, WorkOrderUpdateManagerArgs>(
                new WorkOrderUpdateManagerArgs
                {
                    WorkOrderUpdateDto = entity.WorkOrderUpdate,
                    TenantInfo = tenantInfo
                });
        }
    }
}

