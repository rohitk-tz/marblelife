using Core.Application.ViewModel;
using System;

namespace Core.Organizations.ViewModel
{
    public class DocumentViewModel
    {
        public FileModel FileModel { get; set; }
        public long Id { get; set; }
        public string FileName { get; set; }
        public string Caption { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string FranchiseeName { get; set; }
        public long FranchiseeId { get; set; }
        public long? FileId { get; set; }
        public DateTime UploadedOn { get; set; }
        public string UploadedBy { get; set; }
        public string Type { get; set; }
        public bool IsImportant { get; set; }
        public bool IsExpired { get; set; }
        public string DocumentType { get; set; }
        public long? DocumentTypeId { get; set; }
        public bool IsUploadedBySuperAdmin { get; set; }
        public string UserName { get; set; }
        public long? TypeId { get; set; }
        public bool? showToUser { get; set; }
        public long? uploadedByUserId { get; set; }
        public long? userId { get; set; }
        public string UploadFor { get; set; }
        public bool IsDateDefault { get; set; }
        public bool IsRejected { get; set; }
        public bool IsPerpetuity { get; set; }
    }
}
