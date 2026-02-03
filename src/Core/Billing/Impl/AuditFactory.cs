using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System;
using Core.Geo.Domain;
using Core.Application;
using Core.Sales.Enum;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class AuditFactory : IAuditFactory
    {
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<AuditFranchiseeSales> _auditFranchiseeSalesRepository;
        private readonly IRepository<AnnualSalesDataUpload> _annualSalesDataUploadRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<AnnualReportType> _annualReportTypelRepository;
        private readonly IRepository<InvoiceAddress> _invoiceAddresslRepository;
        public AuditFactory(IUnitOfWork unitOfWork)
        {
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _auditFranchiseeSalesRepository = unitOfWork.Repository<AuditFranchiseeSales>();
            _annualSalesDataUploadRepository = unitOfWork.Repository<AnnualSalesDataUpload>();
            _cityRepository = unitOfWork.Repository<City>();
            _stateRepository = unitOfWork.Repository<State>();
            _annualReportTypelRepository = unitOfWork.Repository<AnnualReportType>();
            _invoiceAddresslRepository = unitOfWork.Repository<InvoiceAddress>();
        }
        public AuditInvoice CreateDomain(InvoiceEditModel model)
        {
            return new AuditInvoice()
            {
                IsNew = model.Id < 1,
                Id = model.Id,
                GeneratedOn = model.GeneratedOn,
                InvoiceId = model.InvoiceId != null ? model.InvoiceId : null,
                StatusId = model.StatusId,
                AnnualUploadId = model.AnnualSalesDataUploadId,
                AuditInvoiceItems = model.InvoiceItems != null ? new List<AuditInvoiceItem>(model.InvoiceItems.Select(x => CreateDomain(x))) : null,
                isActive = true

            };
        }
        private AuditInvoiceItem CreateDomain(InvoiceItemEditModel model)
        {
            var invoiceItem = new AuditInvoiceItem()
            {
                Id = model.Id,
                Amount = model.Amount,
                Description = model.Description,
                Quantity = model.Quantity,
                Rate = model.Rate,
                ItemTypeId = model.ItemTypeId,
                ItemId = model.ItemId,
                AuditInvoiceId = model.InvoiceId,
                IsNew = model.Id <= 0,
            };

            return invoiceItem;
        }

        public AuditPayment CreateDomain(FranchiseeSalesPaymentEditModel model)
        {
            return new AuditPayment()
            {
                Id = model.Id,
                Date = model.Date,
                Amount = model.Amount,
                InstrumentTypeId = (long)InstrumentType.Cash,
                AuditPaymentItems = (model.PaymentItems != null) ? new List<AuditPaymentItem>(model.PaymentItems.Select(x => CreatePaymentItemDomain(x))) : null,
                IsNew = model.Id <= 0,
            };
        }

        private AuditPaymentItem CreatePaymentItemDomain(PaymentItemEditModel model)
        {
            return new AuditPaymentItem()
            {
                ItemId = model.ItemId,
                ItemTypeId = model.ItemTypeId,
                IsNew = model.Id <= 0
            };
        }

        public AuditInvoicePayment CreateDomain(long invoiceId, long paymentId)
        {
            return new AuditInvoicePayment
            {
                InvoiceId = invoiceId,
                PaymentId = paymentId,
                IsNew = true
            };
        }

        public Auditaddress CreateViewModel(AuditInvoice auditInvoice)
        {
            try
            {
                var auditCustomerAddress = _auditFranchiseeSalesRepository.Table.FirstOrDefault(x => x.AuditInvoiceId == auditInvoice.Id && x.QbInvoiceNumber == auditInvoice.QBInvoiceNumber);
                var customer = !string.IsNullOrEmpty(auditCustomerAddress?.AuditCustomer?.Name)? auditCustomerAddress.AuditCustomer.Name: auditCustomerAddress?.Customer?.Name ?? "";


                var reportDescription = _annualReportTypelRepository.Table.Where(x => x.Id == auditInvoice.ReportTypeId).FirstOrDefault();
                var description = auditInvoice.ReportTypeId != null ? reportDescription : null;
                var descriptionName = description != null ? description.ReportTypeName + " " + description.Description : "";
                var model = new Auditaddress
                {

                    CustomerName = customer,
                    QbInvoice = auditInvoice.QBInvoiceNumber,
                    AuditInvoiceId = auditInvoice.Id,
                    InvoiceId = auditCustomerAddress.CustomerInvoiceId > 0 ? auditCustomerAddress.CustomerInvoiceId : auditInvoice.InvoiceId,
                    WeeklyPaidAmount = auditInvoice.Invoice != null ? auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount) : 0,
                    AnnualUploadId = auditInvoice.AnnualUploadId,
                    ReportTypeDescription = auditInvoice.AnnualReportType != null ? auditInvoice.AnnualReportType.ReportTypeName + " " + auditInvoice.AnnualReportType.Description : descriptionName,
                    AnnuallyDifference = auditInvoice.Invoice != null ? (auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount) - auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount)) : 0,  //A-B,
                    WeeklyDifference = auditInvoice.Invoice != null ? (auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)) : 0, //C-D,
                    AnnuallySalesDifference = auditInvoice.Invoice != null && auditInvoice.AuditInvoiceItems != null ? (auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount) - auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount))
                                                               : auditInvoice.Invoice != null ? (0 - auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount)) : auditInvoice.AuditInvoiceItems != null ? (auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount) - 0)
                                                               : 0, //A-C
                    AnnuallyPaidDifference = auditInvoice.Invoice != null && auditInvoice.AuditInvoicePayments.Count() > 0 ? auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount) - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)
                    : auditInvoice.Invoice != null ? (0 - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)) : (auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount) - 0), //B-D

                    Id = auditInvoice.Id,
                    AnnualSalesAmount = auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount),
                    AnnualSalesAmountForExcel = auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount).ToString(),
                    AnnualPaidAmountForExcel = auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount).ToString(),
                    AnnualPaidAmount = auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount),
                    WeeklySalesAmount = auditInvoice.Invoice != null ? auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) : 0,
                    WeeklySalesAmountForExcel = auditInvoice.Invoice != null ? auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount).ToString() : 0.ToString(),

                    WeeklyPaidAmountForExcel = auditInvoice.Invoice != null ? auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount).ToString() : 0.ToString(),
                    InvoiceDate = auditInvoice.GeneratedOn,
                    InvoiceDateForExcel = auditInvoice.GeneratedOn.ToShortDateString(),
                    FranchiseeName = auditInvoice.AnnualSalesDataUpload.Franchisee.Organization.Name,
                    CurrencyCode = auditInvoice.AnnualSalesDataUpload.Franchisee.Currency,
                    CurrencyRate = auditInvoice.AnnualSalesDataUpload.CurrencyExchangeRate.Rate,

                    FranchiseeId = auditInvoice.AnnualSalesDataUpload != null ? auditInvoice.AnnualSalesDataUpload.FranchiseeId : 0,



                    AnnuallyDifferenceForExcel = auditInvoice.Invoice != null ? (auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount) - auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount)).ToString() : 0.ToString(),
                    WeeklyDifferenceForExcel = auditInvoice.Invoice != null ? (auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)).ToString() : 0.ToString(),


                    AnnuallySalesDifferenceForExcel = auditInvoice.Invoice != null && auditInvoice.AuditInvoiceItems != null ? (auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount) - auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount)).ToString()
                                                               : auditInvoice.Invoice != null ? (0 - auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount)).ToString() : auditInvoice.AuditInvoiceItems != null ? (auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount) - 0).ToString()
                                                               : 0.ToString(), //A-C,


                    AnnuallyPaidDifferenceForExcel = auditInvoice.Invoice != null && auditInvoice.AuditInvoicePayments.Count() > 0 ? (auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount) - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)).ToString()
                    : auditInvoice.Invoice != null ? (0 - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)).ToString() : (auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount) - 0).ToString(), //B-D
                    GroupTypeId = GetReportTypeId(auditInvoice.AnnualReportType),
                    ReportTypeId = auditInvoice.AnnualReportType != null ? auditInvoice.AnnualReportType.Id : default(long),

                };
                return model;
            }
            catch(Exception ex)
            {
                return new Auditaddress();
            }
        }

        private long GetReportTypeId(AnnualReportType report)
        {
            long reportTypeId = default(long);
            if (report != null && report.Id == (long)AuditReportType.Type1H)
            {
                reportTypeId = (long)AnnualGroupType.Type1H;
            }

            //else if (report.Id == (long)AuditReportType.type)
            //{

            //}

            return reportTypeId;
        }
        public Auditaddress CreateViewModel(SystemAuditRecord auditInvoice)
        {
            string reportTypeText = "";
            var auditCustomerAddress = _franchiseeSalesRepository.Table.FirstOrDefault(x => x.InvoiceId == auditInvoice.InvoiceId && x.FranchiseeId == auditInvoice.FranchiseeId);
            var customer = auditCustomerAddress != null && auditCustomerAddress.Customer != null ? auditCustomerAddress.Customer.Name : "";
            var annualSalesData = auditInvoice.AnnualSalesDataUpload != null ? auditInvoice.AnnualSalesDataUpload : _annualSalesDataUploadRepository.Get(auditInvoice.AnnualUploadId);
            var date = auditInvoice.AnnualSalesDataUpload != null ? auditInvoice.AnnualSalesDataUpload.DataRecorderMetaData.DateCreated.Date : annualSalesData.DataRecorderMetaData.DateCreated.Date;

            var model = new Auditaddress
            {
                CustomerName = customer,
                QbInvoice = auditInvoice.QBIdentifier,
                InvoiceId = auditInvoice.InvoiceId,
                InvoiceDate = auditInvoice.Invoice != null ? auditInvoice.Invoice.GeneratedOn : default(DateTime),
                AnnualSalesAmount = 0,
                AnnualPaidAmount = 0,
                WeeklySalesAmount = auditInvoice.Invoice != null ? auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) : 0,
                WeeklyPaidAmount = auditInvoice.Invoice != null ? auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount) : 0,
                WeeklyDifference = auditInvoice.Invoice != null ? (auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)) : 0,
                AnnuallyDifference = 0,
                AnnuallySalesDifference = auditInvoice.Invoice != null ? 0 - auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) : 0,
                AnnuallyPaidDifference = auditInvoice.Invoice != null ? 0 - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount) : 0,
                Id = auditInvoice.Id,
                AuditInvoiceId = 0,
                AnnualUploadId = auditInvoice.AnnualUploadId,

                FranchiseeName = annualSalesData.Franchisee.Organization.Name,
                CurrencyCode = annualSalesData.Franchisee.Currency,
                CurrencyRate = annualSalesData.CurrencyExchangeRate.Rate,
                ReportTypeDescription = auditInvoice.AnnualReportType != null ? auditInvoice.AnnualReportType.ReportTypeName + " " + auditInvoice.AnnualReportType.Description : "",


                WeeklySalesAmountForExcel = auditInvoice.Invoice != null ? (auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount)).ToString() : 0.ToString(),
                WeeklyPaidAmountForExcel = auditInvoice.Invoice != null ? (auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)).ToString() : 0.ToString(),


                AnnuallyDifferenceForExcel = 0.ToString(),
                WeeklyDifferenceForExcel = auditInvoice.Invoice != null ? (auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)).ToString() : 0.ToString(),

                AnnuallySalesDifferenceForExcel = auditInvoice.Invoice != null ? (0 - auditInvoice.Invoice.InvoiceItems.ToList().Sum(x => x.Amount)).ToString() : 0.ToString(),

                AnnuallyPaidDifferenceForExcel = auditInvoice.Invoice != null ? (0 - auditInvoice.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)).ToString() : 0.ToString(),
                ReportTypeId = auditInvoice.AnnualReportType.Id

            };
            var sDate = new DateTime(2019, 1, 1);
            var eDate = new DateTime(2019, 12, 31);
            if (!(date >= sDate && date <= eDate))
            {
                reportTypeText = "";
            }
            return model;
        }

        public InvoiceItemEditModel CreateViewModel(AuditInvoiceItem domain)
        {
            var model = new InvoiceItemEditModel();
            SetInvoiceItemBasicDetails(domain, model);
            return model;
        }

        private static void SetInvoiceItemBasicDetails(AuditInvoiceItem domain, InvoiceItemEditModel model)
        {
            model.Id = domain.Id;
            model.Amount = domain.Amount;
            model.Description = domain.Description;
            model.Quantity = domain.Quantity;
            model.Rate = domain.Rate;
            model.ItemTypeId = domain.ItemTypeId;
            model.ItemId = domain.ItemId;
            model.InvoiceId = domain.AuditInvoiceId;
            model.CurrencyExchangeRateId = domain.AuditInvoice.AnnualSalesDataUpload.CurrencyExchangeRateId;
            model.CurrencyRate = domain.AuditInvoice.AnnualSalesDataUpload.CurrencyExchangeRate.Rate;
        }

        public SystemAuditRecord CreateDomain(FranchiseeSales franchiseeSales, AnnualSalesDataUpload annualUpload)
        {
            var domain = new SystemAuditRecord
            {
                InvoiceId = franchiseeSales.InvoiceId.Value,
                FranchiseeId = annualUpload.FranchiseeId,
                AnnualUploadId = annualUpload.Id,
                QBIdentifier = franchiseeSales.QbInvoiceNumber != null ? franchiseeSales.QbInvoiceNumber : "",
                IsNew = true,
            };
            return domain;
        }

        public AuditAddressDiscrepancy CreateViewModelAudit(CustomerCreateEditModel customer, long? id, bool isUpdated, string countryName, long annualSalesUploadId, long? marketingClassId = null)
        {
            var addressInfo = new AuditAddressDiscrepancy
            {
                AddressLine1 = customer.Address != null ? customer.Address.AddressLine1 : "",
                AddressLine2 = customer.Address != null ? customer.Address.AddressLine2 : "",
                City = customer.Address != null ? customer.Address.City : "",
                State = customer.Address != null ? customer.Address.State : "",
                ZipCode = customer.Address != null ? customer.Address.ZipCode : "",
                FranchiseeSalesId = id.GetValueOrDefault(),
                Country = countryName,
                FranchiseeSales = _franchiseeSalesRepository.Get(id.GetValueOrDefault()),
                AnnualSalesDataUpload = _annualSalesDataUploadRepository.Get(annualSalesUploadId),
                IsNew = true,
                isUpdated = isUpdated,
                email = (customer.CustomerEmails.Count() > 0) ? customer.CustomerEmails.FirstOrDefault().Email : "",
                annualSalesdatauploadId = annualSalesUploadId,
                phoneNumber = customer.Phone,
                MarketingClassId = marketingClassId

            };
            return addressInfo;
        }


        public AddressHistryLog CreateViewModel(Customer customer, long? id, bool isUpdated, long? countryId, long franchiseeSalesId, long? typeId, long? marketingClassId = null)
        {
            var addressInfo = new AddressHistryLog
            {
                AddressLine1 = customer.Address != null ? customer.Address.AddressLine1 : "",
                AddressLine2 = customer.Address != null ? customer.Address.AddressLine2 : "",
                CityId = customer.Address != null ? customer.Address.CityId : null,
                StateId = customer.Address != null ? customer.Address.StateId : null,
                ZipCode = customer.Address != null ? customer.Address.ZipCode : null,
                addressId = customer.Address != null ? (long?)customer.Address.Id : null,
                CountryId = countryId != default(long) ? countryId.GetValueOrDefault() : 1,
                IsNew = true,
                EmailId = (customer.CustomerEmails.Count() > 0) ? customer.CustomerEmails.FirstOrDefault().Email : "",
                ZipId = customer.Address != null ? customer.Address.ZipId : null,
                TypeId = typeId != default(long) ? typeId : null,
                FranchiseeSalesId = franchiseeSalesId,
                phoneNumber = customer.Phone,
                StateName = customer.Address != null && customer.Address.StateId == null ? customer.Address.StateName : "",
                CityName = customer.Address != null && customer.Address.CityId == null ? customer.Address.CityName : "",
                MarketingClassId = marketingClassId

            };
            return addressInfo;
        }
        public Address CreateViewModel(CustomerCreateEditModel customer, long? id, bool isUpdated, Customer obj, long annualSalesUploadId)
        {
            long stateId = default(long);
            long cityId = default(long);
            if (customer.Address != null && customer.Address.StateId == default(long))
            {
                stateId = _stateRepository.Table.Where(x => x.Name == customer.Address.State || x.ShortName == customer.Address.State).Select(x => x.Id).FirstOrDefault();
            }
            if (customer.Address != null && (customer.Address.CityId == null))
            {
                cityId = _cityRepository.Table.Where(x => x.Name == customer.Address.City).Select(x => x.Id).FirstOrDefault();
            }
            var addressInfo = new Address
            {
                AddressLine1 = customer.Address != null ? customer.Address.AddressLine1 : "",
                AddressLine2 = customer.Address != null ? customer.Address.AddressLine2 : "",
                CityId = customer.Address != null ? customer.Address.CityId : null,
                StateId = customer.Address != null && (long?)customer.Address.StateId != 0 ? (long?)customer.Address.StateId : null,
                CityName = customer.Address != null ? customer.Address.City : null,
                StateName = customer.Address != null ? customer.Address.State : null,
                ZipCode = customer.Address != null ? customer.Address.ZipCode : null,
                CountryId = obj.Address != null ? obj.Address.Country.Id : 1,
                IsNew = obj.Id > 0 ? false : true,
                TypeId = obj.Address != null ? obj.Address.TypeId : 11,
                Id = obj.Address != null ? obj.Address.Id : 0,

            };


            return addressInfo;
        }


        public AnnualSalesDataCustomerViewModel CreateViewModelForCustomers(AuditAddressDiscrepancy auditAddress, List<QbInvoiceList> qbInvoiceList)
        {
            bool canAddressUpdate = false;
            long franchiseeSalesId = auditAddress.FranchiseeSalesId.GetValueOrDefault();
            var franchiseeSales = _franchiseeSalesRepository.IncludeMultiple(x => x.Franchisee.Organization, x => x.Invoice, x => x.Customer.CustomerEmails).Where(x => x.Id == franchiseeSalesId).FirstOrDefault();
            long? invoiceId = auditAddress.FranchiseeSales.InvoiceId;
            var invoiceAddress = _invoiceAddresslRepository.Table.OrderBy(x => x.Id).Where(x => x.InvoiceId == invoiceId).FirstOrDefault();
            string qbInvoice = auditAddress.FranchiseeSales.QbInvoiceNumber;

            var customerEmail = franchiseeSales.Customer;

            if (qbInvoiceList.Select(x => x.InvoiceId).Contains(franchiseeSales.InvoiceId) && qbInvoiceList.Select(x => x.QbInvoice).Contains(franchiseeSales.QbInvoiceNumber))
            {
                canAddressUpdate = true;
            }
            var model = new AnnualSalesDataCustomerViewModel
            {
                Id = auditAddress.Id,
                newAddress = auditAddress.AddressLine1 + " " + auditAddress.AddressLine2 + " " + auditAddress.Country + " " + auditAddress.State + " " + auditAddress.City + " " + auditAddress.ZipCode + " ",
                oldAddress = franchiseeSales.Customer.Address.AddressLine1 + " " + franchiseeSales.Customer.Address.AddressLine2 + " " + franchiseeSales.Customer.Address.Country.Name + " " + (franchiseeSales.Customer.Address.State != null ? franchiseeSales.Customer.Address.State.Name :
                                franchiseeSales.Customer.Address.StateName != null ? franchiseeSales.Customer.Address.StateName : "") + " " + (franchiseeSales.Customer.Address.City != null ? franchiseeSales.Customer.Address.City.Name : franchiseeSales.Customer.Address.CityName != null ? franchiseeSales.Customer.Address.CityName : "") + " " +
                                " " + (franchiseeSales.Customer.Address.ZipCode != null ? franchiseeSales.Customer.Address.ZipCode : franchiseeSales.Customer.Address.Zip != null ? franchiseeSales.Customer.Address.Zip.Code : "") +
                                " ",
                customerName = franchiseeSales.Customer.Name,
                invoiceId = franchiseeSales.CustomerInvoiceId > 0 ? franchiseeSales.CustomerInvoiceId.ToString() : franchiseeSales.InvoiceId.ToString(),
                qbInvoiceId = franchiseeSales.QbInvoiceNumber,
                franchiseeName = franchiseeSales.Franchisee.Organization.Name,
                isDiscrepancyAddress = true,
                invoiceDate = franchiseeSales != null ? (DateTime?)franchiseeSales.Invoice.GeneratedOn.Date : null,
                newphoneNumber = auditAddress.phoneNumber,
                newemailId = auditAddress.email,
                oldphoneNumber = franchiseeSales.Customer.Phone,
                oldemailId = invoiceAddress != null ? invoiceAddress.EmailId : "",
                oldAddressLine1 = invoiceAddress != null ? invoiceAddress.AddressLine1 : "",
                oldAddressLine2 = invoiceAddress != null ? invoiceAddress.AddressLine2 : "",
                oldCity = invoiceAddress != null ? invoiceAddress.City != null ? invoiceAddress.City.Name : invoiceAddress.CityName : "",
                oldState = invoiceAddress != null ? invoiceAddress.State != null ? invoiceAddress.State.ShortName : invoiceAddress.StateName : "",
                oldZip = invoiceAddress != null ? invoiceAddress.Zip != null ? invoiceAddress.Zip.Code : invoiceAddress.ZipCode : "",
                newAddressLine1 = auditAddress != null ? auditAddress.AddressLine1 : "",
                newAddressLine2 = auditAddress != null ? auditAddress.AddressLine2 : "",
                newCity = auditAddress != null ? auditAddress.City : "",
                newState = auditAddress != null ? auditAddress.State : "",
                newZip = auditAddress != null ? auditAddress.ZipCode : "",
                newCountry = franchiseeSales != null && franchiseeSales.Customer != null && franchiseeSales.Customer.Address != null ? franchiseeSales.Customer.Address.Country != null ?
                                                 franchiseeSales.Customer.Address.Country.Name : "" : "",
                oldCountry = invoiceAddress != null ? invoiceAddress.Country != null ? invoiceAddress.Country.Name : "" : "",
                canAddressEditable = canAddressUpdate,
                oldMarketingClass = franchiseeSales.MarketingClass != null ? franchiseeSales.MarketingClass.Name : "",
                newMarketingClass = auditAddress != null && auditAddress.MarketingClass != null ? auditAddress.MarketingClass.Name : ""
            };
            return model;
        }

        public AnnualSalesDataCustomerViewModel CreateViewModelForCustomers(AddressHistryLog addressLog, List<QbInvoiceList> qbInvoiceList)
        {
            bool canAddressUpdate = false;
            long? invoiceId = addressLog.FranchiseeSales.InvoiceId;
            var invoiceAddress = _invoiceAddresslRepository.Table.OrderBy(x => x.Id).Where(x => x.InvoiceId == addressLog.FranchiseeSales.InvoiceId).FirstOrDefault();
            string qbInvoice = addressLog.FranchiseeSales.QbInvoiceNumber;
            var franchiseeSales = _franchiseeSalesRepository.Get(addressLog.FranchiseeSalesId.GetValueOrDefault());
            if (qbInvoiceList.Select(x => x.InvoiceId).Contains(invoiceId) && qbInvoiceList.Select(x => x.QbInvoice).Contains(qbInvoice))
            {
                canAddressUpdate = true;
            }
            var model = new AnnualSalesDataCustomerViewModel
            {
                Id = addressLog.Id,
                customerName = franchiseeSales != null ? franchiseeSales.Customer.Name : "",
                invoiceId = franchiseeSales != null && franchiseeSales.CustomerInvoiceId <= 0 ? franchiseeSales.InvoiceId.ToString() : franchiseeSales.CustomerInvoiceId.ToString(),
                qbInvoiceId = franchiseeSales != null ? franchiseeSales.QbInvoiceNumber : "",
                franchiseeName = franchiseeSales != null ? franchiseeSales.Franchisee.Organization.Name : "",
                isDiscrepancyAddress = false,
                invoiceDate = franchiseeSales != null ? (DateTime?)franchiseeSales.Invoice.GeneratedOn.Date : null,
                newemailId = (franchiseeSales.Customer.CustomerEmails.Count() > 0 ? franchiseeSales.Customer.CustomerEmails.FirstOrDefault().Email : ""),
                newphoneNumber = (franchiseeSales.Customer.Phone != null ? franchiseeSales.Customer.Phone : ""),
                oldemailId = addressLog.EmailId,
                oldphoneNumber = addressLog.phoneNumber,
                oldAddressLine1 = addressLog != null ? (addressLog.AddressLine1) : "",
                oldAddressLine2 = addressLog != null ? (addressLog.AddressLine2) : "",
                oldCity = (addressLog != null && addressLog.City != null) ? addressLog.City.Name : addressLog.CityName,
                oldState = (addressLog.State != null ? addressLog.State.ShortName : addressLog.StateName),
                oldZip = (addressLog.ZipCode != null ? addressLog.ZipCode : addressLog.Zip != null ? addressLog.Zip.Code : ""),
                newAddressLine1 = invoiceAddress != null ? invoiceAddress.AddressLine1 : "",
                newAddressLine2 = invoiceAddress != null ? invoiceAddress.AddressLine2 : "",
                newCity = (invoiceAddress != null && invoiceAddress.City != null) ? invoiceAddress.City.Name : invoiceAddress.CityName,
                newState = (invoiceAddress != null && invoiceAddress.State != null) ? invoiceAddress.State.ShortName : invoiceAddress.StateName,
                newZip = (invoiceAddress != null && invoiceAddress.Zip != null) ? invoiceAddress.Zip.Code : invoiceAddress.ZipCode,
                newCountry = (invoiceAddress != null && invoiceAddress.Country != null) ? invoiceAddress.Country.Name : "",
                oldCountry = (addressLog != null && addressLog.Country != null) ? addressLog.Country.Name : "",
                canAddressEditable = canAddressUpdate,
                oldMarketingClass = franchiseeSales.MarketingClass.Name,
                newMarketingClass = addressLog != null && addressLog.MarketingClass != null ? addressLog.MarketingClass.Name : ""
            };
            return model;
        }

        public CustomerEmail CreateDomain(Customer customer, string email)
        {
            var newcustomerInfo = new CustomerEmail()
            {
                Customer = customer,
                CustomerId = customer.Id,
                Email = email != null ? email : "",
                DateCreated = DateTime.Now,
                IsNew = true
            };

            return newcustomerInfo;
        }


        public AuditAddress CreateViewModel(CustomerCreateEditModel auditInvoice)
        {
            if (auditInvoice.MarketingClassId >= 14 && auditInvoice.MarketingClassId <= 20)
            {
                auditInvoice.MarketingClassId = null;
            }
            else if (auditInvoice.MarketingClassId >= 8 && auditInvoice.MarketingClassId <= 10)
            {
                auditInvoice.MarketingClassId = null;
            }

            var newcustomerInfo = new AuditAddress()
            {
                AddressLine1 = auditInvoice.Address != null ? auditInvoice.Address.AddressLine1 : "",
                AddressLine2 = auditInvoice.Address != null ? auditInvoice.Address.AddressLine2 : "",
                CityId = auditInvoice.Address != null && auditInvoice.Address.CityId != null ? auditInvoice.Address.CityId : null,
                CityName = auditInvoice.Address != null ? auditInvoice.Address.City : "",
                CountryId = auditInvoice.Address != null && auditInvoice.Address.CountryId != null ? auditInvoice.Address.CountryId : 1,
                StateId = auditInvoice.Address != null && auditInvoice.Address.StateId != default(long) ? (long?)auditInvoice.Address.StateId : null,
                StateName = auditInvoice.Address != null ? auditInvoice.Address.State : "",
                ZipCode = auditInvoice.Address != null ? auditInvoice.Address.ZipCode : "",
                ZipId = auditInvoice.Address != null ? auditInvoice.Address.ZipId : null,
                IsNew = true,
                //TypeId = auditInvoice.MarketingClassId != default(long?) || auditInvoice.MarketingClassId != null ? auditInvoice.MarketingClassId.GetValueOrDefault() : 11,
                TypeId = 11,
                Phone = auditInvoice.Address != null ? auditInvoice.Phone : "",
                Email = auditInvoice.Address != null ? auditInvoice.CustomerEmails.Select(x => x.Email).FirstOrDefault() : null,

            };

            return newcustomerInfo;
        }
        public AuditCustomer CreateModel(CustomerCreateEditModel auditInvoice)
        {
            var newcustomerInfo = new AuditCustomer()
            {
                Name = auditInvoice.Name,
                ContactPerson = auditInvoice.ContactPerson,
                Phone = auditInvoice.Phone,
                ClassTypeId = auditInvoice.MarketingClassId.GetValueOrDefault(),
                ReceiveNotification = auditInvoice.ReceiveNotification,
                DateCreated = DateTime.Now,
                TotalSales = auditInvoice.TotalSales,
                NoOfSales = auditInvoice.NoOfSales,
                AvgSales = auditInvoice.AvgSales,
                IsNew = true,

            };

            return newcustomerInfo;
        }

        public InvoiceAddress CreateModel(CustomerCreateEditModel customer, long? invoiceId, long? invoiceAddressId)
        {
            var newcustomerInfo = new InvoiceAddress()
            {
                AddressLine1 = customer.Address != null ? customer.Address.AddressLine1 : "",
                AddressLine2 = customer.Address != null ? customer.Address.AddressLine2 : "",
                TypeId = customer.Address != null ? customer.Address.AddressType : 11,
                CityId = customer.Address != null && (long?)customer.Address.CityId != 0 ? customer.Address.CityId : null,
                StateId = customer.Address != null && (long?)customer.Address.StateId != 0 ? (long?)customer.Address.StateId : null,
                CityName = customer.Address != null ? customer.Address.City : null,
                StateName = customer.Address != null ? customer.Address.State : null,
                InvoiceId = invoiceId,
                CountryId = customer.Address != null ? (long?)customer.Address.CountryId : 1,
                ZipCode = customer.Address != null ? customer.Address.ZipCode : null,
                ZipId = customer.Address != null ? customer.Address.ZipId : null,
                IsNew = invoiceAddressId != null ? false : true,
                EmailId = (customer.CustomerEmails.Count() > 0) ? customer.CustomerEmails.FirstOrDefault().Email : null,
                Phone = customer.Phone,
                Id = invoiceAddressId != null ? invoiceAddressId.GetValueOrDefault() : default(long)
            };
            return newcustomerInfo;
        }

        public AuditFranchiseeSales CreateViewModel(ParsedFileParentModel model, long franchiseeId, long? customerId, long? auditCustomerId, long? accountCreditid = null)
        {

            return new AuditFranchiseeSales()
            {
                IsNew = true,
                FranchiseeId = franchiseeId,
                CustomerId = customerId != null ? customerId : null,
                ClassTypeId = model.MarketingClassId,
                QbInvoiceNumber = model.QbIdentifier,
                AccountCreditId = (accountCreditid != null || accountCreditid != 0) ? accountCreditid : null,
                SalesRep = model.SalesRep,
                AuditCustomerId = auditCustomerId != null ? auditCustomerId : null,
            };
        }

    }
}
