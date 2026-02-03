using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class InvoiceLineImageModel
    {
        public long? FileId { get; set; }
        public string RelativeLocation { get; set; }

        public bool? IsIFrame { get; set; }

        public string IFrameLocation { get; set; }
    }
}
