using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.TokenModel;
using UtNhanDrug_BE.Models.UserModel;
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
        [HttpPost("auth/user/login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        public async Task<IActionResult> LoginByUsernamePassword([FromForm]LoginModel model)
        {
            var result = await _authenticationService.Authenticate(model);
            if(result.AccessToken == null) return BadRequest(model);
            return StatusCode(result.StatusCode, result);
        }
    }
}
