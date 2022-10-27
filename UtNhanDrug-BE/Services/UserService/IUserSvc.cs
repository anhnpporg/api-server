using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.PagingModel;
using UtNhanDrug_BE.Models.StaffModel;
using UtNhanDrug_BE.Models.UserLoginModel;
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
        Task<bool> ChangePassword(int userId, ChangePasswordModel model);
        //Task<bool> UpdateManagerProfile(int userId, UpdateManagerModel model);
        Task<bool> CreateCustomer(CreateCustomerModel model);
        Task<bool> CreateStaff(CreateStaffModel model);
        Task<object> GetUserProfile(int userId);
        Task<bool> CheckUser(int userId);
        Task<bool> RecoveryPassword(int userId, RecoveryPasswordModel model);
        Task<TokenVerifyResponse> CreateTokenVerifyEmail(int userId);
        Task<bool> CheckTokenVerifyEmail(int userId, TokenVerifyModel model);
        Task<int> CheckEmail(int userId);
        Task<bool> CheckPassword(int userId, string password);
        Task<bool> CheckTimeVerifyEmail(int userId);
        Task<TokenVerifyResponse> CreateTokenVerifyPassword(int userId);
        Task<bool> CheckVerifyPassword(int userId, string token);
        Task<bool> CheckTimeVerifyPassword(int userId);
        Task<bool> UpdateEmail(int userId, string email);
        Task<PageResult<CustomerViewModel>> SearchCustomer(CustomerPagingRequest request);

    }
}
