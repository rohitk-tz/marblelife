namespace Core.Review.ViewModel
{
    public class ReviewAPIResponseModel
    {
        public string businessId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string jobId { get; set; }
        public string CustomId { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public long CustomerId { get; set; }
        public long ReviewSystemRecordId { get; set; }
        public string DataPacket { get; set; }
        public bool IsvalidName { get; set; }
        public bool IsQueued { get; set; }  
    }
}
