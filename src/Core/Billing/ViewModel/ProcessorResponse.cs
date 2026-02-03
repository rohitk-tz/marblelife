using Core.Billing.Enum;

namespace Core.Billing.ViewModel
{
    public class ProcessorResponse
    {
        public ProcessorResponseResult ProcessorResult { get; set; }
        public string Message { get; set; }
        public string RawResponse { get; set; }
        public long InstrumentId { get; set; }
        public string CustomerProfileId { get; set; }
    }
}
