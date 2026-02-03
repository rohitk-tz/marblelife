using Core.Application.Attribute;
using Core.Application.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.Domain
{
    public class FranchiseeRegistrationHistry : DomainBase
    {

        public DateTime RegistrationDate{ get; set; }
        public long FranchiseeId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

    }
}
