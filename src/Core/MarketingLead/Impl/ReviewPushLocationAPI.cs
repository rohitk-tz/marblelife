using Core.Application;
using Core.Application.Attribute;
using System.Net;
using System.Web.Script.Serialization;
using Core.Organizations.ViewModel;
using Core.Organizations.Domain;
using System.Linq;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    class ReviewPushLocationAPI : IReviewPushLocationAPI
    {
        private ILogService _logService;
        private ISettings _settings;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<ReviewPushAPILocation> _reviewPushAPILocationRepository;
        public ReviewPushLocationAPI(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _reviewPushAPILocationRepository = unitOfWork.Repository<ReviewPushAPILocation>();
        }
        public void ProcessRecord()
        {
            var apiKey = _settings.ReviewPushApiKey;

            if (string.IsNullOrEmpty(apiKey))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }

            var result = GetFranciseeWithRPIDfromAPI(apiKey);
            var response = (new JavaScriptSerializer()).Deserialize<ReviewPushFranchiseeListModel>(result);
            var franchiseeList = response.info;
            foreach (var franchisee in franchiseeList)
            {
                var reviewPushAPILocationDomain = _reviewPushAPILocationRepository.Table.FirstOrDefault(x => x.Location_Id == franchisee.Id);
                if (reviewPushAPILocationDomain != null)
                {
                    reviewPushAPILocationDomain.Name = franchisee.Name;
                    reviewPushAPILocationDomain.Rp_ID = franchisee.RP_ID;
                    _reviewPushAPILocationRepository.Save(reviewPushAPILocationDomain);
                }
                else
                {
                    var franchiseeWithRPID = new ReviewPushAPILocation()
                    {
                        IsNew = true,
                        Location_Id = franchisee.Id,
                        Name = franchisee.Name,
                        Rp_ID = franchisee.RP_ID
                    };
                    _reviewPushAPILocationRepository.Save(franchiseeWithRPID);
                }
                _unitOfWork.SaveChanges();
            }


        }
        private string GetFranciseeWithRPIDfromAPI(string apiKey)
        {
            string url = string.Format("https://marblelife.com/ziplocator/API/getReviewLocations/token/{0}", apiKey);

            var result = string.Empty;
            using (var client = new WebClient())
            {
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Franchisee With RPID Exception :", ex.Message));
                }
            }
            return result;
        }
    }
}
