using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class ChargeCardService : IChargeCardService
    {
        private readonly IRepository<ChargeCard> _chargeCardRepository;
        private readonly IChargeCardFactory _chargeCardFactory;
        public ChargeCardService(IUnitOfWork unitOfWork, IChargeCardFactory chargeCardfactory)
        {
            _chargeCardRepository = unitOfWork.Repository<ChargeCard>();
            _chargeCardFactory = chargeCardfactory;
        }

        public ChargeCard Save(ChargeCardEditModel model)
        {
            var chargeCard = _chargeCardFactory.CreateChargeCard(model);
            _chargeCardRepository.Save(chargeCard);
            return chargeCard;
        }

    }
}
