using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.SamplePrescriptionModel;
using UtNhanDrug_BE.Services.SamplePrescriptionService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/sale-management")]
    public class SamplePrescriptionController : ControllerBase
    {
        private readonly ISamplePrescriptionSvc _spSvc;
        public SamplePrescriptionController(ISamplePrescriptionSvc spSvc)
        {
            _spSvc = spSvc;
        }

        [Authorize(Roles = "MANAGER")]
        [Route("sample-prescriptions")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllSamplePrescription()
        {
            var result = await _spSvc.GetAllSamplePrescription();
            return Ok(result);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("sample-prescriptions/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetSamplePrescriptionById([FromRoute] int id)
        {
            var result = await _spSvc.GetSamplePrescriptionById(id);
            if (result == null) return NotFound(new { message = "Not found this sample prescriptions" });
            return Ok(result);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("sample-prescriptions")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreatePrescription([FromForm] CreateSamplePrescriptionModel model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId;
            try
            {
                userId = Convert.ToInt32(claim[0].Value);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
            var result = await _spSvc.CreateSamplePrescription(userId, model);
            if (!result) return BadRequest(new { message = "Create sample prescriptions fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("sample-prescriptions/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateSamplePrescription([FromRoute] int id, [FromForm] UpdateSamplePrescriptionModel model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId;
            try
            {
                userId = Convert.ToInt32(claim[0].Value);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
            var isExit = await _spSvc.CheckSamplePrescription(id);
            if (!isExit) return NotFound(new { message = "Not found this sample prescriptions" });
            var result = await _spSvc.UpdateSamplePrescription(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("sample-prescriptions/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteSamplePrescription([FromRoute] int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId;
            try
            {
                userId = Convert.ToInt32(claim[0].Value);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }

            var isExit = await _spSvc.CheckSamplePrescription(id);
            if (!isExit) return NotFound(new { message = "Not found this sample prescriptions" });
            var result = await _spSvc.DeleteSamplePrescription(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
