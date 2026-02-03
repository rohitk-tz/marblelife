using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
   public class AttachInvoiceWithJobViewModel
    {
        public DateTime UploadDateTime { get; set; }
        public long? JobId { get; set; }
        public long? QbInvoiceId { get; set; }
    }
}
