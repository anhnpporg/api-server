using Newtonsoft.Json;

namespace UtNhanDrug_BE.Models.TwilioOTP
{
    public class ErrorResponseModel
    {
        [JsonProperty("isSuccess")]
        public string IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

    }
}
