using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ShelfModel;
using UtNhanDrug_BE.Services.ProductService;
using UtNhanDrug_BE.Services.ShelfService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/shelves-management")]
    public class ShelfController : ControllerBase
    {
        private readonly IShelfSvc _shelfSvc;
        private readonly IProductSvc _productSvc;
        public ShelfController(IShelfSvc shelfSvc, IProductSvc productSvc)
        {
            _shelfSvc = shelfSvc;
            _productSvc = productSvc;
        }

        //CONTROLLER CATEGORY
        [Authorize]
        [Route("shelves")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllCategory()
        {
            var categories = await _shelfSvc.GetAllShelves();
            return Ok(categories);
        }

        [Authorize]
        [Route("shelves/{id}/products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProducts([FromRoute] int id)
        {
            var products = await _shelfSvc.GetListProduct(id);
            foreach (var product in products)
            {
                var activeSubstance = await _productSvc.GetListActiveSubstances(product.Id);
                product.ActiveSubstances = activeSubstance;
            }

            return Ok(products);
        }

        [Authorize]
        [Route("shelves/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCategoryById([FromRoute] int id)
        {
            var shelf = await _shelfSvc.GetShelfById(id);
            if (shelf == null) return NotFound(new { message = "Not found this shelf" });
            return Ok(shelf);
        }

        [Authorize]
        [Route("shelves")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateShelf([FromForm] CreateShelfModel model)
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
            var result = await _shelfSvc.CreateShelf(userId, model);
            if (!result) return BadRequest(new { message = "Create shelf fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize]
        [HttpPut("shelves/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateShelf([FromRoute] int id, [FromForm] UpdateShelfModel model)
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
            var isExit = await _shelfSvc.CheckShelf(id);
            if (!isExit) return NotFound(new { message = "Not found this shelf" });
            var result = await _shelfSvc.UpdateShelf(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize]
        [HttpPatch("shelves/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteShelf([FromRoute] int id)
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

            var isExit = await _shelfSvc.CheckShelf(id);
            if (!isExit) return NotFound(new { message = "Not found this shelf" });
            var result = await _shelfSvc.DeleteShelf(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
