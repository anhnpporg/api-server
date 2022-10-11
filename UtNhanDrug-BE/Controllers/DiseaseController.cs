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

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/product-management")]
    public class DiseaseController : ControllerBase
    {
        private readonly IDiseaseSvc _diseaseSvc;
        public DiseaseController(IDiseaseSvc diseaseSvc)
        {
            _diseaseSvc = diseaseSvc;
        }

        [Authorize(Roles = "MANAGER")]
        [Route("diseases")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllDisease()
        {
            var disease = await _diseaseSvc.GetAllDisease();
            return Ok(disease);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("diseases/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetDiseaseById([FromRoute] int id)
        {
            var disease = await _diseaseSvc.GetDiseaseById(id);
            if (disease == null) return NotFound(new { message = "Not found this disease" });
            return Ok(disease);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("diseases")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateDisease([FromForm] CreateDiseaseModel model)
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
            var result = await _diseaseSvc.CreateDisease(userId, model);
            if (!result) return BadRequest(new { message = "Create disease fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("diseases/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateDisease([FromRoute] int id, [FromForm] UpdateDiseaseModel model)
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
            var isExit = await _diseaseSvc.CheckDisease(id);
            if (!isExit) return NotFound(new { message = "Not found this disease" });
            var result = await _diseaseSvc.UpdateDisease(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("diseases/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteDisease([FromRoute] int id)
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

            var isExit = await _diseaseSvc.CheckDisease(id);
            if (!isExit) return NotFound(new { message = "Not found this disease" });
            var result = await _diseaseSvc.DeleteDisease(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
