using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.StaffModel;
using UtNhanDrug_BE.Models.UserModel;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public interface IUserSvc
    {
        Task<List<ManagerViewModel>> GetManagers();
        Task<List<ViewStaffModel>> GetStaffs();
        Task<List<CustomerViewModel>> GetCustomers();
        Task<int> BanAccount(int Userid);
        Task<int> UnBanAccount(int UserId);
        Task<bool> UpdateStaffProfile(int userId, UpdateStaffModel model);
        //Task<bool> UpdateManagerProfile(int userId, UpdateManagerModel model);
        Task<bool> CreateCustomer(CreateCustomerModel model);
        Task<bool> CreateStaff(CreateStaffModel model);
        Task<object> GetUserProfile(int userId);
        Task<bool> CheckUser(int userId);
        Task<bool> RecoveryPassword(int userId, RecoveryPasswordModel model);

    }
}
