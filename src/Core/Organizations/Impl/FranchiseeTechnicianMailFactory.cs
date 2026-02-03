using Core.Application.Attribute;
using Core.Organizations.ViewModel;
using Core.Scheduler.Domain;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeTechnicianMailFactory : IFranchiseeTechnicianMailFactory
    {
        public FranchiseeEmailEditModel CreateEditModel(FranchiseeTechMailService franchiseeTechMailService)
        {
            return new FranchiseeEmailEditModel()
            {
                Amount = franchiseeTechMailService == null ? 0 : franchiseeTechMailService.Amount,
                IsGeneric = franchiseeTechMailService == null ? false : franchiseeTechMailService.IsGeneric,
                TechnianCount = franchiseeTechMailService == null ? 0 : (int)franchiseeTechMailService.TechCount,
                Id = franchiseeTechMailService == null ? 0 : franchiseeTechMailService.Id,
                MultiplacationFactor = franchiseeTechMailService == null ? 2.75 : franchiseeTechMailService.MultiplicationFactor,
                isTechMailFees = franchiseeTechMailService == null ? false : !franchiseeTechMailService.IsGeneric,
                ChargesForPhone= franchiseeTechMailService == null ? 15 : default(double?),
            };
        }
    }
}
