using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Geo.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System.Collections.Generic;

namespace Core.Billing
{
    public interface IAuditFactory
    {
        AuditInvoice CreateDomain(InvoiceEditModel model);
        AuditPayment CreateDomain(FranchiseeSalesPaymentEditModel model);
        AuditInvoicePayment CreateDomain(long invoiceId, long paymentId);
        Auditaddress CreateViewModel(AuditInvoice auditInvoice);
        InvoiceItemEditModel CreateViewModel(AuditInvoiceItem domain);
        SystemAuditRecord CreateDomain(FranchiseeSales franchiseeSales, AnnualSalesDataUpload annualUpload);
        Auditaddress CreateViewModel(SystemAuditRecord auditInvoice);

        AuditAddressDiscrepancy CreateViewModelAudit(CustomerCreateEditModel auditInvoice,long? id, bool isUpdated,string countryName,long annualSalesUploadId, long? marketingClassId = null);
        AddressHistryLog CreateViewModel(Customer customer, long? id, bool isUpdated, long? countryId, long annualSalesUploadId,long? classid,long? marketingClassId = null);
        Address CreateViewModel(CustomerCreateEditModel customer, long? id, bool isUpdated, Customer obj, long annualSalesUploadId);
        AnnualSalesDataCustomerViewModel CreateViewModelForCustomers(AuditAddressDiscrepancy auditInvoice, List<QbInvoiceList> qbInvoiceList);
        AnnualSalesDataCustomerViewModel CreateViewModelForCustomers(AddressHistryLog addressLog, List<QbInvoiceList> qbInvoiceList);
        CustomerEmail CreateDomain(Customer customer, string email);

        AuditAddress CreateViewModel(CustomerCreateEditModel auditInvoice);

        AuditCustomer CreateModel(CustomerCreateEditModel auditInvoice);
        InvoiceAddress CreateModel(CustomerCreateEditModel customer,long? invoiceId,long? invoiceAddressId);

        AuditFranchiseeSales CreateViewModel(ParsedFileParentModel model, long franchiseeId,long? customerId,long?auditCustomerId,long? accountCreditId=null);


    } 
}
