using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeDocumentListModel
    {
        public List<FranchiseeDocumentViewModel> FranchiseeViewModel { get; set; }
        public List<string> DocumentList { get; set; }
        public List<string> ColumnList { get; set; }
    }

    public class FranchiseeDocumentViewModel
    {
        public FranchiseeDocumentViewModel()
        {
            IsPresent = new List<bool>();
            DocumentStatusViewModel = new List<DocumentStatusViewModel>();
            IsDeclined = new List<bool>();
            ExpiryDate= new List<DateTime?>();
            IsPerpetuity = new List<bool>();
        }
        public List<DateTime?> ExpiryDate { get; set; }
        public string FranchiseeName { get; set; }
        public List<bool> IsPresent { get; set; }
        public List<bool> IsDeclined { get; set; }
        public List<bool> IsPerpetuity { get; set; }
        public List<DocumentStatusViewModel> DocumentStatusViewModel { get; set; }
    }
    public class DocumentStatusViewModel
    {
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public string UserName { get; set; }
    }

}
