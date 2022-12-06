using Quartz;
using System.Threading.Tasks;
using UtNhanDrug_BE.Services.HandlerService;

namespace UtNhanDrug_BE.Services.ScheduleService
{
    public class CheckExpiryBatch3MonthSchedule : IJob
    {
        private readonly IHandlerSvc _handlerSvc;
        public CheckExpiryBatch3MonthSchedule(IHandlerSvc handlerSvc)
        {
            _handlerSvc = handlerSvc;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _handlerSvc.CheckExpiryBatch1();
        }
    }
}
