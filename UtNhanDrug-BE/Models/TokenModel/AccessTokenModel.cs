﻿namespace UtNhanDrug_BE.Models.TokenModel
{
    public class AccessTokenModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public bool IsAdmin { get; set; }
    }
}
