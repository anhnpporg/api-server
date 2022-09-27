﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.UserModel;
using System.Linq;
using System.Collections.Generic;
using UtNhanDrug_BE.Models.ManagerModel;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public class ManagerSvc : IManagerSvc
    {
        private readonly utNhanDrugStoreManagementContext _context;
        private const string defaultAvatar = "https://firebasestorage.googleapis.com/v0/b/utnhandrug.appspot.com/o/image-profile.png?alt=media&token=928ea13d-d43f-4c0e-a8ba-ab1999059530";
        public ManagerSvc(utNhanDrugStoreManagementContext context)
        {
            _context = context;
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

    }
}
