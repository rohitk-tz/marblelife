using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.MarketingLead;
using Core.MarketingLead.Domain;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class HomeAdvisorParser : IHomeAdvisorParser
    {
        private readonly ILogService _logService;
        private ISettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<HomeAdvisor> _homeAdvisorrRepository;
        private readonly IClock _clock;
        private readonly IRepository<JobEstimateImage> _jobEstimateImageRepository;
        public HomeAdvisorParser(IUnitOfWork unitOfWork, ISettings settings)
        {
            _settings = settings;
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _unitOfWork = unitOfWork;
            _homeAdvisorrRepository = unitOfWork.Repository<HomeAdvisor>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _jobEstimateImageRepository = unitOfWork.Repository<JobEstimateImage>();
        }

        public void ProcessRecords()
        {
            _logService.Info("Starting Home Advisor Prccessing Records at " + _clock.UtcNow);

            if (_settings.HomeAdvisorParsingIsDisabled)
            {
                _logService.Debug("Home Advisor Prccessing is disabled");
                return;
            }
            else
            {
                //CallingHomeAdvisor();
            }
            if (!_settings.BeforeAfterImageParsing)
            {
                _logService.Debug("Before After Image Prccessing is disabled");
                return;
            }
            else
            {
                //BeforeAfterImageProcessing();
            }
        }

        private void CallingHomeAdvisor()
        {
            DataTable data;
            try
            {
                IList<HomeAdvisorParentModel> collection = new List<HomeAdvisorParentModel>();
                var homeAdvisorFileParser = ApplicationManager.DependencyInjection.Resolve<IHomeAdvisorFileParser>();

                var filePath = MediaLocationHelper.FilePath(System.Web.Hosting.HostingEnvironment.MapPath("~/bin"), "HomeAdvisor1.xlsx").ToFullPathForHomeAdvisor();
                data = HomeAdvisorFileParser.ReadExcel(filePath);
                homeAdvisorFileParser.PrepareHeaderIndex(data);
                collection = homeAdvisorFileParser.PrepareDomainFromDataTable(data);

                foreach (var viewModel in collection)
                {
                    var homeAddvisorDomain = _homeAdvisorrRepository.Table.FirstOrDefault(x => x.SRID == viewModel.SRID);
                    if (homeAddvisorDomain == null)
                    {
                        var homeAdivorDomainModel = new HomeAdvisor()
                        {
                            CityName = viewModel.CityName,
                            CityId = viewModel.CityId,
                            StateId = viewModel.StateId != 0 && viewModel != null ? viewModel.StateId : default(long?),
                            StateName = viewModel.StateName,
                            CompanyName = viewModel.CompanyName,
                            HAAccount = viewModel.HAAccount,
                            LeadType = viewModel.LeadType,
                            NetLeadDollar = viewModel.NetLeadDollar,
                            SRID = viewModel.SRID,
                            SRSubmittedDate = viewModel.SRSubmittedDate,
                            ZipCode = viewModel.ZipCode,
                            Task = viewModel.Task,
                            IsNew = true,
                            FranchiseeId = GetFranchiseeIdByCompanyName(viewModel.CompanyName)
                        };
                        _homeAdvisorrRepository.Save(homeAdivorDomainModel);

                    }
                    else
                    {
                        var homeAdivorDomainModel = new HomeAdvisor()
                        {
                            CityName = viewModel.CityName,
                            CityId = viewModel.CityId,
                            StateId = viewModel.StateId != 0 && viewModel != null ? viewModel.StateId : default(long?),
                            StateName = viewModel.StateName,
                            CompanyName = viewModel.CompanyName,
                            HAAccount = viewModel.HAAccount,
                            LeadType = viewModel.LeadType,
                            NetLeadDollar = viewModel.NetLeadDollar,
                            SRID = viewModel.SRID,
                            SRSubmittedDate = viewModel.SRSubmittedDate,
                            ZipCode = viewModel.ZipCode,
                            Task = viewModel.Task,
                            IsNew = false,
                            Id = homeAddvisorDomain.Id,
                            FranchiseeId = GetFranchiseeIdByCompanyName(viewModel.CompanyName)
                        };
                        _homeAdvisorrRepository.Save(homeAdivorDomainModel);
                    }
                    _unitOfWork.SaveChanges();
                }
                _logService.Info("Ends Home Advisor Prccessing Records at " + _clock.UtcNow);
            }
            catch (Exception e1)
            {

            }
        }


        private void BeforeAfterImageProcessing()
        {
            //var jobEstimateImageList = _jobEstimateImageRepository.Table.Where(x => x.Base64ImageUrl == null).OrderByDescending(x => x.Id).ToList();
            //foreach (var jobEstimateImage in jobEstimateImageList)
            //{
            //    var base64string = GetBase64String(jobEstimateImage.File);
            //    var base64StringFirstHalf = base64string.Substring(0, base64string.Length / 2-1);
            //    var base64StringSecondHalf = base64string.Substring(base64string.Length / 2-1, base64string.Length-1);
            //    jobEstimateImage.Base64ImageUrl = base64StringFirstHalf;
            //    jobEstimateImage.Base64ImageUrl2 = base64StringSecondHalf;
            //    //jobEstimateImage.Base64ImageUrl = base64string != "" ? base64string : null;
            //    _jobEstimateImageRepository.Save(jobEstimateImage);
            //    _unitOfWork.SaveChanges();
            //}
        }
        private long? GetFranchiseeIdByCompanyName(string companyName)
        {
            switch (companyName)
            {
                case "Marblelife of Southeast Michigan":
                    {
                        return 62;
                    }
                case "MarbleLife of Connecticut":
                    {
                        return 11;
                    }
                case "Marblelife of Minnesota":
                    {
                        return 70;
                    }
                case "MarbleLife of Cincinnati":
                    {
                        return 40;
                    }
                case "MarbleLife of Tampa Bay, Inc.":
                    {
                        return 14;
                    }
                case "Marblelife of Southwest Alabama":
                    {
                        return 3;
                    }

                case "Central Alabama Marblelife, LLC":
                    {
                        return 82;
                    }
                case "MarbleLife of Utah, Inc.":
                    {
                        return 48;
                    }
                case "Marblelife of Portland":
                    {
                        return 41;
                    }
                case "MarbleLife of Atlanta":
                    {
                        return 21;
                    }
                case "MarbleLife of St. Louis":
                    {
                        return 30;
                    }

                case "MarbleLife of the Carolina's":
                    {
                        return 31;
                    }
                case "Marblelife Philadelphia":
                    {
                        return 42;
                    }
                case "MarbleLife of Nashville":
                    {
                        return 45;
                    }

                case "Marblelife of Pittsburgh":
                    {
                        return 64;
                    }
                case "Marblelife of Indianapolis":
                    {
                        return 75;
                    }
                case "MarbleLife of Las Vegas":
                    {
                        return 77;
                    }
                case "Marblelife of Kansas City":
                    {
                        return 81;
                    }
                case "Marblelife of Denver":
                    {
                        return 85;
                    }
                default:
                    {
                        return null;
                    }
            }
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
    }
}
