using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.UserModel;

namespace UtNhanDrug_BE.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly utNhanDrugStoreManagementContext _context;
        private readonly IMapper _mapper;

        public UserService(utNhanDrugStoreManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<bool?> DeleteAccount(int userId)
        {
            throw new NotImplementedException();
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
    }
}
