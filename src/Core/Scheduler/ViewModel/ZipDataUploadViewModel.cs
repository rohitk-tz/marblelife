using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class ZipDataUploadViewModel
    {
        public long Id { get; set; }
        public DateTime UploadedOn { get; set; }
        public string Status { get; set; }
        public long StatusId { get; set; }
        public string UploadedBy { get; set; }
        public string Notes { get; set; }
        public bool IsEditable { get; set; }
        public string TempNotes { get; set; }
        public long FileId { get; set; }
        public long LogFileId { get; set; }
        public long LogForCountyFileId { get; set; }
        public long LogForZipFileId { get; set; }
        public string CountiesCount { get; set; }
        public string ZipCodeCount { get; set; }
    }


}
