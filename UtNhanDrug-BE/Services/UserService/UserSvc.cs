using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.UserModel;
using System.Linq;
using System.Collections.Generic;
using UtNhanDrug_BE.Models.ManagerModel;
using AutoMapper;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.StaffModel;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public class UserSvc : IUserSvc
    {
        private readonly utNhanDrugStoreManagementContext _context;
        private readonly IMapper _mapper;
        private const string defaultAvatar = "https://firebasestorage.googleapis.com/v0/b/utnhandrug.appspot.com/o/image-profile.png?alt=media&token=928ea13d-d43f-4c0e-a8ba-ab1999059530";
        public UserSvc(utNhanDrugStoreManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> BanAccount(int UserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == UserId);
            if (user != null)
            {
                user.IsBan = true;
                user.BanDate = System.DateTime.Now;
            }
            else
            {
                return -1;
            }

            return await _context.SaveChangesAsync();

        }

        public async Task<int> UnBanAccount(int UserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == UserId);
            if (user != null)
            {
                user.IsBan = false;
                user.BanDate = System.DateTime.Now;
            }
            else
            {
                return -1;
            }

            return await _context.SaveChangesAsync();

        }

        public async Task<bool> CreateAccount(string email, string fullname)
        {
            var exitsEmail = await _context.Managers.FirstOrDefaultAsync(x => x.Email == email);
            if (exitsEmail == null)
            {
                var user = new User()
                {
                    IsBan = false,
                    CreateDate = DateTime.Now,
                    Avatar = defaultAvatar,
                    Fullname = fullname
                };
                _context.Users.Add(user);
                var isSavedUser = await _context.SaveChangesAsync();
                if (isSavedUser != 0)
                {
                    var manager = new Manager()
                    {
                        Email = email,
                        UserId = user.Id

                    };
                    _context.Managers.Add(manager);
                    var isSavedStaff = await _context.SaveChangesAsync();
                    if (isSavedStaff != 0) return true;
                }
            }
            return false;
        }

        public async Task<PageResult<ManagerViewModel>> GetManagers(PagingModel paging)
        {
            var query = from m in _context.Managers
                        join u in _context.Users on m.UserId equals u.Id
                        select new { u, m };

            //filter

            if (!string.IsNullOrEmpty(paging.Keyword))
                query = query.Where(x => x.u.Fullname.Contains(paging.Keyword));
            //paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((paging.PageIndex - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .Select(x => new ManagerViewModel()
                {
                    Email = x.m.Email,
                    IsAdmin = x.m.IsAdmin,
                    User = x.u,
                }).ToListAsync();

            // select and projection
            var pagedResult = new PageResult<ManagerViewModel>()
            {
                TotalRecord = totalRow,
                PageSize = paging.PageSize,
                PageIndex = paging.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<Manager> IsExitsAccount(string email)
        {
            return await _context.Managers.FirstOrDefaultAsync(manager => manager.Email == email);
        }

        public async Task<bool> UpdateProfile(int userId, UpdateUserModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                user = _mapper.Map<User>(model);
                var isSaved = await _context.SaveChangesAsync();
                if (isSaved != 0) return true;
            }

            return false;
        }

        public async Task<bool> CreateCustomer(string phoneNumber, string fullName)
        {
            var customer = await FindByPhoneNumber(phoneNumber);

            if (customer == null)
            {
                var user = new User()
                {
                    IsBan = false,
                    CreateDate = DateTime.Now,
                    Avatar = defaultAvatar,
                    Fullname = fullName
                };
                _context.Users.Add(user);
                var isSavedUser = await _context.SaveChangesAsync();
                if (isSavedUser != 0)
                {
                    var cus = new Customer()
                    {
                        PhoneNumber = phoneNumber,
                        UserId = user.Id

                    };
                    _context.Customers.Add(cus);
                    var isSavedStaff = await _context.SaveChangesAsync();
                    if (isSavedStaff != 0) return true;
                }
            }
            return false;
        }

        public async Task<Customer> FindByPhoneNumber(string phoneNumber)
        {
            CustomerExitsModel model = new CustomerExitsModel();
            Customer customer = _mapper.Map<Customer>(model);
            customer = await _context.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            return customer;
        }

        public async Task<PageResult<CustomerViewModel>> GetCustomers(PagingModel paging)
        {
            var query = from c in _context.Customers
                        join u in _context.Users on c.UserId equals u.Id
                        select new { u, c };

            //filter

            if (!string.IsNullOrEmpty(paging.Keyword))
                query = query.Where(x => x.u.Fullname.Contains(paging.Keyword));

            //paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((paging.PageIndex - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .Select(x => new CustomerViewModel()
                {
                    PhoneNumber = x.c.PhoneNumber,
                    User = x.u
                }).ToListAsync();

            // select and projection
            var pagedResult = new PageResult<CustomerViewModel>()
            {
                TotalRecord = totalRow,
                PageSize = paging.PageSize,
                PageIndex = paging.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<bool> CreateStaff(string email, string fullname)
        {
            var exitsEmail = await _context.Staffs.FirstOrDefaultAsync(x => x.Email == email);
            if (exitsEmail == null)
            {
                var user = new User()
                {
                    IsBan = false,
                    CreateDate = DateTime.Now,
                    Avatar = defaultAvatar,
                    Fullname = fullname
                };
                _context.Users.Add(user);
                var isSavedUser = await _context.SaveChangesAsync();
                if (isSavedUser != 0)
                {
                    var staff = new Staff()
                    {
                        UserId = user.Id,
                        Email = email
                    };
                    _context.Staffs.Add(staff);
                    var isSavedStaff = await _context.SaveChangesAsync();
                    if (isSavedStaff != 0) return true;
                }
            }


            return false;
        }

        public async Task<PageResult<ViewStaffModel>> GetStaffs(PagingModel paging)
        {
            var query = from s in _context.Staffs
                        join u in _context.Users on s.UserId equals u.Id
                        select new { u, s };

            //filter

            if (!string.IsNullOrEmpty(paging.Keyword))
                query = query.Where(x => x.u.Fullname.Contains(paging.Keyword));

            //paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((paging.PageIndex - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .Select(x => new ViewStaffModel()
                {
                    Email = x.s.Email,
                    User = x.u
                }).ToListAsync();

            // select and projection
            var pagedResult = new PageResult<ViewStaffModel>()
            {
                TotalRecord = totalRow,
                PageSize = paging.PageSize,
                PageIndex = paging.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<Staff> IsExitsStaff(string email)
        {
            return await _context.Staffs.FirstOrDefaultAsync(staff => staff.Email == email);
        }

        public async Task<UserViewModel> GetManager(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user != null & manager != null)
            {
                UserViewModel model = new UserViewModel()
                {
                    Avatar = user.Avatar,
                    BanDate = user.BanDate,
                    CreateDate = user.CreateDate,
                    DateOfBirth = user.DateOfBirth,
                    Email = manager.Email,
                    Fullname = user.Fullname,
                    GenderId = user.GenderId,
                    IsBan = user.IsBan,
                    PhoneNumber = user.PhoneNumber,
                    UpdateDate = user.UpdateDate
                };
                return model;
            }
            return null;
        }
        
        public async Task<UserViewModel> GetCustomer(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user != null & customer != null)
            {
                UserViewModel model = new UserViewModel()
                {
                    Avatar = user.Avatar,
                    BanDate = user.BanDate,
                    CreateDate = user.CreateDate,
                    DateOfBirth = user.DateOfBirth,
                    Email = "",
                    Fullname = user.Fullname,
                    GenderId = user.GenderId,
                    IsBan = user.IsBan,
                    PhoneNumber = user.PhoneNumber,
                    UpdateDate = user.UpdateDate
                };
                return model;
            }
            return null;
        }
        
        public async Task<UserViewModel> GetStaff(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user != null & staff != null)
            {
                UserViewModel model = new UserViewModel()
                {
                    Avatar = user.Avatar,
                    BanDate = user.BanDate,
                    CreateDate = user.CreateDate,
                    DateOfBirth = user.DateOfBirth,
                    Email = staff.Email,
                    Fullname = user.Fullname,
                    GenderId = user.GenderId,
                    IsBan = user.IsBan,
                    PhoneNumber = user.PhoneNumber,
                    UpdateDate = user.UpdateDate
                };
                return model;
            }
            return null;
        }

        public async Task<UserViewModel> GetUserProfile(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            var manager = await _context.Managers.FirstOrDefaultAsync(x => x.UserId == userId);
            var staff = await _context.Staffs.FirstOrDefaultAsync(x => x.UserId == userId);
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);

            UserViewModel u = new UserViewModel()
            {
                Fullname = user.Fullname,
                Avatar = user.Avatar,
                PhoneNumber = user.PhoneNumber,
                UpdateDate = user.UpdateDate,
                CreateDate = user.CreateDate,
                DateOfBirth = user.DateOfBirth,
                IsBan = user.IsBan,
                BanDate = user.BanDate,
                GenderId = user.GenderId
            };

            if(manager != null)
            {
                u.Email = manager.Email;
                return u;
            }else if(staff != null)
            {
                u.Email = staff.Email;
                return u;
            }
            else if(customer != null)
            {
                u.PhoneNumber = customer.PhoneNumber;
                u.Email = "";
                return u;
            }

            return null;
        }
    }
}
