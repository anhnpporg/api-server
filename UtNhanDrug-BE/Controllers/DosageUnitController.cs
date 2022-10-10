using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.DosageUnitModel;
using UtNhanDrug_BE.Services.DosageUnitService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/product-management")]
    public class DosageUnitController : ControllerBase
    {
        private readonly IDosageUnitSvc _dosageUnitSvc;
        public DosageUnitController(IDosageUnitSvc dosageUnitSvc)
        {
            _dosageUnitSvc = dosageUnitSvc;
        }

        [Authorize(Roles = "MANAGER")]
        [Route("dosage-units")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllDosageUnit()
        {
            var dosageUnit = await _dosageUnitSvc.GetAllDosageUnit();
            return Ok(dosageUnit);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("dosage-units/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetDosageUnitById([FromRoute] int id)
        {
            var dosageUnit = await _dosageUnitSvc.GetDosageUnitById(id);
            if (dosageUnit == null) return NotFound(new { message = "Not found this dosage unit" });
            return Ok(dosageUnit);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("dosage-units")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateDosageUnit([FromForm] CreateDosageUnitModel model)
        {
            var result = await _dosageUnitSvc.CreateDosageUnit(model);
            if (!result) return BadRequest(new { message = "Create dosage unit fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("dosage-units/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateDosageUnit([FromRoute] int id, [FromForm] UpdateDosageUnitModel model)
        {
            var isExit = await _dosageUnitSvc.CheckDosageUnit(id);
            if (!isExit) return NotFound(new { message = "Not found this dosage unit" });
            var result = await _dosageUnitSvc.UpdateDosageUnit(id, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }
    }
}
