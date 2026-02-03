using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Core.ToDo.Domain
{
    public class ToDoFollowUpList : DomainBase
    {
        public DateTime Date { get; set; }

        public string Comment { get; set; }
        public string Task { get; set; }
        public long StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Lookup Lookup { get; set; }
        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }

        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual JobCustomer Customer { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerName { get; set; }

        public long? TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual LookupType LookupType { get; set; }

        public long? SchedulerId { get; set; }
        [ForeignKey("SchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }


        public long? TaskChoiceId { get; set; }
        [ForeignKey("TaskChoiceId")]
        public virtual Lookup TaskChoice { get; set; }

        public long? DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        public ToDoFollowUpList()
        {
        }
    }
}
