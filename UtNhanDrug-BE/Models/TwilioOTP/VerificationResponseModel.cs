using Newtonsoft.Json;

namespace UtNhanDrug_BE.Models.TwilioOTP
{
    public class VerificationResponseModel
    {
        [JsonProperty("isSuccess")]
        public string IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }

    }
}
