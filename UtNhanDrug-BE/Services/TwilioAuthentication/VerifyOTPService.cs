using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using UtNhanDrug_BE.Configurations;
using UtNhanDrug_BE.Models.FcmNoti;
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

        public async Task<Object> Verification(string phonenumber)
        {
            VerificationResponseModel responseModel = new VerificationResponseModel();
            ErrorResponseModel errorModel = new ErrorResponseModel();
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

                
                responseModel.To = verification.To;
                responseModel.Valid = (bool)verification.Valid;
                responseModel.Status = verification.Status;
                responseModel.Date_create = (DateTime)verification.DateCreated;
            }
            catch (Exception ex)
            {
                errorModel.IsSuccess = "Fail";
                errorModel.Message = ex.Message.ToString();
                return errorModel;
            }

            return responseModel;
        }

        public async Task<Object> VerificationCheck(string phonenumber, string code)
        {
            VerificationResponseModel responseModel = new VerificationResponseModel();
            ErrorResponseModel errorModel = new ErrorResponseModel();

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

                responseModel.To = verificationCheck.To;
                responseModel.Valid = (bool)verificationCheck.Valid;
                responseModel.Status = verificationCheck.Status;
                responseModel.Date_create = (DateTime)verificationCheck.DateCreated;

            }
            catch(Exception ex)
            {
                errorModel.IsSuccess = "Fail";
                errorModel.Message = ex.Message.ToString();
                return errorModel;
            }

            return responseModel;
        }
    }
}
