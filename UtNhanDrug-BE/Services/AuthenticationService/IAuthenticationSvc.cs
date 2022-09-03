namespace UtNhanDrug_BE.Services.AuthenticationService
{
    public interface IAuthenticationSvc
    {
        string Authenticate(string accessToken);
    }
}
