using Core.Application;
using Core.Application.Attribute;
using System.Linq;
using Taxjar;
using Core.Organizations;
using Core.Scheduler.Domain;
using System;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class SalesTaxAPIServices : ISalesTaxAPIServices
    {
        private ILogService _logService;
        private ISettings _settings;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly IJobFactory _jobFactory;
        private readonly IPdfFileService _pdfFileService;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<SalesTaxRates> _salesTaxRatesRepository;

        public SalesTaxAPIServices(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock, IJobFactory jobFactory, IEstimateInvoiceFactory estimateInvoiceFactory, IPdfFileService pdfFileService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _pdfFileService = pdfFileService;
            _jobFactory = jobFactory;
            _organizationRepository = unitOfWork.Repository<Organization>();
            _salesTaxRatesRepository = unitOfWork.Repository<SalesTaxRates>();
        }

        public void GetSalesTaxAPI()
        {
            _unitOfWork.StartTransaction();

            _logService.Info("Starting Gettting Data from  Sale Tax ");
           //GetSalesTaxData();
            _logService.Info("Starting Deleting PDF Files ");
            DeletePDFFiles();
        }

        private bool DeletePDFFiles()
        {
            return true;
        }
        private bool GetSalesTaxData()
        {
            var client = new TaxjarApi("3274fe33d4c34faade2ef62560796d81", new
            {
                apiUrl = "https://api.sandbox.taxjar.com"
            });
            var organizationList = _organizationRepository.Table.Where(x => x.Id != 1).ToList();

            foreach (var organization in organizationList)
            {
                foreach (var address in organization.Address)
                {
                    try
                    {
                        var rates = client.RatesForLocation(address.Zip != null ? address.Zip.Code : address.ZipCode, new
                        {
                            city = address.City != null ? address.City.Name : address.CityName,
                            state = address.State != null ? address.State.Name : address.StateName,
                            country = address.Country != null ? address.Country.ShortName : "US"
                        });

                        SaveValueInDb(rates, address, organization.Id);
                    }
                    catch (Exception e1)
                    {
                        var zipCode = address.Zip != null ? address.Zip.Code : address.ZipCode;
                        var stateName = address.State != null ? address.State.Name : address.StateName;
                        var cityName = address.City != null ? address.City.Name : address.CityName;

                        _logService.Info("Error in  Gettting Data from  Sale Tax for ZipCode " + zipCode + " StateId " + stateName + " City " + cityName + " for franchisee " + organization.Id);
                    }
                }

            }
            return true;
        }

        private bool SaveValueInDb(RateResponseAttributes rate, Core.Geo.Domain.Address address, long organizationId)
        {
            var domain = new SalesTaxRates()
            {
                City = rate.City,
                StandardRate = rate.StandardRate,
                State = rate.State,
                CityId = address.CityId,
                StateId = address.StateId,
                CombinedRate = rate.CombinedRate,
                CombinedDistrictRate = rate.CombinedDistrictRate,
                CountryId = address.CountryId,
                DistanceSalesThreshold = rate.DistanceSaleThreshold,
                CountryRate = rate.CountryRate,
                FranchiseeId = organizationId,
                FreightTaxable = rate.FreightTaxable,
                ParkingRate = rate.ParkingRate,
                ReducedRate = rate.ReducedRate,
                IsNew = true,
                SuperReducedRate = rate.SuperReducedRate,
                StateRate = rate.StateRate,
                ZipCode = address.ZipCode,
                ZipId = address.ZipId,
                Country = rate.Country

            };
            _salesTaxRatesRepository.Save(domain);
            _unitOfWork.SaveChanges();
            return true;
        }
    }
}



