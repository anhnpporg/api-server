using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using UtNhanDrug_BE.Configurations;
using UtNhanDrug_BE.Models.TwilioOTP;

namespace UtNhanDrug_BE.Services.TwilioAuthentication
{
    public class VerifyOTPService : IVerifyOTPService
    {
        private readonly TwilioConfig _twilioConfig;
        public VerifyOTPService(IOptions<TwilioConfig> settings)
        {
            _twilioConfig = settings.Value;
        }

        public async Task<VerificationResponseModel> Verification(string phonenumber)
        {
            VerificationResponseModel responseModel = new VerificationResponseModel();
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = _twilioConfig.AccountSID;
            string authToken = _twilioConfig.AuthToken;
            try
            {
                TwilioClient.Init(accountSid, authToken);

                var verification = await VerificationResource.CreateAsync(
                    to: phonenumber,
                    channel: "sms",
                    pathServiceSid: _twilioConfig.ServiceSID
                );
                if (verification.Status.Equals("pending"))
                {
                    responseModel.IsSuccess = "True";
                    responseModel.Status = verification.Status;
                    responseModel.Message = "Send OTP successfully";
                }
            }
            catch (Exception ex)
            {
                responseModel.IsSuccess = "Fasle";
                responseModel.Message = ex.Message.ToString();
                responseModel.Status = "cancle";
                return responseModel;
            }

            return responseModel;
        }

        public async Task<VerificationResponseModel> VerificationCheck(string phonenumber, string code)
        {
            VerificationResponseModel responseModel = new VerificationResponseModel();

            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = _twilioConfig.AccountSID;
            string authToken = _twilioConfig.AuthToken;

            try
            {
                TwilioClient.Init(accountSid, authToken);

                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: phonenumber,
                    code: code,
                    pathServiceSid: _twilioConfig.ServiceSID
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_twilioConfig.AuthToken);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim("phone_number", phonenumber),
                    new Claim("status", verificationCheck.Status),
                }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                responseModel.IsSuccess = "True";
                responseModel.Status = verificationCheck.Status;
                responseModel.Message = jwtToken;
                //responseModel.To = verificationCheck.To;
                //responseModel.Valid = (bool)verificationCheck.Valid;
                //responseModel.Status = verificationCheck.Status;
                //responseModel.Date_create = (DateTime)verificationCheck.DateCreated;

            }
            catch(Exception ex)
            {
                responseModel.IsSuccess = "False";
                responseModel.Message = ex.Message.ToString();
                return responseModel;
            }

            return responseModel;
        }
    }
}
