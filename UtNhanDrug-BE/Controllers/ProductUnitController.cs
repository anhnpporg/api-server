using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductUnitModel;
using UtNhanDrug_BE.Services.ProductUnitService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/product-units-management")]
    public class ProductUnitController : ControllerBase
    {
        private readonly IProductUnitSvc _productUnitSvc;
        public ProductUnitController(IProductUnitSvc productUnitSvc)
        {
            _productUnitSvc = productUnitSvc;
        }

        [Authorize]
        [Route("{id}/product-units")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductUnitByProductId([FromRoute] int id)
        {
            var result = await _productUnitSvc.GetProductUnitByProductId(id);
            return Ok(result);
        }

        [Authorize]
        [Route("product-units/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductUnitById([FromRoute] int id)
        {
            var result = await _productUnitSvc.GetProductUnitById(id);
            if (result == null) return NotFound(new { message = "Not found this product unit" });
            return Ok(result);
        }

        [Authorize]
        [Route("product-units")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateProductUnit([FromForm] CreateProductUnitModel model)
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
            var result = await _productUnitSvc.CreateProductUnit(model);
            if (!result) return BadRequest(new { message = "Create product unit fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize]
        [HttpPut("product-units/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateProductUnit([FromRoute] int id, [FromForm] UpdateProductUnitModel model)
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
            var isExit = await _productUnitSvc.CheckProductUnit(id);
            if (!isExit) return NotFound(new { message = "Not found this product unit" });
            var result = await _productUnitSvc.UpdateProductUnit(id, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }
    }
}
