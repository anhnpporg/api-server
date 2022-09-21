using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Hepper.Paging;
using UtNhanDrug_BE.Models.ManagerModel;
using UtNhanDrug_BE.Models.StaffModel;

namespace UtNhanDrug_BE.Services.StaffService
{
    public interface IStaffService
    {
        Task<PageResult<ViewStaffModel>> GetStaffs(PagingModel paging);
        Task<bool> CreateAccount(string email);
        Task<Staff> IsExitsAccount(string email);

    }
}
