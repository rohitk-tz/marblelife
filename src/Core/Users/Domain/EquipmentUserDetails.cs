using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Domain
{
    public class EquipmentUserDetails : DomainBase
    {
        public long UserId { get; set; }
        public bool IsActive { get; set; }
        public bool IsLock { get; set; }
        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }
    }
}
