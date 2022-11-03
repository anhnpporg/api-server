using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.SupplierModel;
using UtNhanDrug_BE.Services.SupplierService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/suppliers-management")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierSvc _supplierSvc;
        public SupplierController(ISupplierSvc supplierSvc)
        {
            _supplierSvc = supplierSvc;
        }
        [Authorize]
        [Route("suppliers")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllSupplier()
        {
            var suppliers = await _supplierSvc.GetAllSupplier();
            return Ok(suppliers);
        }

        [Authorize]
        [Route("suppliers/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetSupplierById([FromRoute] int id)
        {
            var supplier = await _supplierSvc.GetSupplierById(id);
            if (supplier == null) return NotFound(new { message = "Not found this supplier" });
            return Ok(supplier);
        }

        [Authorize]
        [Route("suppliers")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateSupplier([FromForm] CreateSupplierModel model)
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
            var result = await _supplierSvc.CreateSupplier(userId, model);
            if (!result) return BadRequest(new { message = "Create supplier fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize]
        [HttpPut("suppliers/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateSupplier([FromRoute] int id, [FromForm] UpdateSupplierModel model)
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
            var isExit = await _supplierSvc.CheckSupplier(id);
            if (!isExit) return NotFound(new { message = "Not found this supplier" });
            var result = await _supplierSvc.UpdateSupplier(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize]
        [HttpPatch("suppliers/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteBrand([FromRoute] int id)
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

            var isExit = await _supplierSvc.CheckSupplier(id);
            if (!isExit) return NotFound(new { message = "Not found this supplier" });
            var result = await _supplierSvc.DeleteSupplier(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
