using Core.Application;
using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class RoyaltyFeeSlabsFactory : IRoyaltyFeeSlabsFactory
    {
        private readonly IRepository<RoyaltyFeeSlabs> _royaltyFeeSlabRepository;

        public RoyaltyFeeSlabsFactory(IUnitOfWork unitOfWork)
        {
            _royaltyFeeSlabRepository = unitOfWork.Repository<RoyaltyFeeSlabs>();
        }

        public RoyaltyFeeSlabsEditModel CreateEditModel(RoyaltyFeeSlabs domain)
        {
            return new RoyaltyFeeSlabsEditModel()
            {
                ChargePercentage = domain.ChargePercentage,
                MaxValue = domain.MaxValue,
                MinValue = domain.MinValue
            };
        }

        public MinRoyaltyFeeSlabsEditModel CreateEditModelForMinRoyality(MinRoyaltyFeeSlabs domain)
        {
            return new MinRoyaltyFeeSlabsEditModel()
            {
                MinRoyality = domain.MinRoyality,
                StartValue = domain.StartValue,
                EndValue = domain.EndValue!=2900?domain.EndValue:null,
                Id = domain.Id
            };
        }

        public IEnumerable<RoyaltyFeeSlabsEditModel> CreateModelCollection(IEnumerable<RoyaltyFeeSlabs> domainCollection)
        {
            return domainCollection.Select(CreateEditModel).ToList();
        }

        public IEnumerable<MinRoyaltyFeeSlabsEditModel> CreateModelForMinRoyalityCollection(IEnumerable<MinRoyaltyFeeSlabs> domainCollection)
        {
            return domainCollection.Select(CreateEditModelForMinRoyality).ToList();
        }

        public IEnumerable<RoyaltyFeeSlabs> CreateDomainCollection(IEnumerable<RoyaltyFeeSlabsEditModel> modelCollection, FeeProfile feeProfile)
        {
            var isDeleted = false;
            IList<RoyaltyFeeSlabs> inDb = new List<RoyaltyFeeSlabs>();
            if (feeProfile.Id > 0)
            {
                inDb = _royaltyFeeSlabRepository.Fetch(x => x.RoyaltyFeeProfile.Id == feeProfile.Id).ToList();
            }

            var index = 0;
            foreach (var item in modelCollection)
            {
                var obj = new RoyaltyFeeSlabs();
                if (index >= inDb.Count())
                {
                    obj.IsNew = true;
                    obj.RoyaltyFeeProfile = feeProfile;
                    inDb.Add(obj);
                }
                else
                {
                    obj = inDb.ElementAt(index);
                }

                obj.ChargePercentage = item.ChargePercentage;
                obj.MaxValue = item.MaxValue;
                obj.MinValue = item.MinValue;

                index++;
            }

            while (index < inDb.Count)
            {
                if (!isDeleted)
                {
                    isDeleted = true;
                    foreach (var domain in inDb)
                    {
                        _royaltyFeeSlabRepository.Delete(domain);
                    }
                }
                inDb.RemoveAt(index);
            }

            return inDb;
        }

    }
}
