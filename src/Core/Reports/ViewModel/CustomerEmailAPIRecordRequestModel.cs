namespace Core.Reports.ViewModel
{
    public class CustomerEmailAPIRecordRequestModel
    {
        public string email_address { get; set; }
        public string status { get; set; }
        public CustomerInfoModel merge_fields { get; set; }

        public CustomerEmailAPIRecordRequestModel()
        {
            merge_fields = new CustomerInfoModel();
        }
    }
}
