using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class BeforeAfterImageService : IBeforeAfterImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IRepository<JobEstimateImage> _jobEstimateImage;
        private IRepository<JobEstimateServices> _jobEstimateServicesCategory;
        private IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IJobFactory _jobFactory;
        private readonly ISettings _settings;
        public BeforeAfterImageService(IUnitOfWork unitOfWork, IJobFactory jobFactory, ISettings settings)
        {
            _unitOfWork = unitOfWork;
            _jobEstimateImage = unitOfWork.Repository<JobEstimateImage>();
            _jobEstimateServicesCategory = unitOfWork.Repository<JobEstimateServices>();
            _jobFactory = jobFactory;
            _settings = settings;
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
        }
        public List<BeforeAfterForImageViewModel> GetBeforeAfterImagesForFranchiseeAdmin(List<long> schedulerIds, DateTime startDate,
            DateTime endDates, BeforeAfterImageFilter filter, List<JobScheduler> jobSchedulerList
            , List<JobEstimateServices> jobEstimateServicesList, List<JobEstimateImage> jobEstimateImagesList)
        {
            var beforeAfterForImageViewModel = new List<BeforeAfterForImageViewModel>();
            var beforeAfterForImageViewModelList = new List<BeforeAfterForImageViewModel>();
            var schedulerSelectedList = new List<SelectedScheduler>();

            foreach (var schedulerId in schedulerIds)
            {
                var jobEstimateServices = jobEstimateServicesList.Where(x => (x.JobEstimateImageCategory.SchedulerId != null) && (filter.MarketingClassId == 0 || (x.JobEstimateImageCategory.MarkertingClassId == filter.MarketingClassId)) && (schedulerId == (x.JobEstimateImageCategory.SchedulerId.Value))).ToList();

                var jobEstimateImageWithPairList = jobEstimateServices.Where(x => x.PairId != null && (x.TypeId == (long?)(BeforeAfterImagesType.After) || (x.TypeId == (long?)(BeforeAfterImagesType.Before) || (x.TypeId == (long?)(BeforeAfterImagesType.During))))).ToList();
                var jobEstimateImageWithOutPairList = jobEstimateServices.Where(x => x.PairId == null && (x.TypeId == (long?)(BeforeAfterImagesType.After) || (x.TypeId == (long?)(BeforeAfterImagesType.Before) || (x.TypeId == (long?)(BeforeAfterImagesType.During))))).ToList();

                var jobEstimateImageIdWithPairList = jobEstimateImageWithPairList.Select(x => x.Id).ToList();
                var jobEstimateImageIdWithoutPairList = jobEstimateImageWithOutPairList.Select(x => x.Id).ToList();


                var jobEstimateServicesId = jobEstimateServices.Select(x => x.Id).ToList();
                var jobEstimateImages = jobEstimateImagesList.Where(x => jobEstimateServicesId.Contains(x.ServiceId.Value)).ToList();

                var buildingExteriorImagesService = jobEstimateServices.FirstOrDefault(x => (x.TypeId == (long?)(BeforeAfterImagesType.ExteriorBuilding)));

                //var jobestimateBeforeCategory = jobEstimateImageWithOutPairList != null ? jobEstimateImageWithOutPairList.FirstOrDefault(x1 => x1.Id == x.PairId) : null;
                beforeAfterForImageViewModel = jobEstimateImageWithPairList.Select(x => _jobFactory.CreateNeforeAfterViewModel(jobEstimateImageWithOutPairList != null ? jobEstimateImageWithOutPairList.FirstOrDefault(x1 => x1.Id == x.PairId) : null, x,
                    jobEstimateImages, buildingExteriorImagesService)).ToList();

                if (jobEstimateImageWithPairList.Count() == 0 && jobEstimateImageWithOutPairList.Count() > 0)
                {
                    beforeAfterForImageViewModel = jobEstimateImageWithOutPairList.Select(x => _jobFactory.CreateNeforeAfterViewModel(x, null,
                    jobEstimateImages, buildingExteriorImagesService)).ToList();
                }


                //beforeAfterForImageViewModel = beforeAfterForImageViewModel.Where(x => x.AfterServiceId != null && x.BeforeServiceId != null).ToList();
                beforeAfterForImageViewModel = beforeAfterForImageViewModel.Where(x => x.AfterServiceId != default(long?) && x.BeforeServiceId != default(long?)).ToList();


                if ((jobEstimateImageWithPairList.Count() == 0 && ((filter.SurfaceColor == "" && filter.SurfaceMaterial == "" && filter.FinishMaterial == ""
                    && filter.BuildingType == "" && filter.ServiceTypeId == 0 && filter.ManagementCompany == "" && filter.MaidService == ""
                    && filter.SurfaceTypeId == null && filter.MarketingClassId == 0 && filter.BuildingLocation == "" && filter.BuildingLocation == "")))
                    || jobEstimateImageWithPairList.Count() == 0 && buildingExteriorImagesService != default(JobEstimateServices))
                {
                    var jobScheduler = jobSchedulerList.FirstOrDefault(x => x.Id == schedulerId);
                    var isNonResistianCLass = jobScheduler.Job != null ? (jobScheduler.Job.JobTypeId == (long)NonResidentalClassEnum.FLOORING) || (jobScheduler.Job.JobTypeId == (long)NonResidentalClassEnum.HOMEMANAGEMENT) ||
                  (jobScheduler.Job.JobTypeId == (long)NonResidentalClassEnum.INTERIORDESIGN) || (jobScheduler.Job.JobTypeId == (long)NonResidentalClassEnum.RESIDENTIAL)
                  || jobScheduler.Job.JobTypeId == (long)NonResidentalClassEnum.RESIDENTIALPROPERTYMGMT || (jobScheduler.Job.JobTypeId == (long)NonResidentalClassEnum.UNCLASSIFIED) :
                  (jobScheduler.Estimate.TypeId == (long)NonResidentalClassEnum.FLOORING) || (jobScheduler.Estimate.TypeId == (long)NonResidentalClassEnum.HOMEMANAGEMENT) ||
                  (jobScheduler.Estimate.TypeId == (long)NonResidentalClassEnum.INTERIORDESIGN) || (jobScheduler.Estimate.TypeId == (long)NonResidentalClassEnum.RESIDENTIAL)
                  || jobScheduler.Estimate.TypeId == (long)NonResidentalClassEnum.RESIDENTIALPROPERTYMGMT || (jobScheduler.Estimate.TypeId == (long)NonResidentalClassEnum.UNCLASSIFIED);

                    string linkUrl = "";
                    var jobEstimateImageBuildingExterior = jobEstimateImages.FirstOrDefault(x => x.TypeId == (long?)(BeforeAfterImagesType.ExteriorBuilding));

                    var isFilterUsed = (filter.SurfaceColor != "" || filter.SurfaceMaterial != "" || filter.FinishMaterial != "" ||
                filter.BuildingType != "" || filter.ServiceTypeId != 0 || filter.ManagementCompany != "" || filter.MaidService != ""
                || filter.SurfaceTypeId != null || filter.BuildingLocation != "") ? true : false;
                    if (isFilterUsed)
                    {
                        beforeAfterForImageViewModelList.AddRange(beforeAfterForImageViewModel);
                        continue;
                    }
                    if (jobScheduler.JobId != null)
                    {
                        linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + jobScheduler.JobId + "/edit/" + jobScheduler.Id;
                    }
                    else
                    {
                        linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + jobScheduler.EstimateId + "/manage/" + jobScheduler.Id;
                    }
                    var marketingClass = jobScheduler.Job != null ? jobScheduler.Job.JobType.Name : jobScheduler.Estimate.MarketingClass.Name;
                    var viewModel = new BeforeAfterForImageViewModel()
                    {
                        AfterImageId = 0,
                        IsBestPicture = false,
                        RelactiveLocationBeforeImageUrl = "",
                        RelactiveLocationAfterImageUrl = "",
                        IsImageEmpty = true,
                        Title = jobScheduler.Title,
                        ServicesType = "",
                        SurfaceType = "",
                        SurfaceColor = "",
                        RelactiveLocationBeforeImage = "",
                        RelactiveLocationAfterImage = "",
                        SchedulerUrl = linkUrl,
                        IsJob = jobScheduler.JobId != null ? true : false,
                        JobId = jobScheduler.JobId,
                        EstimateId = jobScheduler.EstimateId,
                        IsComercialClass = isNonResistianCLass,
                        MarketingClass = marketingClass,
                        OrderNo = isNonResistianCLass ? 1 : 100,
                        RelactiveLocationExteriorImageUrl = jobEstimateImageBuildingExterior != null ? GetBase64String(jobEstimateImageBuildingExterior.File) : "",
                        SchedulerNames = jobScheduler.Job != null ? "J" + jobScheduler.JobId : "E" + jobScheduler.EstimateId,
                        JobEstimateId = jobScheduler.Job != null ? jobScheduler.JobId : jobScheduler.EstimateId,
                        CustomerName = jobScheduler.Job != null ? jobScheduler.Job.JobCustomer.CustomerName : jobScheduler.Estimate.JobCustomer.CustomerName,
                        RelactiveLocationAfterImageUrlThumb = "/Content/images/no_image_thumb.gif",
                        RelactiveLocationBeforeImageUrlThumb = "/Content/images/no_image_thumb.gif",
                    };
                    var isAlreadyPresentInBeforeAfter =
                        beforeAfterForImageViewModelList.Any(x => (x.Description == jobScheduler.Title || x.Title == jobScheduler.Title) && (x.JobId == jobScheduler.JobId && x.EstimateId == jobScheduler.EstimateId));
                    if (!isAlreadyPresentInBeforeAfter)
                    {
                        beforeAfterForImageViewModelList.Add(viewModel);
                    }

                }
                else
                {
                    beforeAfterForImageViewModelList.AddRange(beforeAfterForImageViewModel);
                }

            }
            return beforeAfterForImageViewModelList;
        }

        private string GetBase64String(Application.Domain.File file)
        {

            if (file == null)
            {
                return "";
            }
            try
            {
                var filePath = (file.RelativeLocation + @"\" + file.Name).ToPath();
                byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                base64ImageRepresentation = "data:image/png;base64," + base64ImageRepresentation;
                return base64ImageRepresentation;
            }
            catch (Exception e1)
            {
                return "";
            }
        }



        public class SelectedScheduler
        {
            public long? JobId { get; set; }
            public string Description { get; set; }
            public long? EstimateId { get; set; }
            public string BeforeImageUrl { get; set; }
            public string AfterImageUrl { get; set; }
        }
    }
}
