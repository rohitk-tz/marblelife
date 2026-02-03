using Core.Application;
using Core.Application.Attribute;
using Core.Geo.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Reports.ViewModel;
using Core.Review.Domain;
using Core.ReviewApi.ViewModel;
using Core.Scheduler.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Core.ReviewApi.Impl
{
    [DefaultImplementation]
    public class ReviewApiFactory : IReviewApiFactory
    {
        private readonly IRepository<ZipCode> _zipCodeRepository;

        public ReviewApiFactory(IUnitOfWork unitOfWork)
        {
            _zipCodeRepository = unitOfWork.Repository<ZipCode>();

        }
        public ReviewCustomerViewModel CreateModel(CustomerFeedbackRequest domain)
        {
            var firstName = "";
            var lastName = "";
            var customerInfo = domain.Customer;
            var nameSplit = customerInfo.Name.Split(',');
            if (nameSplit.Length > 1)
            {
                firstName = nameSplit[0];
                lastName = nameSplit[1];
            }
            else
                firstName = nameSplit[0];

            var customerModel = new ReviewCustomerViewModel
            {
                CustomerEmailAddress = domain.CustomerEmail,
                CustomerFirstName = firstName,
                CustomerLastName = lastName,
                CustomerPhoneNumber = customerInfo.Phone,
                FranchiseName = domain.Franchisee.Organization.Name,
                FranchiseWebsite = domain.Franchisee.WebSite,
                Rp_Id = domain.Franchisee.Reviewpush != null ? domain.Franchisee.Reviewpush.Rp_ID : default(long?),
                IsFromQA = ApplicationManager.Settings.IsFromQA,
                ReviewId = domain.Id

            };
            return customerModel;
        }

        public ZipCodeViewModel CreateViewModel(ZipCode domain, List<Franchisee> franchiseeList = null)
        {
            long? franchiseeId = null;
            string franchiseeEmail = "";
            if (domain.County.Franchisee == null && domain.County.FranchiseeName != null && franchiseeList.Any(x => x.Organization.Name == domain.County.FranchiseeName) && franchiseeList != null)
            {
                franchiseeEmail = franchiseeList.FirstOrDefault(x => x.Organization.Name == domain.County.FranchiseeName).MarketingPersonEmail != null ? franchiseeList.FirstOrDefault(x => x.Organization.Name == domain.County.FranchiseeName).MarketingPersonEmail : franchiseeList.FirstOrDefault(x => x.Organization.Name == domain.County.FranchiseeName).Organization.Email;
                franchiseeId = franchiseeList.FirstOrDefault(x => x.Organization.Name == domain.County.FranchiseeName).Organization.Id;
            }
            var viewModel = new ZipCodeViewModel()
            {
                AreaCodes = domain.AreaCode,
                City = domain.City != null ? domain.City != null ? domain.City.Name : domain.CityName : domain.CityName,
                County = domain.County != null ? domain.County.CountyName : "",
                Country = domain.County != null ? domain.County.Country.Name : "",
                TransferableNumber = domain.TransferableNumber,
                State = domain.StateCode,
                ZipCode = domain.Zip,
                FranchiseeName = domain.County != null ? domain.County.Franchisee != null ? domain.County.Franchisee.Organization.Name : domain.County.FranchiseeName : "",
                FranchiseeEmail = domain.County != null ? domain.County.Franchisee != null ? (domain.County.Franchisee.MarketingPersonEmail != "" || domain.County.Franchisee.MarketingPersonEmail != null) ? domain.County.Franchisee.MarketingPersonEmail : domain.County.Franchisee.Organization.Email : franchiseeEmail : franchiseeEmail,
                FranchiseeId = domain.County.Franchisee != null ? domain.County.Franchisee.Id : franchiseeId
            };
            return viewModel;
        }

        public CountyViewModel CreateViewModel(County domain)
        {
            var zipCodeDomain = _zipCodeRepository.Table.FirstOrDefault(x => x.CountyName == domain.CountyName && x.StateCode == domain.StateCode);
            var viewModel = new CountyViewModel()
            {
                City = zipCodeDomain != null ? zipCodeDomain.City != null ? zipCodeDomain.City.Name : zipCodeDomain.CityName : "",
                Country = domain.Country != null ? domain.Country.Name : "",
                TransferableNumber = zipCodeDomain != null ? zipCodeDomain.TransferableNumber : "",
                State = domain.StateCode,
                TaazaaFranchiseeEmail = domain.Franchisee != null ? domain.Franchisee.Organization.Email : "",
                County = domain.CountyName,
                TaazaaFranchisee = domain.Franchisee != null ? domain.Franchisee.Organization.Name : domain.FranchiseeName
            };
            return viewModel;
        }

        public FranchiseeViewModel CreateViewModel(Address address, string franchiseeName, string countryName, Franchisee franchisee)
        {
            var viewModel = new FranchiseeViewModel()
            {
                City = address != null && address.City != null ? address.City.Name : address.CityName,
                Country = address.Country != null ? address.Country.Name : "",
                TransferableNumber = franchisee != null && franchisee.Organization.Phones != null ? franchisee.Organization.Phones.FirstOrDefault().Number : "",
                State = address.State != null ? address.State.Name : address.StateName,
                FranchiseeEmail = franchisee != null ? franchisee.Organization.Email : "",
                County = countryName,
                FranchiseeName = franchiseeName
            };
            return viewModel;
        }
    }
}
