using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.StaffModel;

namespace UtNhanDrug_BE.Services.StaffService
{
    public class StaffService : IStaffService
    {
        private readonly utNhanDrugStoreManagementContext _context;

        public StaffService(utNhanDrugStoreManagementContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAccount(string email)
        {
            var exitsEmail = await _context.Staffs.FirstOrDefaultAsync(x => x.Email == email);
            if (exitsEmail == null)
            {
                var user = new User()
                {
                    IsBan = false,
                    CreateDate = DateTime.Now
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

            if (!string.IsNullOrEmpty(paging.keyword))
                query = query.Where(x => x.u.Fullname.Contains(paging.keyword));

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
    }
}
