using System.Threading.Tasks;
using UtNhanDrug_BE.Models.TokenModel;
using UtNhanDrug_BE.Models.UserModel;

namespace UtNhanDrug_BE.Services.AuthenticationService
{
    public interface IAuthenticationSvc
    {
        //Task<AccessTokenModel> Authenticate(string accessToken);
        Task<AccessTokenModel> Authenticate(LoginModel model);

    }
}
