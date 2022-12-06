using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.DiseaseModel;
using UtNhanDrug_BE.Services.DiseaseService;
using UtNhanDrug_BE.Services.SamplePrescriptionService;
using static UtNhanDrug_BE.Models.DiseaseModel.DiseaseViewModel;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/sale-management")]
    public class DiseaseController : ControllerBase
    {
        private readonly IDiseaseSvc _diseaseSvc;
        private readonly ISamplePrescriptionSvc _spSvc;

        public DiseaseController(IDiseaseSvc diseaseSvc, ISamplePrescriptionSvc spSvc)
        {
            _diseaseSvc = diseaseSvc;
            _spSvc = spSvc;
        }

        [Authorize]
        [Route("diseases")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllDisease()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();

            string role;
            try
            {
                role = claim[2].Value;

                if (role.Equals("MANAGER"))
                {
                    var diseasesForManager = await _diseaseSvc.GetDiseasesForManager();
                    return StatusCode(diseasesForManager.StatusCode, diseasesForManager);
                }
                else
                {
                    var diseasesForStaff = await _diseaseSvc.GetDiseasesForStaff();
                    return StatusCode(diseasesForStaff.StatusCode, diseasesForStaff);
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize]
        [Route("diseases/{diseaseId}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetDiseaseById([FromRoute] int diseaseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();

            string role;
            try
            {
                role = claim[2].Value;

                if (role.Equals("MANAGER"))
                {
                    var diseaseForManager = await _diseaseSvc.GetDiseaseForManager(diseaseId);
                    return StatusCode(diseaseForManager.StatusCode, diseaseForManager);
                }
                else
                {
                    var diseaseForStaff = await _diseaseSvc.GetDiseaseForStaff(diseaseId);
                    return StatusCode(diseaseForStaff.StatusCode, diseaseForStaff);
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize(Roles = "MANAGER")]
        [Route("diseases")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateDisease([FromForm] DiseaseForCreation newDisease)
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
                var createdDisease = await _diseaseSvc.CreateDisease(newDisease, userAccountId);
                return StatusCode(createdDisease.StatusCode, createdDisease);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("diseases/{diseaseId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateDisease([FromRoute] int diseaseId, [FromForm] DiseaseForUpdate newDisease)
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
                var updatedDisease = await _diseaseSvc.UpdateDisease(diseaseId, newDisease, userAccountId);
                return StatusCode(updatedDisease.StatusCode, updatedDisease);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("diseases/{diseaseId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteDisease([FromRoute] int diseaseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userAccountId;

            try
            {
                userAccountId = Convert.ToInt32(claim[0].Value);
                var deletedDisease = await _diseaseSvc.DeleteDisease(diseaseId, userAccountId);
                return StatusCode(deletedDisease.StatusCode, deletedDisease);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }

        [Authorize]
        [Route("diseases/{diseaseId}/sample-prescriptions")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllSamplePrescription([FromRoute] int diseaseId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();

            string role;
            try
            {
                role = claim[2].Value;

                if (role.Equals("MANAGER"))
                {
                    var samplePrescriptionsForManager = await _spSvc.GetSamplePrescriptionsForManager(diseaseId);
                    return StatusCode(samplePrescriptionsForManager.StatusCode, samplePrescriptionsForManager);
                }
                else
                {
                    var samplePrescriptionsForStaff = await _spSvc.GetSamplePrescriptionsForStaff(diseaseId);
                    return StatusCode(samplePrescriptionsForStaff.StatusCode, samplePrescriptionsForStaff);
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
        }
    }
}
