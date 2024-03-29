﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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

        //[Authorize]
        //[Route("notification/filter")]
        //[HttpGet]
        //[MapToApiVersion("1.0")]
        //public async Task<ActionResult> GetFilterNotification()
        //{
        //    var result = await _inventoryReport.ViewFilterNoti();
        //    return StatusCode(result.StatusCode, result);
        //}
        
        [Authorize]
        [Route("notification/filter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> IGetFilterNotification()
        {
            var result = await _inventoryReport.ShowFilterNoti();
            return StatusCode(result.StatusCode, result);
        }
        
        [Authorize]
        [Route("notification")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllNotification()
        {
            var result = await _inventoryReport.ViewAllNoti();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("notification/{key}/detail")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetDetailNotification([FromRoute] DateTime key)
        {
            var result = await _inventoryReport.ViewDetailNoti(key);
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
