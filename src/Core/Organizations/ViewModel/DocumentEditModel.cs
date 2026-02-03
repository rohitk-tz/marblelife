using Core.Application.Attribute;
using System;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
   public class DocumentEditModel
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string Caption { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string FranchiseeName { get; set; }
        public long FranchiseeId { get; set; }
        public long FileId { get; set; }
        public DateTime UploadedOn { get; set; }
        public string UploadedBy { get; set; }
        public string Type { get; set; }
        public bool IsImportant { get; set; }
        public bool IsExpired { get; set; }
        public string DocumentType { get; set; }
        public long? DocumentTypeId { get; set; }
        public bool IsUploadedBySuperAdmin { get; set; }
    }
}
