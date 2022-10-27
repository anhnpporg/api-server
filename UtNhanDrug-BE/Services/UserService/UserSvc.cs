using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.UserModel;
using System.Linq;
using System.Collections.Generic;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.StaffModel;
using UtNhanDrug_BE.Hepper.HashingAlgorithms;
using UtNhanDrug_BE.Hepper;
using UtNhanDrug_BE.Models.UserLoginModel;
using UtNhanDrug_BE.Services.EmailSenderService;
using UtNhanDrug_BE.Models.EmailModel;
using UtNhanDrug_BE.Models.PagingModel;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public class UserSvc : IUserSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly ISenderService _senderService;
        private const string defaultAvatar = "https://firebasestorage.googleapis.com/v0/b/utnhandrug.appspot.com/o/image-profile.png?alt=media&token=928ea13d-d43f-4c0e-a8ba-ab1999059530";
        public UserSvc(ut_nhan_drug_store_databaseContext context, ISenderService senderService)
        {
            _context = context;
            _senderService = senderService;
        }
        public async Task<int> BanAccount(int UserId)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(user => user.Id == UserId);
            if (user != null)
            {
                user.IsActive = false;
            }
            else
            {
                return -1;
            }

            return await _context.SaveChangesAsync();

        }

        public async Task<int> UnBanAccount(int UserId)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(user => user.Id == UserId);
            if (user != null)
            {
                user.IsActive = true;
            }
            else
            {
                return -1;
            }

            return await _context.SaveChangesAsync();

        }

        public async Task<List<ManagerViewModel>> GetManagers()
        {
            var query = from m in _context.Managers
                        join u in _context.UserAccounts on m.UserAccountId equals u.Id
                        select new { u, m };

            var data = await query
                .Select(x => new ManagerViewModel()
                {
                    UserId = x.u.Id,
                    Fullname = x.u.FullName,
                    Email = x.u.UserLoginDatum.EmailAddressRecovery,
                    CreatedAt = x.u.CreatedAt,
                    IsActive = x.u.IsActive
                }).ToListAsync();

            return data;
        }
        public async Task<List<CustomerViewModel>> GetCustomers()
        {
            var query = from c in _context.Customers
                        join u in _context.UserAccounts on c.UserAccountId equals u.Id
                        select new { u, c };

            var data = await query
                .Select(x => new CustomerViewModel()
                {
                    UserId = x.u.Id,
                    Fullname = x.u.FullName,
                    PhoneNumber = x.c.PhoneNumber,
                    CreatedAt = x.u.CreatedAt,
                    IsActive = x.u.IsActive
                }).ToListAsync();
            return data;
        }
        public async Task<List<ViewStaffModel>> GetStaffs()
        {
            var query = from s in _context.Staffs
                        join u in _context.UserAccounts on s.UserAccountId equals u.Id
                        select new { u, s };


            var data = await query
                .Select(x => new ViewStaffModel()
                {
                    UserId = x.u.Id,
                    Fullname = x.u.FullName,
                    Email = x.u.UserLoginDatum.EmailAddressRecovery,
                    PhoneNumber = x.s.PhoneNumber,
                    DateOfBirth = x.s.DateOfBirth,
                    IsMale = x.s.IsMale,
                    Avatar = x.s.Avatar,
                    CreatedAt = x.u.CreatedAt,
                    IsActive = x.u.IsActive
                }).ToListAsync();
            return data;
        }

        public async Task<bool> UpdateStaffProfile(int userId, UpdateStaffModel model)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
            var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            string avatar;
            if(model.Avatar == null)
            {
                avatar = defaultAvatar;
            }
            else
            {
                avatar = model.Avatar;
            }

            var updateEmail = await UpdateEmail(userId, model.EmailAddressRecovery);

            if(user != null && staff != null)
            {
                user.FullName = model.Fullname;
                staff.Avatar = avatar;
                staff.IsMale = model.IsMale;
                staff.DateOfBirth = model.Dob;
                staff.PhoneNumber = model.PhoneNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ChangePassword(int userId, ChangePasswordModel model)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);

            if (userData != null)
            {
                string passwordEncode;
                Random rnd = new Random();
                int hashingId = rnd.Next(1, 3);
                if (hashingId == 1)
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashMD5(model.NewPassword);
                }
                else if (hashingId == 2)
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashSHA1(model.NewPassword);
                }
                else
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashSHA512(model.NewPassword);
                }
                userData.PasswordHash = passwordEncode;
                userData.HashingAlgorithmId = hashingId;
                var result = await _context.SaveChangesAsync();
                if (result > 0) return true;
            }
            return false;
        }

        //public async Task<bool> UpdateManagerProfile(int userId, UpdateManagerModel model)
        //{
        //    return false;
        //}

        public async Task<bool> CreateCustomer(CreateCustomerModel model)
        {
            var isExits = await FindCustomer(model.PhoneNumber);
            if (isExits != true)
            {
                var userId = await CreateUser(model.Fullname);
                if (userId != 0)
                {
                    var customer = new Customer()
                    {
                        PhoneNumber = model.PhoneNumber,
                        UserAccountId = userId
                    };
                    _context.Customers.Add(customer);
                    var result = await _context.SaveChangesAsync();
                    if (result != 0) return true;
                }
            }
            return false;

        }

        private async Task<int> CreateUser(string fullname)
        {
            var user = new UserAccount()
            {
                FullName = fullname,
            };
            _context.UserAccounts.Add(user);
            var result = await _context.SaveChangesAsync();
            if (result != 0) return user.Id;
            return 0;
        }

        private async Task<bool> FindCustomer(string phoneNumber)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (customer != null) return true;
            return false;
        }

        public async Task<bool> CreateStaff(CreateStaffModel model)
        {
            var isExits = await FindStaff(model.LoginName);
            if (isExits == false)
            {
                var userId = await CreateUser(model.Fullname);
                if (userId != 0)
                {
                    string passwordEncode;
                    Random rnd = new Random();
                    int hashingId = rnd.Next(1, 3);
                    if (hashingId == 1)
                    {
                        passwordEncode = HashingAlgorithmPassword.PasswordHashMD5(model.Password);
                    }
                    else if (hashingId == 2)
                    {
                        passwordEncode = HashingAlgorithmPassword.PasswordHashSHA1(model.Password);
                    }
                    else
                    {
                        passwordEncode = HashingAlgorithmPassword.PasswordHashSHA512(model.Password);
                    }
                    String avatar;
                    if(model.Avatar != null)
                    {
                        avatar = model.Avatar;
                    }
                    else
                    {
                        avatar = defaultAvatar;
                    }
                    var userLoginData = await CreateUserLoginData(userId, model.LoginName, passwordEncode, hashingId);
                    if (userLoginData != false)
                    {
                        Staff s = new Staff()
                        {
                            UserAccountId = userId,
                            Avatar = avatar,
                            DateOfBirth = model.Dob,
                            IsMale = model.IsMale,
                            PhoneNumber = model.PhoneNumber,
                        };
                        _context.Staffs.Add(s);
                        var result = await _context.SaveChangesAsync();
                        if (result != 0) return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> FindStaff(string loginName)
        {
            var staff = await _context.UserLoginData.FirstOrDefaultAsync(x => x.LoginName == loginName);
            if (staff != null) return true;
            return false;
        }

        private async Task<bool> CreateUserLoginData(int userId, string loginName, string passwordEncode, int hashAlgorithmsId)
        {
            UserLoginDatum u = new UserLoginDatum()
            {
                UserAccountId = userId,
                LoginName = loginName,
                PasswordHash = passwordEncode,
                HashingAlgorithmId = hashAlgorithmsId,
            };
            _context.UserLoginData.Add(u);

            var result = await _context.SaveChangesAsync();
            if (result != 0) return true;
            return false;
        }

        public async Task<object> GetUserProfile(int userId)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);

            if(user == null) return null;
            //filter user
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserAccountId == user.Id);
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserAccountId == userId);

            if (customer != null)
            {
                CustomerViewModel model = new CustomerViewModel()
                {
                    UserId = userId,
                    CreatedAt = user.CreatedAt,
                    Fullname = user.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    IsActive = user.IsActive
                };

                return model;
            }
            else if (manager != null)
            {
                ManagerViewModel model = new ManagerViewModel()
                {
                    CreatedAt = user.CreatedAt,
                    Fullname = user.FullName,
                    Email = userLogin.EmailAddressRecovery,
                    UserId = userId,
                    IsActive = user.IsActive
                };
                return model;
            }
            else if (staff != null)
            {
                ViewStaffModel model = new ViewStaffModel()
                {
                    UserId = userId,
                    PhoneNumber = staff.PhoneNumber,
                    Avatar = staff.Avatar,
                    CreatedAt = user.CreatedAt,
                    DateOfBirth = staff.DateOfBirth,
                    Fullname = user.FullName,
                    Email = userLogin.EmailAddressRecovery,
                    IsMale = staff.IsMale,
                    IsActive = user.IsActive
                };
                return model;
            }
            return null;
        }

        public async Task<bool> CheckUser(int userId)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null) return true;
            return false;
        }

        public async Task<bool> RecoveryPassword(int userId, RecoveryPasswordModel model)
        {
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if (userLogin != null)
            {
                string passwordEncode;
                Random rnd = new Random();
                int hashingId = rnd.Next(1, 3);
                if (hashingId == 1)
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashMD5(model.NewPassword);
                }
                else if (hashingId == 2)
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashSHA1(model.NewPassword);
                }
                else
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashSHA512(model.NewPassword);
                }
                userLogin.HashingAlgorithmId = hashingId;
                userLogin.PasswordHash = passwordEncode;
                var result = await _context.SaveChangesAsync();
                if (result > 0) return true;
            }
            return false;
        }

        public async Task<TokenVerifyResponse> CreateTokenVerifyEmail(int userId)
        {
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if(userLogin != null)
            {
                userLogin.ConfirmationToken = KeyGenerator.GetUniqueKey(6);
                userLogin.TokenGenerationTime = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var message = new MessageModel(new string[] { userLogin.EmailAddressRecovery }, "Code verification email", userLogin.ConfirmationToken);
                    await _senderService.SendEmail(message);

                    return new TokenVerifyResponse()
                    {
                        Token = userLogin.ConfirmationToken,
                        CreateAt = userLogin.TokenGenerationTime
                    };
                }
                
            }
            return null;
        }

        public async Task<bool> CheckTokenVerifyEmail(int userId, TokenVerifyModel model)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if(userData.EmailValidationStatusId == 2)
            {
                if(userData.ConfirmationToken == model.Token)
                {
                    userData.EmailValidationStatusId = 3;
                    var result = await _context.SaveChangesAsync();
                    if(result > 0) return true;
                }
            }
            return false;
        }

        public async Task<int> CheckEmail(int userId)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if(userData.EmailValidationStatusId == 1)
            {
                return 1;
            }else if(userData.EmailValidationStatusId == 2)
            {
                return 2;
            }
            return 3;
        }

        public async Task<bool> CheckPassword(int userId, string password)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if(userData != null)
            {
                string passwordEncode;
                int hashingId = userData.HashingAlgorithmId;
                if (hashingId == 1)
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashMD5(password);
                }
                else if (hashingId == 2)
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashSHA1(password);
                }
                else
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashSHA512(password);
                }

                if (userData.PasswordHash.Trim().Equals(passwordEncode.Trim())) return true;
            }
            return false;
        }

        public async Task<bool> CheckTimeVerifyEmail(int userId)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            var s = (DateTime.Now - userData.TokenGenerationTime);
            if (s.Value.Minutes > 5) return false;
            return true;
        }
        
        public async Task<bool> CheckTimeVerifyPassword(int userId)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            var s = (DateTime.Now - userData.RecoveryTokenTime);
            if (s.Value.Minutes > 5) return false;
            return true;
        }

        public async Task<TokenVerifyResponse> CreateTokenVerifyPassword(int userId)
        {
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if (userLogin != null)
            {
                userLogin.PasswordRecoveryToken = KeyGenerator.GetUniqueKey(6);
                userLogin.RecoveryTokenTime = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0) 
                {
                    var message = new MessageModel(new string[] { userLogin.EmailAddressRecovery }, "Code verification password", userLogin.PasswordRecoveryToken);
                    await _senderService.SendEmail(message);

                    return new TokenVerifyResponse()
                    {
                        Token = userLogin.PasswordRecoveryToken,
                        CreateAt = userLogin.RecoveryTokenTime
                    };
                } 
            }
            return null;
        }

        public async Task<bool> CheckVerifyPassword(int userId, string token)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            {
                if (userData.PasswordRecoveryToken == token) return true;
            }
            return false;
        }

        public async Task<bool> UpdateEmail(int userId, string email)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);

            int EmailValidationStatusId;
            if (userData.EmailAddressRecovery.Equals(email.Trim()) && userData.EmailValidationStatusId == 3)
            {
                EmailValidationStatusId = 3;
            }
            else if (userData.EmailAddressRecovery == null && email.Trim() == null)
            {
                EmailValidationStatusId = 1;
            }
            else
            {
                EmailValidationStatusId = 2;
            }
            if(userData != null)
            {
                userData.EmailAddressRecovery = email.Trim();
                userData.EmailValidationStatusId = EmailValidationStatusId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<PageResult<CustomerViewModel>> SearchCustomer(CustomerPagingRequest request)
        {
            var query = from c in _context.Customers
                        where c.PhoneNumber.Contains(request.PhoneNumber)
                        select c;
            var data = query.Distinct();
            var result = await data.Select(c => new CustomerViewModel()
            {
                UserId = c.UserAccountId,
                Fullname = c.UserAccount.FullName,
                PhoneNumber = c.PhoneNumber,
                CreatedAt = c.UserAccount.CreatedAt,
                IsActive = c.UserAccount.IsActive
            }).ToListAsync();
            //paging
            int totalRow = await data.CountAsync();

            var pagedResult = new PageResult<CustomerViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                Items = result
            };

            return pagedResult;
        }
    }
}
