using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.InvoiceModel;
using UtNhanDrug_BE.Services.InvoiceService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/invoice-management")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceSvc _invoiceSvc;
        public InvoiceController(IInvoiceSvc invoiceSvc)
        {
            _invoiceSvc = invoiceSvc;
        }


        [Authorize]
        [Route("invoices/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetInvoiceById([FromRoute] int id)
        {
            var result = await _invoiceSvc.ViewInvoiceById(id);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("invoices/barcode")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetInvoiceByBarcode([FromQuery] string barcode)
        {
            var result = await _invoiceSvc.GetInvoiceByInvoiceBarcode(barcode);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("invoices")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllInvoice()
        {
            var result = await _invoiceSvc.GetAllInvoice();
            return StatusCode(result.StatusCode, result);
        }
        
        [Authorize]
        [Route("users/{id}/invoices")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetInvoiceByUserId([FromRoute] int id)
        {
            var result = await _invoiceSvc.GetInvoiceByUserId(id);
            return StatusCode(result.StatusCode, result);
        }
        
        [Authorize]
        [Route("customers/{id}/invoices")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetInvoiceByCustomerId([FromRoute] int id)
        {
            var result = await _invoiceSvc.GetInvoiceCustomerId(id);
            return StatusCode(result.StatusCode, result);
        }

        
        [Authorize]
        [Route("invoices/{id}/invoice-detail")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetInvoiceDetailByInvoiceId([FromRoute] int id)
        {
            var result = await _invoiceSvc.ViewOrderDetailByInvoiceId(id);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [Route("invoices")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateInvoice([FromBody] CreateInvoiceModel model)
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
            var result = await _invoiceSvc.CreateInvoice(userId, model);
            return StatusCode(result.StatusCode, result);
        }

    }
}
