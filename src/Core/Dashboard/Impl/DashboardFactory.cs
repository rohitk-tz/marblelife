using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Dashboard.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.Linq;

namespace Core.Dashboard.Impl
{
    [DefaultImplementation]
    public class DashboardFactory : IDashboardFactory
    {
        public FranchiseeDirectoryViewModel CreateViewModel(Organization domain)
        {
            return new FranchiseeDirectoryViewModel
            {
                Id = domain.Id,
                Email = domain.Email,
                Name = domain.Name,
                PhoneNumbers = domain.Phones.Select(x => new { x.Number, x.Lookup.Name }).ToList()
            };
        }

        public SalesSummaryViewModel CreateViewModel(SalesDataUpload domain)
        {
            return new SalesSummaryViewModel
            {
                Franchise = domain.Franchisee.Organization.Name,
                PeriodStartDate = domain.PeriodStartDate,
                PeriodEndDate = domain.PeriodEndDate,
                TotalCustomers = domain.NumberOfCustomers,
                TotalInvoices = domain.NumberOfInvoices,
                TotalSales = domain.TotalAmount,
                Received = domain.PaidAmount,
                Accrued = (domain.TotalAmount) - (domain.PaidAmount),
                FileId = domain.FileId,
                CurrencyRate = domain.CurrencyExchangeRate.Rate,
                CurrencyCode = domain.Franchisee.Currency
            };
        }

        public RecentInvoiceViewModel CreateViewModel(FranchiseeInvoice domain)
        {
            var itemTypeId = domain.Invoice.InvoiceItems.Select(x => x.ItemTypeId).First();
            var isLateFeeCharged = domain.Invoice.InvoiceItems.Any(x => x.ItemTypeId == (long)InvoiceItemType.LateFees);

            var startDate = DateTime.Now;
            var endDate = DateTime.Now;
            if (itemTypeId == (long)InvoiceItemType.LateFees)
            {
                startDate = domain.Invoice.InvoiceItems.Select(x => x.LateFeeInvoiceItem.StartDate).FirstOrDefault();
                endDate = domain.Invoice.InvoiceItems.Select(x => x.LateFeeInvoiceItem.EndDate).FirstOrDefault();
            }
            else
            {
                startDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodStartDate : DateTime.Now;
                endDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodEndDate : DateTime.Now;
            }
            return new RecentInvoiceViewModel
            {
                FranchiseeId = domain.Franchisee.Id,
                FranchiseName = domain.Franchisee.Organization.Name,
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = domain.SalesDataUpload != null ? domain.SalesDataUpload.TotalAmount : 0,
                PayableAmount = domain.Invoice.InvoiceItems.ToList().Sum(x => x.Amount),
                Status = domain.Invoice.Lookup.Name,
                GeneratedOn = domain.Invoice.GeneratedOn,
                InvoiceId = domain.Invoice.Id,
                CurrencyRate = domain.Invoice.InvoiceItems.Select(x => x.CurrencyExchangeRate.Rate).FirstOrDefault(),
                CurrencyCode = domain.Franchisee.Currency,
                DueDate = domain.Invoice.DueDate,
                IsLateFeeCharged = isLateFeeCharged,
                AccountTypeId = domain.Invoice.InvoiceItems != null ? GetProfileType(domain.Invoice.InvoiceItems.Select(x => x.ItemTypeId).First()) : 0,
            };
        }
        private long GetProfileType(long itemTypeId)
        {
            if (itemTypeId == (long)InvoiceItemType.RoyaltyFee)
            {
                return (long)AuthorizeNetAccountType.Royalty;
            }
            else if (itemTypeId == (long)InvoiceItemType.AdFund)
            {
                return (long)AuthorizeNetAccountType.AdFund;
            }
            return 0;
        }

        public DocumentSummaryViewModel CreateViewModel(FranchiseDocument domain, long franchiseeId)
        {
            return new DocumentSummaryViewModel
            {
                DocumentName = domain.DocumentType.Name,
                ExpiryDate = domain.ExpiryDate.GetValueOrDefault(),
                isExpiring = true,
            };
        }
        public DocumentType CreateViewModel(DocumentType domain, long franchiseeId)
        {
            return new DocumentType
            {
                Name = domain.Name,
                Id = domain.Id,
                CategoryId = domain.CategoryId
            };
        }
    }
}
