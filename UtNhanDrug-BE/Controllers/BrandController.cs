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
        private readonly IProductSvc _productSvc;
        public BrandController(IBrandSvc brandSvc, IProductSvc productSvc)
        {
            _brandSvc = brandSvc;
            _productSvc = productSvc;
        }

        [Authorize(Roles = "MANAGER")]
        [Route("brands")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllBrand()
        {
            var brands = await _brandSvc.GetAllBrand();
            return Ok(brands);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("brands/{id}/products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProducts([FromRoute] int id)
        {
            var products = await _brandSvc.GetListProduct(id);
            foreach (var product in products)
            {
                var activeSubstance = await _productSvc.GetListActiveSubstances(product.Id);
                product.ActiveSubstances = activeSubstance;
            }

            return Ok(products);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("brands/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetBrandById([FromRoute] int id)
        {
            var brand = await _brandSvc.GetBrandById(id);
            if(brand == null) return NotFound(new { message = "Not found this brand" });
            return Ok(brand);
        }

        [Authorize(Roles = "MANAGER")]
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
            if(!result) return BadRequest(new { message = "Create brand fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize(Roles = "MANAGER")]
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
            var isExit = await _brandSvc.CheckBrand(id);
            if(!isExit) return NotFound(new { message = "Not found this brand" });
            var result = await _brandSvc.UpdateBrand(id,userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize(Roles = "MANAGER")]
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

            var isExit = await _brandSvc.CheckBrand(id);
            if (!isExit) return NotFound(new { message = "Not found this brand" });
            var result = await _brandSvc.DeleteBrand(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
        
    }
}
