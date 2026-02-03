using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Reports.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Sales
{
    public interface IDownloadFileHelperService
    {
        List<DownloadInvoiceModel> CreateDataForAdFundInvoice(long[] invoiceIds);
        List<DownloadInvoiceModel> CreateDataForRoyaltyInvoice(long[] invoiceIds);
        List<DownloadPaymentModel> CreateDataForRoyaltyPayments(long[] invoiceIds);
        List<DownloadPaymentModel> CreateDataForAdFundPayments(long[] invoiceIds);
        List<DownloadInvoiceModel> CreateDataForRoyaltyInvoiceFilter(List<FranchiseeInvoice> frInvoice); 
        List<DownloadInvoiceModel> CreateDataForAdFundInvoiceFilter(List<FranchiseeInvoice> frInvoice);
        List<DownloadPaymentModel> CreateDataForRoyaltyPaymentFilter(List<FranchiseeInvoice> frInvoice);
        List<DownloadPaymentModel> CreateDataForAdFundPaymentFilter(List<FranchiseeInvoice> frInvoice);
        List<BatchUploadRecord> CreateDataForBatchRecord(DateTime startDateTime, DateTime endDateTime);
        List<DownloadCountyModel> CreateDataForCounty(List<County> frInvoice);
        List<DownloadZipCodeModel> CreateDataForZipCode(List<ZipCode> countyList);
        List<DownloadInstructionModel> CreateDataForInstruction();
        List<DownloadPaymentModel> CreateDataForWebSeoPaymentFilter(List<FranchiseeInvoice> frInvoice);
        List<DownloadInvoiceModel> CreateDataForWebSeoInvoiceFilter(List<FranchiseeInvoice> frInvoice);
        List<DownloadInvoiceModel> CreateDataForWebSeoInvoice(long[] invoiceIds);
        List<DownloadPaymentModel> CreateDataForWebSeoPayments(long[] invoiceIds);
        List<DownloadInvoiceModel> CreateDataForWebSeoRoyaltyInvoice(long[] invoiceIds);
    }
}
