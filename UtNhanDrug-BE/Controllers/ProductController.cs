using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Services.ProductActiveSubstanceService;
using UtNhanDrug_BE.Services.ProductService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/product-management")]
    public class ProductController : ControllerBase
    {
        private readonly IProductSvc _productSvc;
        private readonly IPASSvc _pasSvc;
        public ProductController(IProductSvc productSvc, IPASSvc pasSvc)
        {
            _productSvc = productSvc;
            _pasSvc = pasSvc;
        }

        [Authorize(Roles = "MANAGER")]
        [Route("products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllProduct()
        {
            var products = await _productSvc.GetAllProduct();
            foreach (var product in products)
            {
                var pas = await _pasSvc.GetPASById(product.Id);
                product.ProductActiveSubstance = pas;
            }
            
            return Ok(products);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("products/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductById([FromRoute] int id)
        {
            var product = await _productSvc.GetProductById(id);
            if (product == null) return NotFound(new { message = "Not found this product" });
            return Ok(product);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("products")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateProduct([FromForm] CreateProductModel model)
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
            var result = await _productSvc.CreateProduct(userId, model);
            if (!result) return BadRequest(new { message = "Create product fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("products/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateProduct([FromRoute] int id, [FromForm] UpdateProductModel model)
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
            var isExit = await _productSvc.CheckProduct(id);
            if (!isExit) return NotFound(new { message = "Not found this product" });
            var result = await _productSvc.UpdateProduct(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("products/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
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

            var isExit = await _productSvc.CheckProduct(id);
            if (!isExit) return NotFound(new { message = "Not found this product" });
            var result = await _productSvc.DeleteProduct(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
