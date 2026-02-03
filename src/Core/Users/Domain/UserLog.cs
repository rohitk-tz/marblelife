using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Users.Domain
{
    public class UserLog  :DomainBase
    {
        public long UserId { get; set; }
        public DateTime LoggedInAt { get; set; }
        public DateTime? LoggedOutAt { get; set; }
        public string SessionId { get; set; }
        public string DeviceKey { get; set; }
        public string ClientIp { get; set; }
        public string Browser { get; set; }
        public string OS { get; set; }
        public string UserAgent { get; set; }

        [ForeignKey("UserId")]
        public virtual UserLogin UserLogin { get; set; }
    }
}
