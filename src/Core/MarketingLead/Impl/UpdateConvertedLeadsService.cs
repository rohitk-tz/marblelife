using Core.Application;
using Core.Application.Attribute;
using Core.MarketingLead.Domain;
using Core.MarketingLead.Enum;
using Core.Organizations.Domain;
using Core.Sales;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class UpdateConvertedLeadsService : IUpdateConvertedLeadsService
    {
        private readonly IRepository<WebLead> _webLeadsRepository;
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadsCallDetailRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<RoutingNumber> _routingNumberRepository;
        private readonly ILogService _logService;
        private readonly ICustomerService _customerService;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<MarketingLeadCallDetailV2> _marketingLeadsCallDetailV2Repository;

        public UpdateConvertedLeadsService(IUnitOfWork unitOfWork, IClock clock, ISettings settings, ILogService logService,
            ICustomerService customerService)
        {
            _marketingLeadsCallDetailRepository = unitOfWork.Repository<MarketingLeadCallDetail>();
            _webLeadsRepository = unitOfWork.Repository<WebLead>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _logService = logService;
            _customerService = customerService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _routingNumberRepository = unitOfWork.Repository<RoutingNumber>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _marketingLeadsCallDetailV2Repository = unitOfWork.Repository<MarketingLeadCallDetailV2>();
        }

        public void UpdateLeads()
        {
            _logService.Info("starting update for converted leads helloo: CallDetails");
            UpdateCallConvertedLeads();
            _logService.Info("Stop update for converted leads : CallDetails");

            _logService.Info("starting update for converted leads : WebLeads");
            UpdateConvertedWebLeads();
            _logService.Info("stop update for converted leads : WebLeads");
            _logService.Info("start Updating Franchise for call detail By New Way");
            if (_settings.IsMapFranchiseeToFranchiseePhoneWithFranchiseeId)
            {
                MapFranchiseeToFranchiseePhoneWithFranchiseeId();
            }
            _logService.Info("stop Updating Franchise for call detail By new Way");
        }

        private void UpdateCallConvertedLeads()
        {
            _unitOfWork.StartTransaction();

            _logService.Info("start Updating Franchise for call detail");
            UpdateFranchisee();
            _logService.Info("stop Updating Franchise for call detail");
           
            var currentDate = _clock.UtcNow;
            var previousDate = currentDate.AddMonths(-2);

            var callDetailListofPreviousMonth = _marketingLeadsCallDetailRepository.Table.Where(x => x.DateAdded >= previousDate && x.DateAdded <= currentDate
                                                    && x.Invoice == null).ToList();

            _logService.Info("start Updating Invoice for call detail");
            UpdateInvoice(callDetailListofPreviousMonth);
            _logService.Info("stop Updating Invoice for call detail");

        }

        private void UpdateInvoice(IList<MarketingLeadCallDetail> callDetailList)
        {
            foreach (var callDetail in callDetailList)
            {
                try
                {
                    var invoiceId = GetInvoiceIdForCallDetail(callDetail.CallerId, callDetail.DateAdded.GetValueOrDefault());

                    if (invoiceId > 0)
                        callDetail.InvoiceId = invoiceId;

                    if (callDetail.InvoiceId > 0)
                    {
                        _marketingLeadsCallDetailRepository.Save(callDetail);
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error updating Invoice: {0}", ex.Message));
                    _unitOfWork.Rollback();
                }
                finally
                {
                    _unitOfWork.ResetContext();
                }
            }
        }

        private void UpdateFranchisee()
        {
            MapFranchiseeToPhoneLabel();
            MapFranchiseeToFranchiseePhone();
           
        }

        private void MapFranchiseeToPhoneLabel()
        {
            var routingNumberList = _routingNumberRepository.Table.Where(x => x.Franchisee != null).OrderByDescending(x=>x.Id).ToList();
            foreach (var routingNumber in routingNumberList)
            {
                long? franchieeId = routingNumber.FranchiseeId;
                try
                {
                    var callDetailListDialedNumber = _marketingLeadsCallDetailRepository.Table.Where(x => x.FranchiseeId == null
                                        && string.IsNullOrEmpty(x.TransferToNumber)
                                        && (x.PhoneLabel.Equals(routingNumber.PhoneLabel) && x.DialedNumber.Equals(routingNumber.PhoneNumber))).ToList();

                   

                    foreach (var item in callDetailListDialedNumber)
                    {
                        item.FranchiseeId = routingNumber.FranchiseeId;
                        item.TagId = (long)TagType.FranchiseDirect;
                        _marketingLeadsCallDetailRepository.Save(item);
                    }

                    var callDetailListtransferToNumber = _marketingLeadsCallDetailRepository.Table.Where(x => x.FranchiseeId == null
                                        && !string.IsNullOrEmpty(x.TransferToNumber)
                                        && x.TransferToNumber.Equals(routingNumber.PhoneNumber)).ToList();

                    foreach (var item in callDetailListtransferToNumber)
                    {
                        item.FranchiseeId = routingNumber.FranchiseeId;
                        item.TagId = (long)TagType.FranchiseDirect;
                        _marketingLeadsCallDetailRepository.Save(item);
                    }

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("error saving FranchiseeId : {0}", ex.Message));
                    _unitOfWork.Rollback();
                }
                finally
                {
                    _unitOfWork.ResetContext();
                }
            }
        }

        private void MapFranchiseeToFranchiseePhone()
        {
            var franchiseeList = _franchiseeRepository.Table.Where(x => x.Organization != null && x.Organization.Phones.Any()
                                    && x.Organization.IsActive).ToList();
            foreach (var franchisee in franchiseeList)
            {
                try
                {
                    _logService.Info(string.Format("start Updating Marketing Lead For Franchisee : {{0}}",franchisee.Organization.Name));
                    var phoneNumbers = franchisee.Organization.Phones.Select(p => p.Number).ToList();

                    var listDialedNumber = _marketingLeadsCallDetailRepository.Table.Where(x => x.FranchiseeId == null
                                        && string.IsNullOrEmpty(x.TransferToNumber)
                                        && (phoneNumbers.Contains(x.DialedNumber))).ToList();

                    foreach (var item in listDialedNumber)
                    {
                        item.FranchiseeId = franchisee.Id;
                        item.TagId = (long)TagType.FranchiseDirect;
                        _marketingLeadsCallDetailRepository.Save(item);
                    }

                    var listTransferToNumbers = _marketingLeadsCallDetailRepository.Table.Where(x => x.FranchiseeId == null
                                             && !string.IsNullOrEmpty(x.TransferToNumber)
                                             && (phoneNumbers.Contains(x.TransferToNumber))).ToList();

                    foreach (var item in listTransferToNumbers)
                    {
                        item.FranchiseeId = franchisee.Id;
                        item.TagId = (long)TagType.FranchiseDirect;
                        _marketingLeadsCallDetailRepository.Save(item);
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("error saving FranchiseeId : {0}", ex.Message));
                    _unitOfWork.Rollback();
                }
            }
        }

        private void MapFranchiseeToFranchiseePhoneWithFranchiseeId()
        {
            var currentDate = DateTime.UtcNow;
            var previousDate = currentDate.AddMonths(-9);
            var franchiseeList = _franchiseeRepository.Table.Where(x => x.Organization.Phones.Any()
                                    && x.Organization.IsActive).ToList();
            var marketingLeadCallDetailList = _marketingLeadsCallDetailRepository.Table.Where(x => (x.FranchiseeId != null || x.FranchiseeId == null)
                                         && (x.DateAdded >= previousDate && x.DateAdded <= currentDate)).ToList();
            _logService.Info("start Mapping  Franchise for call detail By Phone ");
            foreach (var franchisee in franchiseeList)
            {
                try
                {
                    var phoneNumbers = franchisee.Organization.Phones.Select(p => p.Number).ToList();

                    var listDialedNumber = marketingLeadCallDetailList.Where(x => (x.FranchiseeId != null || x.FranchiseeId == null)
                                        && string.IsNullOrEmpty(x.TransferToNumber)
                                        && phoneNumbers.Contains(x.DialedNumber)
                                        && (x.DateAdded >= previousDate && x.DateAdded <= currentDate)).ToList();

                    foreach (var item in listDialedNumber)
                    {
                        item.FranchiseeId = franchisee.Id;
                        item.TagId = (long)TagType.FranchiseDirect;
                        _marketingLeadsCallDetailRepository.Save(item);
                    }

                    var listTransferToNumbers = marketingLeadCallDetailList.Where(x => (x.FranchiseeId != null || x.FranchiseeId == null) &&
                                             !string.IsNullOrEmpty(x.TransferToNumber)
                                             && (phoneNumbers.Contains(x.TransferToNumber))
                                             && (x.DateAdded >= previousDate && x.DateAdded <= currentDate)).ToList();

                    foreach (var item in listTransferToNumbers)
                    {
                        item.FranchiseeId = franchisee.Id;
                        item.TagId = (long)TagType.FranchiseDirect;
                        _marketingLeadsCallDetailRepository.Save(item);
                        var marketingLeadCallDetailsV2 = _marketingLeadsCallDetailV2Repository.Table.FirstOrDefault(x => x.MarketingLeadCallDetailId == item.Id);
                        if (marketingLeadCallDetailsV2 != null)
                        {
                            marketingLeadCallDetailsV2.FranchiseeId= franchisee.Id;
                            _marketingLeadsCallDetailV2Repository.Save(marketingLeadCallDetailsV2);
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("error saving FranchiseeId : {0}", ex.Message));
                    _unitOfWork.Rollback();
                }
            }
        }

        private long GetInvoiceIdForCallDetail(string callerId, DateTime dateTime)
        {
            var invoiceId = _franchiseeSalesRepository.Table.Where(x => x.Customer.Phone.Equals(callerId)
                                                    && x.Invoice.GeneratedOn >= dateTime && (x.InvoiceId != null)).Select(x => x.InvoiceId).FirstOrDefault();
            if (invoiceId != null)
                return invoiceId.Value;
            return 0;
        }

        private void UpdateConvertedWebLeads()
        {
            var currentDate = _clock.UtcNow;
            var previousDate = currentDate.AddMonths(_settings.DefaultMonthCountForMarketingLeads);
            _unitOfWork.StartTransaction();

            var webLeadList = _webLeadsRepository.Table.Where(x => x.Invoice == null && x.CreatedDate >= previousDate
                                    && x.CreatedDate <= currentDate).ToList();
            foreach (var webLead in webLeadList)
            {
                var invoiceId = GetInvoiceIdForWebLead(webLead);
                if (invoiceId > 0)
                {
                    webLead.InvoiceId = invoiceId;
                    _webLeadsRepository.Save(webLead);
                    _unitOfWork.SaveChanges();
                }
            }
        }

        public long GetInvoiceIdForWebLead(WebLead webLead)
        {
            var customerName = webLead.LastName.ToLower() + ", " + webLead.Firstname.ToLower();
            var customerNameFormatted = webLead.Firstname.ToLower() + " " + webLead.LastName.ToLower();
            var salesInfo = _franchiseeSalesRepository.Table.Where(x => (x.Customer.Name.ToLower().Equals(customerName)
                               || x.Customer.Name.ToLower().Equals(customerNameFormatted))
                               && x.InvoiceId != null && x.Invoice.GeneratedOn >= webLead.CreatedDate);

            if (!salesInfo.Any())
                return 0;

            var invoiceId = salesInfo.Where(x => x.Customer.CustomerEmails.Any(y => y.Email.ToLower().Equals(webLead.Email.ToLower())))
                .Select(x => x.InvoiceId).FirstOrDefault();

            if (invoiceId == null)
                invoiceId = salesInfo.Where(x => x.Customer.Phone.Equals(webLead.Phone)).Select(x => x.InvoiceId).FirstOrDefault();

            if (invoiceId != null)
                return invoiceId.Value;
            return 0;
        }
    }
}
