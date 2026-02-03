using Core.Application;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using Core.Users;
using Core.Users.Enum;
using Core.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Organizations.Impl
{
    public class FranchiseeMigrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFranchiseeInfoService _franchiseeService;
        private readonly IUserService _userService;
        private readonly List<ServiceType> _franchiseeServices;

        public FranchiseeMigrationService(IUnitOfWork unitOfWork, IFranchiseeInfoService franchiseeService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _franchiseeService = franchiseeService;
            _userService = userService;

            var repoFranchiseeService = unitOfWork.Repository<ServiceType>();
            _franchiseeServices = repoFranchiseeService.Table.ToList();
        }

        public void ProcessRecords(IList<FranchiseeDetailsModel> list)
        {

            foreach (var record in list)
            {
                var franchisee = record.Franchisee;
                CompleteFranchiseeServiceRecords(franchisee);

                try
                {
                    _unitOfWork.StartTransaction();
                    _franchiseeService.Save(franchisee);

                    var users = record.users ?? new List<UserEditModel>();

                    foreach (var user in users)
                    {
                        user.OrganizationId = franchisee.Id;
                        user.RoleId = (long)RoleType.FranchiseeAdmin;
                        _userService.Save(user);
                    }

                    Console.WriteLine("Franchisee {0} created ", franchisee.Name);

                    _unitOfWork.SaveChanges();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Franchisee {0} exception {1} \r\n{2}", franchisee.Name, ex.Message, ex.StackTrace);

                    _unitOfWork.Rollback();
                    throw ex;
                }
            }
        }

        private void CompleteFranchiseeServiceRecords(FranchiseeEditModel franchiseeEditModel)
        {
            franchiseeEditModel.FranchiseeServices = _franchiseeServices.Select(x =>
            {
                var obj = franchiseeEditModel.FranchiseeServices.SingleOrDefault(m => m.ServiceTypeId == x.Id);
                if (obj != null)
                    return obj;

                return new FranchiseeServiceEditModel
                {
                    ServiceTypeId = x.Id
                };

            }).ToList();
        }
    }
}
