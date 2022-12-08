
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper.HashingAlgorithms;
using UtNhanDrug_BE.Models.TokenModel;
using UtNhanDrug_BE.Models.UserModel;

namespace UtNhanDrug_BE.Services.AuthenticationService
{
    public class AuthenticationSvc : IAuthenticationSvc
    {
        private readonly IConfiguration _configuration;
        private readonly ut_nhan_drug_store_databaseContext _context;

        public AuthenticationSvc(IConfiguration configuration, ut_nhan_drug_store_databaseContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<AccessTokenModel> Authenticate(LoginModel model)
        {
            var checkLogin = await CheckLogin(model);
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.LoginName == model.Username);
            var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserAccountId == userLogin.UserAccountId);
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.UserAccountId == userLogin.UserAccountId);
            var userAccount = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userLogin.UserAccountId);
            if(userAccount != null)
            {
                if(userAccount.IsActive == false)
                {
                    return new AccessTokenModel()
                    {
                        AccessToken = "",
                        Message = "Tài khoản đã bị cấm"
                    };
                }
            }
            bool isAdmin = true;
            if(staff != null)
            {
                isAdmin = false;
            }
            if (checkLogin == -1) return new AccessTokenModel() { AccessToken = "", Message = "Không tìm thấy tên đăng nhặp" };
            if (checkLogin == 0) return new AccessTokenModel() { AccessToken = "", Message = "Mật khẩu không đúng" };
            if (checkLogin == 1)
            {
                if(isAdmin == true)
                {
                    manager.FcmToken = model.FCMToken;
                    await _context.SaveChangesAsync();
                }
                var accessToken = await CreateToken(model);
                return new AccessTokenModel() { Message = "Thành công", 
                                                AccessToken = accessToken.Trim(),
                                                IsAdmin = isAdmin
                };
            }
            return new AccessTokenModel() { Message = "Fail", AccessToken = "" };
        }

        private async Task<String> CreateToken(LoginModel model)
        {
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.LoginName == model.Username);
            var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserAccountId == userLogin.UserAccountId);
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.UserAccountId == userLogin.UserAccountId);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("userId", userLogin.UserAccountId.ToString()),
                        staff != null ? new Claim("roleUserId", staff.Id.ToString()) : new Claim("roleUserId", manager.Id.ToString()),
                        staff != null ? new Claim(ClaimTypes.Role, "STAFF") : new Claim(ClaimTypes.Role, "MANAGER"),
                        new Claim("FCMToken", model.FCMToken)
                    }),
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        private async Task<int> CheckLogin(LoginModel model)
        {
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.LoginName == model.Username);
            if (userLogin == null) return -1;
            var hashingId = userLogin.HashingAlgorithmId;
            string passwordHash;
            if (hashingId == 1)
            {
                passwordHash = HashingAlgorithmPassword.PasswordHashMD5(model.Password);
            }
            else if (hashingId == 2)
            {
                passwordHash = HashingAlgorithmPassword.PasswordHashSHA1(model.Password);
            }
            else
            {
                passwordHash = HashingAlgorithmPassword.PasswordHashSHA512(model.Password);
            }
            if (userLogin.PasswordHash.ToString().Trim() != passwordHash.Trim())
            {
                return 0;
            }else
            return 1;
        }




        //authen manager
        //public Task<AccessTokenModel> Authenticate(string uid)
        //{
        //    AccessTokenModel token = new AccessTokenModel();
        //    var user = LoadUser(uid);
        //    var isUserExits = _userSvc.IsExitsAccount(user.Result.Email);
        //    if (isUserExits.Result != null)
        //    {
        //        var customTokenAsync = CreateCustomToken(uid);
        //        token.Message = "Successfully";
        //        token.AccessToken = customTokenAsync;
        //        return token;
        //    }
        //    token.Message = "Fail";
        //    token.AccessToken = "";
        //    return token;
        //}

        //private string CreateCustomToken(string uid)
        //{
        //    var userFirebase = LoadUser(uid);
        //    var isUserExits = _userSvc.IsExitsAccount(userDb.Result.Email);
        //    var isBan = _userSvc.GetManager(isUserExits.Result.UserId).Result.IsBan;
        //    //var uidAdmin = _configuration.GetSection("AppSettings").GetSection("AdminUID").Value;
        //    var user = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        //    var isAdmin = isUserExits.Result.IsAdmin;
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
        //    Console.WriteLine(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim("uid", uid),
        //            new Claim("userId", isUserExits.Result.UserId.ToString()),
        //            isAdmin ? new Claim(ClaimTypes.Role, "ADMIN") : new Claim(ClaimTypes.Role, "MANAGER")
        //        }),
        //        Expires = DateTime.UtcNow.AddDays(7),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        //    var jwtToken = tokenHandler.WriteToken(token);
        //    return jwtToken;
        //}

        //private async Task<UserRecord> LoadUser(string uid)
        //{
        //    UserRecord user = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        //    return user;
        //}

        //private async Task<int> GetUserId(string uid)
        //{
        //    var userLogin = _context.UserLoginDataExternals.FirstOrDefaultAsync(x => x.UserId == uid);
        //}



    }
}
