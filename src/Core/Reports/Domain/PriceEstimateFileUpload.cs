using Core.Application.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
    public class PriceEstimateFileUpload : DomainBase
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

        public string Notes { get; set; }

        public long? ParsedLogFileId { get; set; }
        [ForeignKey("ParsedLogFileId")]
        public virtual File LogFile { get; set; }
        public bool IsFranchiseeAdmin { get; set; }

        //public long? ParsedLogForCountyFileId { get; set; }
        //[ForeignKey("ParsedLogForCountyFileId")]
        //public virtual File LogFileForCountyZilFile { get; set; }
        //public long? ParsedLogForZipFileId { get; set; }
        //[ForeignKey("ParsedLogForCountyFileId")]
        //public virtual File ParsedLogForZipFile { get; set; }
        //public long CountiesCount { get; set; }
        //public long ZipCodeCount { get; set; }
    }

}
