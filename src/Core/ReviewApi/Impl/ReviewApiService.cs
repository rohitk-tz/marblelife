using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Geo.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Review;
using Core.Review.Domain;
using Core.Review.ViewModel;
using Core.ReviewApi.ViewModel;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using Ical.Net.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.ReviewApi.Impl
{
    [DefaultImplementation]
    public class ReviewApiService : IReviewApiService
    {
        private readonly IRepository<CustomerFeedbackRequest> _customerFeedbackRequestRepository;
        private readonly IReviewApiFactory _reviewApiFactory;
        private readonly IRepository<ZipCode> _zipCodeRepository;
        private readonly IRepository<County> _countyRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<JobEstimateImage> _jobEstimateImageRepository;
        private readonly IRepository<JobEstimateServices> _jobEstimateServicesRepository;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
        private readonly ICustomerFeedbackReportFactory _customerFeedbackReportFactory;
        private readonly IRepository<CustomerFeedbackResponse> _customerFeedbackResponseRepository;
        private readonly IRepository<ReviewPushCustomerFeedback> _reviewPushCustomerFeedbackRepository;
        private readonly IRepository<CustomerEmail> _customerEmailRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private IClock _clock;
        public ReviewApiService(IUnitOfWork unitOfWork, IReviewApiFactory reviewApiFactory, ICustomerFeedbackReportFactory customerFeedbackReportFactory, IClock clock)
        {
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _clock = clock;
            _reviewApiFactory = reviewApiFactory;
            _customerFeedbackRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _zipCodeRepository = unitOfWork.Repository<ZipCode>();
            _countyRepository = unitOfWork.Repository<County>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _addressRepository = unitOfWork.Repository<Address>();
            _countryRepository = unitOfWork.Repository<Country>();
            _jobEstimateImageRepository = unitOfWork.Repository<JobEstimateImage>();
            _jobEstimateServicesRepository = unitOfWork.Repository<JobEstimateServices>();
            _beforeAfterImagesRepository = unitOfWork.Repository<BeforeAfterImages>();
            _customerFeedbackReportFactory = customerFeedbackReportFactory;
            _customerFeedbackResponseRepository = unitOfWork.Repository<CustomerFeedbackResponse>();
            _reviewPushCustomerFeedbackRepository = unitOfWork.Repository<ReviewPushCustomerFeedback>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
        }

        public ReviewCustomerListModel GetReviewCustomerList(DateTime startDate, DateTime endDate)
        {
            var customerList = _customerFeedbackRequestRepository.Table.Where(x => x.DateSend >= startDate && x.DateSend <= endDate && x.IsQueued).ToList();
            return new ReviewCustomerListModel
            {
                Status = "Success",
                Info = customerList.Select(x => _reviewApiFactory.CreateModel(x)).ToList()
            };
        }

        public ZipCodeListModel GetZipCodeList()
        {
            var zipCodeDomain = _zipCodeRepository.Table.ToList();

            return new ZipCodeListModel
            {
                Status = "Success",
                Info = zipCodeDomain.Select(x => _reviewApiFactory.CreateViewModel(x)).ToList()
            };
        }

        public CountyListModel GetCountyList()
        {
            var countyDomain = _countyRepository.Table.ToList();

            return new CountyListModel
            {
                Status = "Success",
                Info = countyDomain.Select(x => _reviewApiFactory.CreateViewModel(x)).ToList()
            };
        }

        public bool SaveReviewPushResponse(ReviewPushResponseModel model)
        {
            try
            {
                var customerFeedBackRequest = _customerFeedbackRequestRepository.Get(model.ReviewId.GetValueOrDefault());
                customerFeedBackRequest.IsQueued = !model.IsSendSuccess;
                customerFeedBackRequest.DataPacket = model.FeedBackResponse;
                _customerFeedbackRequestRepository.Save(customerFeedBackRequest);
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }

        public ZipCodeListModel SearchZipCode(string zipCode)
        {
            var countryNameList = zipCode.Take(2).ToList();
            string countryCode = string.Join("", countryNameList);
            zipCode = zipCode.Remove(0, 2);
            var splittedChar = zipCode.ToCharArray().Length;
            if (splittedChar > 6)
            {
                return new ZipCodeListModel
                {
                    Status = "Error",
                    Info = null,
                    ErrorMessage = "Zip Code Length cannot be Greater than 4."
                };
            }
            var countryDomain = _countryRepository.Table.FirstOrDefault(x => x.ShortName == countryCode);
            if (countryDomain == null)
            {
                return new ZipCodeListModel
                {
                    Status = "Error",
                    Info = null,
                    ErrorMessage = "Invalid Country Code."
                };
            }
            var zipCodeWithout0 = zipCode.TrimStart(new Char[] { '0' });
            var zipCodeDomain = _zipCodeRepository.IncludeMultiple(x => x.County, y => y.County.Franchisee).Where(x => x.Zip == zipCodeWithout0).ToList();
            var franchiseeList = _franchiseeRepository.IncludeMultiple(x => x.Organization).ToList();
            if (countryDomain != null)
            {
                zipCodeDomain = zipCodeDomain.Where(x => x.County != null && x.County.CountryId == countryDomain.Id).ToList();
            }
            return new ZipCodeListModel
            {
                Status = "Success",
                Info = zipCodeDomain.Select(x => _reviewApiFactory.CreateViewModel(x, franchiseeList)).ToList()
            };
        }

        public FranchiseeListModel SearchFranchisee(string countryName)
        {
            var addresslist = _addressRepository.Table.Where(x => x.Country != null && x.Country.Name == countryName).Select(x => x.Id).ToList();
            var organizationAddress = _organizationRepository.Table.Select(x => new OrganizationAddress
            {
                AddressId = x.Address.FirstOrDefault().Id,
                OrganizationId = x.Id,
                CountryName = x.Address.FirstOrDefault().Country.Name
            }).Where(x1 => x1.CountryName == countryName).Select(x => x.OrganizationId);

            var organizationDomainList = _organizationRepository.Table.Where(x => organizationAddress.Contains(x.Id) && x.IsActive && x.Id != 1).ToList();
            return new FranchiseeListModel
            {
                Status = "Success",
                Count = organizationDomainList.Count(),
                Info = organizationDomainList.Select(x => _reviewApiFactory.CreateViewModel(x.Address.FirstOrDefault(), x.Name, countryName, x.Franchisee)).ToList()
            };
        }


        public CountryListModel GetAllCountries()
        {
            var countryList = _countryRepository.Table.Distinct().ToList();
            var countryViewMode = countryList.Select(x => new CountryViewModel
            {
                Name = x.Name,
                ShortName = x.ShortName
            }).Distinct().ToList();

            return new CountryListModel
            {
                Status = "Success",
                Count = countryList.Count(),
                Info = countryViewMode
            };
        }


        public BeforeAfterListModel GetBeforeAfterImages(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.AddDays(1).Date;
            var jobEstimateServices = _jobEstimateServicesRepository.Table.Where(x => x.PairId != null).Select(x => x.Id).ToList();
            var jobEstimateServicesList = _jobEstimateServicesRepository.Table.ToList();
            var jobEstimateImageList = _jobEstimateImageRepository.IncludeMultiple(x => x.JobEstimateServices).ToList();
            var beforeAfterImageListHavingAfterPair = _jobEstimateImageRepository.Table.Where(x => jobEstimateServices.Contains(x.ServiceId.Value) && x.BestFitMarkDateTime != null).ToList();


            var beforeAfterImageListModel = beforeAfterImageListHavingAfterPair.Select(x => new BeforeAfterViewModel
            {
                ServiceId = x.ServiceId,
                SurfaceColor = x.JobEstimateServices.SurfaceColor,
                BuildingLocation = x.JobEstimateServices.BuildingLocation,
                FinishType = x.JobEstimateServices.FinishMaterial,
                ServiceType = x.JobEstimateServices.ServiceType != null ? x.JobEstimateServices.ServiceType.Name : "",
                SurfaceMaterial = x.JobEstimateServices.SurfaceMaterial,
                SurfaceType = x.JobEstimateServices.SurfaceType,
                AfterImageUrl = GetBase64String(x.File),
                BeforeImageUrl = GetBase64String(GetJobEstimateService(x, jobEstimateServicesList, jobEstimateImageList)),
                FranchiseeName = x.JobEstimateServices.JobEstimateImageCategory != null && x.JobEstimateServices.JobEstimateImageCategory.JobScheduler != null ? x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Franchisee.Organization.Name : "",
                FranchiseeUrl = x.JobEstimateServices.JobEstimateImageCategory != null && x.JobEstimateServices.JobEstimateImageCategory.JobScheduler != null ? x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Franchisee.Organization.Franchisee.WebSite : "",
                FranchiseeId = x.JobEstimateServices.JobEstimateImageCategory != null ? x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Franchisee.Organization.Id : default(long?),
                ImageId = x.Id,
                MarketingClass = x.JobEstimateServices.JobEstimateImageCategory.MarketingClass != null ? x.JobEstimateServices.JobEstimateImageCategory.MarketingClass.Name : ""
            }).Distinct().ToList();

            var list = new List<BeforeAfterMainModel>();
            var beforeAfterImageListModelGrouped = beforeAfterImageListModel.GroupBy(x => x.FranchiseeName).ToList();

            foreach (var beforeAfterImageListLocalModel in beforeAfterImageListModelGrouped)
            {
                list.Add(new BeforeAfterMainModel()
                {
                    FranchiseeId = beforeAfterImageListLocalModel.Any() ? beforeAfterImageListLocalModel.FirstOrDefault().FranchiseeId : default(long?),
                    FranchiseeName = beforeAfterImageListLocalModel.Key,
                    List = beforeAfterImageListLocalModel.ToList()
                });
            }



            return new BeforeAfterListModel
            {
                Data = list,
                BeforeAfterList = beforeAfterImageListModel,
                count = beforeAfterImageListModel.Count(),
                Status = "Success"
            };
        }
        private File GetJobEstimateService(JobEstimateImage jobEstimateImage, List<JobEstimateServices> jobEstimateServicesList, List<JobEstimateImage> jobEstimateImageList)
        {
            var jobEstimateServiceAfterDomain = jobEstimateServicesList.FirstOrDefault(x => x.Id == jobEstimateImage.ServiceId);
            var jobEstimateServiceBeforeDomain = jobEstimateServicesList.FirstOrDefault(x => x.Id == jobEstimateServiceAfterDomain.PairId);
            var jobEstimateImageDomain = jobEstimateImageList.FirstOrDefault(x => x.ServiceId == jobEstimateServiceBeforeDomain.Id);
            if (jobEstimateImageDomain == null)
                return default(File);
            return jobEstimateImageDomain.File;
        }
        private string GetBase64String(File file)
        {

            if (file == null)
            {
                return "";
            }
            try
            {
                var filePath = (file.RelativeLocation + @"\" + file.Name).ToPath();
                return filePath;
                //byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
                //string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                //return base64ImageRepresentation;
            }
            catch (Exception e1)
            {
                return "";
            }
        }
        public class OrganizationAddress
        {
            public long? OrganizationId { get; set; }
            public long? AddressId { get; set; }
            public string CountryName { get; set; }
        }


        public BeforeAfterListModel GetBeforeAfterImagesByFranchiseeId(DateTime startDate, DateTime endDate, long? franchiseeId)
        {
            startDate = startDate.Date;
            endDate = endDate.AddDays(1).Date;
            var jobEstimateServices = _jobEstimateServicesRepository.Table.Where(x => x.PairId != null && x.JobEstimateImageCategory.JobScheduler.FranchiseeId == franchiseeId).Select(x => x.Id).ToList();
            var jobEstimateServicesList = _jobEstimateServicesRepository.Table.ToList();
            var jobEstimateImageList = _jobEstimateImageRepository.IncludeMultiple(x => x.JobEstimateServices).ToList();
            var beforeAfterImageListHavingAfterPair = _jobEstimateImageRepository.Table.Where(x => (x.AddToGalleryDateTime >= startDate && x.AddToGalleryDateTime <= endDate)
            && jobEstimateServices.Contains(x.ServiceId.Value)).ToList();

            var beforeAfterImageListModel = beforeAfterImageListHavingAfterPair.Select(x => new BeforeAfterViewModel
            {
                ServiceId = x.ServiceId,
                SurfaceColor = x.JobEstimateServices.SurfaceColor,
                BuildingLocation = x.JobEstimateServices.BuildingLocation,
                FinishType = x.JobEstimateServices.FinishMaterial,
                ServiceType = x.JobEstimateServices.ServiceType != null ? x.JobEstimateServices.ServiceType.Name : "",
                SurfaceMaterial = x.JobEstimateServices.SurfaceMaterial,
                SurfaceType = x.JobEstimateServices.SurfaceType,
                AfterImageUrl = GetBase64String(x.File),
                BeforeImageUrl = GetBase64String(GetJobEstimateService(x, jobEstimateServicesList, jobEstimateImageList)),
                FranchiseeName = x.JobEstimateServices.JobEstimateImageCategory != null ? x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Franchisee.Organization.Name : "",
                FranchiseeUrl = x.JobEstimateServices.JobEstimateImageCategory != null ? x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Franchisee.Organization.Franchisee.WebSite : "",
                FranchiseeId = x.JobEstimateServices.JobEstimateImageCategory != null ? x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Franchisee.Organization.Id : default(long?),
                ImageId = x.Id,
                MarketingClass = x.JobEstimateServices.JobEstimateImageCategory.MarketingClass != null ? x.JobEstimateServices.JobEstimateImageCategory.MarketingClass.Name : ""
            }).Distinct().ToList();

            return new BeforeAfterListModel
            {
                BeforeAfterList = beforeAfterImageListModel,
                count = beforeAfterImageListModel.Count(),
                Status = "Success"
            };
        }

        public BeforeAfterListModel GetBeforeAfterImagesWithProperties(BeforeAfterFilterModel filter)
        {
            var beforeAfterImageListModel = _beforeAfterImagesRepository.IncludeMultiple(x => x.Franchisee, x2 => x2.Franchisee.Organization, x2 => x2.ServiceType, x2 => x2.JobScheduler.ServiceType).Where(x =>
            (x.TypeId == ((long?)LookupTypes.AfterWork) || x.TypeId == ((long?)LookupTypes.BeforeWork) || x.TypeId == ((long?)LookupTypes.ExteriorBuilding) || x.TypeId == (long?)LookupTypes.DuringWork)
            && (x.TypeId == ((long?)LookupTypes.ExteriorBuilding) || (x.AddToGalleryDateTime >= filter.StartDate && x.AddToGalleryDateTime <= filter.EndDate))).ToList();

            var beforeAfterImageModel = beforeAfterImageListModel.Where(x => x.PairId == null && (x.TypeId == ((long?)LookupTypes.AfterWork) || x.TypeId == ((long?)LookupTypes.BeforeWork) || x.TypeId == ((long?)LookupTypes.DuringWork))).Select(x => new BeforeAfterViewModel
            {
                Id = x.Id,
                ServiceId = x.ServiceId,
                SurfaceColor = x.SurfaceColor,
                BuildingLocation = x.BuildingLocation,
                FinishType = x.FinishMaterial,
                ServiceType = x.ServiceType != null ? x.ServiceType.Name : "",
                SurfaceMaterial = x.SurfaceMaterial,
                SurfaceType = x.JobScheduler != null && x.JobScheduler.ServiceType != null ? x.JobScheduler.ServiceType.Name : "",
                BeforeImageUrl = x.S3BucketURL,
                AfterImageUrl = (GetJobEstimateServiceForNewAPI(beforeAfterImageListModel.FirstOrDefault(x1 => x1.PairId == x.Id))),
                ExteriorImageUrl = (GetJobEstimateServiceForNewAPI(beforeAfterImageListModel.FirstOrDefault(x1 => x1.CategoryId == x.CategoryId && x1.TypeId == ((long?)LookupTypes.ExteriorBuilding)))),
                FranchiseeName = x.Franchisee != null ? x.Franchisee.Organization.Name : "",
                FranchiseeUrl = x.Franchisee != null ? x.Franchisee.WebSite : "",
                FranchiseeId = x.Franchisee != null ? x.Franchisee.Id : default(long?),
                ImageId = x.FileId,
                MarketingClass = x.JobEstimateServices.JobEstimateImageCategory.MarketingClass != null ? x.JobEstimateServices.JobEstimateImageCategory.MarketingClass.Name : "",
                BeforeImageUrlThumb = x.S3BucketThumbURL,
                AfterImageUrlThumb = GetJobEstimateServiceForNewAPIThumb(beforeAfterImageListModel.FirstOrDefault(x1 => x1.PairId == x.Id)),
                ExteriorImageUrlThumb = GetJobEstimateServiceForNewAPIThumb(beforeAfterImageListModel.FirstOrDefault(x1 => x1.CategoryId == x.CategoryId && x1.TypeId == ((long?)LookupTypes.ExteriorBuilding))),

            }).Distinct().OrderByDescending(y => y.Id).ToList();

            var list = new List<BeforeAfterMainModel>();
            var beforeAfterImageListModelGrouped = beforeAfterImageModel.GroupBy(x => x.FranchiseeName).ToList();

            foreach (var beforeAfterImageListLocalModel in beforeAfterImageListModelGrouped)
            {
                list.Add(new BeforeAfterMainModel()
                {
                    FranchiseeId = beforeAfterImageListLocalModel.Any() ? beforeAfterImageListLocalModel.FirstOrDefault().FranchiseeId : default(long?),
                    FranchiseeName = beforeAfterImageListLocalModel.Key,
                    List = beforeAfterImageListLocalModel.ToList(),
                    OrderBy = beforeAfterImageListLocalModel.Any() ? beforeAfterImageListLocalModel.FirstOrDefault().FranchiseeId == filter.FranchiseeId ? 1 : 2 : 2,
                });
            }
            return new BeforeAfterListModel
            {
                Data = list.OrderBy(x => x.OrderBy).ToList(),
                //BeforeAfterList = beforeAfterImageModel,
                count = beforeAfterImageListModel.Count(),
                Status = "Success"
            };
        }

        private string GetJobEstimateServiceForNewAPI(BeforeAfterImages jobEstimateBeforeImage)
        {
            if (jobEstimateBeforeImage == null)
                return default(string);
            return jobEstimateBeforeImage.S3BucketURL;
        }

        private string GetJobEstimateServiceForNewAPIThumb(BeforeAfterImages jobEstimateBeforeImage)
        {
            if (jobEstimateBeforeImage == null)
                return default(string);
            return jobEstimateBeforeImage.S3BucketThumbURL;
        }

        public CustomerFeedbackReportListModel GetCustomersFeebBack(CustomerFeedbackReportFilter filter)
        {
            var feedbackListFromReviewPush = new List<ReviewPushCustomerFeedback>();
            var collection = new List<CustomerFeedbackReportViewModel>();

            var feedbackListFromReviewPush2 = GetCustomerFeedbackListFilter(filter);
            var model = feedbackListFromReviewPush2.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList();


            var feedbackList = GetCustomerFeedbackForReviewPushListFilter(filter);
            var collections = feedbackList.ToList().Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
            collection.AddRange(collections);
            feedbackListFromReviewPush = GetCustomerFeedbackFromRListFilter(filter).ToList();
            var collectionForReviewPush = feedbackListFromReviewPush.Select(_customerFeedbackReportFactory.CreateViewModel).ToList();
            collection.AddRange(collectionForReviewPush);

            return new CustomerFeedbackReportListModel
            {
                Collection = collection.OrderByDescending(x => x.Id),
                Filter = filter,
            };
        }


        private IQueryable<CustomerFeedbackRequest> GetCustomerFeedbackListFilter(CustomerFeedbackReportFilter filter)
        {
            var startDate = filter.StartDate.HasValue ? filter.StartDate.Value.Date : (DateTime?)null;
            var endDate = filter.EndDate.HasValue ? filter.EndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var responseEndDate = filter.ResponseEndDate.HasValue ? filter.ResponseEndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var feedbackList = _customerFeedbackRequestRepository.Table.Where(x => (filter.FranchiseeId <= 0 || (x.CustomerReviewSystemRecord.FranchiseeId == filter.FranchiseeId))
                                                          && x.AuditActionId == (long)AuditActionType.Approved);



            return feedbackList;

        }


        private IQueryable<CustomerFeedbackResponse> GetCustomerFeedbackForReviewPushListFilter(CustomerFeedbackReportFilter filter)
        {
            var startDate = filter.StartDate.HasValue ? filter.StartDate.Value.Date : (DateTime?)null;
            var endDate = filter.EndDate.HasValue ? filter.EndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var responseEndDate = filter.ResponseEndDate.HasValue ? filter.ResponseEndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var feedbackList = _customerFeedbackResponseRepository.Table.Where(x => (filter.FranchiseeId <= 0 || (x.FranchiseeId == filter.FranchiseeId))
                                                         && (x.IsFromNewReviewSystem && x.AuditActionId == (long)AuditActionType.Approved)
                                                       );



            return feedbackList;
        }

        private IQueryable<ReviewPushCustomerFeedback> GetCustomerFeedbackFromRListFilter(CustomerFeedbackReportFilter filter)
        {
            var startDate = filter.StartDate.HasValue ? filter.StartDate.Value.Date : (DateTime?)null;
            var endDate = filter.EndDate.HasValue ? filter.EndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var responseEndDate = filter.ResponseEndDate.HasValue ? filter.ResponseEndDate.Value.Date.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var feedbackList = _reviewPushCustomerFeedbackRepository.IncludeMultiple(x => x.Franchisee).Where(x => (filter.FranchiseeId <= 0 || (x.FranchiseeId == filter.FranchiseeId))
                                                          && (responseEndDate == null || (x.Rp_date <= responseEndDate))
                                                          && (x.AuditActionId == (long)AuditActionType.Approved)
                                                         );


            return feedbackList;
        }

        public CustomerFeedbackReportResponseModel SaveCustomersFeebBack(CustomerFeedbackReportDomainModel filter)
        {
            try
            {

                var emailId = "";
                var customerId = default(long?);
                var urlSplit = filter.Url.Split(new string[] { "mailto:" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (urlSplit.Length > 0)
                {
                    emailId = urlSplit[0];
                    var customerDomain = _customerEmailRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Email == (emailId));
                    if (customerDomain != null)
                    {
                        customerId = customerDomain.Id;
                    }
                }

                var franchiseeId = GetFranchiseeFromDataBase(filter.FranchiseeName);
                var customerFeedBackDomain = new CustomerFeedbackResponse()
                {
                    FranchiseeId = franchiseeId != null ? franchiseeId : null,
                    AuditActionId = filter.Isapproved.GetValueOrDefault() == 1 ? (long)AuditActionType.Approved : (long)AuditActionType.Rejected,
                    CustomerName = filter.Name,
                    DateOfReview = _clock.ToUtc(filter.ResponseStartDate.GetValueOrDefault()),
                    Rating = filter.Rating.GetValueOrDefault(),
                    IsFromNewReviewSystem = true,
                    ResponseContent = filter.Text,
                    IsFromGoogleAPI = false,
                    IsFromSystemReviewSystem = true,
                    DateOfDataInDataBase = _clock.ToUtc(filter.ResponseStartDate.GetValueOrDefault()),
                    Url = filter.Url,
                    FeedbackId = null,
                    CustomerId = customerId,
                    ReviewId = null,
                    IsNew = true,
                    Id = 0,
                    Recommend = filter.Rating != null ? Convert.ToDouble(filter.Rating) + 5 : 0,


                };

                _customerFeedbackResponseRepository.Save(customerFeedBackDomain);
                return new CustomerFeedbackReportResponseModel()
                {
                    IsSuccess = true,
                    ErrorMessage = ""
                };
            }
            catch (Exception e1)
            {
                return new CustomerFeedbackReportResponseModel()
                {
                    IsSuccess = false,
                    ErrorMessage = e1.InnerException.ToString()
                };
            }
        }

        private long? GetFranchiseeFromDataBase(string taazaaFranchiseeName)
        {
            var organizationDomain = _organizationRepository.Table.FirstOrDefault(x => x.Name == taazaaFranchiseeName);
            var franchiseeId = default(long?);
            if (organizationDomain == null)
            {
                franchiseeId = default(long?);
            }
            else
            {
                franchiseeId = organizationDomain.Id;
            }
            return franchiseeId;

        }


        public BeforeAfterListModel GetBeforeAfterSelectedImagesWithProperties(BeforeAfterFilterModel filter)
        {
            var beforeAfterImageListModel = _beforeAfterImagesRepository.IncludeMultiple(x => x.Franchisee, x2 => x2.Franchisee.Organization, x2 => x2.ServiceType, x2 => x2.JobScheduler.ServiceType).Where(x =>
            (x.TypeId == ((long?)LookupTypes.AfterWork) || x.TypeId == ((long?)LookupTypes.BeforeWork) || x.TypeId == ((long?)LookupTypes.ExteriorBuilding) || x.TypeId == (long?)LookupTypes.DuringWork)
       && x.IsAddToLocalGallery == true && (x.TypeId == ((long?)LookupTypes.ExteriorBuilding) || (x.IsAddToLocalGallery && x.AddToGalleryDateTime >= filter.StartDate && x.AddToGalleryDateTime <= filter.EndDate))).ToList();

            var beforeAfterImageModel = beforeAfterImageListModel.Where(x => x.PairId == null && (x.TypeId == ((long?)LookupTypes.AfterWork) || x.TypeId == ((long?)LookupTypes.BeforeWork) || x.TypeId == ((long?)LookupTypes.DuringWork))).Select(x => new BeforeAfterViewModel
            {
                ServiceId = x.ServiceId,
                SurfaceColor = x.SurfaceColor,
                BuildingLocation = x.BuildingLocation,
                FinishType = x.FinishMaterial,
                ServiceType = x.ServiceType != null ? x.ServiceType.Name : "",
                SurfaceMaterial = x.SurfaceMaterial,
                SurfaceType = x.JobScheduler != null && x.JobScheduler.ServiceType != null ? x.JobScheduler.ServiceType.Name : "",
                BeforeImageUrl = x.S3BucketURL,
                AfterImageUrl = (GetJobEstimateServiceForNewAPI(beforeAfterImageListModel.FirstOrDefault(x1 => x1.PairId == x.Id))),
                ExteriorImageUrl = (GetJobEstimateServiceForNewAPI(beforeAfterImageListModel.FirstOrDefault(x1 => x1.CategoryId == x.CategoryId && x1.TypeId == ((long?)LookupTypes.ExteriorBuilding)))),
                FranchiseeName = x.Franchisee != null ? x.Franchisee.Organization.Name : "",
                FranchiseeUrl = x.Franchisee != null ? x.Franchisee.WebSite : "",
                FranchiseeId = x.Franchisee != null ? x.Franchisee.Id : default(long?),
                ImageId = x.Id,
                MarketingClass = x.JobEstimateServices.JobEstimateImageCategory.MarketingClass != null ? x.JobEstimateServices.JobEstimateImageCategory.MarketingClass.Name : "",
                BeforeImageUrlThumb = x.S3BucketThumbURL,
                AfterImageUrlThumb = GetJobEstimateServiceForNewAPIThumb(beforeAfterImageListModel.FirstOrDefault(x1 => x1.PairId == x.Id)),
                ExteriorImageUrlThumb = GetJobEstimateServiceForNewAPIThumb(beforeAfterImageListModel.FirstOrDefault(x1 => x1.CategoryId == x.CategoryId && x1.TypeId == ((long?)LookupTypes.ExteriorBuilding))),

            }).Distinct().ToList();

            var list = new List<BeforeAfterMainModel>();
            var beforeAfterImageListModelGrouped = beforeAfterImageModel.GroupBy(x => x.FranchiseeName).ToList();

            foreach (var beforeAfterImageListLocalModel in beforeAfterImageListModelGrouped)
            {
                list.Add(new BeforeAfterMainModel()
                {
                    FranchiseeId = beforeAfterImageListLocalModel.Any() ? beforeAfterImageListLocalModel.FirstOrDefault().FranchiseeId : default(long?),
                    FranchiseeName = beforeAfterImageListLocalModel.Key,
                    List = beforeAfterImageListLocalModel.ToList(),
                    OrderBy = beforeAfterImageListLocalModel.Any() ? beforeAfterImageListLocalModel.FirstOrDefault().FranchiseeId == filter.FranchiseeId ? 1 : 2 : 2,
                });
            }
            return new BeforeAfterListModel
            {
                Data = list.OrderBy(x => x.OrderBy).ToList(),
                //BeforeAfterList = beforeAfterImageModel,
                count = beforeAfterImageListModel.Count(),
                Status = "Success"
            };
        }
    }
}
