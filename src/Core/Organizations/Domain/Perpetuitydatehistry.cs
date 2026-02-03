
using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class Perpetuitydatehistry : DomainBase
    {
        public DateTime LastDateChecked { get; set; }
        public long? FranchiseeId { get; set; }
        public bool? IsPerpetuity { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    }
}
