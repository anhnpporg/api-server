using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Models.ProductModel;
using UtNhanDrug_BE.Services.BatchService;
using UtNhanDrug_BE.Services.ProductService;
using UtNhanDrug_BE.Services.ProductUnitService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/product-management")]
    public class ProductController : ControllerBase
    {
        private readonly IProductSvc _productSvc;

        public ProductController(IProductSvc productSvc)
        {
            _productSvc = productSvc;
        }

        //[Authorize]
        //[Route("products")]
        //[HttpGet]
        //[MapToApiVersion("1.0")]
        //public async Task<ActionResult> GetAllProduct()
        //{
        //    var products = await _productSvc.GetAllProduct();
        //    return StatusCode(products.StatusCode, products);
        //}
        
        [Authorize]
        [Route("products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProducts([FromQuery] FilterProduct request)
        {
            var products = await _productSvc.GetAllProduct(request);
            return StatusCode(products.StatusCode, products);
        }
        
        [Authorize]
        [Route("products/filter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductPaging([FromQuery] ProductFilterRequest request)
        {
            var products = await _productSvc.GetProductFilter(request);
            return Ok(products);
        }
        
        [Authorize]
        [Route("route-of-administrations")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetRouteOfAdministrations()
        {
            var result = await _productSvc.GetListRouteOfAdmin();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("products/{id}/batches")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetbatchesByProductId([FromRoute] int id)
        {
            var disease = await _productSvc.GetBatchesByProductId(id);
            return StatusCode(disease.StatusCode, disease);
        }

        [Authorize]
        [Route("products/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductById([FromRoute] int id)
        {
            var product = await _productSvc.GetProductById(id);
            return StatusCode(product.StatusCode, product);
        }

        [Authorize]
        [Route("products")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductModel model )
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
            var result = await _productSvc.CreateProduct(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("products/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateProduct([FromRoute] int id, [FromForm] UpdateProductModel model)
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
            var result = await _productSvc.UpdateProduct(id, userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPatch("products/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
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
            var result = await _productSvc.DeleteProduct(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
