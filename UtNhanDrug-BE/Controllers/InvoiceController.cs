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
                userId = Convert.ToInt32(claim[1].Value);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
            var result = await _invoiceSvc.CreateInvoice(userId, model);
            if (!result) return BadRequest(new { message = "Create invoice fail" });
            return Ok(new { message = "create successfully" });
        }
    }
}
