using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.TwilioOTP;

namespace UtNhanDrug_BE.Services.TwilioAuthentication
{
    public interface IVerifyOTPService
    {
        public Task<Object> Verification(string phoneNumber);
        public Task<Object> VerificationCheck(string phoneNumber, string code);
     }
}
