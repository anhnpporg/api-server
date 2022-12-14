using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.PagingModel;
using UtNhanDrug_BE.Models.ResponseModel;
using UtNhanDrug_BE.Models.StaffModel;
using UtNhanDrug_BE.Models.UserLoginModel;
using UtNhanDrug_BE.Models.UserModel;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public interface IUserSvc
    {
        Task<Response<List<ManagerViewModel>>> GetManagers();
        Task<Response<List<ViewStaffModel>>> GetStaffs();
        Task<Response<List<CustomerViewModel>>> GetCustomers();
        Task<Response<bool>> BanAccount(int Userid);
        Task<Response<bool>> UnBanAccount(int UserId);
        Task<Response<bool>> UpdateStaffProfile(int userId, UpdateStaffModel model);
        Task<Response<bool>> UpdateStaffProfile(int userId, UpdateStaffBaseModel model);
        Task<Response<bool>> ChangePassword(ChangePasswordModel model);
        Task<Response<bool>> ChangePasswordByUserId(int userId, ChangePasswordByIdModel model);
        Task<Response<Customer>> CreateCustomer(int UserId, CreateCustomerModel model);
        Task<Response<bool>> CreateStaff(CreateStaffModel model);
        Task<Response<object>> GetUserProfile(int userId);
        Task<Response<TokenVerifyResponse>> CreateTokenVerifyEmail(int userId);
        Task<Response<bool>> CheckTokenVerifyEmail(int userId, TokenVerifyModel model);
        Task<Response<TokenVerifyResponse>> CreateTokenVerifyPassword(ForgotPasswordModel request);
        Task<Response<bool>> UpdateEmail(int userId, string email);
        Task<Response<CustomerViewModel>> GetCustomerProfile(int id);
        Task<PageResult<CustomerViewModel>> SearchCustomer(CustomerPagingRequest request);
        Task<Response<bool>> UpdateManagerProfile(int userId, UpdateManagerModel model);
        //Task<Response<bool>> CheckVerifyPassword(int userId, string token);
        //Task<Response<bool>> CheckTimeVerifyPassword(int userId);
        Task<Response<bool>> CheckPassword(int userId, string password);
        //Task<Response<bool>> CheckTimeVerifyEmail(int userId);
        //Task<Response<bool>> CheckUser(int userId);
        Task<Response<string>> RecoveryPassword(int userId);
        //Task<bool> UpdateManagerProfile(int userId, UpdateManagerModel model);
        //Task<Response<ForgotPasswordResponse>> GetUserForgotPassword(ForgotPasswordModel request);
    }
}
