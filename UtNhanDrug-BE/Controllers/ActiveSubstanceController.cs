using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.ActiveSubstanceModel;
using UtNhanDrug_BE.Services.ActiveSubstanceService;
using UtNhanDrug_BE.Services.ProductService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/active-substance-management")]
    public class ActiveSubstanceController : ControllerBase
    {
        private readonly IActiveSubstanceSvc _activeSubstanceSvc;
        private readonly IProductSvc _productSvc;
        public ActiveSubstanceController(IActiveSubstanceSvc activeSubstanceSvc, IProductSvc productSvc)
        {
            _activeSubstanceSvc = activeSubstanceSvc;
            _productSvc = productSvc;
        }

        [Authorize(Roles = "MANAGER")]
        [Route("active-substances")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllActiveSubstance()
        {
            var result = await _activeSubstanceSvc.GetAllActiveSubstance();
            return Ok(result);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("active-substances/{id}/products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProducts([FromRoute] int id)
        {
            var products = await _activeSubstanceSvc.GetListProducts(id);
            foreach (var product in products)
            {
                var activeSubstance = await _productSvc.GetListActiveSubstances(product.Id);
                product.ActiveSubstances = activeSubstance;
            }

            return Ok(products);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("active-substances/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetActiveSubstanceById([FromRoute] int id)
        {
            var result = await _activeSubstanceSvc.GetActiveSubstanceById(id);
            if (result == null) return NotFound(new { message = "Not found this active substance" });
            return Ok(result);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("active-substances")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateBrand([FromForm] CreateActiveSubstanceModel model)
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
            var result = await _activeSubstanceSvc.CreateActiveSubstance(userId, model);
            if (!result) return BadRequest(new { message = "Create active substance fail" });
            return Ok(new { message = "create successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("active-substances/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateBrand([FromRoute] int id, [FromForm] UpdateActiveSubstanceModel model)
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
            var isExit = await _activeSubstanceSvc.CheckActiveSubstance(id);
            if (!isExit) return NotFound(new { message = "Not found this active substance" });
            var result = await _activeSubstanceSvc.UpdateActiveSubstance(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("active-substances/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeleteBrand([FromRoute] int id)
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

            var isExit = await _activeSubstanceSvc.CheckActiveSubstance(id);
            if (!isExit) return NotFound(new { message = "Not found this active substance" });
            var result = await _activeSubstanceSvc.DeleteActiveSubstance(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }

    }
}
