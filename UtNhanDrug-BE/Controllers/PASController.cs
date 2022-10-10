using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductActiveSubstance;
using UtNhanDrug_BE.Services.ProductActiveSubstanceService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/product-management")]
    public class PASController : ControllerBase
    {
        private readonly IPASSvc _pasSvc;
        public PASController(IPASSvc pasSvc)
        {
            _pasSvc = pasSvc;
        }

        [Authorize(Roles = "MANAGER")]
        [Route("product-active-substance")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllPAS()
        {
            var unit = await _pasSvc.GetAllPAS();
            return Ok(unit);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("product-active-substance/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetPASById([FromRoute] int id)
        {
            var unit = await _pasSvc.GetPASById(id);
            if (unit == null) return NotFound(new { message = "Not found this product active substance" });
            return Ok(unit);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("product-active-substance")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateUnit([FromForm] CreatePASModel model)
        {
            var result = await _pasSvc.CreatePAS(model);
            if (!result) return BadRequest(new { message = "Create product active substance fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("product-active-substance/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateUnit([FromRoute] int id, [FromForm] UpdatePASModel model)
        {
            var isExit = await _pasSvc.CheckPAS(id);
            if (!isExit) return NotFound(new { message = "Not found this product active substance" });
            var result = await _pasSvc.UpdatePAS(id, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }
    }
}
