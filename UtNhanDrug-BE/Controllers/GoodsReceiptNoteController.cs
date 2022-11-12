using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.GoodsReceiptNoteModel;
using UtNhanDrug_BE.Services.GoodsReceiptNoteService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/goods-receipt-note-management")]
    public class GoodsReceiptNoteController : ControllerBase
    {
        private readonly IGRNSvc _grnSvc;
        public GoodsReceiptNoteController(IGRNSvc grnSvc)
        {
            _grnSvc = grnSvc;
        }
        [Authorize]
        [Route("goods-receipt-notes")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllGRN()
        {
            var result = await _grnSvc.GetAllGoodsReceiptNote();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("goods-receipt-note-types")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllGRNTypes()
        {
            var result = await _grnSvc.GetListNoteTypes();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("goods-receipt-notes/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetGRNById([FromRoute] int id)
        {
            var result = await _grnSvc.GetGoodsReceiptNoteById(id);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("goods-receipt-notes")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateGRN([FromBody] List<CreateGoodsReceiptNoteModel> model)
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
            var result = await _grnSvc.CreateGoodsReceiptNote(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("goods-receipt-notes/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateGoodsReceiptNote([FromRoute] int id, [FromBody] UpdateGoodsReceiptNoteModel model)
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
            //var isExit = await _grnSvc.CheckGoodsReceiptNote(id);
            //if (!isExit) return NotFound(new { message = "Not found this goods receipt note" });
            var result = await _grnSvc.UpdateGoodsReceiptNote(id, userId, model);
            return StatusCode(result.StatusCode, result);
        }

    }
}
