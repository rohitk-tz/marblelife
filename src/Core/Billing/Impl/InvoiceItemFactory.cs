using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using Core.Sales;
using Core.Sales.Domain;
using System.Linq;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class InvoiceItemFactory : IInvoiceItemFactory
    {
        private IRepository<ServiceType> _serviceTypeRepository;
        private IRepository<OneTimeProjectFee> _oneTimeProjectFeeRepository;
        private IRepository<FranchiseeLoanSchedule> _franchiseeLoneSchedulerRepository;

        private IMarketingClassService _marketingClassService;

        public InvoiceItemFactory(IUnitOfWork unitOfWork, IMarketingClassService marketingClassService)
        {
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
            _marketingClassService = marketingClassService;
            _oneTimeProjectFeeRepository = unitOfWork.Repository<OneTimeProjectFee>();
            _franchiseeLoneSchedulerRepository = unitOfWork.Repository<FranchiseeLoanSchedule>();
        }

        public InvoiceItem CreateDomain(InvoiceItemEditModel model)
        {
            var invoiceItem = new InvoiceItem()
            {
                Id = model.Id,
                Amount = model.Amount,
                Description = model.Description,
                Quantity = model.Quantity,
                Rate = model.Rate,
                ItemTypeId = model.ItemTypeId,
                ItemId = model.ItemId,
                InvoiceId = model.InvoiceId,
                IsNew = model.Id <= 0,
                CurrencyExchangeRateId = model.CurrencyExchangeRateId,
                ItemOriginal=model.ItemOriginal
               
            };

            if (model is RoyaltyInvoiceItemEditModel)
            {
                SetRoyaltyInvoiceItem(model, invoiceItem);
            }
            if (model is AdFundInvoiceItemEditModel)
            {
                SetAdFundInvoiceItem(model, invoiceItem);
            }
            if (model is LateFeeInvoiceItemEditModel)
            {
                SetLateFeeInvoiceItem(model, invoiceItem);
            }
            if (model is InterestRateInvoiceItemEditModel)
            {
                SetInterestRateInvoiceItem(model, invoiceItem);
            }
            if (model is ServiceFeeInvoiceItemEditModel)
            {
                SetServiceFeeInvoiceItem(model, invoiceItem);
            }
            return invoiceItem;
        }

        private static void SetServiceFeeInvoiceItem(InvoiceItemEditModel model, InvoiceItem invoiceItem)
        {
            var serviceFeeItemModel = model as ServiceFeeInvoiceItemEditModel;
            invoiceItem.ServiceFeeInvoiceItem = new ServiceFeeInvoiceItem
            {
                EndDate = serviceFeeItemModel.EndDate,
                StartDate = serviceFeeItemModel.StartDate,
                InvoiceItem = invoiceItem,
                ServiceFeeTypeId = serviceFeeItemModel.ServiceFeeTypeId,
                Percentage = serviceFeeItemModel.Percentage
            };
        }

        private static void SetRoyaltyInvoiceItem(InvoiceItemEditModel model, InvoiceItem invoiceItem)
        {
            var royInvItemModel = model as RoyaltyInvoiceItemEditModel;
            invoiceItem.RoyaltyInvoiceItem = new RoyaltyInvoiceItem
            {
                EndDate = royInvItemModel.EndDate,
                StartDate = royInvItemModel.StartDate,
                Percentage = royInvItemModel.Percentage,
                InvoiceItem = invoiceItem,
                Amount = royInvItemModel.SalesAmount
            };
        }

        private static void SetAdFundInvoiceItem(InvoiceItemEditModel model, InvoiceItem invoiceItem)
        {
            var adFInvItemModel = model as AdFundInvoiceItemEditModel;
            invoiceItem.AdFundInvoiceItem = new AdFundInvoiceItem
            {
                EndDate = adFInvItemModel.EndDate,
                StartDate = adFInvItemModel.StartDate,
                Percentage = adFInvItemModel.Percentage,
                InvoiceItem = invoiceItem,
                Amount = adFInvItemModel.SalesAmount
            };
        }

        public InvoiceItemEditModel CreateViewModel(InvoiceItem domain)
        {
            var item = domain.Lookup != null ? domain.Lookup.Name : null;

            var model = new InvoiceItemEditModel();

            if (domain.ItemId != null && domain.ItemTypeId == (long)InvoiceItemType.Service)
            {
                var serviceType = _serviceTypeRepository.Get(domain.ItemId.Value);
                var marketingClass = _marketingClassService.GetMarketingClassByInvoiceId(domain.InvoiceId);

                var subMarketingClass = _marketingClassService.GetSubMarketingClassByInvoiceId(domain.InvoiceId);
                if (serviceType != null) item = serviceType.Name;
                if (subMarketingClass != null && subMarketingClass.Length > 0)
                {
                    item = subMarketingClass + ":" + item;
                }
                else
                {
                    item = marketingClass + ":" + item;
                }
               
            }
            else if (domain.ItemTypeId == (long)InvoiceItemType.AdFund && domain.AdFundInvoiceItem != null)
            {
                model = SetFranchiseeFeeBasedInvoiceItem(domain.AdFundInvoiceItem, new AdFundInvoiceItemEditModel());
            }
            else if (domain.ItemTypeId == (long)InvoiceItemType.RoyaltyFee && domain.RoyaltyInvoiceItem != null)
            {
                model = SetFranchiseeFeeBasedInvoiceItem(domain.RoyaltyInvoiceItem, new RoyaltyInvoiceItemEditModel());
            }
            else if (domain.ItemTypeId == (long)InvoiceItemType.FranchiseeTechMail)
            {
                model = SetFranchiseeFeeBasedInvoiceItem(domain.FranchiseeFeeEmailInvoiceItem, new RoyaltyInvoiceItemEditModel());
            }
            else if (domain.ItemTypeId == (long)InvoiceItemType.LateFees && domain.LateFeeInvoiceItem != null)
            {
                var lateFeeType = string.Empty;
                if (domain.LateFeeInvoiceItem != null && domain.LateFeeInvoiceItem.LateFeeTypeId > 0)
                {
                    if (domain.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.SalesData)
                        lateFeeType = LateFeeType.SalesData.ToString();
                    else
                        lateFeeType = LateFeeType.Royalty.ToString();
                }
                model = SetFranchiseeFeeBasedInvoiceItem(domain.LateFeeInvoiceItem, new LateFeeInvoiceItemEditModel());

                item = domain.ItemTypeId == (long)InvoiceItemType.LateFees ? ("Late Fees" + "(" + lateFeeType + ")") : "";
            }
            else if ((domain.ItemTypeId == (long)InvoiceItemType.InterestRatePerAnnum) && domain.InterestRateInvoiceItem != null)
            {
                model = SetFranchiseeFeeBasedInvoiceItem(domain.InterestRateInvoiceItem, new InterestRateInvoiceItemEditModel());
                item = "Interest Rate(p.a.)";
            }
            else if ((domain.ItemTypeId == (long)InvoiceItemType.ServiceFee || domain.ItemTypeId == (long)InvoiceItemType.LoanServiceFee || domain.ItemTypeId == (long)InvoiceItemType.LoanServiceFeeInterestRatePerAnnum) && domain.ServiceFeeInvoiceItem != null)
            {
                var serviceFeeType = string.Empty;
                var isOneTimeProject = false;
                var isLone = false;
                if (domain.ServiceFeeInvoiceItem != null)
                {
                    if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.PayrollProcessing)
                        serviceFeeType = Organizations.Enum.ServiceFeeType.PayrollProcessing.ToString();
                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.Recruiting)
                        serviceFeeType = Organizations.Enum.ServiceFeeType.Recruiting.ToString();
                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.PHONECALLCHARGES)
                    {
                        //serviceFeeType = Organizations.Enum.ServiceFeeType.PHONECALLCHARGES.ToString();
                        serviceFeeType = "Back Up Service – INVOICE";
                    }
                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.Bookkeeping)
                        serviceFeeType = Organizations.Enum.ServiceFeeType.Bookkeeping.ToString();
                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.VarBookkeeping)
                        serviceFeeType = Organizations.Enum.ServiceFeeType.Bookkeeping.ToString() + "(var)";
                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.OneTimeProject)
                    {
                        //serviceFeeType = Organizations.Enum.ServiceFeeType.OneTimeProject.ToString();
                        serviceFeeType = "One time Charge – INVOICE";
                        isOneTimeProject = true;
                    }
                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.SEOCharges)
                    {
                        serviceFeeType = Organizations.Enum.ServiceFeeType.SEOCharges.ToString();
                        //serviceFeeType = "Back Up Services – INVOICE";
                    }

                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.Loan)
                    {
                        isLone = true;
                        serviceFeeType = Organizations.Enum.ServiceFeeType.Loan.ToString();
                    }
                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.NationalCharge)
                        serviceFeeType = Organizations.Enum.ServiceFeeType.NationalCharge.ToString();
                    else if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.InterestAmount)
                        serviceFeeType = "Loan " + Organizations.Enum.ServiceFeeType.InterestAmount.ToString();
                }
                model = SetFranchiseeFeeBasedInvoiceItem(domain.ServiceFeeInvoiceItem, new ServiceFeeInvoiceItemEditModel());
                if (isOneTimeProject)
                {
                    var oneTimeDescription = _oneTimeProjectFeeRepository.Table.Where(x => x.InvoiceItemId == domain.Id).FirstOrDefault();
                    model.OneTimeProjectDescription = oneTimeDescription != null ? oneTimeDescription.Description : "";
                    //model.OneTimeProjectDescription = "";
                }
                if (isLone)
                {
                    var loneDescription = _franchiseeLoneSchedulerRepository.Table.Where(x => x.InvoiceItemId == domain.Id).FirstOrDefault();
                    model.OneTimeProjectDescription = loneDescription != null ? loneDescription.FranchiseeLoan != null ? loneDescription.FranchiseeLoan.Description : "" : "";
                    //model.OneTimeProjectDescription = "";
                }
                item = item + " (" + serviceFeeType + ")";
            }

            SetInvoiceItemBasicDetails(domain, item, model);
            return model;
        }

        private static InvoiceItemEditModel SetFranchiseeFeeBasedInvoiceItem(dynamic domain, dynamic model)
        {
            if (domain is LateFeeInvoiceItem)
            {
                model.StartDate = domain.StartDate;
                model.EndDate = domain.EndDate;
                model.SalesAmount = domain.Amount != null ? domain.Amount : 0;
                model.GeneratedOn = domain.GeneratedOn;
            }
            else if (domain is InterestRateInvoiceItem)
            {
                model.StartDate = domain.StartDate;
                model.Percentage = domain.Percentage;
                model.EndDate = domain.EndDate;
                model.SalesAmount = domain.Amount != null ? domain.Amount : 0;
            }
            else if (domain is ServiceFeeInvoiceItem)
            {
                model.StartDate = domain.StartDate;
                model.EndDate = domain.EndDate;
                model.ServiceFeeTypeId = domain.ServiceFeeTypeId;
                model.ServiceFeeType = domain.ServiceFee.Name;
                model.Percentage = (domain.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.Bookkeeping) ? 0 : (domain.Percentage ?? 0);
            }
            else if (domain is FranchiseeFeeEmailInvoiceItem)
            {
                model.StartDate = domain.StartDate;
                model.EndDate = domain.EndDate;
            }
            else
            {
                model.Percentage = domain.Percentage;
                model.StartDate = domain.StartDate;
                model.EndDate = domain.EndDate;
                model.SalesAmount = domain.Amount;
            }
            return model;
        }

        private static void SetInvoiceItemBasicDetails(InvoiceItem domain, string item, InvoiceItemEditModel model)
        {
            model.Id = domain.Id;
            model.Amount = domain.Amount;
            model.Description = domain.Description;
            model.Quantity = domain.Quantity;
            model.Rate = domain.Rate;
            model.ItemTypeId = domain.ItemTypeId;
            model.ItemId = domain.ItemId;
            model.InvoiceId = domain.InvoiceId;
            model.Item = item;
            model.CurrencyExchangeRateId = domain.CurrencyExchangeRateId;
            model.CurrencyRate = domain.CurrencyExchangeRate.Rate;
        }

        private static void SetLateFeeInvoiceItem(InvoiceItemEditModel model, InvoiceItem invoiceItem)
        {
            var latefeeItemModel = model as LateFeeInvoiceItemEditModel;
            invoiceItem.LateFeeInvoiceItem = new LateFeeInvoiceItem
            {
                LateFeeTypeId = latefeeItemModel.LateFeeTypeId,
                Amount = latefeeItemModel.SalesAmount,
                StartDate = latefeeItemModel.StartDate,
                EndDate = latefeeItemModel.EndDate,
                WaitPeriod = latefeeItemModel.WaitPeriod,
                InvoiceItem = invoiceItem,
                GeneratedOn = latefeeItemModel.GeneratedOn
            };
        }

        private static void SetInterestRateInvoiceItem(InvoiceItemEditModel model, InvoiceItem invoiceItem)
        {
            var interestrateItemModel = model as InterestRateInvoiceItemEditModel;
            invoiceItem.InterestRateInvoiceItem = new InterestRateInvoiceItem
            {
                IsNew = model.Id <= 0,
                Id = model.Id,
                Amount = interestrateItemModel.SalesAmount,
                StartDate = interestrateItemModel.StartDate,
                EndDate = interestrateItemModel.EndDate,
                InvoiceItem = invoiceItem,
                WaitPeriod = interestrateItemModel.WaitPeriod,
                Percentage = interestrateItemModel.Percentage
            };
        }
    }
}
