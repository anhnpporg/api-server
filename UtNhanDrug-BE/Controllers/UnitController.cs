using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.UnitModel;
using UtNhanDrug_BE.Services.UnitService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/product-management")]
    public class UnitController : ControllerBase
    {
        private readonly IUnitSvc _unitSvc;
        public UnitController(IUnitSvc unitSvc)
        {
            _unitSvc = unitSvc;
        }

        [Authorize]
        [Route("units")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllUnit()
        {
            var unit = await _unitSvc.GetAllUnit();
            return Ok(unit);
        }

        [Authorize]
        [Route("units/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetUnitById([FromRoute] int id)
        {
            var unit = await _unitSvc.GetUnitById(id);
            if (unit == null) return NotFound(new { message = "Not found this unit" });
            return Ok(unit);
        }

        //[Authorize(Roles = "MANAGER")]
        //[Route("units")]
        //[HttpPost]
        //[MapToApiVersion("1.0")]
        //public async Task<ActionResult> CreateUnit([FromForm] CreateUnitModel model)
        //{
        //    var result = await _unitSvc.CreateUnit(model);
        //    if (!result) return BadRequest(new { message = "Create dosage unit fail" });
        //    return Ok(new { message = "create successfully" });
        //}

        //[Authorize(Roles = "MANAGER")]
        //[HttpPut("units/{id}")]
        //[MapToApiVersion("1.0")]
        //public async Task<ActionResult> UpdateUnit([FromRoute] int id, [FromForm] UpdateUnitModel model)
        //{
        //    var isExit = await _unitSvc.CheckUnit(id);
        //    if (!isExit) return NotFound(new { message = "Not found this unit" });
        //    var result = await _unitSvc.UpdateUnit(id, model);
        //    if (!result) return BadRequest(new { message = "Update fail" });
        //    return Ok(new { message = "update succesfully" });
        //}
    }
}
