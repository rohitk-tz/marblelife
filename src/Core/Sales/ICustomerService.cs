using Core.Geo.ViewModel;
using Core.MarketingLead.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Sales
{
    public interface ICustomerService
    {
        bool DoesCustomerExists(string email);
        Customer SaveCustomer(CustomerCreateEditModel model);
        IEnumerable<Customer> GetCustomerByEmail(IList<string> emails, List<Customer> customerRepository);
        IEnumerable<Customer> GetCustomerByPhone(string phone, List<Customer> customerRepository);
        CustomerListModel GetCustomers(CustomerListFilter filter, int pageNumber, int pageSize);
        bool DownloadCustomerFile(CustomerListFilter filter, out string fileName);
        Customer GetCustomerByNameAndAddress(string name, AddressEditModel address, List<Customer> customerRepository);
        void Save(CustomerFileUploadCreateModel model);
        CustomerEditModel Get(long id);
        void Save(CustomerEditModel customerEditModel);
        bool UpdateMarketingClass(long id, long classTypeId);
    }
}
