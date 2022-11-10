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
            return StatusCode(suppliers.StatusCode, suppliers);
        }

        [Authorize]
        [Route("suppliers/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetSupplierById([FromRoute] int id)
        {
            var supplier = await _supplierSvc.GetSupplierById(id);
            return StatusCode(supplier.StatusCode, supplier);
        }

        [Authorize]
        [Route("suppliers/{id}/batches")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetBatchesBySupplierId([FromRoute] int id)
        {
            var batches = await _supplierSvc.GetListBatch(id);
            return StatusCode(batches.StatusCode, batches);
        }
        
        [Authorize]
        [Route("suppliers/{id}/products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductsBySupplierId([FromRoute] int id)
        {
            var products = await _supplierSvc.GetListProduct(id);
            return StatusCode(products.StatusCode, products);
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
                return BadRequest(new { message = "Bạn chưa đăng nhập" });
            }
            var result = await _supplierSvc.CreateSupplier(userId, model);
            return StatusCode(result.StatusCode, result);
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
            var result = await _supplierSvc.UpdateSupplier(id, userId, model);
            return StatusCode(result.StatusCode, result);
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

            var result = await _supplierSvc.IsDeleteSupplier(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
