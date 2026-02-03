using Api.Areas.Application.Controller;
using Api.Impl;
using Core.Application;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Review.ViewModel;
using Core.ReviewApi;
using Core.ReviewApi.ViewModel;
using Core.Scheduler.ViewModel;
using Core.Users;
using Core.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using AllowAnonymousAttribute = System.Web.Http.AllowAnonymousAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;

namespace API.Areas.ReviewApi.Controllers
{
    //[AllowAnonymous]a
    public class ReviewSystemController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IReviewApiService _reviewApiService;
        private readonly ILoginAuthenticationModelValidator _loginAuthenticationModelValidator;
        private readonly IUserLoginService _userLoginService;
        public ReviewSystemController(ISessionContext sessionContext, IReviewApiService reviewApiService,
            ILoginAuthenticationModelValidator loginAuthenticationModelValidator, IUserLoginService userLoginService)
        {
            _sessionContext = sessionContext;
            _reviewApiService = reviewApiService;
            _loginAuthenticationModelValidator = loginAuthenticationModelValidator;
            _userLoginService = userLoginService;
        }

        // GET: ReviewApi/ReviewSystem
        //[AllowAnonymous]
        [HttpGet]
        public ReviewCustomerListModel GetReviewCustomerList([FromUri] DateTime startDate, [FromUri] DateTime endDate)
        {
            return _reviewApiService.GetReviewCustomerList(startDate, endDate);
        }
        //[AllowAnonymous]
        //[System.Web.Http.HttpPost]
        //public bool SaveReviewPushResponse(ReviewPushResponseModel model)
        //{
        //    return _reviewApiService.SaveReviewPushResponse(model);
        //}

        [System.Web.Http.HttpPost]
        [AllowAnonymous]
        public string LoginForReviewSystem([FromBody] LoginAuthenticationModel model)
        {
            var isValid = _loginAuthenticationModelValidator.IsValidForReviewAPI(model);

            if (!isValid)
            {
                PostResponseModel.Message = model.Message;
                return null;
            }

            return SessionHelper.CreateNewSession(
                ((HttpContextBase)ControllerContext.Request.Properties["MS_HttpContext"]).Request,
                _userLoginService.GetbyUserName(model.Username));
        }
       // [AllowAnonymous]
        [HttpGet]
        public ZipCodeListModel GetZipCodeList()
        {
            return _reviewApiService.GetZipCodeList();
        }
        //[AllowAnonymous]
        [HttpGet]
        public CountyListModel GetCountyList()
        {
            return _reviewApiService.GetCountyList();
        }
        [AllowAnonymous]
        [HttpGet]
        public ZipCodeListModel SearchZipCode([FromUri] string zipCode)
        {
            return _reviewApiService.SearchZipCode(zipCode);
        }
        //[AllowAnonymous]
        [HttpGet]
        public FranchiseeListModel SearchFranchisee([FromUri] string countryName)
        {
            return _reviewApiService.SearchFranchisee(countryName);
        }
        //[AllowAnonymous]
        [HttpGet]
        public CountryListModel GetAllCountries()
        {
            return _reviewApiService.GetAllCountries();
        }
        //[AllowAnonymous]
        [HttpGet]
        public BeforeAfterListModel GetBeforeAfterImages([FromUri] DateTime startDate, [FromUri] DateTime endDate)
        {
            return _reviewApiService.GetBeforeAfterImages(startDate, endDate);
        }

        //[AllowAnonymous]
        [HttpGet]
        public BeforeAfterListModel GetBeforeAfterImagesByFranchisee([FromUri] DateTime startDate, [FromUri] DateTime endDate, [FromUri] long? franchiseeid)
        {
            return _reviewApiService.GetBeforeAfterImagesByFranchiseeId(startDate, endDate, franchiseeid);
        }

        [HttpGet]
        public BeforeAfterListModel GetBeforeAfterImagesWithProperties([FromBody] BeforeAfterFilterModel filter)
        {
            return _reviewApiService.GetBeforeAfterImagesWithProperties(filter);
        }


        [HttpGet]
        public CustomerFeedbackReportListModel GetCustomersFeebBack([FromBody] CustomerFeedbackReportFilter filter)
        {
            return _reviewApiService.GetCustomersFeebBack(filter);
        }

        [System.Web.Http.HttpPost]
        public CustomerFeedbackReportResponseModel SaveCustomersFeebBack([FromBody] CustomerFeedbackReportDomainModel filter)
        {
            return _reviewApiService.SaveCustomersFeebBack(filter);
        }


        [HttpGet]
        public BeforeAfterListModel GetBeforeAfterSelectedImagesWithProperties([FromBody] BeforeAfterFilterModel filter)
        {
            return _reviewApiService.GetBeforeAfterImagesWithProperties(filter);
        }
    }
}