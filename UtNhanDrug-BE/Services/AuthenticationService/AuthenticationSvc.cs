
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UtNhanDrug_BE.Services.AuthenticationService
{
    public class AuthenticationSvc : IAuthenticationSvc
    {
        private const string API_KEY = "AIzaSyB-9fD6pq0a7yjziqoxGIdHhaZEC5m2KG8";
        private readonly IConfiguration _configuration;

        public AuthenticationSvc(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Authenticate(string uid)
        {
            var user = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserAsync(uid);

            if (user.Result != null)
            {
                var customTokenAsync = CreateCustomToken(uid, user.Result.Uid);
                return customTokenAsync;
            }
            return "";
        }

        private string CreateCustomToken(string uid, string id)
        {
            //var uidAdmin = _configuration.GetSection("AppSettings").GetSection("AdminUID").Value;
            var user = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
            Console.WriteLine(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("uid", uid),
                    new Claim("email", user.Result.Email),
                    new Claim("name", user.Result.DisplayName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
