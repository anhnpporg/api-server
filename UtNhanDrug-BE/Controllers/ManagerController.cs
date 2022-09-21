using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Services.CustomerService;
using UtNhanDrug_BE.Services.ManagerService;
using UtNhanDrug_BE.Services.StaffService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerSvc _managerSvc;
        private readonly IStaffService _staffSvc;
        private readonly ICustomerSvc _customerSvc;

        public ManagerController(IManagerSvc managerSvc, IStaffService staffSvc, ICustomerSvc customerSvc)
        {
            _managerSvc = managerSvc;
            _staffSvc = staffSvc;
            _customerSvc = customerSvc;
        }

        [Route("pagingManager")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetManager([FromQuery] PagingModel pagingParameters)
        {
            if(pagingParameters.Keyword == null)
            {
                pagingParameters.Keyword = " ";
            }
            var manager = await _managerSvc.GetManagers(pagingParameters);
            return Ok(manager);
        }
        
        [Route("pagingCustomer")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCustomer([FromQuery] PagingModel pagingParameters)
        {
            var customer = await _customerSvc.GetCustomers(pagingParameters);
            return Ok(customer);
        }
        
        [Route("pagingStaff")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetStaff([FromQuery] PagingModel pagingParameters)
        {
            var staff = await _staffSvc.GetStaffs(pagingParameters);
            return Ok(staff);
        }

        [Route("createManager")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateManager([FromForm] string email)
        {
            var manager = await _managerSvc.CreateAccount(email);
            if (manager == false) return BadRequest("Email exits");
            return Ok("Create manager successfully");
        }
        
        [Route("createStaff")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateStaff([FromForm] string email)
        {
            var staff = await _staffSvc.CreateAccount(email);
            if (staff == false) return BadRequest("Email exits");
            return Ok("Create manager successfully");
        }

        [HttpPut("ban/{id}")]
        [MapToApiVersion("1.0")] 
        public async Task<ActionResult> BanAccount([FromRoute] int id)
        {
            var result = await _managerSvc.BanAccount(id);
            if (result == -1) return NotFound("Not found this account");
            return Ok("ban successfully");
        }
    }
}
