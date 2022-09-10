using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Chat.V2;
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
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = _twilioConfig.AccountSID;
            string authToken = _twilioConfig.AuthToken;

            TwilioClient.Init(accountSid, authToken);

            var verification = await VerificationResource.CreateAsync(
                to: phonenumber,
                channel: "sms",
                pathServiceSid: _twilioConfig.ServiceSID
            );

            VerificationResponseModel responseModel = new VerificationResponseModel();

            if (verification != null)
            {
                responseModel.IsSuccess = "Success";
                responseModel.To = verification.To;
                responseModel.Valid = (bool)verification.Valid;
                responseModel.Status = verification.Status;
                responseModel.Date_create = (DateTime)verification.DateCreated;
            }
            else
            {
                responseModel.IsSuccess = "Fail";
            }

            return responseModel;
        }

        public async Task<VerificationResponseModel> VerificationCheck(string phonenumber, string code)
        {
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = _twilioConfig.AccountSID;
            string authToken = _twilioConfig.AuthToken;

            TwilioClient.Init(accountSid, authToken);

            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: phonenumber,
                code: code,
                pathServiceSid: _twilioConfig.ServiceSID
            );
            VerificationResponseModel responseModel = new VerificationResponseModel();

            if (verificationCheck != null)
            {
                responseModel.IsSuccess = "Success";
                responseModel.To = verificationCheck.To;
                responseModel.Valid = (bool)verificationCheck.Valid;
                responseModel.Status = verificationCheck.Status;
                responseModel.Date_create = (DateTime)verificationCheck.DateCreated;
            }else
            {
                responseModel.IsSuccess = "Fail";
            }

            return responseModel;
        }
    }
}
