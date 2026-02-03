using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System;

namespace Core.Geo
{
    public interface IEmailFactory
    {
        EmailEditModel CreateEditModel(CustomerEmail domain, string email = null);
        CustomerEmail CreateDomain(EmailEditModel model, long id);
        CustomerEmail CreateDomain(CustomerEmail model, long id);
    }
}
