﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IProductUnitPriceSvc _productUnitSvc;
        private readonly IBatchSvc _batchSvc;

        public ProductController(IProductSvc productSvc, IProductUnitPriceSvc productUnitSvc, IBatchSvc batchSvc)
        {
            _productSvc = productSvc;
            _productUnitSvc = productUnitSvc;
            _batchSvc = batchSvc;
        }

        [Authorize]
        [Route("products")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllProduct()
        {
            var products = await _productSvc.GetAllProduct();
            foreach (var product in products)
            {
                var activeSubstance = await _productSvc.GetListActiveSubstances(product.Id);
                product.ActiveSubstances = activeSubstance;
                var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
                product.ProductUnits = productUnits;
                var batches = await _batchSvc.GetBatchesByProductId(product.Id);
                product.Batches = batches.Data;
            }
            

            return Ok(products);
        }
        
        [Authorize]
        [Route("products/filter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductPaging([FromQuery] ProductFilterRequest request)
        {
            var products = await _productSvc.GetProductFilter(request);
            foreach (var product in products.Items)
            {
                var activeSubstance = await _productSvc.GetListActiveSubstances(product.Id);
                product.ActiveSubstances = activeSubstance;
                var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
                product.ProductUnits = productUnits;
                List<ViewBatchModel> batches;
                if (request.SearchValue.Contains("BAT"))
                {
                    batches = new List<ViewBatchModel>();
                    var batch = await _batchSvc.GetBatchesByBarcode(request.SearchValue);
                    if(batch.Data == null)
                    {
                        return StatusCode(batch.StatusCode, batch);
                    }
                    batches.Add(batch.Data);
                }
                else
                {
                    var b = await _batchSvc.GetBatchesByProductId(product.Id);
                    batches = b.Data;
                }
                
                product.Batches = batches;
            }
            

            return Ok(products);
        }
        
        [Authorize]
        [Route("route-of-administrations")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetRouteOfAdministrations()
        {
            var result = await _productSvc.GetListRouteOfAdmin();
            return Ok(result);
        }

        [Authorize]
        [Route("products/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProductById([FromRoute] int id)
        {
            var product = await _productSvc.GetProductById(id);
            if (product == null) return NotFound(new { message = "Not found this product" });
            var activeSubstance = await _productSvc.GetListActiveSubstances(product.Id);
            product.ActiveSubstances = activeSubstance;
            var productUnits = await _productUnitSvc.GetProductUnitByProductId(product.Id);
            product.ProductUnits = productUnits;
            var batches = await _batchSvc.GetBatchesByProductId(product.Id);
            product.Batches = batches.Data;
            return Ok(product);
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
            if (!result) return BadRequest(new { message = "Create product fail" });
            return Ok(new { message = "create successfully" });
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
            var isExit = await _productSvc.CheckProduct(id);
            if (!isExit) return NotFound(new { message = "Not found this product" });
            var result = await _productSvc.UpdateProduct(id, userId, model);
            if (!result) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "update succesfully" });
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

            var isExit = await _productSvc.CheckProduct(id);
            if (!isExit) return NotFound(new { message = "Not found this product" });
            var result = await _productSvc.DeleteProduct(id, userId);
            if (!result) return BadRequest(new { message = "Delete fail" });
            return Ok(new { message = "Delete successfully" });
        }
    }
}
