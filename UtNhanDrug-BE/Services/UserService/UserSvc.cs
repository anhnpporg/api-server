﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.UserModel;
using System.Linq;
using System.Collections.Generic;
using UtNhanDrug_BE.Models.ManagerModel;
using AutoMapper;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.StaffModel;
using UtNhanDrug_BE.Hepper.HashingAlgorithms;
using UtNhanDrug_BE.Hepper;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public class UserSvc : IUserSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;
        private const string defaultAvatar = "https://firebasestorage.googleapis.com/v0/b/utnhandrug.appspot.com/o/image-profile.png?alt=media&token=928ea13d-d43f-4c0e-a8ba-ab1999059530";
        public UserSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
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

            if(user != null && staff != null)
            {
                user.FullName = model.Fullname;
                staff.Avatar = model.Avatar;
                staff.IsMale = model.IsMale;
                staff.DateOfBirth = model.Dob;
                staff.PhoneNumber = model.PhoneNumber;
                var result = await _context.SaveChangesAsync();
                if(result > 0) return true;
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
                    var userLoginData = await CreateUserLoginData(userId, model.LoginName, passwordEncode, hashingId, model.EmailAddressRecovery);
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

        private async Task<bool> CreateUserLoginData(int userId, string loginName, string passwordEncode, int hashAlgorithmsId, string emailAddressRecovery)
        {
            int emailValidationStatusId;
            if (emailAddressRecovery == null)
            {
                emailValidationStatusId = 1;
            }
            else
            {
                emailValidationStatusId = 2;
            }
            UserLoginDatum u = new UserLoginDatum()
            {
                UserAccountId = userId,
                LoginName = loginName,
                PasswordHash = passwordEncode,
                HashingAlgorithmId = hashAlgorithmsId,
                EmailAddressRecovery = emailAddressRecovery,
                EmailValidationStatusId = emailValidationStatusId,

            };
            _context.UserLoginData.Add(u);

            var result = await _context.SaveChangesAsync();
            if (result != 0) return true;
            return false;
        }

        public async Task<object> GetUserProfile(int userId)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);

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
    }
}
