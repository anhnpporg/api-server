namespace UtNhanDrug_BE.Services.AuthenticationService
{
    public interface IAuthenticationSvc
    {
        string AuthenticateManager(string accessToken);
        string AuthenticateStaff(string accessToken);
    }
}
