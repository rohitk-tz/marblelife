using Core.Application.Attribute;
using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
  public  class OnetimeprojectfeeAddFundRoyality : DomainBase
    {
        public long FranchiseeId { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Organization Organization { get; set; }

        public bool IsInRoyality { get; set; }
        public bool IsSEOInRoyalty { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    }
}
