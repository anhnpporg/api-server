using System.Collections.Generic;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.CustomerModel;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.StaffModel;
using UtNhanDrug_BE.Models.UserModel;

namespace UtNhanDrug_BE.Services.ManagerService
{
    public interface IUserSvc
    {
        Task<PageResult<ManagerViewModel>> GetManagers(PagingModel paging);
        Task<bool> CreateAccount(string email, string fullname);
        Task<int> BanAccount(int Userid);
        Task<int> UnBanAccount(int UserId);
        Task<Manager> IsExitsAccount(string email);
        Task<bool> UpdateProfile(int userId, UpdateUserModel model);
        Task<PageResult<CustomerViewModel>> GetCustomers(PagingModel paging);
        Task<Customer> FindByPhoneNumber(string phoneNumber);
        Task<bool> CreateCustomer(string phoneNumber, string fullName);
        Task<PageResult<ViewStaffModel>> GetStaffs(PagingModel paging);
        Task<bool> CreateStaff(string email, string fullname);
        Task<Staff> IsExitsStaff(string email);
        Task<UserViewModel> GetStaff(int userId);
        Task<UserViewModel> GetCustomer(int userId);
        Task<UserViewModel> GetManager(int userId);
    }
}
