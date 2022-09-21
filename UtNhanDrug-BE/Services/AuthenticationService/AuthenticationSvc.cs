
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UtNhanDrug_BE.Services.ManagerService;

namespace UtNhanDrug_BE.Services.AuthenticationService
{
    public class AuthenticationSvc : IAuthenticationSvc
    {
        private const string API_KEY = "AIzaSyB-9fD6pq0a7yjziqoxGIdHhaZEC5m2KG8";
        private readonly IConfiguration _configuration;
        private readonly IManagerSvc _managerSvc;

        public AuthenticationSvc(IConfiguration configuration, IManagerSvc managerSvc)
        {
            _configuration = configuration;
            _managerSvc = managerSvc;
        }
        //authen manager
        public string AuthenticateManager(string uid)
        {
            var user = LoadUser(uid);
            var isUserExits = _managerSvc.IsExitsAccount(user.Result.Email);
            if (isUserExits.Result != null)
            {
                var customTokenAsync = CreateCustomManagerToken(uid);
                return customTokenAsync;
            }
            return "";
        }
        private async Task<UserRecord> LoadUser(string uid)
        {
            UserRecord user = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            return user;
        }
        private string CreateCustomManagerToken(string uid)
        {
            var userDb = LoadUser(uid);
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
                    new Claim("id", userDb.Id.ToString()),
                    new Claim("email", user.Result.Email),
                    new Claim("name", user.Result.DisplayName),
                    new Claim("avatar", user.Result.PhotoUrl),
                    new Claim(ClaimTypes.Role, "MANAGER")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        //authen staff
        public string AuthenticateStaff(string uid)
        {
            var user = LoadUser(uid);
            var isUserExits = _managerSvc.IsExitsAccount(user.Result.Email);
            if (isUserExits.Result != null)
            {
                var customTokenAsync = CreateCustomStaffToken(uid);
                return customTokenAsync;
            }
            return "";
        }

        private string CreateCustomStaffToken(string uid)
        {
            var userDb = LoadUser(uid);
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
                    new Claim("id", userDb.Id.ToString()),
                    new Claim("email", user.Result.Email),
                    new Claim("name", user.Result.DisplayName),
                    new Claim("avatar", user.Result.PhotoUrl),
                    new Claim(ClaimTypes.Role, "STAFF")
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
