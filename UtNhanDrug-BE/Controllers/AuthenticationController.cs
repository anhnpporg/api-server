using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.TokenModel;
using UtNhanDrug_BE.Services.AuthenticationService;

namespace UtNhanDrug_BE.Controllers
{
    
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationSvc _authenticationService;

        public AuthenticationController(IAuthenticationSvc authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("auth/managers/login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        public async Task<IActionResult> LoginManagerWithIdTokenAsync([FromHeader] string idToken)
        {
            if (idToken == null) return BadRequest();
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;
                AccessTokenModel jwtToken = _authenticationService.AuthenticateManager(uid);
                if (jwtToken.AccessToken.Length != 0)
                    return Ok(jwtToken);
                else
                    return NotFound(new { message = "User not register" });
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [AllowAnonymous]
        [HttpPost("auth/staffs/login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        public async Task<IActionResult> LoginStaffWithIdTokenAsync([FromHeader] string idToken)
        {
            if (idToken == null) return BadRequest();
            try
            {
                
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;
                AccessTokenModel jwtToken = _authenticationService.AuthenticateStaff(uid);
                if (jwtToken.AccessToken.Length != 0)
                    return Ok(jwtToken);
                else
                    return NotFound(new { message = "User not register" });
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [AllowAnonymous]
        [HttpPost("auth/staffs/accounts/login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        public async Task<IActionResult> LoginStaffWithUsernamePasswordAsync([FromHeader] string idToken)
        {
            if (idToken == null) return BadRequest();
            try
            {

                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;
                AccessTokenModel jwtToken = _authenticationService.AuthenticateStaff(uid);
                if (jwtToken.AccessToken.Length != 0)
                    return Ok(jwtToken);
                else
                    return NotFound(new { message = "User not register" });
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }




    }
}
