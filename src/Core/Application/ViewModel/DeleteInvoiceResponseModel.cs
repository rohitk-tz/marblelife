namespace Core.Application.ViewModel
{
    public class DeleteInvoiceResponseModel
    {
        public string Response { get; set; }
        public bool IsLastItem { get; set; }
        public bool IsSuccess { get; set; } 
        public bool IsStatusChanged { get; set; }
    }
}
