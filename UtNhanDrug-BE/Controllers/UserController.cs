using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.RoleModel;
using UtNhanDrug_BE.Models.StaffModel;
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


        [Authorize(Roles = "MANAGER")]
        [Route("managers")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetManager()
        {
            var manager = await _userSvc.GetManagers();
            return Ok(manager);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("customers")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCustomer()
        {
            var customer = await _userSvc.GetCustomers();
            return Ok(customer);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("staffs")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetStaff()
        {
            var staff = await _userSvc.GetStaffs();
            return Ok(staff);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("staffs")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateStaff([FromForm] CreateStaffModel model)
        {
            if (model.Password != model.PasswordConfirm) return BadRequest(new { message = "Confirm password incorrect" });
            var staff = await _userSvc.CreateStaff(model);
            if (staff == false) return BadRequest(new { message = "LoginName exits" });
            return Ok(new { message = "Create manager successfully" });
        }


        [Authorize(Roles = "MANAGER")]
        [HttpPut("users/password/{userId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> RecoveryPassword([FromRoute] int userId, [FromForm] RecoveryPasswordModel model)
        {
            var user = await _userSvc.CheckUser(userId);
            if (user == false) return NotFound(new { message = "Not found login name" });
            if (model.NewPassword.Trim() != model.ConfirmPassword.Trim()) return BadRequest(new { message = "Confirm password incorect" });
            var result = await _userSvc.RecoveryPassword(userId, model);
            if (result == false) return BadRequest(new { message = "Change password fail" });
            return Ok(new { message = "Change password successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("users/ban/{userId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> BanAccount([FromRoute] int userId)
        {
            var result = await _userSvc.BanAccount(userId);
            if (result == -1) return NotFound(new { message = "Not found this account" });
            if (result == 0) return BadRequest(new { message = "Ban fail" });
            return Ok();
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("users/unban/{userId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UnBanAccount([FromRoute] int userId)
        {
            var result = await _userSvc.UnBanAccount(userId);
            if (result == -1) return NotFound(new { message = "Not found this account" });
            if (result == 0) return BadRequest(new { message = "Ban fail" });
            return Ok(new { message = "unban successfully" });
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("staffs/{userId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateStaffAccount([FromRoute] int userId,   [FromForm] UpdateStaffModel model)
        {
            var user = await _userSvc.CheckUser(userId);
            if (user == false) return NotFound(new { message = "Not found account" });
            bool result = await _userSvc.UpdateStaffProfile(userId, model);
            if (result == false) return BadRequest(new { message = "Update fail"});
            return Ok(new { message = "Update successfully" });
        }
        
        //[Authorize(Roles = "MANAGER")]
        //[HttpPut("managers/{userId}")]
        //[MapToApiVersion("1.0")]
        //public async Task<ActionResult> UpdateManagerAccount([FromRoute] int userId, [FromForm] UpdateManagerModel model)
        //{
        //    bool result = await _userSvc.UpdateManagerProfile(userId, model);
        //    if (result == false) return NotFound(new { message = "Not found account" });
        //    return Ok(new { message = "Update successfully" });
        //}

        [Authorize(Roles = "MANAGER, STAFF")]
        [Route("customers")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateCustomer([FromForm] CreateCustomerModel model)
        {
            var customer = await _userSvc.CreateCustomer(model);
            if (customer == false) return BadRequest(new { message = "Phone number is exits" });
            return Ok(new { message = "Create customer successfully" });
        }

        [Route("auth/user/profile")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetUserProfile()
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


            var manager = await _userSvc.GetUserProfile(userId);
            if (manager == null) return NotFound(new { message = "Not found user" });
            return Ok(manager);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("profile/{userId}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProfile([FromRoute] int userId)
        {
            var user = await _userSvc.GetUserProfile(userId);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }
    }
}
