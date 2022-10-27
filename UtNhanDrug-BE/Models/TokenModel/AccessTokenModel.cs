namespace UtNhanDrug_BE.Models.TokenModel
{
    public class AccessTokenModel
    {
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public bool IsAdmin { get; set; }
    }
}
