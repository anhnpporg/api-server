using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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
        [HttpPost("manager/login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        public async Task<IActionResult> LoginManagerWithIdTokenAsync(string idToken)
        {
            if (idToken == null) return BadRequest();
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;
                string jwtToken = _authenticationService.AuthenticateManager(uid);
                if (jwtToken.Length != 0)
                    return Ok(jwtToken);
                else
                    return NotFound("User not register");
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
        [AllowAnonymous]
        [HttpPost("staff/login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        public async Task<IActionResult> LoginStaffWithIdTokenAsync(string idToken)
        {
            if (idToken == null) return BadRequest();
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;
                string jwtToken = _authenticationService.AuthenticateStaff(uid);
                if (jwtToken.Length != 0)
                    return Ok(jwtToken);
                else
                    return NotFound("User not register");
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
