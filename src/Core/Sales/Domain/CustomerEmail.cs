using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class CustomerEmail : DomainBase
    {
        public long CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual  Customer Customer { get; set; }

        public DateTime DateCreated { get; set; }

        public string Email { get; set; }
    }
}
