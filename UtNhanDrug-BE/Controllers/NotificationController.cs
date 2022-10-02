﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.FcmNoti;
using UtNhanDrug_BE.Services.FcmNotificationService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Route("send-notification")]
        [HttpPost]
        public async Task<IActionResult> SendNotification([FromForm] NotificationModel notificationModel)
        {
            var result = await _notificationService.SendNotification(notificationModel);
            return Ok(result);  
        }
    }
}
