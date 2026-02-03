using Core.Application.Attribute;
using Core.Billing.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeLoanCompletionNotificationModel
    {
        public string FullName { get; set; }
        public string Franchisee { get; set; }
        public string LoanAmount { get; set; }
        public string Description { get; set; }
        public DateTime LoanCreatedOn { get; set; }
        public string LoanTensure { get; set; }
        public string LoanId { get; set; }

        public EmailNotificationModelBase Base { get; private set; }
        public FranchiseeLoanCompletionNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
