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
    [Route("api/v{version:apiVersion}")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationSvc _authenticationService;

        public AuthenticationController(IAuthenticationSvc authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        public async Task<IActionResult> LoginWithIdTokenAsync(string idToken)
        {
            if (idToken == null) return BadRequest();
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;
                string jwtToken = _authenticationService.Authenticate(uid);
                if (jwtToken.Length != 0)
                    return Ok(jwtToken);
                else
                    return Ok(uid);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
