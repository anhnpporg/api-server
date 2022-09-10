using Newtonsoft.Json;
using System;

namespace UtNhanDrug_BE.Models.TwilioOTP
{
    public class VerificationResponseModel
    {
        [JsonProperty("isSuccess")]
        public string IsSuccess { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("valid")]
        public Boolean Valid { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("date_create")]
        public DateTime Date_create { get; set; }
    }
}
