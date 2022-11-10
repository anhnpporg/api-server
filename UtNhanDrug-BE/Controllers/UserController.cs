using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.StaffModel;
using UtNhanDrug_BE.Models.UserLoginModel;
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

        [Authorize(Roles = "MANAGER, STAFF")]
        [Route("customers")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCustomer()
        {
            var customer = await _userSvc.GetCustomers();
            return Ok(customer);
        }

        //search customer
        [Authorize]
        [Route("customers/filter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCustomerPaging([FromQuery] CustomerPagingRequest request)
        {
            var customers = await _userSvc.SearchCustomer(request);
            return Ok(customers);
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

        [Authorize]
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
                return BadRequest(new { message = "Bạn chưa đăng nhập" });
            }


            var manager = await _userSvc.GetUserProfile(userId);
            if (manager == null) return NotFound(new { message = "Không tìm thấy tài khoản" });
            return Ok(manager);
        }
        
        [Authorize]
        [Route("customers/{id}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetCustomerById(int id)
        {
            var customer = await _userSvc.GetCustomerProfile(id);
            if (customer == null) return NotFound(new { message = "Không tìm thấy khách hàng" });
            return Ok(customer);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("profile/{userId}")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetProfile([FromRoute] int userId)
        {
            var user = await _userSvc.GetUserProfile(userId);
            if (user == null) return NotFound(new { message = "Tài khoản đang tìm không tồn tại" });
            return Ok(user);
        }

        [Authorize(Roles = "MANAGER")]
        [Route("staffs")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateStaff([FromForm] CreateStaffModel model)
        {
            var staff = await _userSvc.CreateStaff(model);
            return StatusCode(staff.StatusCode, staff);
        }

        [Authorize(Roles = "MANAGER, STAFF")]
        [Route("customers")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateCustomer([FromForm] CreateCustomerModel model)
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
                return BadRequest(new { message = "Bạn chưa đăng nhập" });
            }
            var customer = await _userSvc.CreateCustomer(userId, model);
            return StatusCode(customer.StatusCode, customer);
        }

        [Authorize]
        [Route("token-verify-email")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateTokenVerifyEmail()
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
                return BadRequest(new { message = "Bạn chưa đăng nhập" });
            }

            var token = await _userSvc.CreateTokenVerifyEmail(userId);
            return StatusCode(token.StatusCode, token);
        }

        [Authorize]
        [Route("token-verify-password")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> CreateTokenVerifyPassword()
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
                return BadRequest(new { message = "Bạn chưa đăng nhập" });
            }
            var token = await _userSvc.CreateTokenVerifyPassword(userId);
            return StatusCode(token.StatusCode, token);
        }

        [Authorize]
        [Route("verify-email")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> VerifyEmail([FromForm] TokenVerifyModel model)
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
            //// check email
            //var checkExits = await _userSvc.CheckEmail(userId);
            //if (checkExits == 1) return BadRequest(new { message = "Account not have an email" });
            //if (checkExits == 3) return BadRequest(new { message = "Email is verified" });

            //check time
            //var checkTime = await _userSvc.CheckTimeVerifyEmail(userId);
            //if (checkTime == false) return BadRequest(new { message = "Verification code expired" });

            //check result
            var result = await _userSvc.CheckTokenVerifyEmail(userId, model);
            return StatusCode(result.StatusCode, result);
        }


        //[Authorize(Roles = "MANAGER")]
        //[HttpPut("users/recovery-password/{userId}")]
        //[MapToApiVersion("1.0")]
        //public async Task<ActionResult> RecoveryPassword([FromRoute] int userId)
        //{
        //    var user = await _userSvc.CheckUser(userId);
        //    if (user == false) return NotFound(new { message = "Không tìm thấy nhân viên" });
        //    var result = await _userSvc.RecoveryPassword(userId);
        //    return StatusCode(result.StatusCode, result);
        //}

        [Authorize(Roles = "STAFF")]
        [HttpPut("staffs/profile")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateStaffAccount([FromForm] UpdateStaffModel model)
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
            var result = await _userSvc.UpdateStaffProfile(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("staffs/profile/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateStaffAccountByManager([FromRoute] int id ,[FromForm] UpdateStaffBaseModel model)
        {
            var result = await _userSvc.UpdateStaffProfile(id, model);
            return StatusCode(result.StatusCode, result);
        }
        
        [Authorize]
        [HttpPut("accounts/reset-password")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> ChangePassword([FromForm] ChangePasswordModel model)
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
                return BadRequest(new { message = "Bạn chưa đăng nhập" });
            }

            ////check email
            //var user = await _userSvc.CheckUser(userId);
            //if (user == false) return NotFound(new { message = "Không tìm thấy tài khoản" });
            ////check confirm password
            //if (model.NewPassword != model.ConfirmPassword) return BadRequest(new { message = "Mật khẩu xác nhận chưa khớp" });
            ////check current password
            //var checkPassword = await _userSvc.CheckPassword(userId, model.CurrentPassword);
            //if (checkPassword == false) return BadRequest(new { message = "Mật khẩu hiện tại sai" });
            ////check time token
            //var checkTime = await _userSvc.CheckTimeVerifyPassword(userId);
            //if (checkTime == false) return BadRequest(new { message = "Mã xác thực hết hạn" });
            ////check password recovery token
            //var checkToken = await _userSvc.CheckVerifyPassword(userId, model.TokenRecovery);
            //if (checkToken == false) return BadRequest(new { message = "Mã xác thực sai" });
            //check change password
            var result = await _userSvc.ChangePassword(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("accounts/change-password")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> ChangePasswordById([FromForm] ChangePasswordByIdModel model)
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
                return BadRequest(new { message = "Bạn chưa đăng nhập" });
            }
            //check change password
            var result = await _userSvc.ChangePasswordByUserId(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPut("managers/profile")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateManagerAccount([FromForm] UpdateManagerModel model)
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
            var result = await _userSvc.UpdateManagerProfile(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        //[Authorize(Roles = "STAFF")]
        //[HttpPut("staffs/profile")]
        //[MapToApiVersion("1.0")]
        //public async Task<ActionResult> UpdateStaffAccount( [FromForm] string email)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    IList<Claim> claim = identity.Claims.ToList();
        //    int userId;
        //    try
        //    {
        //        userId = Convert.ToInt32(claim[0].Value);
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest(new { message = "You are not login" });
        //    }
        //    var user = await _userSvc.CheckUser(userId);
        //    if (user == false) return NotFound(new { message = "Not found account" });
        //    bool result = await _userSvc.UpdateEmail(userId, email);
        //    if (result == false) return BadRequest(new { message = "Update fail" });
        //    return Ok(new { message = "Update successfully" });
        //}

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("users/ban/{userId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> BanAccount([FromRoute] int userId)
        {
            var result = await _userSvc.BanAccount(userId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "MANAGER")]
        [HttpPatch("users/unban/{userId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UnBanAccount([FromRoute] int userId)
        {
            var result = await _userSvc.UnBanAccount(userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
