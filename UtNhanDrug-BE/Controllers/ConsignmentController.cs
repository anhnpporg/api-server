using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ConsignmentModel;
using UtNhanDrug_BE.Services.ConsignmentService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/consignment-management")]
    public class ConsignmentController : ControllerBase
    {
        private readonly IConsignmentSvc _consignmentSvc;
        public ConsignmentController(IConsignmentSvc consignmentSvc)
        {
            _consignmentSvc = consignmentSvc;
        }

        [Authorize]
        [Route("consignments")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllConsignment()
        {
            var disease = await _consignmentSvc.GetAllConsignment();
            return Ok(disease);
        }

        [Authorize]
        [Route("consignments/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetConsignmentById([FromRoute] int id)
        {
            var disease = await _consignmentSvc.GetConsignmentById(id);
            if (disease == null) return NotFound(new { message = "Not found this consignment" });
            return Ok(disease);
        }

        [Authorize]
        [Route("consignments")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateConsignment([FromForm] CreateConsignmentModel model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId;
            try
            {
                userId = Convert.ToInt32(claim[1].Value);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
            var result = await _consignmentSvc.CreateConsignment(userId, model);
            if (!result) return BadRequest(new { message = "Create consignment fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize]
        [HttpPut("consignments/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateConsignment([FromRoute] int id, [FromForm] UpdateConsignmentModel model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId;
            try
            {
                userId = Convert.ToInt32(claim[1].Value);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
            var isExit = await _consignmentSvc.CheckConsignment(id);
            if (!isExit) return NotFound(new { message = "Not found this consignment" });
            var result = await _consignmentSvc.UpdateConsignment(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }
        
        [Authorize]
        [HttpPatch("consignments/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteConsignment([FromRoute] int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId;
            try
            {
                userId = Convert.ToInt32(claim[1].Value);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }

            var isExit = await _consignmentSvc.CheckConsignment(id);
            if (!isExit) return NotFound(new { message = "Not found this consignment" });
            var result = await _consignmentSvc.DeleteConsignment(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
