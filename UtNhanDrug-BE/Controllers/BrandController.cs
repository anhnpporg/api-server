using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BrandModel;
using UtNhanDrug_BE.Services.BrandService;
using UtNhanDrug_BE.Services.ProductService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/brand-management")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandSvc _brandSvc;
        public BrandController(IBrandSvc brandSvc)
        {
            _brandSvc = brandSvc;
        }

        [Authorize]
        [Route("brands")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllBrand()
        {
            var brands = await _brandSvc.GetAllBrand();
            return StatusCode(brands.StatusCode, brands);
        }

        [Authorize]
        [Route("brands/{id}/products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProducts([FromRoute] int id)
        {
            var products = await _brandSvc.GetListProduct(id);
            return StatusCode(products.StatusCode, products);
        }

        [Authorize]
        [Route("brands/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetBrandById([FromRoute] int id)
        {
            var brand = await _brandSvc.GetBrandById(id);
            return StatusCode(brand.StatusCode, brand);
        }

        [Authorize]
        [Route("brands")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateBrand([FromForm] CreateBrandModel model)
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
            var result = await _brandSvc.CreateBrand(userId,model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("brands/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateBrand([FromRoute] int id, [FromForm] UpdateBrandModel model)
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
            var result = await _brandSvc.UpdateBrand(id,userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPatch("brands/{id}")]
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
            var result = await _brandSvc.DeleteBrand(id, userId);
            return StatusCode(result.StatusCode, result);
        }
        
    }
}
