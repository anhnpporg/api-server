﻿namespace UtNhanDrug_BE.Models.UserLoginModel
{
    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string TokenRecovery { get; set; }
    }
}