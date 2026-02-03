using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeDocumentEditModel : EditModelBase
    {
        public ICollection<long> FranchiseeIds { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public FileModel FileModel { get; set; } 
        public bool IsImportant { get; set; }
        public bool ShowToUser { get; set; }
        public long FranchiseeId { get; set; }
        public long? DocumentTypeId { get; set; }
        public long? UserId { get; set; }
        public long Id { get; set; }
        public bool IsFromUser { get; set; }
        public string UploadFor { get; set; }
        public bool IsRejected { get; set; }
        public bool IsPerpetuity { get; set; }
    }
}
