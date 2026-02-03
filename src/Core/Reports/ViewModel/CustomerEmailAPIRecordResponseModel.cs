namespace Core.Reports.ViewModel
{
    public class CustomerEmailAPIRecordResponseModel
    {
        public string Id { get; set; } 
        public string email_address { get; set; }
        public string unique_email_id { get; set; }
        public string status { get; set; }
        public string timestamp_opt { get; set; }
        public string list_id { get; set; }
        public CustomerInfoModel merge_fields { get; set; }

        public CustomerEmailAPIRecordResponseModel()
        {
            merge_fields = new CustomerInfoModel();
        }
        public bool IsFailed { get; set; } 
        public string title { get; set; }
        public string detail { get; set; }
    }
}
