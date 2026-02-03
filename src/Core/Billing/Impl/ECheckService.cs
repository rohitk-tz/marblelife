using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class ECheckService : IECheckService
    {
        private readonly IRepository<ECheck> _eCheckRepository;
        private readonly IECheckFactory _eCheckFactory;
        public ECheckService(IUnitOfWork unitOfWork, IECheckFactory eCheckFactory)
        {
            _eCheckRepository = unitOfWork.Repository<ECheck>();
            _eCheckFactory = eCheckFactory;
        }
        public ECheck Save(ECheckEditModel model)
        {
            var eCheck = _eCheckFactory.CreateDomain(model);
            _eCheckRepository.Save(eCheck);
            return eCheck;
        }
    }
}
