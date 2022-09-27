using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.RoleModel;
using UtNhanDrug_BE.Models.UserModel;
using UtNhanDrug_BE.Services.ManagerService;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/user-management")]
    public class UserController : ControllerBase
    {
        private readonly IUserSvc _userSvc;

        public UserController(IUserSvc userSvc)
        {
            _userSvc = userSvc;
        }


        [Authorize(Roles = "ADMIN, MANAGER")]
        [Route("managers")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetManager([FromQuery] PagingModel pagingParameters)
        {
            if(pagingParameters.Keyword == null)
            {
                pagingParameters.Keyword = " ";
            }
            var manager = await _userSvc.GetManagers(pagingParameters);
            return Ok(manager);
        }

        [Authorize(Roles = "ADMIN, MANAGER")]
        [Route("customers")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCustomer([FromQuery] PagingModel pagingParameters)
        {
            var customer = await _userSvc.GetCustomers(pagingParameters);
            return Ok(customer);
        }

        [Authorize(Roles = "ADMIN, MANAGER")]
        [Route("staffs")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetStaff([FromQuery] PagingModel pagingParameters)
        {
            var staff = await _userSvc.GetStaffs(pagingParameters);
            return Ok(staff);
        }

        [Authorize(Roles = "ADMIN")]
        [Route("managers")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateManager([FromForm] string email, [FromForm] string fullname )
        {
            var manager = await _userSvc.CreateAccount(email,fullname);
            if (manager == false) return BadRequest(new { message = "Email exits" });
            return Ok(new { message = "Create manager successfully" });
        }

        [Authorize(Roles = "ADMIN, MANAGER")]
        [Route("staffs")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateStaff([FromForm] string email, [FromForm] string fullname)
        {
            var staff = await _userSvc.CreateStaff(email, fullname);
            if (staff == false) return BadRequest(new { message = "Email exits" });
            return Ok(new { message = "Create manager successfully" });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("users/ban/{userId}")]
        [MapToApiVersion("1.0")] 
        public async Task<ActionResult> BanAccount([FromRoute] int userId)
        {
            var result = await _userSvc.BanAccount(userId);
            if (result == -1) return NotFound(new { message = "Not found this account" });
            return Ok();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("users/unban/{userId}")]
        [MapToApiVersion("1.0")] 
        public async Task<ActionResult> UnBanAccount([FromRoute] int userId)
        {
            var result = await _userSvc.UnBanAccount(userId);
            if (result == -1) return NotFound(new { message = "Not found this account" });
            if (result == 0) return BadRequest(new { message = "Update fail" });
            return Ok(new { message = "unban successfully" });
        }

        [Authorize(Roles = "ADMIN, MANAGER")]
        [HttpPut("users/{userId}")]
        [MapToApiVersion("1.0")] 
        public async Task<ActionResult> UpdateAccount([FromRoute] int userId, [FromForm] UpdateUserModel model)
        {
            bool result = await _userSvc.UpdateProfile(userId, model);
            if (result == false) return NotFound(new { message = "Not found account" });
            return Ok(new { message = "Update successfully" });
        }

        [Authorize(Roles = "ADMIN, MANAGER, STAFF")]
        [Route("customers")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateCustomer([FromForm] string phoneNumber, [FromForm] string fullname)
        {
            var customer = await _userSvc.CreateCustomer(phoneNumber, fullname);
            if (customer == false) return BadRequest(new { message = "Phone number is exits" });
            return Ok(new { message = "Create customer successfully" });
        }

        [Route("auth/customers/profile")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCustomerProfile()
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


            var customer = await _userSvc.GetCustomer(userId);
            if (customer == null) return NotFound(new { message = "Not found user" });
            return Ok(customer);
        }

        [Route("auth/staffs/profile")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetStaffProfile()
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


            var staff = await _userSvc.GetStaff(userId);
            if (staff == null) return NotFound(new { message = "Not found user" });
            return Ok(staff);
        }

        [Route("auth/managers/profile")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetManagerProfile()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId;
            try
            {
                userId = Convert.ToInt32(claim[1].Value);
            }catch (Exception)
            {
                return BadRequest(new { message = "You are not login" });
            }
            

            var manager = await _userSvc.GetManager(userId);
            if (manager == null) return NotFound(new { message = "Not found user" });
            return Ok(manager);
        }


    }
}
