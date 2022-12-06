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
using static UtNhanDrug_BE.Models.SamplePrescriptionModel.SamplePrescriptionViewModel;

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

        [Authorize]
        [Route("sample-prescriptions/{samplePrescriptionId}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetSamplePrescriptionById([FromRoute] int samplePrescriptionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();

            string role;
            try
            {
                role = claim[2].Value;

                if (role.Equals("MANAGER"))
                {
                    var samplePrescriptionForManager = await _spSvc.GetSamplePrescriptionForManager(samplePrescriptionId);
                    return StatusCode(samplePrescriptionForManager.StatusCode, samplePrescriptionForManager);
                }
                else
                {
                    var samplePrescriptionForStaff = await _spSvc.GetSamplePrescriptionForStaff(samplePrescriptionId);
                    return StatusCode(samplePrescriptionForStaff.StatusCode, samplePrescriptionForStaff);
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize(Roles = "MANAGER")]
        [Route("sample-prescriptions")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreatePrescription([FromForm] SamplePrescriptionForCreation newSamplePrescription)
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
                var createSamplePrescription = await _spSvc.CreateSamplePrescription(newSamplePrescription, userAccountId);
                return StatusCode(createSamplePrescription.StatusCode, createSamplePrescription);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("sample-prescriptions/{samplePrescriptionId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateSamplePrescription([FromRoute] int samplePrescriptionId, [FromForm] SamplePrescriptionForUpdate newSamplePrescription)
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
                var updateSamplePrescription = await _spSvc.UpdateSamplePrescription(samplePrescriptionId, newSamplePrescription, userAccountId);
                return StatusCode(updateSamplePrescription.StatusCode, updateSamplePrescription);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("sample-prescriptions/{samplePrescriptionId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteSamplePrescription([FromRoute] int samplePrescriptionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userAccountId;

            try
            {
                userAccountId = Convert.ToInt32(claim[0].Value);
                var deletedSamplePrescription = await _spSvc.DeleteSamplePrescription(samplePrescriptionId, userAccountId);
                return StatusCode(deletedSamplePrescription.StatusCode, deletedSamplePrescription);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }
    }
}
