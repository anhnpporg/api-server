using System.Threading.Tasks;
using UtNhanDrug_BE.Models.UserModel;

namespace UtNhanDrug_BE.Services.UserService
{
    public interface IUserService
    {
        Task<bool> UpdateProfile(int userId, UpdateUserModel model);
        Task<bool?> DeleteAccount(int userId);
    }
}
