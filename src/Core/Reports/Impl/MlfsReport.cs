using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Reports.ViewModel;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class MlfsReport : IMlfsReport
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        private readonly IRepository<MlfsConfigurationSetting> _mlfsConfigurationSettingRepository;
        private readonly IMlfsReportFactory _mlfsReportFactory;
        private int franchiseeIndex = 0;
        public MlfsReport(IUnitOfWork unitOfWork, IMlfsReportFactory mlfsReportFactory)
        {
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _mlfsConfigurationSettingRepository = unitOfWork.Repository<MlfsConfigurationSetting>();
            _mlfsReportFactory = mlfsReportFactory;
        }
        public MLFSReportListModel GetReportForPurchase(MLFSReportListFilter filter)
        {
            var serviseWiseSum = new List<decimal>();
            var quarterSumList = new List<ValueModel>();
            var yearSumList = new List<ValueModel>();
            int month = DateTime.Now.Month;
            var localServiceSumList = new List<List<decimal>>();
            var totalServiceSumList = new List<List<decimal>>();
            var grandTotalServiceSumList = new List<List<decimal>>();
            var lastDateForSearch = default(int);
            var startDateForSearch = new DateTime();
            var endDateForSearch = new DateTime();
            var franchiseeGroup = GetFranchiseeGroup();
            var customerList = _customerRepository.Table.ToList();
            var quarterYearClass = new QuarterClass();
            var quarterYearListClass = new List<QuarterClass>();
            var yearList = new List<int>();
            if (filter.EndDate != null && filter.StartDate != null)
            {
                startDateForSearch = new DateTime(filter.StartDate, 1, 1);
                endDateForSearch = new DateTime(filter.EndDate, 12, 31);
            }
            else
            {
                startDateForSearch = new DateTime(DateTime.Now.Year - 2, 1, 1);
                endDateForSearch = new DateTime(DateTime.Now.Year, month, lastDateForSearch);
            }

            var quarterYearList = DateRangeHelperService.GetQuarterBetweenYears(startDateForSearch, endDateForSearch);

            var a = 1;
            foreach (var quarterYear in quarterYearList)
            {
                if (a == 4)
                {
                    a = 1;
                    yearList.Add(quarterYear.Item1.Year);
                }
                else
                {
                    ++a;
                }
            }
            int index = 0;
            int serviceWiseIndex = 0;
            for (var a1 = 0; a1 < 5; a1++)
            {

                var yearWiseSum = new List<decimal>();
                grandTotalServiceSumList.Add(new List<decimal>());
                totalServiceSumList.Add(new List<decimal>());
                foreach (var year in yearList)
                {
                    yearWiseSum.Add(0);
                }
                grandTotalServiceSumList[serviceWiseIndex].AddRange(yearWiseSum);
                totalServiceSumList[serviceWiseIndex].AddRange(yearWiseSum);
                serviceWiseIndex++;
            }
            foreach (var franchisee in franchiseeGroup)
            {
                franchisee.FranchiseeModel = GetFranchiseePurchase(franchisee.FranchiseeModel, customerList,
                    quarterYearList, yearList, out quarterSumList, out yearSumList, out localServiceSumList);
                franchisee.QuarterWiseSum = quarterSumList;
                franchisee.QuarterWiseSum.Add(new ValueModel { Color="Black",BackGroundColor="transparent" ,Price = franchisee.FranchiseeModel.Sum(x => x.TotalSumPerFranchisee.Price), IsParsed = false }) ;
                franchisee.YearWiseSum = yearSumList;
                franchisee.Index = index;
                ++index;
                grandTotalServiceSumList = CalculateServiceWiseYearlySum(grandTotalServiceSumList, localServiceSumList, yearList.Count());
            }

            a = 1;
            foreach (var quarterYear in quarterYearList)
            {
                if (a > 4)
                {
                    a = 1;
                }
                quarterYearClass = new QuarterClass();
                quarterYearClass.Quarter = a;
                quarterYearClass.Year = quarterYear.Item1.Year;
                ++a;
                quarterYearListClass.Add(quarterYearClass);
            }
            a = 1;

            var list = franchiseeGroup.ToList();
            var saleUploadData = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == 67).OrderByDescending(x => x.Id).FirstOrDefault();
            return new MLFSReportListModel
            {
                FranchiseeGroupModel = franchiseeGroup,
                QuarterList = quarterYearListClass,
                YearList = yearList,
                StatusList = GetMLFSConfigurationForReport(filter.UserId).MLFSReportConfigurationViewModel,
                ColorStatusWithYearList = GetColorStatusCollection(yearList),
                TotalSum = GetTotalSum(list, ""),
                InternationalFranchiseeSum = GetTotalSum(list, "INTERNATIONAL"),
                LocalFranchiseeSum = GetTotalSum(list, "LOCAL"),
                ActiveFranchiseeSum = GetTotalSum(list, "ACTIVEWITHNew"),
                ActiveFranchiseeSumWithoutNew = GetTotalSum(list, "ACTIVEWITHOUtNew"),
                EndDate = saleUploadData.PeriodEndDate.ToString("MM/dd/yyyy"),
                TotalServiceSum = grandTotalServiceSumList,
                ActiveLocalFranchiseeSumWithNew= GetTotalSum(list, "LOCALACTIVEWITHNew"),
                ActiveLocalFranchiseeSumWithoutNew = GetTotalSum(list, "LOCALACTIVEWITHOUTNew"),
            };
        }

        private List<FranchiseeGroupModel> GetFranchiseeGroup()
        {
            var franchiseeList = _franchiseeRepository.Table.Where(x => x.Id != 2 && !x.Organization.Name.ToUpper().StartsWith("0-")).ToList();
            var mlfsGroups = MLFSReportGrouping.GetGroup();
            var mlfsGroupsFranchisee = MLFSReportGrouping.GetGroupedFranchisee(franchiseeList).ToList();
            var franchiseeModelList = new List<FranchiseeGroupModel>();

            foreach (var mlfsGroup in mlfsGroups)
            {
                franchiseeModelList.Add(new FranchiseeGroupModel
                {
                    GroupName = mlfsGroup,
                    FranchiseeModel = mlfsGroupsFranchisee.FirstOrDefault(x => x.GroupName == mlfsGroup).FranchiseeModel.ToList()
                });
            }

            return franchiseeModelList;
        }


        private List<FranchiseeModel> GetFranchiseeSales(List<FranchiseeModel> franchiseeModels
            , List<Franchisee> franchisees, IEnumerable<Tuple<DateTime, DateTime>> quarterYearList, List<int> yearList,
            out List<ValueModel> quarterSumList, out List<ValueModel> yearWiseSumList, out List<List<decimal>> localServiceSumList)
        {
            localServiceSumList = new List<List<decimal>> { };
            var serviceWiseSum = new List<List<decimal>> { };
            var serviceList = new List<string>() { "ENDURACRETE", "GROUTLIFE", "STONELIFE", "VINYLGUARD", "WOODLIFE" };
            quarterSumList = new List<ValueModel>();
            yearWiseSumList = new List<ValueModel>();

            foreach (var quarterYear in quarterYearList)
            {
                quarterSumList.Add(new ValueModel { Price=0,IsParsed=false });
            }
            foreach (var year in yearList)
            {
                yearWiseSumList.Add(new ValueModel { Price=0, Color="Black", BackGroundColor="Transparent", IsParsed=false });
            }
            var indexForFranchisee = 0;
            franchiseeIndex = 0;
            var serviceWiseIndex = 0;
            foreach (var service in serviceList)
            {

                var yearWiseSum = new List<decimal>();
                localServiceSumList.Add(new List<decimal>());
                foreach (var year in yearList)
                {
                    yearWiseSum.Add(0);
                }
                localServiceSumList[serviceWiseIndex].AddRange(yearWiseSum);
                serviceWiseIndex++;
            }

            foreach (var franchiseeModel in franchiseeModels)
            {
                serviceWiseIndex = 0;
                serviceWiseSum = new List<List<decimal>>();
                //localServiceSumList = new List<List<decimal>>();
                foreach (var service in serviceList)
                {

                    var yearWiseSum = new List<decimal>();
                    serviceWiseSum.Add(new List<decimal>());
                    foreach (var year in yearList)
                    {
                        yearWiseSum.Add(0);
                    }
                    serviceWiseSum[serviceWiseIndex].AddRange(yearWiseSum);
                    serviceWiseIndex++;
                }
                indexForFranchisee = 0;
                franchiseeModel.Index = franchiseeIndex;
                ++franchiseeIndex;
                var yearWiseTotalSum = new List<decimal>();
                var yearWiseTotal = new ColorStatusClass();
                var franchiseeDomain = new Franchisee();
                var purchaseYearModel = new PriceAndYearModel();
                var purchaseYearModelList = new List<PriceAndYearModel>();
                var salesModelList = new List<PriceModel>();
                purchaseYearModel.PriceModel = new List<PriceModel>();
                var purchaseModel = new PriceModel();
                var isForFirstYear = true;
                var franchiseeNameUpper = franchiseeModel.FranchiseeName.ToUpper();

                string[] franchiseesplittedNane = new string[3];
                if (!franchiseeNameUpper.StartsWith("I-CANADA"))
                {
                    franchiseesplittedNane = franchiseeNameUpper.Split(new string[] { " (" }, System.StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    franchiseesplittedNane[0] = franchiseeNameUpper;
                }
                franchiseeDomain = franchisees.FirstOrDefault(x => x.Organization.Name.ToUpper().Contains(franchiseesplittedNane[0].ToUpper()));
                serviceWiseIndex = 0;
                if (franchiseeDomain != null)
                {
                    franchiseeModel.Status = franchiseeDomain.Organization.IsActive ? "Active" : "InActive";
                    var invoiceList = _franchiseeSalesRepository.Table.Where(x => x.FranchiseeId == franchiseeDomain.Id
                     && x.Invoice.GeneratedOn != null).Select(x => x.Invoice).ToList();

                    purchaseModel = new PriceModel();
                    salesModelList = new List<PriceModel>();
                    int a = 1;
                    var yearSum = default(decimal);
                    int index = 0;
                    int yearIndex = 0;
                    foreach (var quarterYear in quarterYearList)
                    {
                        yearWiseTotal = new ColorStatusClass();
                        purchaseYearModel = new PriceAndYearModel();
                        salesModelList = new List<PriceModel>();
                        purchaseYearModel.PriceModel = new List<PriceModel>();
                        purchaseYearModel.TotalSum = 0;
                        var startDateForQuarter = quarterYear.Item1;
                        var endDateForQuarter = quarterYear.Item2.AddDays(1);
                        var invoiceIds = invoiceList.Where(x => x.GeneratedOn >= startDateForQuarter
                        && x.GeneratedOn < endDateForQuarter).Select(x => x.Id).ToList();
                        var invoiceItemList = _invoiceItemRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
                        serviceWiseIndex = 0;
                        foreach (var service in serviceList)
                        {
                            purchaseModel = new PriceModel();
                            var serviceDomain = _serviceTypeRepository.Table.FirstOrDefault(x => x.Name.Contains(service)
                            || x.Alias.Contains(service));
                            if (serviceDomain == null) { }
                            var invoiceListForService = invoiceItemList.Where(x => x.ItemId == serviceDomain.Id).ToList();
                            if (invoiceListForService.Count() > 0)
                                purchaseModel.Price = invoiceListForService.Sum(x => x.Amount);
                            else
                                purchaseModel.Price = 0;

                            purchaseModel.MarketingClass = service;
                            salesModelList.Add(purchaseModel);
                            serviceWiseSum[serviceWiseIndex][yearIndex] += purchaseModel.Price;
                            localServiceSumList[serviceWiseIndex][yearIndex] += purchaseModel.Price;
                            //totalServiceWiseSum[serviceWiseIndex][yearIndex] += purchaseModel.Price;
                            serviceWiseIndex++;
                        }
                        serviceWiseIndex = 0;
                        if (isForFirstYear)
                        {
                            if (salesModelList.Sum(x => x.Price) > 0)
                            {
                                isForFirstYear = false;
                                franchiseeModel.FirstYear = startDateForQuarter.Year;
                            }
                        }
                        purchaseYearModel.Year = quarterYear.Item1.Year;
                        purchaseYearModel.Quarter = a;
                        purchaseYearModel.TotalSum = salesModelList.Sum(x => x.Price);
                        purchaseYearModel.BackColor = "transparent";
                        purchaseYearModel.IsParsed = false;
                        purchaseYearModel.Color = "black";
                        ++a;
                        yearSum += salesModelList.Sum(x => x.Price);
                        purchaseYearModel.PriceModel.AddRange(salesModelList);
                        franchiseeModel.PurchasePriceModel.Add(purchaseYearModel);
                        quarterSumList[index].Price += salesModelList.Sum(x => x.Price);
                        ++index;
                        if (a == 5)
                        {
                            ++yearIndex;
                            serviceWiseIndex = 0;
                            //serviceWiseIndex;
                            yearWiseSumList[indexForFranchisee].Price += yearSum;
                            ++indexForFranchisee;
                            yearWiseTotal.Value = yearSum;
                            yearWiseTotal.Color = "Black";
                            yearWiseTotal.BackGroundColor = "Transparent";
                            franchiseeModel.YearWiseTotalSum.Add(yearWiseTotal);
                            yearSum = default(decimal);
                            yearWiseTotalSum = new List<decimal>();
                            a = 1;
                        }
                    }

                }
                else
                {
                    purchaseModel = new PriceModel();
                    salesModelList = new List<PriceModel>();
                    int a = 1;
                    var yearSum = default(decimal);
                    int yearIndex = 0;
                    foreach (var quarterYear in quarterYearList)
                    {
                        purchaseYearModel = new PriceAndYearModel();
                        salesModelList = new List<PriceModel>();
                        purchaseYearModel.PriceModel = new List<PriceModel>();
                        var startDateForQuarter = quarterYear.Item1;
                        var endDateForQuarter = quarterYear.Item2;

                        foreach (var service in serviceList)
                        {
                            purchaseModel = new PriceModel();
                            purchaseModel.Price = 0;
                            purchaseModel.MarketingClass = service;
                            salesModelList.Add(purchaseModel);
                            serviceWiseSum[serviceWiseIndex][yearIndex] += purchaseModel.Price;

                        }
                        purchaseYearModel.Year = quarterYear.Item1.Year;
                        purchaseYearModel.Quarter = a;
                        ++a;
                        purchaseYearModel.BackColor = "transparent";
                        purchaseYearModel.IsParsed = false;
                        purchaseYearModel.Color = "black";
                        yearSum += salesModelList.Sum(x => x.Price);
                        yearWiseTotalSum.Add(yearSum);
                        purchaseYearModel.PriceModel.AddRange(salesModelList);
                        franchiseeModel.PurchasePriceModel.Add(purchaseYearModel);
                        if (a == 5)
                        {
                            ++yearIndex;
                            yearWiseSumList[indexForFranchisee].Price += yearSum;
                            ++indexForFranchisee;
                            yearWiseTotal.Value = yearSum;
                            yearWiseTotal.Color = "Black";
                            yearWiseTotal.BackGroundColor = "Transparent";
                            franchiseeModel.YearWiseTotalSum.Add(yearWiseTotal);
                            yearSum = default(decimal);
                            yearWiseTotalSum = new List<decimal>();
                            yearWiseTotal = new ColorStatusClass();
                            a = 1;
                        }
                    }
                }
                franchiseeModel.ServiceWiseSum = serviceWiseSum;
                franchiseeModel.TotalSumPerFranchisee =new ValueModel { Price= franchiseeModel.PurchasePriceModel.Sum(x => x.TotalSum), IsParsed=false, BackGroundColor="transparent", Color="black" };
                franchiseeModel.IsExpand = false;
                franchiseeModel.YearlyServiceWiseSum = CalculateServiceWiseYearlySum(serviceWiseSum, serviceList.Count());
            }
            return franchiseeModels;
        }


        public MLFSReportListModel GetReportForSale(MLFSReportListFilter filter)
        {
            var saleUploadData = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == 67).OrderByDescending(x => x.Id).FirstOrDefault();
            var yearSumList = new List<ValueModel>();
            var quarterSumList = new List<ValueModel>();
            int month = DateTime.Now.Month;
            var lastDateForSearch = default(int);
            var startDateForSearch = new DateTime();
            var localServiceSumList = new List<List<decimal>>();
            var totalServiceSumList = new List<List<decimal>>();
            var endDateForSearch = new DateTime();
            var franchiseeGroup = GetFranchiseeGroup();
            var franchiseeList = _franchiseeRepository.Table.ToList();
            var yearList = new List<int>();
            var quarterYearClass = new QuarterClass();

            var quarterYearListClass = new List<QuarterClass>();
            if (filter.EndDate != null && filter.StartDate != null)
            {
                startDateForSearch = new DateTime(filter.StartDate, 1, 1);
                endDateForSearch = new DateTime(filter.EndDate, 12, 31);
            }
            else
            {
                startDateForSearch = new DateTime(DateTime.Now.Year - 2, 1, 1);
                endDateForSearch = new DateTime(DateTime.Now.Year, 12, 31);
            }

            var quarterYearList = DateRangeHelperService.GetQuarterBetweenYears(startDateForSearch, endDateForSearch);
            int index = 0;
            var a = 1;

            foreach (var quarterYear in quarterYearList)
            {
                if (a > 3)
                {
                    yearList.Add(quarterYear.Item1.Year);
                    a = 1;
                }
                else
                {
                    ++a;
                }
            }
            int frachiseeIndex = 0;
            int indexes = 0;
            var totalServiceWiseSum = new List<List<decimal>> { };
            int serviceWiseIndex = 0;
            for (var i = 0; i < 5; i++)
            {
                var yearWiseSum = new List<decimal>();
                totalServiceWiseSum.Add(new List<decimal>());
                totalServiceSumList.Add(new List<decimal>());
                foreach (var year in yearList)
                {
                    yearWiseSum.Add(0);
                }
                totalServiceWiseSum[serviceWiseIndex].AddRange(yearWiseSum);
                totalServiceSumList[serviceWiseIndex].AddRange(yearWiseSum);
                serviceWiseIndex++;
            }
            foreach (var franchisee in franchiseeGroup)
            {

                franchisee.FranchiseeModel = GetFranchiseeSales(franchisee.FranchiseeModel, franchiseeList,
                    quarterYearList, yearList, out quarterSumList, out yearSumList, out localServiceSumList);
                franchisee.QuarterWiseSum = quarterSumList;
                franchisee.QuarterWiseSum.Add(new ValueModel { Color = "Black", BackGroundColor = "transparent", Price = franchisee.FranchiseeModel.Sum(x => x.TotalSumPerFranchisee.Price), IsParsed=false });
                franchisee.YearWiseSum = yearSumList;
                franchisee.Index = indexes;
                totalServiceSumList = CalculateServiceWiseYearlySum(totalServiceSumList, localServiceSumList, yearList.Count());
                indexes++;
            }
            a = 1;
            foreach (var quarterYear in quarterYearList)
            {
                if (a > 4)
                {
                    a = 1;
                }
                quarterYearClass = new QuarterClass();
                quarterYearClass.Quarter = a;
                quarterYearClass.Year = quarterYear.Item1.Year;
                ++a;
                quarterYearListClass.Add(quarterYearClass);
            }
            a = 1;

            var list = franchiseeGroup.ToList();
            return new MLFSReportListModel
            {
                FranchiseeGroupModel = franchiseeGroup,
                QuarterList = quarterYearListClass,
                YearList = yearList,
                StatusList = GetMLFSConfigurationForReport(filter.UserId).MLFSReportConfigurationViewModel,
                ColorStatusWithYearList = GetColorStatusCollection(yearList),
                TotalSum = GetTotalSum(list, ""),
                InternationalFranchiseeSum = GetTotalSum(list, "INTERNATIONAL"),
                LocalFranchiseeSum = GetTotalSum(list, "LOCAL"),
                ActiveFranchiseeSum = GetTotalSum(list, "ACTIVEWITHNew"),
                ActiveFranchiseeSumWithoutNew = GetTotalSum(list, "ACTIVEWITHOUtNew"),
                EndDate = saleUploadData.PeriodEndDate.ToString("MM/dd/yyyy"),
                TotalServiceSum = totalServiceSumList,
                ActiveLocalFranchiseeSumWithNew = GetTotalSum(list, "LOCALACTIVEWITHNew"),
                ActiveLocalFranchiseeSumWithoutNew = GetTotalSum(list, "LOCALACTIVEWITHOUTNew"),
            };
        }


        private List<FranchiseeModel> GetFranchiseePurchase(List<FranchiseeModel> franchiseeModels
           , List<Customer> customers, IEnumerable<Tuple<DateTime, DateTime>> quarterYearList, List<int> yearList,
            out List<ValueModel> quarterSumList, out List<ValueModel> yearWiseSumList, out List<List<decimal>> localServiceSumList)
        {
            var serviceWiseSum = new List<List<decimal>> { };
            localServiceSumList = new List<List<decimal>> { };
            var serviceList = new List<string>() { "ENDURACRETE", "GROUTLIFE", "STONELIFE", "VINYLGUARD", "WOODLIFE" };
            quarterSumList = new List<ValueModel>();
            yearWiseSumList = new List<ValueModel>();

            localServiceSumList = new List<List<decimal>>();
            var serviceWiseIndex = 0;
            foreach (var service in serviceList)
            {

                var yearWiseSum = new List<decimal>();
                localServiceSumList.Add(new List<decimal>());
                foreach (var year in yearList)
                {
                    yearWiseSum.Add(0);
                }
                localServiceSumList[serviceWiseIndex].AddRange(yearWiseSum);
                serviceWiseIndex++;
            }

            int index1 = 0;
            foreach (var quarterYear in quarterYearList)
            {
                quarterSumList.Add(new ValueModel { IsParsed=false, Price= 0 });
            }

            foreach (var year in yearList)
            {
                yearWiseSumList.Add(new ValueModel { Color="Black", BackGroundColor="transparent", IsParsed=false,Price= 0 });
            }
            int indexForFranchisee = 0;
            int franchiseeIndex = 0;
            foreach (var franchiseeModel in franchiseeModels)
            {
                serviceWiseIndex = 0;
                serviceWiseSum = new List<List<decimal>>();
                
                foreach (var service in serviceList)
                {

                    var yearWiseSum = new List<ValueModel>();
                    serviceWiseSum.Add(new List<decimal>());
                    foreach (var year in yearList)
                    {
                        yearWiseSum.Add(new ValueModel { Price= 0, IsParsed=false, BackGroundColor="transparent", Color="Black" });
                    }
                    serviceWiseSum[serviceWiseIndex].AddRange(yearWiseSum.Select(x=>x.Price));
                    serviceWiseIndex++;
                }
                franchiseeModel.Index = franchiseeIndex;
                ++franchiseeIndex;
                indexForFranchisee = 0;
                var yearWiseTotalSum = new List<decimal>();
                var yearWiseTotal = new ColorStatusClass();
                var purchaseYearModel = new PriceAndYearModel();
                var purchaseYearModelList = new List<PriceAndYearModel>();
                var purchaseModelList = new List<PriceModel>();
                purchaseYearModel.PriceModel = new List<PriceModel>();
                var purchaseModel = new PriceModel();
                var isForFirstYear = true;
                var franchiseeNameUpper = franchiseeModel.FranchiseeName.ToUpper();
                string[] franchiseesplittedNane = new string[3];
                if (!franchiseeNameUpper.StartsWith("I-CANADA"))
                {
                    franchiseesplittedNane = franchiseeNameUpper.Split(new string[] { " (" }, System.StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    franchiseesplittedNane[0] = franchiseeNameUpper;
                }
                if (franchiseeNameUpper.Contains("GA-COLUMBUS"))
                {
                    franchiseeNameUpper = franchiseesplittedNane[0];
                }

                var franchiseeDomain = _franchiseeRepository.Table.FirstOrDefault(x => franchiseeNameUpper.Contains(x.Organization.Name));
                if (franchiseeDomain != null)
                {
                    franchiseeModel.Status = franchiseeDomain.Organization.IsActive ? "Active" : "Inactive";
                }
                if (franchiseeNameUpper == "CA-LOS ANGELES (SOUTH)".ToUpper())
                {
                    franchiseeNameUpper = "CA-LOS ANGELES-S";

                }
                else if (franchiseeNameUpper == "CA-LOS ANGELES (NORTH)".ToUpper())
                {
                    franchiseeNameUpper = "CA-LOS ANGELES-N";
                }
                var customerDomain = customers.FirstOrDefault(x => x.Name.ToUpper().Contains(franchiseeNameUpper));
                var customerIds = customers.Where(x => x.Name.ToUpper().Contains(franchiseeNameUpper)).Select(x => x.Id).ToList();

                if (customerDomain != null)
                {
                    var invoiceList = _franchiseeSalesRepository.Table.Where(x => customerIds.Contains(x.CustomerId)
                    && x.FranchiseeId == 67 && x.Invoice.GeneratedOn != null).Select(x => x.Invoice).ToList();
                    purchaseModel = new PriceModel();
                    purchaseModelList = new List<PriceModel>();
                    int a = 1;
                    var yearSum = default(decimal);
                    int index = 0;
                    int yearIndex = 0;
                    foreach (var quarterYear in quarterYearList)
                    {
                        yearWiseTotal = new ColorStatusClass();
                        purchaseYearModel = new PriceAndYearModel();
                        purchaseModelList = new List<PriceModel>();
                        purchaseYearModel.PriceModel = new List<PriceModel>();
                        purchaseYearModel.TotalSum = 0;
                        var startDateForQuarter = quarterYear.Item1;
                        var endDateForQuarter = quarterYear.Item2.AddDays(1);
                        var invoiceIds = invoiceList.Where(x => x.GeneratedOn >= startDateForQuarter
                        && x.GeneratedOn < endDateForQuarter).Select(x => x.Id).ToList();
                        var invoiceItemList = _invoiceItemRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
                        serviceWiseIndex = 0;
                        foreach (var service in serviceList)
                        {
                            purchaseModel = new PriceModel();
                            var serviceDomain = _serviceTypeRepository.Table.FirstOrDefault(x => x.Name.Contains(service)
                            || x.Alias.Contains(service));
                            if (serviceDomain == null) continue;
                            var invoiceListForService = invoiceItemList.Where(x => x.ItemId == serviceDomain.Id).ToList();
                            if (invoiceListForService.Count() > 0)
                                purchaseModel.Price = invoiceListForService.Sum(x => x.Amount);
                            else
                                purchaseModel.Price = 0;

                            purchaseModel.MarketingClass = service;
                            purchaseModelList.Add(purchaseModel);
                            localServiceSumList[serviceWiseIndex][yearIndex] += purchaseModel.Price;
                            serviceWiseSum[serviceWiseIndex][yearIndex] += purchaseModel.Price;
                            serviceWiseIndex++;
                        }

                        if (isForFirstYear)
                        {
                            if (purchaseModelList.Sum(x => x.Price) > 0)
                            {
                                isForFirstYear = false;
                                franchiseeModel.FirstYear = startDateForQuarter.Year;
                            }
                        }
                        purchaseYearModel.Year = quarterYear.Item1.Year;
                        purchaseYearModel.Quarter = a;
                        purchaseYearModel.TotalSum = purchaseModelList.Sum(x => x.Price);


                        yearSum += purchaseModelList.Sum(x => x.Price);
                        yearWiseTotalSum.Add(yearSum);
                        purchaseYearModel.PriceModel.AddRange(purchaseModelList);
                        franchiseeModel.PurchasePriceModel.Add(purchaseYearModel);
                        quarterSumList[index].Price += purchaseModelList.Sum(x => x.Price);
                        purchaseYearModel.BackColor = "transparent";
                        purchaseYearModel.IsParsed = false;
                        purchaseYearModel.Color = "black";
                        index++;
                        ++a;
                        if (a == 5)
                        {
                            ++yearIndex;
                            serviceWiseIndex = 0;
                            yearWiseSumList[indexForFranchisee].Price += yearSum;
                            ++indexForFranchisee;
                            yearWiseTotal.Value = yearSum;
                            yearWiseTotal.Color = "Black";
                            yearWiseTotal.Year = quarterYear.Item1.Year;
                            yearWiseTotal.BackGroundColor = "Transparent";
                            franchiseeModel.YearWiseTotalSum.Add(yearWiseTotal);
                            yearSum = default(decimal);
                            yearWiseTotalSum = new List<decimal>();
                            yearWiseTotal = new ColorStatusClass();
                            a = 1;
                        }
                    }
                }
                else
                {

                    purchaseModel = new PriceModel();
                    purchaseModelList = new List<PriceModel>();
                    int a = 1;
                    var yearSum = default(decimal);
                    int yearIndex = 0;
                    foreach (var quarterYear in quarterYearList)
                    {
                        purchaseYearModel = new PriceAndYearModel();
                        purchaseModelList = new List<PriceModel>();
                        purchaseYearModel.PriceModel = new List<PriceModel>();
                        var startDateForQuarter = quarterYear.Item1;
                        var endDateForQuarter = quarterYear.Item2;
                        serviceWiseIndex = 0;
                        foreach (var service in serviceList)
                        {
                            purchaseModel = new PriceModel();
                            purchaseModel.Price = 0;
                            purchaseModel.MarketingClass = service;
                            purchaseModelList.Add(purchaseModel);
                            serviceWiseSum[serviceWiseIndex][yearIndex] += purchaseModel.Price;
                            serviceWiseIndex++;

                        }

                        purchaseYearModel.Year = quarterYear.Item1.Year;
                        purchaseYearModel.Quarter = a;
                        purchaseYearModel.BackColor = "transparent";
                        purchaseYearModel.IsParsed = false;
                        purchaseYearModel.Color = "black";
                        ++a;
                        yearSum += purchaseModelList.Sum(x => x.Price);
                        yearWiseTotalSum.Add(yearSum);
                        purchaseYearModel.PriceModel.AddRange(purchaseModelList);
                        franchiseeModel.PurchasePriceModel.Add(purchaseYearModel);

                        if (a == 5)
                        {
                            ++yearIndex;
                            serviceWiseIndex = 0;
                            yearWiseSumList[indexForFranchisee].Price += yearSum;
                            ++indexForFranchisee;
                            yearWiseTotal.Value = yearSum;
                            yearWiseTotal.Color = "Black";
                            yearWiseTotal.BackGroundColor = "Transparent";
                            franchiseeModel.YearWiseTotalSum.Add(yearWiseTotal);
                            franchiseeModel.YearWiseTotalSum.Add(yearWiseTotal);
                            yearSum = default(decimal);
                            yearWiseTotalSum = new List<decimal>();
                            yearWiseTotal = new ColorStatusClass();
                            a = 1;
                        }
                    }
                }
                franchiseeModel.ServiceWiseSum = serviceWiseSum;
                franchiseeModel.TotalSumPerFranchisee =new ValueModel { Price = franchiseeModel.PurchasePriceModel.Sum(x => x.TotalSum), Color="black", BackGroundColor="transparent", IsParsed=false };
                franchiseeModel.IsExpand = false;
                franchiseeModel.YearlyServiceWiseSum = CalculateServiceWiseYearlySum(serviceWiseSum, serviceList.Count());

            }
            return franchiseeModels;
        }


        public MLFSReportConfigurationListModel GetMLFSConfiguration(MLFSConfigurationFilter filter)
        {
            var mlfsConfigurationList = _mlfsConfigurationSettingRepository.Table.Where(x => x.IsActive && x.UserId == filter.UserId).ToList();
            var mLFSReportConfigurationViewModel = mlfsConfigurationList.Select(x => _mlfsReportFactory.CreateViewModel(x)).ToList();
            return new MLFSReportConfigurationListModel()
            {
                MLFSReportConfigurationViewModel = mLFSReportConfigurationViewModel
            };
        }

        public bool SaveMLFSConfiguration(MLFSEditModel editModel)
        {
            try
            {
                var ids = editModel.StatusList.Select(x => x.Id).ToList();
                var mlfsDomainList = _mlfsConfigurationSettingRepository.Table.Where(x => x.IsActive && x.UserId == editModel.UserId && !ids.Contains(x.Id)).ToList();
                foreach (var status in editModel.StatusList)
                {
                    var domain = _mlfsReportFactory.CreateDomain(status);
                    domain.UserId = editModel.UserId;
                    _mlfsConfigurationSettingRepository.Save(domain);
                }
                if (mlfsDomainList.Count() > 0)
                {
                    foreach (var mlfsDomain in mlfsDomainList)
                    {
                        _mlfsConfigurationSettingRepository.Delete(mlfsDomain);
                    }

                }
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        private MLFSReportConfigurationListModel GetMLFSConfigurationForReport(long? userId)
        {
            var mlfsConfigurationList = _mlfsConfigurationSettingRepository.Table.Where(x => x.IsActive && x.UserId == userId).ToList();
            var mLFSReportConfigurationViewModel = mlfsConfigurationList.Select(x => _mlfsReportFactory.CreateViewModel(x)).ToList();
            return new MLFSReportConfigurationListModel()
            {
                MLFSReportConfigurationViewModel = mLFSReportConfigurationViewModel
            };
        }


        private List<ColorStatusClass> GetColorStatusCollection(List<int> yearList)
        {
            var list = new List<ColorStatusClass>();
            var colorStatusClass = new ColorStatusClass();
            for (int a = 0; a < yearList.Count(); a++)
            {
                colorStatusClass.Color = "black";
                colorStatusClass.Value = yearList[a];
                colorStatusClass.BackGroundColor = "Transparent";
                colorStatusClass.IsParsed = false;
                list.Add(colorStatusClass);
            }
            return list;
        }
        private List<decimal> GetTotalSum(List<FranchiseeGroupModel> franchiseeList, string searchString)
        {
            string[] searchStringList = { "INTERNATIONAL", "INTERNATIONAL CANADIAN" };
            
            if (searchString == "INTERNATIONAL")
            {
                franchiseeList = franchiseeList.Where(x => searchStringList.Contains(x.GroupName)).ToList();
            }
            else if (searchString == "LOCALACTIVEWITHOUTNew")
            {
                franchiseeList = franchiseeList.Where(x => !(x.GroupName == "CLOSED") && !(x.GroupName == "NEW") && !searchStringList.Contains(x.GroupName)).ToList();
            }
            else if (searchString == "ACTIVEWITHNew")
            {
                franchiseeList = franchiseeList.Where(x => !(x.GroupName == "CLOSED")).ToList();
            }
            else if (searchString == "ACTIVEWITHOUtNew")
            {
                franchiseeList = franchiseeList.Where(x => !(x.GroupName == "CLOSED") && !(x.GroupName == "NEW")).ToList();
            }
            else if (searchString == "LOCALACTIVEWITHNew")
            {
                franchiseeList = franchiseeList.Where(x => !(x.GroupName == "CLOSED") && !searchStringList.Contains(x.GroupName)).ToList();
            }
            else if (searchString == "LOCAL")
            {
                franchiseeList = franchiseeList.Where(x => !searchStringList.Contains(x.GroupName)).ToList();
            }
            var count = franchiseeList.FirstOrDefault().QuarterWiseSum.Count();
            var totalSum = new List<decimal>(count);
            for (int a = 0; a < count; a++)
            {
                totalSum.Add(0);
            }
            foreach (var franchisee in franchiseeList)
            {
                int index = 0;
                foreach (var sum in franchisee.QuarterWiseSum)
                {
                    totalSum[index] += sum.Price;
                    ++index;
                }
            }
            return totalSum;
        }


        private List<decimal> GetTotalSumPerYear(List<int> yearList, List<ColorStatusClass> totalSum)
        {
            var totalPerYear = new List<decimal>() { };
            foreach (var year in yearList)
            {
                var listPerYear = totalSum.Where(x => x.Year == year).ToList();
                totalPerYear.Add(listPerYear.Sum(x => x.Value));
            }
            return null;
        }

        private List<decimal> CalculateServiceWiseYearlySum(List<List<decimal>> totalSum, int serviceCount)
        {
            var serviceWiseList = new List<decimal>();
            for (int a = 0; a < serviceCount; a++)
            {
                serviceWiseList.Add(totalSum[a].Sum());
            }
            return serviceWiseList;
        }

        private List<List<decimal>> CalculateServiceWiseYearlySum(List<List<decimal>> totalSum, List<List<decimal>> calculatedSum, decimal yearList)
        {
            int serviceIndex = -1;
            int yearIndex = -1;
            for (int a = 0; a < 5; a++)
            {
                ++serviceIndex;
                yearIndex = -1;
                for (var b = 0; b < yearList; b++)
                {
                    ++yearIndex;
                    totalSum[serviceIndex][yearIndex] += calculatedSum[serviceIndex][yearIndex];
                }
            }
            return totalSum;
        }
    }
}
