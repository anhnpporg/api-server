using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.UserModel;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public interface IManagerSvc
    {
        Task<PageResult<ManagerViewModel>> GetManagers(PagingModel paging);
        Task<bool> CreateAccount(string email, string fullname);
        Task<int> BanAccount(int Userid);
        Task<int> UnBanAccount(int UserId);
        Task<Manager> IsExitsAccount(string email);
    }
}
