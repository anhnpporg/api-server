using Quartz;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Services.HandlerService;

namespace UtNhanDrug_BE.Services.ScheduleService
{
    public class SchedulerService : IJob
    {
        private readonly IHandlerSvc _handlerSvc;
        public SchedulerService(IHandlerSvc handlerSvc)
        {
            _handlerSvc = handlerSvc;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _handlerSvc.CheckExpiryBatch();
        }
    }
}
