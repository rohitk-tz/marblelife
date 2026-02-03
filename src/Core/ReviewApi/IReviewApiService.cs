using Core.Review.ViewModel;
using Core.ReviewApi.ViewModel;
using System;

namespace Core.ReviewApi
{
   public interface IReviewApiService
    {
        ReviewCustomerListModel GetReviewCustomerList(DateTime startDate, DateTime endDate);

        bool SaveReviewPushResponse(ReviewPushResponseModel model);

        ZipCodeListModel GetZipCodeList();
        CountyListModel GetCountyList();

        ZipCodeListModel SearchZipCode(string zipCode);
        FranchiseeListModel SearchFranchisee(string countryName);
        CountryListModel GetAllCountries();
        BeforeAfterListModel GetBeforeAfterImages(DateTime startDate, DateTime endDate);
        BeforeAfterListModel GetBeforeAfterImagesByFranchiseeId(DateTime startDate, DateTime endDate, long? franchiseeId);
        BeforeAfterListModel GetBeforeAfterImagesWithProperties(BeforeAfterFilterModel filter);

        CustomerFeedbackReportListModel GetCustomersFeebBack(CustomerFeedbackReportFilter filter);
        CustomerFeedbackReportResponseModel SaveCustomersFeebBack(CustomerFeedbackReportDomainModel filter);
        BeforeAfterListModel GetBeforeAfterSelectedImagesWithProperties(BeforeAfterFilterModel filter);
    }
}
