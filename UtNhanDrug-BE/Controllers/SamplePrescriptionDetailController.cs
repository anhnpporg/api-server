using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.SamplePrescriptionDetailModel;
using UtNhanDrug_BE.Services.SamplePrescriptionDetailService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/sale-management")]
    public class SamplePrescriptionDetailController : ControllerBase
    {
        private readonly ISamplePrescriptionDetailSvc _spdSvc;
        public SamplePrescriptionDetailController(ISamplePrescriptionDetailSvc spdSvc)
        {
            _spdSvc = spdSvc;
        }

        [Authorize]
        [Route("sample-prescription-details")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllSamplePrescription()
        {
            var result = await _spdSvc.GetAllSamplePrescriptionDetail();
            return Ok(result);
        }

        [Authorize]
        [Route("sample-prescription-details/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetSamplePrescriptionDetailById([FromRoute] int id)
        {
            var result = await _spdSvc.GetSamplePrescriptionDetailById(id);
            if (result == null) return NotFound(new { message = "Not found this sample prescription details" });
            return Ok(result);
        }

        [Authorize]
        [Route("sample-prescription-details")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreatePrescriptionDetail([FromForm] CreateSPDModel model)
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
            var result = await _spdSvc.CreateSamplePrescriptionDetail(userId, model);
            if (!result) return BadRequest(new { message = "Create sample prescription details fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize]
        [HttpPut("sample-prescription-details/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateSamplePrescriptionDetail([FromRoute] int id, [FromForm] UpdateSPDModel model)
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
            var isExit = await _spdSvc.CheckSamplePrescriptionDetail(id);
            if (!isExit) return NotFound(new { message = "Not found this sample prescription details" });
            var result = await _spdSvc.UpdateSamplePrescriptionDetail(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize]
        [HttpPatch("sample-prescription-details/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteSamplePrescriptionDetail([FromRoute] int id)
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

            var isExit = await _spdSvc.CheckSamplePrescriptionDetail(id);
            if (!isExit) return NotFound(new { message = "Not found this sample prescription details" });
            var result = await _spdSvc.DeleteSamplePrescriptionDetail(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
