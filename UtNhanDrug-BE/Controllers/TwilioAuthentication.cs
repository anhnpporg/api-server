using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Services.TwilioAuthentication;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class TwilioAuthentication : Controller
    {
        private readonly IVerifyOTPService _verifyOTPService;

        public TwilioAuthentication(IVerifyOTPService verifyOTPService)
        {
            _verifyOTPService = verifyOTPService;
        }

        [AllowAnonymous]
        [HttpPost("createVerification")]
        public async Task<IActionResult> CreateVerification(string phoneNumber)
        {
            var sid = await _verifyOTPService.Verification(phoneNumber);
            return Ok(sid);
        }
        
        [AllowAnonymous]
        [HttpPost("verificationCheck")]
        public async Task<IActionResult> VerificationCheck(string phoneNumber, string code)
        {
            if (phoneNumber == null || code == null) return BadRequest();
            try
            {
                var status = await _verifyOTPService.VerificationCheck(phoneNumber, code);
                return Ok(status); 
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
