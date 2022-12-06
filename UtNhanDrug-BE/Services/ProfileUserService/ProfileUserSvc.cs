using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.ProfileUserModel;
using static UtNhanDrug_BE.Models.ProfileUserModel.ProfileUserViewModel;

namespace UtNhanDrug_BE.Services.ProfileUserService
{
    public class ProfileUserSvc : IProfileUserSvc
    {
        private readonly ut_nhan_drug_store_databaseContext _context;

        public ProfileUserSvc(ut_nhan_drug_store_databaseContext context)
        {
            _context = context;
        }
        public async Task<ProfileUser> GetProfileUser(int? userAccountId)
        {
            if (userAccountId == null)
            {
                return null;
            }

            var userAccount = await _context.UserAccounts.FirstOrDefaultAsync(userAccount => userAccount.Id.Equals((int)userAccountId));
            var manager = await _context.Managers.FirstOrDefaultAsync(userAccount => userAccount.UserAccountId.Equals((int)userAccountId));

            return new ProfileUser
            {
                Id = userAccount.Id,
                FullName = userAccount.FullName,
                Role = manager != null ? "Quản lí" : "Nhân viên",
                IsActive = userAccount.IsActive,
                CreatedAt = userAccount.CreatedAt
            };
        }
    }
}
