﻿using System;

namespace UtNhanDrug_BE.Models.UserLoginModel
{
    public class TokenVerifyResponse
    {
        public int? UserId { get; set; }

        public string Token { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
