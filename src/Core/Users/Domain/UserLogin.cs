using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Users.Domain
{
    public class UserLogin : DomainBase
    {
        [ForeignKey("Person")]
        public override long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public int? LoginAttemptCount { get; set; }
        public DateTime? LastLoggedInDate { get; set; }

        public string ResetToken { get; set; }
        public DateTime? ResetTokenIssueDate { get; set; }

        public double? ESTOffset { get; set; }

        public double? EDTOffset { get; set; }

        public virtual Person Person { get; set; }

        public const int MaxAttempts = 5;
        public UserLogin()
        {
            IsActive = true;
        }
    }
}
