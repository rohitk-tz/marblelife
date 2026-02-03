using Core.Application.Domain;
using Core.Users.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
   public class MlfsConfigurationSetting : DomainBase
    {
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Status { get; set; }
        public string ColorCode { get; set; }
        public long? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }
        public bool IsActive { get; set; }
    }
}
