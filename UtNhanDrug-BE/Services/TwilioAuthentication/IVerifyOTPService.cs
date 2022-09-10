using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.TwilioOTP;

namespace UtNhanDrug_BE.Services.TwilioAuthentication
{
    public interface IVerifyOTPService
    {
        public Task<VerificationResponseModel> Verification(string phoneNumber);
        public Task<VerificationResponseModel> VerificationCheck(string phoneNumber, string code);
     }
}
