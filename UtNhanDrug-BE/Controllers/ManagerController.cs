using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.UserModel;
using UtNhanDrug_BE.Services.CustomerService;
using UtNhanDrug_BE.Services.ManagerService;
using UtNhanDrug_BE.Services.StaffService;
using UtNhanDrug_BE.Services.UserService;

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
        private readonly IUserService _userService;

        public ManagerController(IManagerSvc managerSvc, IStaffService staffSvc, ICustomerSvc customerSvc, IUserService userService)
        {
            _managerSvc = managerSvc;
            _staffSvc = staffSvc;
            _customerSvc = customerSvc;
            _userService = userService;
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
        public async Task<ActionResult> CreateManager([FromForm] string email, [FromForm] string fullname )
        {
            var manager = await _managerSvc.CreateAccount(email,fullname);
            if (manager == false) return BadRequest("Email exits");
            return Ok("Create manager successfully");
        }
        
        [Route("createStaff")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateStaff([FromForm] string email, [FromForm] string fullname)
        {
            var staff = await _staffSvc.CreateAccount(email, fullname);
            if (staff == false) return BadRequest("Email exits");
            return Ok("Create manager successfully");
        }

        [HttpPut("ban/{userId}")]
        [MapToApiVersion("1.0")] 
        public async Task<ActionResult> BanAccount([FromRoute] int userId)
        {
            var result = await _managerSvc.BanAccount(userId);
            if (result == -1) return NotFound("Not found this account");
            return Ok("ban successfully");
        }
        
        [HttpPut("unban/{userId}")]
        [MapToApiVersion("1.0")] 
        public async Task<ActionResult> UnBanAccount([FromRoute] int userId)
        {
            var result = await _managerSvc.UnBanAccount(userId);
            if (result == -1) return NotFound("Not found this account");
            return Ok("unban successfully");
        }

        [HttpPut("update/{userId}")]
        [MapToApiVersion("1.0")] 
        public async Task<ActionResult> UpdateAccount([FromRoute] int userId, [FromForm] UpdateUserModel model)
        {
            bool result = await _userService.UpdateProfile(userId, model);
            if (result == false) return NotFound("not found account");
            return Ok("Update successfully");
        }
    }
}
