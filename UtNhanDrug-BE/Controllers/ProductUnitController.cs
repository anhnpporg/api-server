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
        private readonly IProductUnitPriceSvc _productUnitSvc;
        public ProductUnitController(IProductUnitPriceSvc productUnitSvc)
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
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("product-units/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductUnitById([FromRoute] int id)
        {
            var result = await _productUnitSvc.GetProductUnitById(id);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("product-units")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateProductUnit([FromForm] CreateProductUnitPriceModel model)
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
            var result = await _productUnitSvc.CreateProductUnit(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("product-units/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateProductUnit([FromRoute] int id, [FromForm] UpdateProductUnitPriceModel model)
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
            var result = await _productUnitSvc.UpdateProductUnit(id, userId, model);
            return StatusCode(result.StatusCode, result);
        }
    }
}
