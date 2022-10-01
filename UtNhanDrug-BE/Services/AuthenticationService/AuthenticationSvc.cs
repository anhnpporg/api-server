
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.RoleModel;
using UtNhanDrug_BE.Models.TokenModel;
using UtNhanDrug_BE.Services.ManagerService;

namespace UtNhanDrug_BE.Services.AuthenticationService
{
    public class AuthenticationSvc : IAuthenticationSvc
    {
        private readonly IConfiguration _configuration;
        private readonly IUserSvc _userSvc;
        private readonly RoleType _roleType;

        public AuthenticationSvc(IConfiguration configuration, IUserSvc managerSvc, RoleType roleType)
        {
            _configuration = configuration;
            _userSvc = managerSvc;
            _roleType = roleType;
        }
        //authen manager
        public AccessTokenModel AuthenticateManager(string uid)
        {
            AccessTokenModel token = new AccessTokenModel();
            var user = LoadUser(uid);
            var isUserExits = _userSvc.IsExitsAccount(user.Result.Email);
            if (isUserExits.Result != null)
            {
                var customTokenAsync = CreateCustomManagerToken(uid);
                token.Message = "Successfully";
                token.AccessToken = customTokenAsync;
                return token;
            }
            token.Message = "Fail";
            token.AccessToken = "";
            return token;
        }
        private async Task<UserRecord> LoadUser(string uid)
        {
            UserRecord user = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            return user;
        }
        private string CreateCustomManagerToken(string uid)
        {
            var userDb = LoadUser(uid);
            var isUserExits = _userSvc.IsExitsAccount(userDb.Result.Email);
            var isBan = _userSvc.GetManager(isUserExits.Result.UserId).Result.IsBan;
            //var uidAdmin = _configuration.GetSection("AppSettings").GetSection("AdminUID").Value;
            var user = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            var isAdmin = isUserExits.Result.IsAdmin;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
            Console.WriteLine(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("uid", uid),
                    new Claim("userId", isUserExits.Result.UserId.ToString()),
                    new Claim("email", user.Result.Email),
                    //new Claim("name", user.Result.DisplayName),
                    //new Claim("avatar", user.Result.PhotoUrl),
                    new Claim("isBan", isBan.ToString()),
                    isAdmin ? new Claim(ClaimTypes.Role, "ADMIN") : new Claim(ClaimTypes.Role, "MANAGER")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        //authen staff
        public AccessTokenModel AuthenticateStaff(string uid)
        {
            AccessTokenModel token = new AccessTokenModel();
            var user = LoadUser(uid);
            var isUserExits = _userSvc.IsExitsStaff(user.Result.Email);
            if (isUserExits.Result != null)
            {
                var customTokenAsync = CreateCustomStaffToken(uid);
                token.Message = "Successfully";
                token.AccessToken = customTokenAsync;
                return token;
            }
            token.Message = "Fail";
            token.AccessToken = "";
            return token;
        }

        private string CreateCustomStaffToken(string uid)
        {
            var userDb = LoadUser(uid);
            var isUserExits = _userSvc.IsExitsStaff(userDb.Result.Email);
            var isBan = _userSvc.GetStaff(isUserExits.Result.UserId).Result.IsBan;
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
                    new Claim("userId", isUserExits.Result.UserId.ToString()),
                    new Claim("email", user.Result.Email),
                    new Claim("name", user.Result.DisplayName),
                    new Claim("avatar", user.Result.PhotoUrl),
                    new Claim("isBan", isBan.ToString()),
                    new Claim(ClaimTypes.Role, _roleType.Staff)
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
