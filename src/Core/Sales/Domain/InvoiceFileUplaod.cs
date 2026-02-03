using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class InvoiceFileUpload : DomainBase
    {
        public long FileId { get; set; }
        public long StatusId { get; set; }
        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        [ForeignKey("StatusId")]
        public virtual Lookup Lookup { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }
    }
}
