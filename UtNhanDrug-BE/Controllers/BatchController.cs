using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.BatchModel;
using UtNhanDrug_BE.Services.BatchService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/batch-management")]
    public class BatchController : ControllerBase
    {
        private readonly IBatchSvc _consignmentSvc;
        public BatchController(IBatchSvc consignmentSvc)
        {
            _consignmentSvc = consignmentSvc;
        }

        [Authorize]
        [Route("batches")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllbatches()
        {
            var disease = await _consignmentSvc.GetAllBatch();
            return StatusCode(disease.StatusCode, disease);
        }

        [Authorize]
        [Route("unit/{id}/inventory")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetInventoryByUnitId([FromRoute] int id)
        {
            var i = await _consignmentSvc.GetInventoryByUnitId(id);
            return StatusCode(i.StatusCode, i);
        }
        
        [Authorize]
        [Route("batches/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetbatchById([FromRoute] int id)
        {
            var disease = await _consignmentSvc.GetBatchById(id);
            return StatusCode(disease.StatusCode, disease);
        }
        
        [Authorize]
        [Route("batches/{id}/goods-receipt-note")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetGRNById([FromRoute] int id)
        {
            var grn = await _consignmentSvc.GetGRNByBatchId(id);
            return StatusCode(grn.StatusCode, grn);
        }

        [Authorize]
        [Route("batches/{id}/goods-issue-note")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetGINById([FromRoute] int id)
        {
            var grn = await _consignmentSvc.GetGINByBatchId(id);
            return StatusCode(grn.StatusCode, grn);
        }
        
        [Authorize]
        [Route("batches/barcode")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetbatchByBarcode([FromQuery] string batchBarcode)
        {
            var batch = await _consignmentSvc.GetBatchesByBarcode(batchBarcode);
            return StatusCode(batch.StatusCode, batch);
        }

        [Authorize]
        [Route("batches")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> Createbatch([FromForm] CreateBatchModel model)
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
            var result = await _consignmentSvc.CreateBatch(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("batches/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> Updatebatch([FromRoute] int id, [FromForm] UpdateBatchModel model)
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
            //var isExit = await _consignmentSvc.CheckBatch(id);
            //if (!isExit) return NotFound(new { message = "Not found this batch" });
            var result = await _consignmentSvc.UpdateBatch(id, userId, model);
            return StatusCode(result.StatusCode, result);
        }
        
        [Authorize]
        [HttpPatch("batches/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> Deletebatch([FromRoute] int id)
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

            //var isExit = await _consignmentSvc.CheckBatch(id);
            //if (!isExit) return NotFound(new { message = "Not found this batch" });
            var result = await _consignmentSvc.DeleteBatch(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
