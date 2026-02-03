using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    public class NewJobOrEstimateReminderNotificationModel
    {
        public string AssigneePhone { get; set; }
        public string AssigneeName { get; set; }
        public string Time { get; set; }
        public string EndTime { get; set; }
        public string FullName { get; set; }
        public string FranchiseeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TechName { get; set; }
        public string Address { get; set; }
        public string AddresLine1 { get; set; }
        public string AddresLine2 { get; set; }
        public string TechImageUrl { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public long OrganizationId { get; set; }
        public long? FileId { get; set; }
        public long? CustomerId { get; set; }
        public string FileName { get; set; }
        public string jobTitle { get; set; }
        public IList<TechListViewModel> TechList { get; set; }
        public string jobType { get; set; }
        public string jobTypeName { get; set; }
        public string TechsList { get; set; }
        public string Description { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string UserEmail { get; set; }
        public string recipientMail { get; set; }
        public string AssigneeEmail { get; set; }
        public string dateType { get; set; }
        public string techNames { get; set; }
        public string fromMail { get; set; }
        public string display { get; set; }
        public string IsDisplayVisible { get; set; }
        public string ConfirmUrl { get; set; }
        public string CancleUrl { get; set; }
        public string JobTitle { get; set; }
        public string LinkUrl { get; set; }
        public string Phone { get; set; }
        public string techDesignation { get; set; }
        public string linkUrl { get; set; }
        public string Date { get; set; }
        public string PersonName { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }

        public NewJobOrEstimateReminderNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
