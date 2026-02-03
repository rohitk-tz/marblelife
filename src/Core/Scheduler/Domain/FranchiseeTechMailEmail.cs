using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Users.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    // This is for Franchisee Phone Charges Table Need to change table name for this.
  public  class FranchiseeTechMailEmail : DomainBase
    {
        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }


        public long? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }


        public double? ChargesForPhone { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long? CallCount { get; set; }
        public DateTime? DateForCharges { get; set; }

       

    }
}
