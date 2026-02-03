using Core.Geo.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Review.Domain;
using Core.ReviewApi.ViewModel;
using Core.Scheduler.Domain;
using System.Collections.Generic;

namespace Core.ReviewApi
{
    public interface IReviewApiFactory
    {
        ReviewCustomerViewModel CreateModel(CustomerFeedbackRequest domain);
        ZipCodeViewModel CreateViewModel(ZipCode domain, List<Franchisee> franchisee = null);
        CountyViewModel CreateViewModel(County domain);

        FranchiseeViewModel CreateViewModel(Address domain, string franchiseeName, string CountryName, Franchisee franchisee);
    }
}
