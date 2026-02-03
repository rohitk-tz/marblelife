using Core.Application;
using Core.Application.Attribute;
using Core.Reports;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System;

namespace Core.Geo.Impl
{
    [DefaultImplementation]
    public class EmailFactory : IEmailFactory
    {
        private IClock _clock;
        private readonly IRepository<CustomerEmail> _customerEmailRepository;
        private readonly ICustomerEmailReportService _customerEmailReportService;

        public EmailFactory(IClock clock, IUnitOfWork unitOfWork, ICustomerEmailReportService customerEmailReportService)
        {
            _clock = clock;
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _customerEmailReportService = customerEmailReportService;
        }
        public CustomerEmail CreateDomain(EmailEditModel model, long id)
        {
            var domain = _customerEmailRepository.Get(model.Id);
            return new CustomerEmail
            {
                Id = model.Id,

                CustomerId = model.CustomerId <= 0 ? id : model.CustomerId,
                Email = model.email,
                DateCreated = (domain != null && domain.DateCreated != null) ? domain.DateCreated : model.DateCreated,
                IsNew = model.Id <= 0
            };
        }

        public CustomerEmail CreateDomain(CustomerEmail model, long id)
        {
            var domain = _customerEmailRepository.Get(model.Id);
            return new CustomerEmail
            {
                Id = model.Id,
                CustomerId = model.CustomerId <= 0 ? id : model.CustomerId,
                Email = model.Email,
                DateCreated = (domain != null && domain.DateCreated != null) ? domain.DateCreated : model.DateCreated,
                IsNew = model.Id <= 0
            };
        }

        public EmailEditModel CreateEditModel(CustomerEmail domain,string email=null)
        {
            var isSynced = _customerEmailReportService.IsCustomerEmailSyncedToEmailAPI(domain.CustomerId, domain.Email);
            return new EmailEditModel
            {
                Id = domain.Id,
                CustomerId = domain.CustomerId,
                DateCreated = domain.DateCreated,
                //email = domain.Email!=null? domain.Email: email,
                email = email == null ? domain.Email : email,
                IsSynced = isSynced
            };
        }
    }
}
