using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.TwilioOTP;
using UtNhanDrug_BE.Services.TwilioAuthentication;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1")]
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
            if(sid == null)
            {
                return NotFound();
            }
            return Ok(sid);
        }
        
        [AllowAnonymous]
        [HttpPost("verificationCheck")]
        public async Task<IActionResult> VerificationCheck(string phoneNumber, string code)
        {
            if (phoneNumber == null || code == null) return BadRequest();
            VerificationResponseModel responseModel = new VerificationResponseModel();
            try
            {
                responseModel = await _verifyOTPService.VerificationCheck(phoneNumber, code);
                if (!responseModel.Status.ToLower().Equals("approved"))
                {
                    return BadRequest();
                }
                return Ok(responseModel); 
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
