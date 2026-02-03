using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Application.Domain
{
    public class File : DomainBase
    {
        public  string Name { get; set; }
        public  string Caption { get; set; }
        public  decimal Size { get; set; }
        public  string RelativeLocation { get; set; }
        public  string MimeType { get; set; }
        public string css { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        //public long? FileReferenceId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public  DataRecorderMetaData DataRecorderMetaData { get; set; }

        public bool? IsFileToBeDeleted { get; set; }

    }
}
