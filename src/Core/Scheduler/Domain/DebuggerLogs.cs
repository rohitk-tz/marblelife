using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
   public class DebuggerLogs : DomainBase
    {
        public string Description { get; set; }



        public long? FranchiseeId { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long? PageId { get; set; }

        [ForeignKey("PageId")]
        public virtual Lookup Page { get; set; }
        public long? ActionId { get; set; }

        [ForeignKey("ActionId")]
        public virtual Lookup Action { get; set; }
        public long? JobestimateservicesId { get; set; }

        [ForeignKey("JobestimateservicesId")]
        public virtual JobEstimateServices JobEstimateServices { get; set; }
        public long? JobestimateimagecategoryId { get; set; }

        [ForeignKey("JobestimateimagecategoryId")]
        public virtual JobEstimateImageCategory JobEstimateImageCategory { get; set; }
        public long? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }
        public long? TypeId { get; set; }

        [ForeignKey("TypeId")]
        public virtual Lookup Type { get; set; }

        public long? JobSchedulerId { get; set; }

        [ForeignKey("JobSchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }
    }
}
