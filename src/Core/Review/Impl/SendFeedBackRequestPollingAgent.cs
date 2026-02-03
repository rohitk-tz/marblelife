using Core.Application;
using Core.Application.Attribute;
using Core.Review.Domain;
using System;
using System.Linq;

namespace Core.Review.Impl
{
    [DefaultImplementation]
    public class SendFeedBackRequestPollingAgent : ISendFeedBackRequestPollingAgent
    {
        private readonly IRepository<CustomerFeedbackRequest> _customerFeedbackRequestRepository;
        private readonly ILogService _logService;
        private readonly ICustomerFeedbackFactory _customerFeedbackFactory;
        private readonly ICustomerFeedbackService _customerReviewService;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;

        public SendFeedBackRequestPollingAgent(IUnitOfWork unitOfWork, ISettings settings, ILogService logService, ICustomerFeedbackFactory customerFeedbackFactory,
            ICustomerFeedbackService customerReviewService)
        {
            _customerFeedbackRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _logService = logService;
            _customerFeedbackFactory = customerFeedbackFactory;
            _customerReviewService = customerReviewService;
            _settings = settings;
            _unitOfWork = unitOfWork;
        }
        public void SendFeedback()
        {
            var sendFeedbackEnabled = _settings.SendFeedbackEnabled;
            if (!sendFeedbackEnabled)
            {
                _logService.Info("Feedback request is Disabled!");
                return;
            }

            var queuedFeedbackRequests = _customerFeedbackRequestRepository.Table.Where(x => x.IsQueued && !x.IsSystemGenerated).ToList();

            foreach (var request in queuedFeedbackRequests)
            {
                try
                {
                    if (!request.Franchisee.IsReviewFeedbackEnabled)
                    {
                        if (request.CustomerReviewSystemRecord != null)
                            _logService.Info(string.Format("Feedback request for the Franchisee {0} is turned off!", request.CustomerReviewSystemRecord.Franchisee.Organization.Name));
                      else
                            _logService.Info(string.Format("Feedback request for the Franchisee is turned off!"));

                        continue;
                    }

                    var clientId = _settings.ClientId;
                    if (clientId == null)
                        _logService.Info("Invalid Client Id!");

                    var customer = _customerReviewService.GetCustomer(request.CustomerReviewSystemRecord.ReviewSystemCustomerId, clientId);
                    if (customer != null && customer.CustomerId > 0)
                    {
                        try
                        {
                            var response = _customerReviewService.SendFeedbackRequest(customer.CustomerId, clientId);

                            _unitOfWork.StartTransaction();

                            if (response != null && response.errorCode == 0)
                                request.IsQueued = false;
                            else
                                request.IsQueued = true;
                            request.DataPacket = response.DataPacket;
                            _customerFeedbackRequestRepository.Save(request);

                            _unitOfWork.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            _logService.Info(string.Format("Exception :", ex.StackTrace));
                            _unitOfWork.Rollback();
                        }
                    }
                }
                catch (Exception e1)
                {

                }
            }

        }
    }
}
