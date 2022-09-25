using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UtNhanDrug_BE.Services.CustomerService;
using UtNhanDrug_BE.Services.StaffService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly ICustomerSvc _customerSvc;

        public StaffController(IStaffService staffService, ICustomerSvc customerSvc)
        {
            _staffService = staffService;
            _customerSvc = customerSvc;
        }

        [Route("createCustomer")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateCustomer([FromForm] string phoneNumber, [FromForm] string fullname)
        {
            var customer = await _customerSvc.CreateCustomer(phoneNumber, fullname);
            if (customer == false) return BadRequest("Phone number is exits");
            return Ok("Create customer successfully");
        }
    }
}
