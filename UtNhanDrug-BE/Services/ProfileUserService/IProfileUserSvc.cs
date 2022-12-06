using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static UtNhanDrug_BE.Models.ProfileUserModel.ProfileUserViewModel;

namespace UtNhanDrug_BE.Services.ProfileUserService
{
    public interface IProfileUserSvc
    {
        Task<ProfileUser> GetProfileUser(int? userAccountId);
    }
}
