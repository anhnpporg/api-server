using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.FcmNoti;
using UtNhanDrug_BE.Services.FcmNotificationService;
using UtNhanDrug_BE.Services.InventorySystemReportsService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IInventoryReport _inventoryReport;
        public NotificationController(INotificationService notificationService, IInventoryReport inventoryReport)
        {
            _notificationService = notificationService;
            _inventoryReport = inventoryReport;
        }

        [Route("send-notification")]
        [HttpPost]
        public async Task<IActionResult> SendNotification([FromForm] NotificationModel notificationModel)
        {
            var result = await _notificationService.SendNotification(notificationModel);
            return Ok(result);  
        }

        [Authorize]
        [Route("notification")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetNotification()
        {
            var result = await _inventoryReport.ViewNoti();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPatch("notification/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CheckReadNotification([FromRoute] int id)
        {
            await _inventoryReport.CheckViewNoti(id);
            return Ok();
        }
    }
}
