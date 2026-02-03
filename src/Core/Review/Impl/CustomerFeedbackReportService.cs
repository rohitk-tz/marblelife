using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Organizations.Domain;
using Core.Review.Domain;
using Core.Review.ViewModel;
using Core.Sales.Enum;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Review.Impl
{
    [DefaultImplementation]
    public class CustomerFeedbackReportService : ICustomerFeedbackReportService
    {

        private readonly IRepository<CustomerFeedbackResponse> _customerFeedbackResponseRepository;
        private readonly ISortingHelper _sortingHelper;
        private readonly ICustomerFeedbackReportFactory _customerFeedbackReportFactory;
        private readonly IRepository<CustomerFeedbackRequest> _customerFeedbackRequestRepository;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IClock _clock;
        private readonly IRepository<ReviewPushCustomerFeedback> _reviewPushCustomerFeedbackRepository;
        private IUnitOfWork _unitOfWork;

        public CustomerFeedbackReportService(IUnitOfWork unitOfWork, ISortingHelper sortingHelper, ICustomerFeedbackReportFactory customerFeedbackReportFactory,
            IExcelFileCreator excelFileCreator, IClock clock)
        {
            _customerFeedbackResponseRepository = unitOfWork.Repository<CustomerFeedbackResponse>();
            _customerFeedbackRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _sortingHelper = sortingHelper;
            _customerFeedbackReportFactory = customerFeedbackReportFactory;
            _excelFileCreator = excelFileCreator;
            _clock = clock;
            _reviewPushCustomerFeedbackRepository = unitOfWork.Repository<ReviewPushCustomerFeedback>();
            _unitOfWork = unitOfWork;
        }
        public CustomerFeedbackReportListModel GetCustomerFeedbackList(CustomerFeedbackReportFilter filter, int pageNumber, int pageSize)
        {
            var feedbackListFromReviewPush = new List<ReviewPushCustomerFeedback>();
            var collection = new List<CustomerFeedbackReportViewModel>();

            if (filter.ResponseFrom == 2 || filter.ResponseFrom == 0)
            {
                var feedbackList = GetCustomerFeedbackForReviewPushListFilter(filter);
                var collections = feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
                collection.AddRange(collections);
                feedbackListFromReviewPush = GetCustomerFeedbackFromRListFilter(filter).ToList();
                var collectionForReviewPush = feedbackListFromReviewPush.Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
                collection.AddRange(collectionForReviewPush);
            }

            if (filter.ResponseFrom == 2 || filter.ResponseFrom == 0)
            {
                var feedbackList = GetCustomerFeedbackListFilter(filter);
                collection.AddRange(feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList());
            }

            if (filter.ResponseFrom == 1 || filter.ResponseFrom == 0)
            {
                var feedbackList = GetCustomerFeedbackForGoogleAPIListFilter(filter);
                collection.AddRange(feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList());
            }

            if (filter.ResponseFrom == 2 || filter.ResponseFrom == 0)
            {
                var feedbackList = GetCustomerFeedbackFromSystemListFilter(filter);
                collection.AddRange(feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList());
            }

            var finalcollection = collection.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new CustomerFeedbackReportListModel
            {
                Collection = finalcollection,
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, collection.Count())
            };
        }

        private IQueryable<CustomerFeedbackRequest> GetCustomerFeedbackListFilter(CustomerFeedbackReportFilter filter)
        {
            var startDate = filter.StartDate.HasValue ? filter.StartDate.Value.Date : (DateTime?)null;
            var endDate = filter.EndDate.HasValue ? filter.EndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var responseEndDate = filter.ResponseEndDate.HasValue ? filter.ResponseEndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var feedbackList = _customerFeedbackRequestRepository.Table.Where(x => (filter.FranchiseeId <= 0 || (x.CustomerReviewSystemRecord.FranchiseeId == filter.FranchiseeId))
                                                        && (!x.IsQueued)
                                                         && (!x.CustomerFeedbackResponse.IsFromNewReviewSystem)
                                                        && (!x.IsSystemGenerated)
                                                        && (x.CustomerReviewSystemRecord != null)
                                                        && (filter.CustomerId <= 0 || x.CustomerReviewSystemRecord.CustomerId == filter.CustomerId)
                                                        && (filter.Response == null || (filter.Response == 1 ? x.CustomerFeedbackResponse != null : x.CustomerFeedbackResponse == null))
                                                        && (startDate == null || x.DateSend >= startDate)
                                                        && (endDate == null || x.DateSend <= endDate)
                                                        && (filter.ResponseStartDate == null
                                                        || (x.CustomerFeedbackResponse.DateOfReview >= filter.ResponseStartDate))
                                                        && (responseEndDate == null || (x.CustomerFeedbackResponse.DateOfReview <= responseEndDate))
                                                        && (filter.Text == null || x.CustomerEmail.Contains(filter.Text)
                                                        || (x.CustomerReviewSystemRecord.Customer.ContactPerson.Contains(filter.Text))
                                                        || x.CustomerReviewSystemRecord.Customer.Name.Contains(filter.Text)));

            feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Id, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Franchisee":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerReviewSystemRecord.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "Customer":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerReviewSystemRecord.Customer.Name, filter.SortingOrder);
                        break;
                    case "CustomerEmail":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerEmail, filter.SortingOrder);
                        break;
                    case "DateReceived":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerFeedbackResponse.DateOfReview, filter.SortingOrder);
                        break;
                    case "DateSend":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.DateSend, filter.SortingOrder);
                        break;
                    case "CustomerId":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerReviewSystemRecord.CustomerId, filter.SortingOrder);
                        break;
                    case "Rating":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerFeedbackResponse.Rating, filter.SortingOrder);
                        break;
                    case "ContactName":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerReviewSystemRecord.Customer.ContactPerson, filter.SortingOrder);
                        break;
                }
            }
            return feedbackList;
        }



        private IQueryable<CustomerFeedbackResponse> GetCustomerFeedbackFromSystemListFilter(CustomerFeedbackReportFilter filter)
        {
            var startDate = filter.StartDate.HasValue ? filter.StartDate.Value.Date : (DateTime?)null;
            var endDate = filter.EndDate.HasValue ? filter.EndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var responseEndDate = filter.ResponseEndDate.HasValue ? filter.ResponseEndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var feedbackList = _customerFeedbackResponseRepository.Table.Where(x => (filter.FranchiseeId <= 0 || (x.FranchiseeId == filter.FranchiseeId))
                                                         && (x.IsFromNewReviewSystem && !x.IsFromGoogleAPI && x.IsFromSystemReviewSystem)
                                                        && (filter.CustomerId <= 0 || x.CustomerId == filter.CustomerId)
                                                        && (startDate == null || x.DateOfReview >= startDate)
                                                        && (endDate == null || x.DateOfReview <= endDate)
                                                        && (filter.ResponseStartDate == null
                                                        || (x.DateOfReview >= filter.ResponseStartDate))
                                                        && (responseEndDate == null || (x.DateOfReview <= responseEndDate))
                                                        && (filter.Text == null || x.Url.Contains(filter.Text)
                                                        || (x.Customer.ContactPerson.Contains(filter.Text))
                                                        || (x.CustomerName.Contains(filter.Text))
                                                        || x.Customer.Name.Contains(filter.Text)));

            feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Id, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Franchisee":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "Customer":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Customer.Name, filter.SortingOrder);
                        break;
                    case "CustomerEmail":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Url, filter.SortingOrder);
                        break;
                    case "DateReceived":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.DateOfReview, filter.SortingOrder);
                        break;
                    case "DateSend":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.DateOfReview, filter.SortingOrder);
                        break;
                    case "CustomerId":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerId, filter.SortingOrder);
                        break;
                    case "Rating":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Rating, filter.SortingOrder);
                        break;
                    case "ContactName":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Customer.ContactPerson, filter.SortingOrder);
                        break;
                }
            }
            return feedbackList;
        }


        private IQueryable<CustomerFeedbackResponse> GetCustomerFeedbackForGoogleAPIListFilter(CustomerFeedbackReportFilter filter)
        {
            var startDate = filter.StartDate.HasValue ? filter.StartDate.Value.Date : (DateTime?)null;
            var endDate = filter.EndDate.HasValue ? filter.EndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var responseEndDate = filter.ResponseEndDate.HasValue ? filter.ResponseEndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var feedbackList = _customerFeedbackResponseRepository.Table.Where(x => (filter.FranchiseeId <= 0 || (x.FranchiseeId == filter.FranchiseeId))
                                                         && (x.IsFromNewReviewSystem && x.IsFromGoogleAPI && !x.IsFromSystemReviewSystem)
                                                        && (filter.CustomerId <= 0 || x.CustomerId == filter.CustomerId)
                                                        && (startDate == null || x.DateOfReview >= startDate)
                                                        && (endDate == null || x.DateOfReview <= endDate)
                                                        && (filter.ResponseStartDate == null
                                                        || (x.DateOfReview >= filter.ResponseStartDate))
                                                        && (responseEndDate == null || (x.DateOfReview <= responseEndDate))
                                                        && (filter.Text == null || x.Url.Contains(filter.Text)
                                                        || (x.Customer.ContactPerson.Contains(filter.Text))
                                                        || (x.CustomerName.Contains(filter.Text))
                                                        || x.Customer.Name.Contains(filter.Text)));

            feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Id, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Franchisee":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "Customer":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Customer.Name, filter.SortingOrder);
                        break;
                    case "CustomerEmail":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Url, filter.SortingOrder);
                        break;
                    case "DateReceived":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.DateOfReview, filter.SortingOrder);
                        break;
                    case "DateSend":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.DateOfReview, filter.SortingOrder);
                        break;
                    case "CustomerId":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerId, filter.SortingOrder);
                        break;
                    case "Rating":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Rating, filter.SortingOrder);
                        break;
                    case "ContactName":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Customer.ContactPerson, filter.SortingOrder);
                        break;
                }
            }
            return feedbackList;
        }

        private IQueryable<CustomerFeedbackResponse> GetCustomerFeedbackForReviewPushListFilter(CustomerFeedbackReportFilter filter)
        {
            var startDate = filter.StartDate.HasValue ? filter.StartDate.Value.Date : (DateTime?)null;
            var endDate = filter.EndDate.HasValue ? filter.EndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var responseEndDate = filter.ResponseEndDate.HasValue ? filter.ResponseEndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var feedbackList = _customerFeedbackResponseRepository.Table.Where(x => (filter.FranchiseeId <= 0 || (x.FranchiseeId == filter.FranchiseeId))
                                                         && (x.IsFromNewReviewSystem && !x.IsFromGoogleAPI && !x.IsFromSystemReviewSystem)
                                                        && (filter.CustomerId <= 0 || x.CustomerId == filter.CustomerId)
                                                        && (startDate == null || x.DateOfReview >= startDate)
                                                        && (endDate == null || x.DateOfReview <= endDate)
                                                        && (filter.ResponseStartDate == null
                                                        || (x.DateOfReview >= filter.ResponseStartDate))
                                                        && (responseEndDate == null || (x.DateOfReview <= responseEndDate))
                                                        && (filter.Text == null || x.Url.Contains(filter.Text)
                                                        || (x.Customer.ContactPerson.Contains(filter.Text))
                                                        || (x.CustomerName.Contains(filter.Text))
                                                        || x.Customer.Name.Contains(filter.Text)));

            feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Id, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Franchisee":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "Customer":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Customer.Name, filter.SortingOrder);
                        break;
                    case "CustomerEmail":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Url, filter.SortingOrder);
                        break;
                    case "DateReceived":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.DateOfReview, filter.SortingOrder);
                        break;
                    case "DateSend":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.DateOfReview, filter.SortingOrder);
                        break;
                    case "CustomerId":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.CustomerId, filter.SortingOrder);
                        break;
                    case "Rating":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Rating, filter.SortingOrder);
                        break;
                    case "ContactName":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Customer.ContactPerson, filter.SortingOrder);
                        break;
                }
            }
            return feedbackList;
        }

        private IQueryable<ReviewPushCustomerFeedback> GetCustomerFeedbackFromRListFilter(CustomerFeedbackReportFilter filter)
        {
            var startDate = filter.StartDate.HasValue ? filter.StartDate.Value.Date : (DateTime?)null;
            var endDate = filter.EndDate.HasValue ? filter.EndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var responseEndDate = filter.ResponseEndDate.HasValue ? filter.ResponseEndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var feedbackList = _reviewPushCustomerFeedbackRepository.IncludeMultiple(x => x.Franchisee).Where(x => (filter.FranchiseeId <= 0 || (x.FranchiseeId == filter.FranchiseeId))
                                                          && (startDate == null || x.Db_date >= startDate)
                                                          && (endDate == null || x.Db_date <= endDate)
                                                          && (filter.ResponseStartDate == null
                                                          || (x.Rp_date >= filter.ResponseStartDate))
                                                          && (responseEndDate == null || (x.Rp_date <= responseEndDate))
                                                          && (filter.Text == null
                                                          || x.Email.Contains(filter.Text)
                                                          || x.Name.Contains(filter.Text)));

            feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Id, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Franchisee":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "Customer":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Name, filter.SortingOrder);
                        break;
                    case "CustomerEmail":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Name, filter.SortingOrder);
                        break;
                    case "DateReceived":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Db_date, filter.SortingOrder);
                        break;
                    case "DateSend":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Rp_date, filter.SortingOrder);
                        break;
                        break;
                    case "Rating":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Rating, filter.SortingOrder);
                        break;
                    case "ContactName":
                        feedbackList = _sortingHelper.ApplySorting(feedbackList, x => x.Name, filter.SortingOrder);
                        break;
                }
            }
            return feedbackList;
        }



        public CustomerFeedbackReportViewModel GetCustomerFeedbackDetail(long responseId, bool isFromNewReviewSystem, bool isFromCustomerReviewTable)
        {
            if (!isFromNewReviewSystem && !isFromCustomerReviewTable)
            {
                var feedbackRequest = _customerFeedbackRequestRepository.Table.Where(x => x.ResponseId != null && x.ResponseId == responseId).OrderByDescending(x => x.Id).FirstOrDefault();
                if (feedbackRequest == null)
                    return null;

                var feedbackDetail = _customerFeedbackReportFactory.CreateViewModel(feedbackRequest);

                return feedbackDetail;
            }
            else if (isFromNewReviewSystem && isFromCustomerReviewTable)
            {
                var feedbackRequest = _reviewPushCustomerFeedbackRepository.Table.Where(x => x.Id != null && x.Review_Id == responseId).OrderByDescending(x => x.Id).FirstOrDefault();
                var feedbackDetail = _customerFeedbackReportFactory.CreateViewModel(feedbackRequest);
                return feedbackDetail;
            }
            else if (isFromNewReviewSystem && !isFromCustomerReviewTable)
            {
                var feedbackRequest = _customerFeedbackResponseRepository.Table.Where(x => x.Id != null && x.Id == responseId).OrderByDescending(x => x.Id).FirstOrDefault();
                if (feedbackRequest == null)
                    return null;

                var feedbackDetail = _customerFeedbackReportFactory.CreateViewModel(feedbackRequest);

                return feedbackDetail;
            }
            else
            {
                return default;
            }
        }

        public bool DownloadFeedbackReport(CustomerFeedbackReportFilter filter, out string fileName)
        {
            var feedbackListFromReviewPush = new List<ReviewPushCustomerFeedback>();
            fileName = string.Empty;

            var reportCollection = new List<CustomerFeedbackReportViewModel>();

            if (filter.ResponseFrom == 1 || filter.ResponseFrom == 0)
            {
                var feedbackList = GetCustomerFeedbackForGoogleAPIListFilter(filter);
                reportCollection.AddRange(feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList());
            }
            if (filter.ResponseFrom == 2 || filter.ResponseFrom == 0)
            {
                var feedbackList = GetCustomerFeedbackForReviewPushListFilter(filter);
                var collections = feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
                reportCollection.AddRange(collections);
                feedbackListFromReviewPush = GetCustomerFeedbackFromRListFilter(filter).ToList();
                var collectionForReviewPush = feedbackListFromReviewPush.Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
                reportCollection.AddRange(collectionForReviewPush);
            }
            if (filter.ResponseFrom == 2 || filter.ResponseFrom == 0)
            {
                var feedbackList = GetCustomerFeedbackListFilter(filter);
                reportCollection.AddRange(feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList());
            }
            if (filter.ResponseFrom == 2 || filter.ResponseFrom == 0)
            {
                var feedbackList = GetCustomerFeedbackFromSystemListFilter(filter);
                reportCollection.AddRange(feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList());
            }

            //IEnumerable<CustomerFeedbackRequest> franchiseeInvoiceList = GetCustomerFeedbackListFilter(filter).ToList();
            //if (filter.ResponseFrom == 2 || filter.ResponseFrom == 0)
            //{
            //    var feedbackListFromReviewPush2 = GetCustomerFeedbackListFilter(filter).ToList();
            //    var model = feedbackListFromReviewPush2.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
            //    reportCollection.AddRange(model);
            //}
            //if (filter.ResponseFrom == 1 || filter.ResponseFrom == 0)
            //{
            //    var feedbackList = GetCustomerFeedbackForReviewPushListFilter(filter);
            //    var collections = feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
            //    reportCollection.AddRange(collections);
            //    feedbackListFromReviewPush = GetCustomerFeedbackFromRListFilter(filter).ToList();
            //    var collectionForReviewPush2 = feedbackListFromReviewPush.Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
            //    reportCollection.AddRange(collectionForReviewPush2);
            //}
            ////prepare item collection
            //var collectionForReviewPush = feedbackListFromReviewPush.Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
            //reportCollection.AddRange(collectionForReviewPush);
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/feedbakcReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public bool ManageCustomerFeedbackStatus(bool isAccept, long customerId, long id, string fromTable)
        {
            if (fromTable == "CustomerFeedbackResponse")
            {
                var customerFeedbackResponse = _customerFeedbackResponseRepository.Table.FirstOrDefault(x => x.Id == id && x.CustomerId == customerId);
                if (customerFeedbackResponse != null)
                {
                    if (isAccept)
                    {
                        customerFeedbackResponse.AuditActionId = (long)AuditActionType.Approved;
                        _customerFeedbackResponseRepository.Save(customerFeedbackResponse);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        customerFeedbackResponse.AuditActionId = (long)AuditActionType.Rejected;
                        _customerFeedbackResponseRepository.Save(customerFeedbackResponse);
                        _unitOfWork.SaveChanges();
                    }
                }
            }
            if (fromTable == "CustomerFeedbackRequest")
            {
                var customerFeedbackRequest = _customerFeedbackRequestRepository.Table.FirstOrDefault(x => x.Id == id && x.CustomerId == customerId);
                if (customerFeedbackRequest != null)
                {
                    if (isAccept)
                    {
                        customerFeedbackRequest.AuditActionId = (long)AuditActionType.Approved;
                        _customerFeedbackRequestRepository.Save(customerFeedbackRequest);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        customerFeedbackRequest.AuditActionId = (long)AuditActionType.Rejected;
                        _customerFeedbackRequestRepository.Save(customerFeedbackRequest);
                        _unitOfWork.SaveChanges();
                    }
                }
            }
            if (fromTable == "ReviewPushCustomerFeedback")
            {
                var reviewPushCustomerFeedback = _reviewPushCustomerFeedbackRepository.Table.FirstOrDefault(x => x.Id == id);
                if (reviewPushCustomerFeedback != null)
                {
                    if (isAccept)
                    {
                        reviewPushCustomerFeedback.AuditActionId = (long)AuditActionType.Approved;
                        _reviewPushCustomerFeedbackRepository.Save(reviewPushCustomerFeedback);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        reviewPushCustomerFeedback.AuditActionId = (long)AuditActionType.Rejected;
                        _reviewPushCustomerFeedbackRepository.Save(reviewPushCustomerFeedback);
                        _unitOfWork.SaveChanges();
                    }
                }
            }
            return true;
        }
    }
}
