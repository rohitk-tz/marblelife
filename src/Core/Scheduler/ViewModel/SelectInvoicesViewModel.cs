using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class SelectInvoicesViewModel
    {
        public long? TypeId { get; set; }
        public bool? IsFromJob { get; set; }
        public long? SchedulerId { get; set; }
        public long? UserId { get; set; }

        public long? LoggedInUserId { get; set; }

        public string Body { get; set; }
        public List<int> ServiceInvoice { get; set; }
        public List<FileModel> FileModel { get; set; }
        public long? IsDynamic { get; set; }
        public long? JobSchedulerId { get; set; }
        public long? EstimateInvoiceId { get; set; }
        public bool? IsCustomerAvailable { get; set; }
        public string Email { get; set; }
        public string ToEmailId { get; set; }
        public string CCEmail { get; set; }
        public long CustomerId { get; set; }
    }

    [NoValidatorRequired]
    public class SendFeedBackMailToCustomerViewModel
    {
        public long? SchedulerId { get; set; }
        public long? EstimateInvoiceId { get; set; }
    }

    [NoValidatorRequired]
    public class SignatureViewModel
    {
        public string PreSignature { get; set; }
        public string PostSignature { get; set; }
        public string PreSignatureDate { get; set; }
        public string PostSignatureDate { get; set; }
        public string IsSigned { get; set; }
        public string Technician { get; set; }
    }

    [NoValidatorRequired]
    public class CustomerAvailableViewModel
    {
        public long? EstimateInvoiceId { get; set; }
        public bool? IsCustomerAvailable { get; set; }
    }
}
