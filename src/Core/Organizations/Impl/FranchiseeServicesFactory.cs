using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Organizations.ViewModels;
using System.Collections.Generic;
using Core.Application;
using System.Linq;
using Core.Application.Enum;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeServicesFactory : IFranchiseeServicesFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<FranchiseeService> _franchiseeServiceRepository;
        private readonly IRepository<ServiceType> _serviceTypeRepository;

        public FranchiseeServicesFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _franchiseeServiceRepository = unitOfWork.Repository<FranchiseeService>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
        }

        public IEnumerable<FranchiseeService> CreateDomainCollection(IEnumerable<FranchiseeServiceEditModel> model, Franchisee franchisee)
        {
            var servicesInDb = new List<FranchiseeService>();
            if (franchisee.Id > 0)
            {
                servicesInDb = _franchiseeServiceRepository.Fetch(x => x.FranchiseeId == franchisee.Id).ToList();
            }

            var index = 0;
            foreach (var item in model)
            {
                FranchiseeService obj = null;
                if (servicesInDb.Count < index + 1)
                {
                    obj = new FranchiseeService();
                    obj.IsNew = true;
                    obj.Franchisee = franchisee;
                    servicesInDb.Add(obj);
                }
                else
                {
                    obj = servicesInDb.ElementAt(index);
                }

                obj.CalculateRoyalty = item.CalculateRoyalty;
                obj.ServiceTypeId = item.ServiceTypeId;
                obj.FranchiseeId = franchisee.Id;
                obj.IsActive = item.IsActive;
                obj.IsCertified = item.IsActive ? item.IsCertified : false;
                index++;
            }

            while (index < servicesInDb.Count)
            {
                servicesInDb.RemoveAt(index);
            }

            return servicesInDb;
        }

        public IEnumerable<FranchiseeServiceEditModel> CreateEditModel(IEnumerable<FranchiseeService> franchiseeServices)
        {
            var services = _serviceTypeRepository.Table.Where(x => (x.CategoryId == (long)LookupTypes.Restoration || x.CategoryId == (long)LookupTypes.Maintenance || x.CategoryId == (long)LookupTypes.FRONTOFFICECALLMANAGEMENT) && x.IsActive && x.DashboardServices).Select(x => x.Id).Distinct().ToList();
            var servicesId = _franchiseeServiceRepository.Table.Where(x => (x.ServiceType.CategoryId == (long)LookupTypes.Restoration || x.ServiceType.CategoryId == (long)LookupTypes.Maintenance || x.ServiceType.CategoryId == (long)LookupTypes.FRONTOFFICECALLMANAGEMENT) && x.ServiceType.DashboardServices).Select(x => x.ServiceTypeId).Distinct().ToList();
            var newServices = services.Except(servicesId).ToList();
            if (franchiseeServices == null || franchiseeServices.Count() < 1)
                return PrepareListModel();

            IList<FranchiseeServiceEditModel> list = new List<FranchiseeServiceEditModel>();
            foreach (var service in franchiseeServices)
            {
                var model = new FranchiseeServiceEditModel();
                model.CalculateRoyalty = service.CalculateRoyalty;
                model.ServiceTypeId = service.ServiceTypeId;
                model.Name = service.ServiceType.Name;
                model.CategoryId = service.ServiceType.CategoryId;
                model.CategoryName = service.ServiceType.Category.Name;
                model.IsActive = service.IsActive;
                model.IsCertified = service.IsCertified;
                list.Add(model);
            }
            foreach (var serviceId in newServices)
            {
                var service = _serviceTypeRepository.Get(serviceId);
                var model = new FranchiseeServiceEditModel();
                model.CalculateRoyalty = true;
                model.ServiceTypeId = service.Id;
                model.Name = service.Name;
                model.CategoryId = service.CategoryId;
                model.CategoryName = service.Category.Name;
                model.IsActive = service.IsActive;
                list.Add(model);
            }
            return list;
        }

        private IList<FranchiseeServiceEditModel> PrepareListModel()
        {
            var validCategories = new[]
            {
                (long)Enum.ServiceTypeCategory.Restoration,
                (long)Enum.ServiceTypeCategory.Maintenance,
                (long)Enum.ServiceTypeCategory.FRONTOFFICECALLMANAGEMENT
            };

            var services = _serviceTypeRepository.Table.Where(s => validCategories.Contains(s.CategoryId)&& s.IsActive&& s.DashboardServices).ToList();
            IList<FranchiseeServiceEditModel> list = new List<FranchiseeServiceEditModel>();
            foreach (var service in services)
            {
                var model = PrepareModel(service);
                list.Add(model);
            }
            return list;
        }

        private FranchiseeServiceEditModel PrepareModel(ServiceType service)
        {
            var model = new FranchiseeServiceEditModel();
            model.ServiceTypeId = service.Id;
            model.Name = service.Name;
            model.CategoryId = service.CategoryId;
            model.CategoryName = service.Category.Name;
            model.CalculateRoyalty = true;
            model.IsActive = true;
            return model;
        }
        public IList<FranchiseeService> GetFranchiseeServicesByFranchiseeId(long franchiseeId)
        {
            //TO DO: Implement this to get Franchisee services
            return new List<FranchiseeService>();
        }
    }
}
