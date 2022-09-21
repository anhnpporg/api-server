using UtNhanDrug_BE.Models.TokenModel;

namespace UtNhanDrug_BE.Services.AuthenticationService
{
    public interface IAuthenticationSvc
    {
        AccessTokenModel AuthenticateManager(string accessToken);
        AccessTokenModel AuthenticateStaff(string accessToken);
    }
}
