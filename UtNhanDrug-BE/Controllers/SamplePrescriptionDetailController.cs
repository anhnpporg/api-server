using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Services.SamplePrescriptionDetailService;
using static UtNhanDrug_BE.Models.SamplePrescriptionDetailModel.SamplePrescriptionDetailViewModel;

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

        [Authorize(Roles = "MANAGER")]
        [Route("sample-prescription-details")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreatePrescriptionDetail([FromForm] SamplePrescriptionDetailForCreation newSamplePrescriptionDetail)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userAccountId;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                userAccountId = Convert.ToInt32(claim[0].Value);
                var updateSamplePrescription = await _spdSvc.CreateSamplePrescriptionDetail(newSamplePrescriptionDetail, userAccountId);
                return StatusCode(updateSamplePrescription.StatusCode, updateSamplePrescription);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("sample-prescription-details/{samplePrescriptionDetailId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateSamplePrescriptionDetail([FromRoute] int samplePrescriptionDetailId, [FromForm] SamplePrescriptionDetailForUpdate newSamplePrescriptionDetail)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userAccountId;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                userAccountId = Convert.ToInt32(claim[0].Value);
                var updateSamplePrescription = await _spdSvc.UpdateSamplePrescriptionDetail(samplePrescriptionDetailId, newSamplePrescriptionDetail, userAccountId);
                return StatusCode(updateSamplePrescription.StatusCode, updateSamplePrescription);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize(Roles = "MANAGER")]
        [HttpDelete("sample-prescription-details/{samplePrescriptionDetailId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteSamplePrescriptionDetail([FromRoute] int samplePrescriptionDetailId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userAccountId;

            try
            {
                userAccountId = Convert.ToInt32(claim[0].Value);
                var deletedSamplePrescription = await _spdSvc.DeleteSamplePrescriptionDetail(samplePrescriptionDetailId, userAccountId);
                return StatusCode(deletedSamplePrescription.StatusCode, deletedSamplePrescription);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }
    }
}
