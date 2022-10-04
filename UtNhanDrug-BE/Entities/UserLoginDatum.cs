using System;
using System.Collections.Generic;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class UserLoginDatum
    {
        public int Id { get; set; }
        public int UserAccountId { get; set; }
        public string LoginName { get; set; }
        public string PasswordHash { get; set; }
        public int HashingAlgorithmId { get; set; }
        public string EmailAddressRecovery { get; set; }
        public string ConfirmationToken { get; set; }
        public DateTime? TokenGenerationTime { get; set; }
        public int EmailValidationStatusId { get; set; }
        public string PasswordRecoveryToken { get; set; }
        public DateTime? RecoveryTokenTime { get; set; }

        public virtual EmailValidationStatus EmailValidationStatus { get; set; }
        public virtual HashingAlgorithm HashingAlgorithm { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
