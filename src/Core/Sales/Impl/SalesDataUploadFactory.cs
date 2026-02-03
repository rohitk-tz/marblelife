using Core.Application.Attribute;
using Core.Organizations;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using System;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class SalesDataUploadFactory : ISalesDataUploadFactory
    {
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        public SalesDataUploadFactory(IOrganizationRoleUserInfoService organizationRoleUserInfoService)
        {
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
        }
        public SalesDataUpload CreateDomain(SalesDataUploadCreateModel model)
        {
            return new SalesDataUpload()
            {
                Id = model.Id,
                FranchiseeId = model.FranchiseeId,
                FileId = model.FileId,
                PeriodStartDate = model.PeriodStartDate,
                PeriodEndDate = model.PeriodEndDate,
                StatusId = model.StatusId,
                AccruedAmount = model.AccruedAmount,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsInvoiceGenerated = model.IsInvoiceGenerated,
                IsNew = model.Id <= 0,
                IsActive = true
            };
        }

        public SalesDataUploadCreateModel CreateModel(SalesDataUpload domain)
        {
            return new SalesDataUploadCreateModel()
            {
                Id = domain.Id,
                FranchiseeId = domain.FranchiseeId,
                FileId = domain.FileId,
                PeriodStartDate = domain.PeriodStartDate,
                PeriodEndDate = domain.PeriodEndDate,
                StatusId = domain.StatusId,
                AccruedAmount = domain.AccruedAmount,
            };
        }

        public SalesDataUploadViewModel CreateListModel(SalesDataUpload domain)
        {
            if (domain == null) return new SalesDataUploadViewModel();

            var userInfo = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value);
            var uploadedBy = userInfo.Email;

            return new SalesDataUploadViewModel()
            {
                Id = domain.Id,
                AccruedAmount = domain.AccruedAmount,
                FileId = domain.FileId,
                LogFileId = domain.ParsedLogFileId,
                FranchiseeName = domain.Franchisee.Organization.Name,
                FranchiseeId = domain.Franchisee.Id,
                //PaidAmount = domain.PaidAmount,
                PaidAmount = domain.AccruedAmount != null && domain.AccruedAmount > 0 ? domain.PaidAmount - domain.AccruedAmount : domain.PaidAmount,
                TotalAmount = domain.TotalAmount,
                NumberOfCustomers = domain.NumberOfCustomers,
                NumberOfInvoices = domain.NumberOfInvoices,
                Status = domain.Lookup.Name,
                StatusId = domain.StatusId,
                FailedRecords = domain.NumberOfFailedRecords,
                PeriodEndDate = domain.PeriodEndDate,
                PeriodStartDate = domain.PeriodStartDate,
                Frequency = (domain.Franchisee.FeeProfile != null && domain.Franchisee.FeeProfile.Lookup != null) ? domain.Franchisee.FeeProfile.Lookup.Name : "",
                CurrencyRate = domain.CurrencyExchangeRate != null ? domain.CurrencyExchangeRate.Rate : 1,
                UploadedOn = domain.DataRecorderMetaData.DateCreated,
                UploadedBy = uploadedBy,
                Currency = domain.Franchisee.Currency,
                TotalCredit = domain.PaidAmount,
                TotalDebit = domain.AccruedAmount
            };
        }

        public AnnualSalesDataUpload CreateAnnualUploadDomain(SalesDataUploadCreateModel model, SalesDataUpload upload)
        {
            var domain = new AnnualSalesDataUpload
            {
                CurrencyExchangeRateId = model.CurrencyExchareRateId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                FranchiseeId = model.FranchiseeId,
                IsNew = model.AnnualUploadId <= 0,
                NoOfFailedRecords = upload.NumberOfFailedRecords,
                NoOfParsedRecords = upload.NumberOfParsedRecords,
                Id = model.AnnualUploadId,
                StatusId = model.StatusId,
                PeriodStartDate = model.AnnualUploadStartDate,
                PeriodEndDate = model.AnnualUploadEndDate,
                SalesDataUploadId = upload.Id > 0 ? upload.Id : (long?)null,
                AuditActionId = model.AuditActionId,
            };
            return domain;
        }

        public SalesDataUploadViewModel CreateListModel(AnnualSalesDataUpload domain)
        {
            if (domain == null) return new SalesDataUploadViewModel();

            var userInfo = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value);
            var uploadedBy = userInfo.Email;

            return new SalesDataUploadViewModel()
            {
                Id = domain.Id,
                FileId = domain.FileId,
                LogFileId = domain.ParsedLogFileId,
                FranchiseeName = domain.Franchisee.Organization.Name,
                FranchiseeId = domain.Franchisee.Id,
                PaidAmount = domain.PaidAmount,
                TotalAmount = domain.TotalAmount,
                Status = domain.Lookup.Name,
                StatusId = domain.StatusId,
                MismatchedRecords = domain.NoOfMismatchedRecords,
                PeriodEndDate = domain.PeriodEndDate,
                PeriodStartDate = domain.PeriodStartDate,
                Frequency = (domain.Franchisee.FeeProfile != null && domain.Franchisee.FeeProfile.Lookup != null) ? domain.Franchisee.FeeProfile.Lookup.Name : "",
                CurrencyRate = domain.CurrencyExchangeRate != null ? domain.CurrencyExchangeRate.Rate : 1,
                UploadedOn = domain.DataRecorderMetaData.DateCreated,
                UploadedBy = uploadedBy,
                Currency = domain.Franchisee.Currency,
                AuditStatusId = domain.AuditActionId,
                AuditStatus = domain.AuditAction.Name,
                WeeklyRoyality = domain.WeeklyRoyality,
                AnnualRoyality = domain.AnnualRoyality
            };
        }

      
    }
}
