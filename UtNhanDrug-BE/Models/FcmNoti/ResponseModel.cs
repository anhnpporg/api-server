using Newtonsoft.Json;

namespace UtNhanDrug_BE.Models.FcmNoti
{
    public class ResponseModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
