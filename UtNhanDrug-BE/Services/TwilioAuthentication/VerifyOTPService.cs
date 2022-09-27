﻿using Microsoft.Extensions.Configuration;
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
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.RoleModel;
using UtNhanDrug_BE.Models.TwilioOTP;
using UtNhanDrug_BE.Services.CustomerService;

namespace UtNhanDrug_BE.Services.TwilioAuthentication
{
    public class VerifyOTPService : IVerifyOTPService
    {
        private readonly TwilioConfig _twilioConfig;
        private readonly IConfiguration _configuration;
        private readonly ICustomerSvc _customer;
        private readonly RoleType _roleType;
        public VerifyOTPService(IOptions<TwilioConfig> settings, IConfiguration configuration, ICustomerSvc customer, RoleType roleType)
        {
            _twilioConfig = settings.Value;
            _configuration = configuration;
            _customer = customer;
            _roleType = roleType;
        }

        public async Task<VerificationResponseModel> Verification(string phonenumber)
        {
            VerificationResponseModel responseModel = new VerificationResponseModel();
            var customer = await _customer.FindByPhoneNumber(phonenumber);
            if(customer != null)
            {
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
                    else
                    {
                        responseModel.IsSuccess = "True";
                        responseModel.Status = verification.Status;
                        responseModel.Message = "Send OTP fail";
                    }
                }
                catch (Exception ex)
                {
                    responseModel.IsSuccess = "Fasle";
                    responseModel.Message = ex.Message.ToString();
                    responseModel.Status = "cancle";
                    return responseModel;
                }
            }
            else
            {
                responseModel.IsSuccess = "Fasle";
                responseModel.Message = "Customer not found";
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
                if (verificationCheck.Status.ToLower().Equals("approved"))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                    new Claim("phone_number", phonenumber),
                    new Claim("status", verificationCheck.Status),
                    new Claim(ClaimTypes.Role, _roleType.Customer)
                    //new Claim(ClaimTypes.Role, "user")
                }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);

                    responseModel.IsSuccess = "True";
                    responseModel.Status = verificationCheck.Status;
                    responseModel.Message = jwtToken;
                }
                else
                {
                    responseModel.IsSuccess = "False";
                    responseModel.Status = verificationCheck.Status;
                    responseModel.Message = "Verify fail";
                }



                //responseModel.To = verificationCheck.To;
                //responseModel.Valid = (bool)verificationCheck.Valid;
                //responseModel.Status = verificationCheck.Status;
                //responseModel.Date_create = (DateTime)verificationCheck.DateCreated;

            }
            catch (Exception ex)
            {
                responseModel.IsSuccess = "False";
                responseModel.Message = ex.Message.ToString();
                return responseModel;
            }

            return responseModel;
        }
    }
}
