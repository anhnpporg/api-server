using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.CategoryModel;
using UtNhanDrug_BE.Services.CategoryService;
using UtNhanDrug_BE.Services.ProductService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/categorie-management")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategorySvc _categorySvc;
        private readonly IProductSvc _productSvc;
        public CategoryController(ICategorySvc categorySvc, IProductSvc productSvc)
        {
            _categorySvc = categorySvc;
            _productSvc = productSvc;
        }

        //CONTROLLER CATEGORY
        [Authorize(Roles = "MANAGER")]
        [Route("categories")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllCategory()
        {
            var categories = await _categorySvc.GetAllCategory();
            return Ok(categories);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("categories/{id}/products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProducts([FromRoute] int id)
        {
            var products = await _categorySvc.GetListProduct(id);
            foreach (var product in products)
            {
                var activeSubstance = await _productSvc.GetListActiveSubstances(product.Id);
                product.ActiveSubstances = activeSubstance;
            }

            return Ok(products);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("categories/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCategoryById([FromRoute] int id)
        {
            var category = await _categorySvc.GetCategoryById(id);
            if (category == null) return NotFound(new { message = "Not found this category" });
            return Ok(category);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("categories")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateCategory([FromForm] CreateCategoryModel model)
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
            var result = await _categorySvc.CreateCategory(userId, model);
            if (!result) return BadRequest(new { message = "Create category fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("categories/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateCategory([FromRoute] int id, [FromForm] UpdateCategoryModel model)
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
            var isExit = await _categorySvc.CheckCategory(id);
            if (!isExit) return NotFound(new { message = "Not found this category" });
            var result = await _categorySvc.UpdateCategory(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("categories/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteCategory([FromRoute] int id)
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

            var isExit = await _categorySvc.CheckCategory(id);
            if (!isExit) return NotFound(new { message = "Not found this category" });
            var result = await _categorySvc.DeleteCategory(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
