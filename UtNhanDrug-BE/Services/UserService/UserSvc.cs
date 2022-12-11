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
using UtNhanDrug_BE.Models.ResponseModel;
using Microsoft.EntityFrameworkCore.Storage;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public class UserSvc : IUserSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private readonly ISenderService _senderService;
        private readonly DateTime today = LocalDateTime.DateTimeNow();
        private const string defaultAvatar = "https://firebasestorage.googleapis.com/v0/b/utnhandrug.appspot.com/o/image-profile.png?alt=media&token=928ea13d-d43f-4c0e-a8ba-ab1999059530";
        public UserSvc(ut_nhan_drug_store_databaseContext context, ISenderService senderService)
        {
            _context = context;
            _senderService = senderService;
        }
        public async Task<Response<bool>> BanAccount(int UserId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(user => user.Id == UserId);
                if (user == null) return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tài khoản không tồn tại"
                };
                user.IsActive = false;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Chạn tài khoản thành công"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    Message = "Chặn tài khoản không thành công",
                    StatusCode = 400
                };
            }
        }

        public async Task<Response<bool>> UnBanAccount(int UserId)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(user => user.Id == UserId);
                if (user == null) return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "Tài khoản không tồn tại"
                    };
                user.IsActive = true;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Mở chặn tài khoản thành công"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Mở chặn tài khoản thất bại"
                };
            }
        }

        public async Task<Response<List<ManagerViewModel>>> GetManagers()
        {
            var query = from m in _context.Managers
                        join u in _context.UserAccounts on m.UserAccountId equals u.Id
                        select new { u, m };

            var data = await query.OrderByDescending(x => x.u.CreatedAt)
                .Select(x => new ManagerViewModel()
                {
                    UserId = x.u.Id,
                    Fullname = x.u.FullName,
                    Email = x.u.UserLoginDatum.EmailAddressRecovery,
                    CreatedAt = x.u.CreatedAt,
                    IsActive = x.u.IsActive
                }).ToListAsync();
            if(data.Count > 0)
            {
                return new Response<List<ManagerViewModel>>(data);
            }
            else
            {
                return new Response<List<ManagerViewModel>>(data)
                {
                    Message = "Không có quản lí"
                };
            }
            
        }
        public async Task<Response<List<CustomerViewModel>>> GetCustomers()
        {
            var query = from c in _context.Customers
                        select c;

            var data = await query.OrderByDescending(x => x.CreatedAt)
                .Select(x => new CustomerViewModel()
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    TotalPoint = x.TotalPoint,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    IsActive = x.IsActive
                }).ToListAsync();
            if (data.Count > 0)
            {
                return new Response<List<CustomerViewModel>>(data);
            }
            else
            {
                return new Response<List<CustomerViewModel>>(data)
                {
                    Message = "Không tìm thấy khách hàng"
                };
            }
        }
        public async Task<Response<List<ViewStaffModel>>> GetStaffs()
        {
            var query = from s in _context.Staffs
                        join u in _context.UserAccounts on s.UserAccountId equals u.Id
                        select new { u, s };


            var data = await query.OrderByDescending(x => x.u.CreatedAt)
                .Select(x => new ViewStaffModel()
                {
                    UserId = x.u.Id,
                    Fullname = x.u.FullName,
                    Email = x.u.UserLoginDatum.EmailAddressRecovery,
                    PhoneNumber = x.s.PhoneNumber,
                    DateOfBirth = x.s.DateOfBirth,
                    IsMale = x.s.IsMale,
                    Avatar = x.s.UrlAvartar,
                    CreatedAt = x.u.CreatedAt,
                    IsActive = x.u.IsActive
                }).ToListAsync();
            if (data.Count > 0)
            {
                return new Response<List<ViewStaffModel>>(data);
            }
            else
            {
                return new Response<List<ViewStaffModel>>(data)
                {
                    Message = "Không tìm thấy nhân viên"
                };
            }
        }

        public async Task<Response<bool>> UpdateStaffProfile(int userId, UpdateStaffModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
                var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserAccountId == userId);
                var checkPhoneNumber = await CheckPhoneNumber(userId, model.PhoneNumber);
                if (checkPhoneNumber.Data != true) return checkPhoneNumber;
                //string avatar;
                //if(model.Avatar == null)
                //{
                //    avatar = defaultAvatar;
                //}
                //else
                //{
                //    avatar = model.Avatar;
                //}
                if (user != null && staff != null)
                {
                    staff.PhoneNumber = model.PhoneNumber;
                    var updateEmail = await UpdateEmail(userId, model.EmailAddressRecovery);
                    if (updateEmail.StatusCode == 400)
                    {
                        await transaction.RollbackAsync();
                        return updateEmail;
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new Response<bool>(true)
                {
                    Message = "Cập nhật nhân viên thành công"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                return new Response<bool>(true)
                {
                    Message = "Cập nhật nhân viên thành công"
                };
            }
            
            //if (user != null && staff != null)
            //{
            //    //user.FullName = model.Fullname;
            //    //staff.UrlAvartar = avatar;
            //    //staff.IsMale = model.IsMale;
            //    //staff.DateOfBirth = model.Dob;
            //    //staff.PhoneNumber = model.PhoneNumber;
            //    await _context.SaveChangesAsync();
            //    return new Response<bool>(true)
            //    {
            //        Message = "Cập nhật nhân viên thành công"
            //    };
            //}
            //return new Response<bool>(false)
            //{
            //    StatusCode = 400,
            //    Message = "Cập nhật nhân viên không thành công"
            //};
        }

        private async Task<Response<bool>> CheckCurrentPassword(int userId ,string currentPassword)
        {
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            int hashingId = userLogin.HashingAlgorithmId;
            string passwordEncode;
            if (hashingId == 1)
            {
                passwordEncode = HashingAlgorithmPassword.PasswordHashMD5(currentPassword);
            }
            else if (hashingId == 2)
            {
                passwordEncode = HashingAlgorithmPassword.PasswordHashSHA1(currentPassword);
            }
            else
            {
                passwordEncode = HashingAlgorithmPassword.PasswordHashSHA512(currentPassword);
            }
            if (!passwordEncode.Equals(userLogin.PasswordHash))
            {
                return new Response<bool>(false)
                {
                    Message = "Mật khẩu hiện tại không đúng",
                    StatusCode = 400
                };
            }
            return new Response<bool>(true)
            {
                Message = "Mật khẩu hiện tại khớp",
            };
        }
        public async Task<Response<bool>> ChangePassword(int userId, ChangePasswordModel model)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            var checkCode = await CheckVerifyPassword(userId, model.TokenRecovery);
            if (checkCode == false) return new Response<bool>(false)
            {
                StatusCode = 400,
                Message = "Mã xác thực mật khẩu sai"
            };
            var checkTime = await CheckTimeVerifyPassword(userId);
            if (checkTime == false) return new Response<bool>(false)
            {
                StatusCode = 400,
                Message = "Mã xác thực mật khẩu đã hết hạn, vui lòng tạo mã khác"
            };
            if(!model.NewPassword.Equals(model.ConfirmPassword)) return new Response<bool>(false)
            {
                StatusCode = 400,
                Message = "Mật khẩu mới không khớp"
            };
            if (userData != null)
            {
                var checkCurrentPassword = await CheckCurrentPassword(userId, model.CurrentPassword);
                if (checkCurrentPassword.Data == false) return checkCurrentPassword;
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
                if (result > 0)
                {
                    return new Response<bool>(true)
                    {
                        Message = "Đổi mật khẩu thành công"
                    };
                }
                else return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Đổi mật khẩu thất bại"
                };
            }
            return new Response<bool>(false)
            {
                StatusCode = 400,
                Message = "Không tìm thấy tài khoản"
            };
        }
        
        public async Task<Response<bool>> ChangePasswordByUserId(int userId, ChangePasswordByIdModel model)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if (!model.NewPassword.Equals(model.ConfirmPassword)) return new Response<bool>(false)
            {
                StatusCode = 400,
                Message = "Mật khẩu mới không khớp"
            };
            if (userData != null)
            {
                var checkCurrentPassword = await CheckCurrentPassword(userId, model.CurrentPassword);
                if (checkCurrentPassword.Data == false) return checkCurrentPassword;
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
                if (result > 0)
                {
                    return new Response<bool>(true)
                    {
                        Message = "Đổi mật khẩu thành công"
                    };
                }
                else return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Đổi mật khẩu thất bại"
                };
            }
            return new Response<bool>(false)
            {
                StatusCode = 400,
                Message = "Không tìm thấy tài khoản"
            };
        }

        //public async Task<bool> UpdateManagerProfile(int userId, UpdateManagerModel model)
        //{
        //    return false;
        //}

        public async Task<Response<Customer>> CreateCustomer(int userId, CreateCustomerModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var isExits = await FindCustomer(model.PhoneNumber);
                if (isExits == false)
                {
                    var user = await CreateUser(model.FullName);
                    if (user.Data > 0)
                    {
                        var customer = new Customer()
                        {
                            PhoneNumber = model.PhoneNumber,
                            FullName = model.FullName,
                            CreatedAt = today,
                            CreatedBy = userId
                        };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<Customer>(customer)
                        {
                            Message = "Tạo khách hàng thành công"
                        };
                    }
                    else
                    {
                        return new Response<Customer>(null)
                        {
                            StatusCode = 400,
                            Message = user.Message
                        };
                    }
                }
                return new Response<Customer>(null)
                {
                    StatusCode = 400,
                    Message = "Khách hàng này đã tồn tại"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<Customer>(null)
                {
                    StatusCode = 500,
                    Message = "Đã có lỗi xảy ra"
                };
            }
            

        }

        private async Task<Response<int>> CreateUser(string fullname)
        {
            try
            {
                var user = new UserAccount()
                {
                    FullName = fullname,
                    CreatedAt = today
                };
                _context.UserAccounts.Add(user);
                await _context.SaveChangesAsync();
                return new Response<int>(user.Id)
                {
                    Message = "Tạo tài khoản thành công"
                };
            }catch
            {
                return new Response<int>(-1)
                {
                    StatusCode = 400,
                    Message = "Tạo tài khoản không thành công"
                };
            }
            
        }

        private async Task<bool> FindCustomer(string phoneNumber)
        {
            var customer = await _context.Customers.OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (customer == null) return false;
            return true;
        }

        private async Task<Response<bool>> CheckExitsEmail(string email)
        {
            var emails = await _context.UserLoginData.ToListAsync();
            foreach(var e in emails)
            {
                if (e.EmailAddressRecovery.Equals(email))
                {
                    return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "email này đã tồn tại"
                    };
                }
            }
            return new Response<bool>(true)
            {
                Message = "Không tìm thấy email này"
            };
        }

        public async Task<Response<bool>> CreateStaff(CreateStaffModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                if (!model.Password.Equals(model.PasswordConfirm)) {
                    return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "Mật khẩu xác nhận không khớp"
                    };
                }
                var isExits = await FindStaff(model.LoginName);
                if (isExits == false)
                {
                    var userId = await CreateUser(model.Fullname);
                    if (userId.Data > 0)
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
                        if (model.Avatar != null)
                        {
                            avatar = model.Avatar;
                        }
                        else
                        {
                            avatar = defaultAvatar;
                        }
                        //check email
                        var checkExitsEmail = await CheckExitsEmail(model.Email);
                        if (checkExitsEmail.Data == false) return checkExitsEmail;
                        var userLoginData = await CreateUserLoginData(userId.Data, model.LoginName, passwordEncode, hashingId, model.Email);
                        if (userLoginData.Data != false)
                        {
                            Staff s = new Staff()
                            {
                                UserAccountId = userId.Data,
                                UrlAvartar = avatar,
                                DateOfBirth = model.Dob,
                                IsMale = model.IsMale,
                                PhoneNumber = model.PhoneNumber
                            };
                            _context.Staffs.Add(s);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return new Response<bool>(true)
                            {
                                Message = "Tạo nhân viên thành công"
                            };
                        }
                    }
                    return new Response<bool>(false)
                    {
                        StatusCode = userId.StatusCode,
                        Message = userId.Message
                    };
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tên đăng nhập của nhân viên này đã tồn tại"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo nhân viên thất bại"
                };
            }
            
        }

        private async Task<bool> FindStaff(string loginName)
        {
            var staff = await _context.UserLoginData.FirstOrDefaultAsync(x => x.LoginName == loginName);
            if (staff != null) return true;
            return false;
        }

        private async Task<Response<bool>> CreateUserLoginData(int userId, string loginName, string passwordEncode, int hashAlgorithmsId, string email)
        {
            try
            {
                var checkExitsEmail = await CheckExitsEmail(email);
                if (checkExitsEmail.Data == false) return checkExitsEmail;
                UserLoginDatum u = new UserLoginDatum()
                {
                    UserAccountId = userId,
                    LoginName = loginName,
                    PasswordHash = passwordEncode,
                    HashingAlgorithmId = hashAlgorithmsId,
                    EmailAddressRecovery = email,
                    EmailValidationStatusId = 2
                };
                _context.UserLoginData.Add(u);

                await _context.SaveChangesAsync();
                return new Response<bool>(true)
                {
                    Message = "Tạo thành công"
                };
            }
            catch
            {
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Tạo thất bại"
                };
            }
            
        }

        public async Task<Response<object>> GetUserProfile(int userId)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);

            if(user == null) return new Response<object>(null)
            {
                StatusCode = 400,
                Message = "Tài khoản không tồn tại"
            };
            //filter user
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserAccountId == userId);

            
            if (manager != null)
            {
                ManagerViewModel model = new ManagerViewModel()
                {
                    UserAccount = user.UserLoginDatum.LoginName,
                    CreatedAt = user.CreatedAt,
                    Fullname = user.FullName,
                    Email = userLogin.EmailAddressRecovery,
                    UserId = userId,
                    IsActive = user.IsActive
                };
                return new Response<object>(model)
                {
                    Message = "Thông tin tài khoản quản lí"
                };
            }
            else if (staff != null)
            {
                ViewStaffModel model = new ViewStaffModel()
                {
                    UserAccount = user.UserLoginDatum.LoginName,
                    UserId = userId,
                    PhoneNumber = staff.PhoneNumber,
                    Avatar = staff.UrlAvartar,
                    CreatedAt = user.CreatedAt,
                    DateOfBirth = staff.DateOfBirth,
                    Fullname = user.FullName,
                    Email = userLogin.EmailAddressRecovery,
                    IsMale = staff.IsMale,
                    IsActive = user.IsActive
                };
                return new Response<object>(model)
                {
                    Message = "Thông tin tài khoản nhân viên"
                };
            }
            return new Response<object>(null)
            {
                StatusCode = 400,
                Message = "Tài khoản không tồn tại"
            };
        }

        //public async Task<bool> CheckUser(int userId)
        //{
        //    var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
        //    if (user != null) return true;
        //    return false;
        //}

        public async Task<Response<string>> RecoveryPassword(int userId)
        {
            string newPassword = "123456";

            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if (userLogin != null)
            {
                string passwordEncode;
                Random rnd = new Random();
                int hashingId = rnd.Next(1, 3);
                if (hashingId == 1)
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashMD5(newPassword);
                }
                else if (hashingId == 2)
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashSHA1(newPassword);
                }
                else
                {
                    passwordEncode = HashingAlgorithmPassword.PasswordHashSHA512(newPassword);
                }
                userLogin.HashingAlgorithmId = hashingId;
                userLogin.PasswordHash = passwordEncode;
                var result = await _context.SaveChangesAsync();
                if (result > 0) return new Response<string>(newPassword)
                {
                    Message = "Tạo mật khẩu mới thành công"
                };
            }
            return new Response<string>(null)
            {
                StatusCode = 400,
                Message = "Tạo mật khẩu mới thất bại"
            };
        }

        public async Task<Response<TokenVerifyResponse>> CreateTokenVerifyEmail(int userId)
        {
            var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if(userLogin != null)
            {
                if(userLogin.EmailValidationStatusId == 1)
                {
                    return new Response<TokenVerifyResponse>()
                    {
                        StatusCode = 400,
                        Message = "Tài khoản không có email"
                    };
                }
                else if(userLogin.EmailValidationStatusId == 3) 
                {
                    return new Response<TokenVerifyResponse>()
                    {
                        StatusCode = 400,
                        Message = "Email đã được xác thực"
                    };
                }
                userLogin.ConfirmationToken = KeyGenerator.GetUniqueKey(6);
                userLogin.TokenGenerationTime = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var message = new MessageModel(new string[] { userLogin.EmailAddressRecovery }, "Mã xác thực email", "Mã xác thực email của bạn là: "+ userLogin.ConfirmationToken +"\n Mã xác thực có hiệu lực 5 phút, không chia sẻ mã xác thực với bất kì ai");
                    await _senderService.SendEmail(message);

                    return new Response<TokenVerifyResponse>(new TokenVerifyResponse()
                    {
                        Token = userLogin.ConfirmationToken,
                        CreateAt = userLogin.TokenGenerationTime
                    }){
                        Message = "Gửi mã xác thực thành công"
                    };
                }
                
            }

            return new Response<TokenVerifyResponse>()
            {
                StatusCode = 400,
                Message = "Gửi mã xác thực thất bại"
            };
        }

        public async Task<Response<bool>> CheckTokenVerifyEmail(int userId, TokenVerifyModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
                if (userData.EmailValidationStatusId == 2)
                {
                    var checkTime = await CheckTimeVerifyEmail(userId);
                    if (checkTime == false) return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "Mã xác thực email đã hết hạn, vui lòng tạo mã khác"
                    };
                    if (userData.ConfirmationToken == model.Token)
                    {
                        userData.EmailValidationStatusId = 3;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new Response<bool>(true)
                        {
                            Message = "Xác thực email thành công"
                        };
                    }
                }
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Xác thực email thất bại"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Xác thực email thất bại"
                };
            }
            
        }

        //private async Task<Response<int>> CheckEmail(int userId)
        //{
        //    var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
        //    if(userData.EmailValidationStatusId == 1)
        //    {
        //        return 1;
        //    }else if(userData.EmailValidationStatusId == 2)
        //    {
        //        return 2;
        //    }
        //    return 3;
        //}

        public async Task<Response<bool>> CheckPassword(int userId, string password)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            if (userData != null)
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

                if (userData.PasswordHash.Trim().Equals(passwordEncode.Trim())) return new Response<bool>(true)
                {
                    Message = "Mật khẩu trùng khớp"
                };
            }
            return new Response<bool>(false)
            {
                StatusCode = 400,
                Message = "Sai mật khẩu"
            };
        }

        private async Task<bool> CheckTimeVerifyEmail(int userId)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            var s = (DateTime.Now - userData.TokenGenerationTime);
            if (s.Value.Minutes > 5) return false;
            return true;
        }
        
        private async Task<bool> CheckTimeVerifyPassword(int userId)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
            var s = (DateTime.Now - userData.RecoveryTokenTime);
            if (s.Value.Minutes > 5) return false;
            return true;
        }

        public async Task<Response<TokenVerifyResponse>> CreateTokenVerifyPassword(int userId)
        {
            try
            {
                var userLogin = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
                if (userLogin != null)
                {
                    if (userLogin.EmailValidationStatusId != 3)
                    {
                        return new Response<TokenVerifyResponse>()
                        {
                            StatusCode = 400,
                            Message = "Bạn chưa xác thực email"
                        };
                    }

                    userLogin.PasswordRecoveryToken = KeyGenerator.GetUniqueKey(6);
                    userLogin.RecoveryTokenTime = DateTime.Now;
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        var message = new MessageModel(new string[] { userLogin.EmailAddressRecovery }, "Mã xác thực mật khẩu", "\n Mã xác thực có hiệu lực 5 phút, không chia sẻ mã xác thực với bất kì ai"+ userLogin.PasswordRecoveryToken);
                        await _senderService.SendEmail(message);

                        return new Response<TokenVerifyResponse>(new TokenVerifyResponse()
                        {
                            Token = userLogin.PasswordRecoveryToken,
                            CreateAt = userLogin.RecoveryTokenTime
                        })
                        {
                            Message = "Đã gửi mã xác thực thành công"
                        };
                    }
                }
                return new Response<TokenVerifyResponse>()
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy tài khoản này"
                };
            }
            catch (Exception)
            {
                return new Response<TokenVerifyResponse>()
                {
                    StatusCode = 400,
                    Message = "Gửi mã xác thực thất bại"
                };
            }
        }

        private async Task<bool> CheckVerifyPassword(int userId, string token)
        {
            try
            {
                var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);
                if(userData != null){
                    if (userData.PasswordRecoveryToken == token) return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Response<bool>> UpdateEmail(int userId, string email)
        {
            var userData = await _context.UserLoginData.FirstOrDefaultAsync(x => x.UserAccountId == userId);

            int EmailValidationStatusId;
            if (userData.EmailAddressRecovery == null )
            {
                EmailValidationStatusId = 1;
            }
            else if(email.Trim() == null)
            {
                EmailValidationStatusId = 1;
            }
            else if (userData.EmailAddressRecovery.Equals(email.Trim()) && userData.EmailValidationStatusId == 3)
            {
                EmailValidationStatusId = 3;
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
                return new Response<bool>(true)
                {
                    Message = "Cập nhật email thành công"
                };
            }
            return new Response<bool>(false)
            {
                StatusCode = 400,
                Message = "Cập nhật email thấy bại"
            };
        }

        public async Task<PageResult<CustomerViewModel>> SearchCustomer(CustomerPagingRequest request)
        {
            var query = from c in _context.Customers
                        where c.PhoneNumber.Contains(request.PhoneNumber)
                        select c;
            var data = query.Distinct();
            var result = await data.Select(c => new CustomerViewModel()
            {
                Id = c.Id,
                FullName = c.FullName,
                PhoneNumber = c.PhoneNumber,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive
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

        public async Task<Response<CustomerViewModel>> GetCustomerProfile(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (customer != null)
            {
                CustomerViewModel model = new CustomerViewModel()
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    TotalPoint = customer.TotalPoint,
                    CreatedAt = customer.CreatedAt,
                    CreatedBy = customer.CreatedBy,
                    UpdatedAt = customer.UpdatedAt,
                    UpdatedBy = customer.UpdatedBy,
                    IsActive = customer.IsActive
                };

                return new Response<CustomerViewModel>(model)
                {
                    Message = "Thông tin khách hàng"
                };
            }
            return new Response<CustomerViewModel>(null)
            {
                StatusCode = 400,
                Message = "Không tìm thấy khách hàng này"
            };
        }

        public async Task<Response<bool>> UpdateManagerProfile(int userId, UpdateManagerModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
                var updateEmail = await UpdateEmail(userId, model.Email);
                if (updateEmail.StatusCode == 400)
                {
                    return updateEmail;
                }
                if (user != null)
                {
                    user.FullName = model.FullName;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật thông tin thành công"
                    };
                }
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy tài khoản này"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy tài khoản này"
                };
            }
            
        }

        public async Task<Response<bool>> UpdateStaffProfile(int userId, UpdateStaffBaseModel model)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
                var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserAccountId == userId);
                var checkPhoneNumber = await CheckPhoneNumber(userId, model.PhoneNumber);
                if (checkPhoneNumber.Data != true) return checkPhoneNumber;
                string avatar;
                if (model.Avartar == null)
                {
                    avatar = defaultAvatar;
                }
                else
                {
                    avatar = model.Avartar;
                }
                if (user != null && staff != null)
                {
                    user.FullName = model.FullName;
                    staff.UrlAvartar = avatar;
                    staff.IsMale = model.IsMale;
                    staff.DateOfBirth = model.DateOfBirth;
                    staff.PhoneNumber = model.PhoneNumber;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<bool>(true)
                    {
                        Message = "Cập nhật nhân viên thành công"
                    };
                }
                else
                {
                    return new Response<bool>(true)
                    {
                        Message = "Nhân viên không tồn tại"
                    };
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Cập nhật nhân viên không thành công"
                };
            }
            
        }

        private async Task<Response<bool>> CheckPhoneNumber (int userId, string phoneNumber)
        {
            var u = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);
            if (u == null)
            {
                return new Response<bool>(false)
                {
                    StatusCode = 400,
                    Message = "Không có nhân viên này"
                };
            }
            var users = await _context.Staffs.ToListAsync();
            foreach(var user in users)
            {
                if (user.PhoneNumber.Equals(phoneNumber) && user.UserAccountId != userId)
                {
                    return new Response<bool>(false)
                    {
                        StatusCode = 400,
                        Message = "Số điện thoại đã có người sử dụng"
                    };
                }
            }
            
            return new Response<bool>(true)
            {
            };
        }
    }
}
