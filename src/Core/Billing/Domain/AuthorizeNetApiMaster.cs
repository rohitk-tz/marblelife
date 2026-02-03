namespace Core.Billing.Domain
{
    public class AuthorizeNetApiMaster:DomainBase
    {
        public string ApiLoginID { get; set; }

        public long AccountTypeId { get; set; }
        public string ApiTransactionKey { get; set; }
    }
}
