using Core.Billing.ViewModel;
using Core.Sales.ViewModel;
using System;

namespace Core.Application.ViewModel
{
    public class ParsedFileParentModel
    {
        public string QbIdentifier { get; set; }

        public DateTime Date { get; set; }

        public long MarketingClassId { get; set; }

       public long? SubMarketingClassId { get; set; }

        public string SalesRep { get; set; }

        public long ServiceTypeId { get; set; }

        public CustomerCreateEditModel Customer { get; set; }

        public InvoiceEditModel Invoice { get; set; }

        public AccountCreditEditModel AccountCredit { get; set; }

        public long CustomerInvoiceId { get; set; }
        public string CustomerInvoiceIdString { get; set; }


        public ParsedFileParentModel()
        {
            Customer = new CustomerCreateEditModel();
            Invoice = new InvoiceEditModel();
        }
    }
}
