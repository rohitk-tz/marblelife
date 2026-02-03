using System.Collections.Generic;
using System.Linq;
using Api.Areas.Application.ViewModel;

using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;

using Core.Geo.Domain;
using Core.Users.Domain;
using Core.Users.Enum;
using Core.Reports.Enum;
using Core.Organizations.Domain;
using API.Areas.Application.ViewModel;
using System;
using Core.Application.Enum;
using Core.Sales.Domain;
using System.Globalization;
using Core.Scheduler.Domain;
using Core.Sales.Enum;
using Core.MarketingLead.Domain;
using System.Security.Cryptography.X509Certificates;

namespace Api.Areas.Application.Impl
{
    [DefaultImplementation]
    public class DropDownHelperService : IDropDownHelperService
    {

        private readonly IUnitOfWork _unitOfWork;
        public DropDownHelperService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IEnumerable<ServiceTypeListItem> GetServiceTypes()
        {
            var serviceTypes = _unitOfWork.Repository<ServiceType>();
            return serviceTypes.Table.OrderBy(x => x.OrderBy).Select(s => new ServiceTypeListItem
            {
                Display = s.Name,
                Value = s.Id.ToString(),
                CategoryId = s.CategoryId.ToString(),
                CategoryName = s.Category.Name
            }).ToArray();
        }
        public IEnumerable<ServiceTypeListItem> GetServiceTypesForInvoice()
        {
            List<long> ides = new List<long>() {1, 2, 3, 4, 5, 6, 7, 8, 11, 13, 15, 16, 17, 18, 19, 33, 34, 35, 38, 40, 42, 43};
            var serviceTypes = _unitOfWork.Repository<ServiceType>().Table.Where(x => ides.Contains(x.Id));

            return serviceTypes.OrderBy(x => x.OrderBy).Select(s => new ServiceTypeListItem
            {
                Display = s.Name,
                Value = s.Id.ToString(),
                CategoryId = s.CategoryId.ToString(),
                CategoryName = s.Category.Name
            }).ToArray();
        }
        
        public IEnumerable<ServiceTypeListItem> GetServiceTypesForInvoiceNew()
        {
            List<long> ides = new List<long>() {1, 2, 3, 4, 5, 6, 7, 8, 11, 13, 15, 16, 17, 18, 19, 33, 34, 35, 38, 40, 42, 44, 45, 43};
            var serviceTypes = _unitOfWork.Repository<ServiceType>().Table.Where(x => ides.Contains(x.Id));

            return serviceTypes.OrderBy(x => x.OrderBy).Select(s => new ServiceTypeListItem
            {
                Display = s.Name,
                Value = s.Id.ToString(),
                CategoryId = s.CategoryId.ToString(),
                CategoryName = s.Category.Name
            }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetFranchiseeNameValuePair()
        {
            var franchiseeCollection = _unitOfWork.Repository<Franchisee>();
            return franchiseeCollection.Table.Select(f => new DropdownListItem
            { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
        }
        public IEnumerable<DropdownListItem> GetActiveFranchiseeList()
        {
            var franchiseeCollection = _unitOfWork.Repository<Franchisee>();
            return franchiseeCollection.Table.Where(x => x.Organization.IsActive).
                    Select(f => new DropdownListItem { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
        }
        public IEnumerable<DropdownListItem> GetActiveFranchiseeListWithOut0ML(long? userId, long roleId)
        {
            var franchiseeCollection = _unitOfWork.Repository<Franchisee>();
            var orgUserId = _unitOfWork.Repository<OrganizationRoleUser>();
            var frannchiseeIds = orgUserId.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).ToList();
            var franchiseeIds = franchiseeCollection.Table.Where(x => frannchiseeIds.Contains(x.Id)).Select(x => x.Id).ToList();
            if (roleId == (long?)RoleType.SuperAdmin)
            {
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive && x.Organization.Id != 63).
                        Select(f => new DropdownListItem { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
            }
            else
            {
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive && x.Organization.Id != 63 && franchiseeIds.Contains(x.Organization.Id)).
                        Select(f => new DropdownListItem { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();

            }
        }
        public IEnumerable<DropdownListItem> GetStateItems()
        {
            var stateRepository = _unitOfWork.Repository<State>();
            return stateRepository.Table.Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetLookupItems(long lookupTypeId)
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Fetch(x => x.LookupTypeId == lookupTypeId && x.IsActive).OrderBy(x => x.RelativeOrder)
                .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetRoles(long roleId)
        {
            var roleRepository = _unitOfWork.Repository<Role>();
            if (roleId == (long)RoleType.SuperAdmin)
            {
                return roleRepository.Table.Where(x => x.Id != (long)RoleType.FrontOfficeExecutive)
                    .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString(), Id = s.Id }).ToArray();
            }
            else if (roleId == (long)RoleType.FranchiseeAdmin)
            {
                return roleRepository.Table.Where(x => x.Id != (long)RoleType.SuperAdmin && x.Id != (long)RoleType.FrontOfficeExecutive).Select(s =>
                  new DropdownListItem { Display = s.Name, Value = s.Id.ToString(), Id = s.Id }).ToArray();
            }
            else if (roleId == (long)RoleType.FrontOfficeExecutive)
            {
                return roleRepository.Table.Where(x => x.Id == (long)RoleType.Technician || x.Id == (long)RoleType.SalesRep || x.Id == (long)RoleType.Equipment).Select(s =>
                  new DropdownListItem { Display = s.Name, Value = s.Id.ToString(), Id = s.Id }).ToArray();
            }
            else
            {
                return roleRepository.Table.Where(x => x.Id != (long)RoleType.SuperAdmin && x.Id != (long)RoleType.FranchiseeAdmin
                && x.Id != (long)RoleType.SalesRep && x.Id != (long)RoleType.FrontOfficeExecutive).Select(s =>
                  new DropdownListItem { Display = s.Name, Value = s.Id.ToString(), Id = s.Id }).ToArray();
            }
        }
        public IEnumerable<DropdownListItem> GetYears()
        {
            var list = new List<DropdownListItem>();
            var currentYear = DateTime.Now.Year;
            for (var i = currentYear; i <= currentYear + 15; i++)
            {
                list.Add(new DropdownListItem { Display = i.ToString(), Value = i.ToString() });
            }
            return list;
        }
        public IEnumerable<DropdownListItem> GetLastTwentyYears()
        {
            var list = new List<DropdownListItem>();
            var currentYear = DateTime.Now.Year;
            for (var i = currentYear - 20; i <= currentYear; i++)
            {
                list.Add(new DropdownListItem { Display = i.ToString(), Value = i.ToString() });
            }
            return list.OrderByDescending(x => x.Value);
        }
        public IEnumerable<DropdownListItem> GetYearsForGrowthReport()
        {
            var list = new List<DropdownListItem>();
            var currentYear = DateTime.Now.Year;
            var startYear = 1993;
            for (var i = startYear; i <= currentYear; i++)
            {
                list.Add(new DropdownListItem { Display = i.ToString(), Value = i.ToString() });
            }
            return list.OrderByDescending(x => x.Value);
        }
        public IEnumerable<DropdownListItem> GetMonths()
        {
            var list = new List<DropdownListItem>();
            for (var i = 1; i <= 12; i++)
            {
                list.Add(new DropdownListItem { Display = i.ToString("00"), Value = i.ToString() });
            }
            return list;
        }
        public IEnumerable<DropdownListItem> GetMonthNames()
        {
            var list = new List<DropdownListItem>();

            var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            for (int i = 0; i < months.Length; i++)
            {
                if (!string.IsNullOrEmpty(months[i]))
                {
                    list.Add(new DropdownListItem { Display = months[i], Value = (i + 1).ToString() });
                }
            }

            return list;
        }
        public IEnumerable<DropdownListItem> GetCardType()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Fetch(x => x.LookupTypeId == (long)LookupTypes.ChargeCardType && x.IsActive).OrderBy(x => x.RelativeOrder)
                .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetAccountType()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Fetch(x => x.LookupTypeId == (long)LookupTypes.AccountType && x.IsActive).OrderBy(x => x.RelativeOrder)
                .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetInstrumentType()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Fetch(x => x.LookupTypeId == (long)LookupTypes.InstrumentType && x.IsActive).OrderBy(x => x.RelativeOrder)
                .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetCountryItems()
        {
            var countryRepository = _unitOfWork.Repository<Country>();
            return countryRepository.Table.Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetUploadStatus()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Fetch(x => x.LookupTypeId == (long)LookupTypes.SalesDataUploadStatus && x.IsActive).OrderBy(x => x.RelativeOrder)
                .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetInvoiceStatus()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Fetch(x => x.LookupTypeId == (long)LookupTypes.InvoiceStatus && x.IsActive).OrderBy(x => x.RelativeOrder)
                .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();

        }
        public IEnumerable<DropdownListItem> GetMarketingClass()
        {
            var marketingClassCollection = _unitOfWork.Repository<MarketingClass>();
            return marketingClassCollection.Table.Select(m => new DropdownListItem
            { Display = m.Name, Value = m.Id.ToString() }).OrderBy(x => x.Value).ToArray();

        }
        public IEnumerable<DropdownListItem> GetLateFeeItemType()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Table.Where(x => x.LookupTypeId == (long)LookupTypes.LateFee).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString() }).OrderBy(y => y.Display).ToArray();
        }
        public IEnumerable<DropdownListItem> GetCallType()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Table.Where(x => x.LookupTypeId == (long)LookupTypes.CallType).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString() }).OrderBy(y => y.Value).ToArray();
        }
        public IEnumerable<MultiSelectListItem> GetServicesForDropdown()
        {
            var serviceTypes = _unitOfWork.Repository<ServiceType>();
            return serviceTypes.Table.Where(x => x.CategoryId != (long)Core.Organizations.Enum.ServiceTypeCategory.ProductChannel)
                .Select(s => new MultiSelectListItem
                {
                    Label = s.Name,
                    Id = s.Id,
                }).OrderBy(x => x.Label).ToArray();
        }
        public IEnumerable<MultiSelectListItem> GetClassForDropdown()
        {
            var serviceTypes = _unitOfWork.Repository<MarketingClass>();
            return serviceTypes.Table.Select(s => new MultiSelectListItem
            {
                Label = s.Name,
                Id = s.Id,
            }).OrderBy(x => x.Id).ToArray();
        }
        public IEnumerable<DropdownListItem> GetYearsForBatch()
        {
            var list = new List<DropdownListItem>();
            var currentYear = DateTime.Now.Year;
            var startYear = 2017;
            for (var i = startYear; i <= currentYear; i++)
            {
                list.Add(new DropdownListItem { Display = i.ToString(), Value = i.ToString() });
            }
            return list.OrderByDescending(x => x.Value);
        }
        public IEnumerable<MultiSelectListItem> GetAssigneeList(long franchiseeId, long roleId, long userId, bool? status, string Role, long? isEmpty)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var equipmentUserDetailsRepository = _unitOfWork.Repository<EquipmentUserDetails>();
            var userLoginRepository = _unitOfWork.Repository<UserLogin>();
            var roleRepository = _unitOfWork.Repository<Role>();
            long userRoleId = roleRepository.Table.Where(x => x.Name == Role).Select(x => x.Id).FirstOrDefault();
            var orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId && x.IsActive));
            //bool? isUserLocked = Convert.ToBoolean(status);
            if (roleId == (long)RoleType.SalesRep || roleId == (long)RoleType.OperationsManager)
                orgRoleUsers = orgRoleUsers.Where(x => x.RoleId == (long)RoleType.Technician || x.UserId == userId);
            else
                orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId && x.IsActive == true)
                               && (x.RoleId == (long)RoleType.Technician || x.RoleId == (long)RoleType.SalesRep || x.RoleId == (long)RoleType.Equipment));
            if (status != null)
            {
                if (status == false)
                {
                    var orgRoleUser = (from i in userLoginRepository.Table
                                       join o in orgRoleUsers on i.Id equals o.UserId
                                       where ((i.IsLocked == true) && status != null
                                       && (o.RoleId == userRoleId || (userRoleId == 0))
                                       && ((o.RoleId == (long)RoleType.SalesRep) || o.RoleId == (long)RoleType.Technician))
                                       select new MultiSelectListItem
                                       {
                                           Label = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName,
                                           Id = o.Id,
                                           ColorCode = o.ColorCode,
                                           Alias = o.Person.FirstName.Substring(0, 1).ToUpper() + "" + o.Person.LastName.Substring(0, 1).ToUpper(),
                                           Role = o.Role.Name,
                                           UserId = o.UserId,
                                           isLocked = i.IsLocked
                                       }).OrderBy(y => y.Label).ToArray();

                    var equipmentUses = (from i in equipmentUserDetailsRepository.Table
                                         join o in orgRoleUsers on i.UserId equals o.UserId
                                         where ((userRoleId == 0 && o.RoleId == (long)RoleType.Equipment)
                                         && o.OrganizationId == franchiseeId &&
                                         status != null && i.IsLock == true)
                                         select new MultiSelectListItem
                                         {
                                             Label = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName,
                                             Id = o.Id,
                                             ColorCode = o.ColorCode,
                                             Alias = o.Person.FirstName.Substring(0, 1).ToUpper() + "" + o.Person.LastName.Substring(0, 1).ToUpper(),
                                             Role = o.Role.Name,
                                             UserId = o.UserId,
                                         }).OrderBy(y => y.Label).ToArray();
                    var a = orgRoleUser.Concat(equipmentUses).OrderBy(x => x.Label);
                    return a.Distinct();
                }
                else
                {
                    var orgRoleUser = (from i in userLoginRepository.Table
                                       join o in orgRoleUsers on i.Id equals o.UserId
                                       where (!(i.IsLocked == status) && status != null
                                       && (o.RoleId == userRoleId || (userRoleId == 0))
                                       && ((o.RoleId == (long)RoleType.SalesRep) || o.RoleId == (long)RoleType.Technician))
                                       select new MultiSelectListItem
                                       {
                                           Label = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName,
                                           Id = o.Id,
                                           ColorCode = o.ColorCode,
                                           Alias = o.Person.FirstName.Substring(0, 1).ToUpper() + "" + o.Person.LastName.Substring(0, 1).ToUpper(),
                                           Role = o.Role.Name,
                                           UserId = o.UserId,
                                           isLocked = i.IsLocked
                                       }).OrderBy(y => y.Label).ToArray();

                    var equipmentUses = (from i in equipmentUserDetailsRepository.Table
                                         join o in orgRoleUsers on i.UserId equals o.UserId
                                         where ((userRoleId == 0 && o.RoleId == (long)RoleType.Equipment)
                                         && o.OrganizationId == franchiseeId &&
                                         status != null && !(i.IsLock == status))
                                         select new MultiSelectListItem
                                         {
                                             Label = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName,
                                             Id = o.Id,
                                             ColorCode = o.ColorCode,
                                             Alias = o.Person.FirstName.Substring(0, 1).ToUpper() + "" + o.Person.LastName.Substring(0, 1).ToUpper(),
                                             Role = o.Role.Name,
                                             UserId = o.UserId,
                                         }).OrderBy(y => y.Label).ToArray();
                    var a = orgRoleUser.Concat(equipmentUses).OrderBy(x => x.Label); ;
                    return a.Distinct();
                }
            }
            else
            {
                var orgRoleUser = (from i in userLoginRepository.Table
                                   join o in orgRoleUsers on i.Id equals o.UserId
                                   where ((o.RoleId == userRoleId || (userRoleId == 0)))
                                   select new MultiSelectListItem
                                   {
                                       Label = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName,
                                       Id = o.Id,
                                       ColorCode = o.ColorCode,
                                       Alias = o.Person.FirstName.Substring(0, 1).ToUpper() + "" + o.Person.LastName.Substring(0, 1).ToUpper(),
                                       Role = o.Role.Name,
                                       UserId = o.UserId,
                                       isLocked = i.IsLocked
                                   }).OrderBy(y => y.Label).ToArray();

                var equipmentUses = (from o in orgRoleUsers
                                     where ((userRoleId == 0 && o.RoleId == (long)RoleType.Equipment)
                                     && o.OrganizationId == franchiseeId)
                                     select new MultiSelectListItem
                                     {
                                         Label = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName,
                                         Id = o.Id,
                                         ColorCode = o.ColorCode,
                                         Alias = o.Person.FirstName.Substring(0, 1).ToUpper() + "" + o.Person.LastName.Substring(0, 1).ToUpper(),
                                         Role = o.Role.Name,
                                         UserId = o.UserId,
                                     }).OrderBy(y => y.Label).ToArray();
                var a = orgRoleUser.Concat(equipmentUses).OrderBy(x => x.Label); ;
                return a.Distinct();
            }
        }
        public IEnumerable<DropdownListItem> GetJobStatus()
        {
            var jobStatusRepository = _unitOfWork.Repository<JobStatus>();
            return jobStatusRepository.Table.Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString(), Alias = l.ColorCode }).OrderBy(y => y.Value).ToArray();
        }
        public IEnumerable<MultiSelectListItem> GetTechList(long franchiseeId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            return orgRoleUserRepository.Table.Where(x => (x.RoleId == (long)RoleType.Technician)
                                && (franchiseeId <= 0 || x.OrganizationId == franchiseeId) && x.IsActive && x.Person.UserLogin != null
                                && !x.Person.UserLogin.IsLocked && x.RoleId != 7)
                .Select(l => new MultiSelectListItem
                {
                    Label = l.Person.FirstName + " " + l.Person.MiddleName + " " + l.Person.LastName,
                    Id = l.Id,
                    ColorCode = l.ColorCode,
                    Alias = l.Person.FirstName.Substring(0, 1).ToUpper() + "" + l.Person.LastName.Substring(0, 1).ToUpper(),
                    Role = l.Role.Name,
                    UserId = l.UserId
                }).OrderBy(y => y.Label).ToArray();
        }
        public IEnumerable<MultiSelectListItem> GetTechAndSalesList(long franchiseeId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId)
                                && x.RoleId == (long)RoleType.SalesRep && x.IsActive && x.Person.UserLogin != null
                                && !x.Person.UserLogin.IsLocked && x.RoleId != 7);

            return orgRoleUsers.Select(l => new MultiSelectListItem
            {
                Label = l.Person.FirstName + " " + l.Person.MiddleName + " " + l.Person.LastName,
                Id = l.Id,
                ColorCode = l.ColorCode,
                Alias = l.Person.FirstName.Substring(0, 1).ToUpper() + "" + l.Person.LastName.Substring(0, 1).ToUpper(),
                Role = l.Role.Name,
                UserId = l.UserId

            }).OrderBy(y => y.Label).ToArray();
        }
        public IEnumerable<DropdownListItem> GetTimeZoneList()
        {
            var timeZoneInfoRepository = _unitOfWork.Repository<TimeZoneInformation>();
            return timeZoneInfoRepository.Table.Where(x => x.IsActive)
                .Select(l => new DropdownListItem
                {
                    Display = l.TimeDifference > 0 ? l.TimeZone + " (UTC" + l.TimeDifference + ")" : l.TimeZone + " (UTC+" + -(l.TimeDifference) + ")",
                    Id = l.Id,
                    Value = l.Id.ToString(),
                }).OrderBy(y => y.Display).ToArray();
        }
        public IEnumerable<DropdownListItem> GetReviewStatus()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Fetch(x => x.LookupTypeId == (long)LookupTypes.AuditActionType).OrderBy(x => x.RelativeOrder)
                .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<DropdownListItem> GetFranchiseeForMissingAudit()
        {
            var year = DateTime.Now.Year - 1;
            var year1 = DateTime.Now.Year - 2;
            var annualFileUploadRepository = _unitOfWork.Repository<AnnualSalesDataUpload>();
            var franchiseeRepository = _unitOfWork.Repository<Franchisee>();

            var annualUploadFranchiseeIds = annualFileUploadRepository.Table.Where(au => au.PeriodStartDate.Year == year
                                            && au.Franchisee != null
                                            && au.Franchisee.Organization.IsActive
                                            && au.DataRecorderMetaData.DateCreated.Year != DateTime.Now.Year
                                            && au.StatusId != (long)SalesDataUploadStatus.Failed
                                            && au.AuditActionId != (long)AuditActionType.Rejected).Select(x => x.FranchiseeId).ToList();

            var list = franchiseeRepository.Table.Where(x => x.Organization.IsActive && !annualUploadFranchiseeIds.Contains(x.Id) && x.SalesDataUploads.Any())
                .Select(f => new DropdownListItem { Display = f.Organization.Name, Value = f.Id.ToString() });
            return list;
        }
        public IEnumerable<DropdownListItem> GetUserList(long franchiseeId, long roleId, long userId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId)
                                && (x.RoleId == (long)RoleType.SalesRep || x.RoleId == (long)RoleType.Technician) && x.IsActive
                                && x.Person.UserLogin != null && !x.Person.UserLogin.IsLocked).
                                GroupBy(y => y.Person);

            if (roleId == (long)RoleType.SalesRep)
                orgRoleUsers = orgRoleUsers.Where(x => x.Key.Id == userId);

            return orgRoleUsers.Select(l => new DropdownListItem
            {
                Display = l.Key.FirstName + " " + l.Key.MiddleName + " " + l.Key.LastName,
                Value = l.Key.Id.ToString(),
            }).OrderBy(y => y.Display).ToArray();
        }
        public IEnumerable<DropdownListItem> GetTagList()
        {
            var tagRepository = _unitOfWork.Repository<Tag>();
            return tagRepository.Fetch(x => x.IsActive).OrderBy(x => x.Name)
                .Select(s => new DropdownListItem { Display = s.Name, Value = s.Id.ToString() }).ToArray();
        }
        public IEnumerable<MultiSelectListItem> GetProductChannel()
        {
            var serviceTypes = _unitOfWork.Repository<ServiceType>();
            return serviceTypes.Fetch(x => x.CategoryId == (long)Core.Organizations.Enum.ServiceTypeCategory.ProductChannel)
                .Select(s => new MultiSelectListItem
                {
                    Label = s.Name,
                    Id = s.Id,
                }).OrderBy(x => x.Id).ToArray();
        }
        public IEnumerable<DropdownListItem> GetCreditType()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Table.Where(x => x.LookupTypeId == (long)LookupTypes.AccountCreditType).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString() }).OrderBy(y => y.Value).ToArray();
        }
        public IEnumerable<DropdownListItem> GetRoutingNumberList()
        {
            var routingNumberRepository = _unitOfWork.Repository<RoutingNumber>();
            return routingNumberRepository.Table.Select(x => new DropdownListItem { Display = x.PhoneLabel }).Distinct().OrderBy(x => x.Display).ToArray();
        }
        public IEnumerable<DropdownListItem> GetUrlList()
        {
            var webLeadRepository = _unitOfWork.Repository<WebLead>();
            return webLeadRepository.Table.Select(x => new DropdownListItem { Display = x.URL }).Distinct().OrderBy(x => x.Display).ToArray();
        }
        public IEnumerable<DropdownListItem> GetRepeatFrequency()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Table.Where(x => x.LookupTypeId == (long)LookupTypes.RepeatFrequency).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString() }).OrderBy(y => y.Value).ToArray();
        }
        public IEnumerable<DropdownListItem> GetDocumentType()
        {
            var documentTypeRepository = _unitOfWork.Repository<DocumentType>();
            return documentTypeRepository.Table.Where(x => x.CategoryId >= 0).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString() }).OrderBy(y => y.Value).ToArray();
        }

        public IEnumerable<DropdownListItem> GetNationalDocumentType()
        {

            var documentTypeRepository = _unitOfWork.Repository<DocumentType>();
            return documentTypeRepository.Table.Where(x => x.CategoryId == (long)Core.Organizations.Enum.DocumentCategory.NationalAccountDocuments).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString() }).OrderBy(y => y.Value).ToArray();
        }
        public IEnumerable<MultiSelectListItem> GetPhoneLabelCategory()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Table.Where(x => x.LookupTypeId == (long)LookupTypes.RoutingNumberCategory).Select(l => new MultiSelectListItem
            { Label = l.Name, Id = l.Id }).OrderBy(y => y.Id).ToArray();
        }

        public IEnumerable<MultiSelectListItem> GetTechListForMeeting(long franchiseeId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var equipmentUserDetailsRepository = _unitOfWork.Repository<EquipmentUserDetails>();
            var orgRoleUserRepositorys = (from i in equipmentUserDetailsRepository.Table
                                          join o in orgRoleUserRepository.Table on i.UserId equals o.UserId
                                          where i.IsLock == false
                                          select o).ToList();
            var orgUsers = orgRoleUserRepositorys.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId) && x.IsActive
                               && x.RoleId == (long)(RoleType.Equipment))
                               .Select(x => new { x.UserId, x.Person.FirstName, x.Person.MiddleName, x.Person.LastName })
                               .Distinct()
                               .OrderBy(x => x.UserId)
                               .ToArray();


            var orgUserList = orgUsers.Select(l => new MultiSelectListItem
            {
                Label = l.FirstName + " " + l.MiddleName + " " + l.LastName,
                Id = l.UserId,
            });

            return orgUserList;
        }

        public IEnumerable<DropdownListItem> GetSalesRepList(long franchiseeId, long roleId, long userId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId)
                                && x.RoleId == (long)RoleType.SalesRep && x.IsActive && x.Person.UserLogin != null
                                && !x.Person.UserLogin.IsLocked);

            if (roleId == (long)RoleType.SalesRep)
                orgRoleUsers = orgRoleUsers.Where(x => x.UserId == userId);

            return orgRoleUsers.Select(l => new DropdownListItem
            {
                Display = l.Person.FirstName + " " + l.Person.MiddleName + " " + l.Person.LastName,
                Value = l.Id.ToString(),
            }).OrderBy(y => y.Display).ToArray();
        }

        // Copy of GetSalesRepList to get both Sales and Tech
        public IEnumerable<DropdownListItem> GetRepresentativeList(long franchiseeId, long roleId, long userId, bool? status, string Role)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var userLoginRepository = _unitOfWork.Repository<UserLogin>();
            var roleRepository = _unitOfWork.Repository<Role>();
            long userRoleId = roleRepository.Table.Where(x => x.Name == Role).Select(x => x.Id).FirstOrDefault();
            var orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId));
            //bool? isUserLocked = Convert.ToBoolean(status);
            if (roleId == (long)RoleType.SalesRep || roleId == (long)RoleType.OperationsManager)
                orgRoleUsers = orgRoleUsers.Where(x => x.RoleId == (long)RoleType.Technician || x.UserId == userId);
            else
                orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId && x.IsActive == true)
                               && (x.RoleId == (long)RoleType.Technician || x.RoleId == (long)RoleType.SalesRep));
            if (status != null)
            {
                var orgRoleUser = (from i in userLoginRepository.Table
                                   join o in orgRoleUsers on i.Id equals o.UserId
                                   where (!(i.IsLocked == status) && status != null && (o.RoleId == userRoleId || (userRoleId == 0)))
                                   select new DropdownListItem
                                   {
                                       Display = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName + " " + "(" + o.Role.Name + ")",
                                       Value = o.Id.ToString()
                                   }).OrderBy(y => y.Display).ToArray();
                return orgRoleUser;
            }
            else
            {
                var orgRoleUser = (from i in userLoginRepository.Table
                                   join o in orgRoleUsers on i.Id equals o.UserId
                                   where ((o.RoleId == userRoleId || (userRoleId == 0)))
                                   select new DropdownListItem
                                   {
                                       Display = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName + " " + "(" + o.Role.Name + ")",
                                       Value = o.Id.ToString()
                                   }).OrderBy(y => y.Display).ToArray();
                return orgRoleUser;
            }
        }

        public IEnumerable<MultiSelectListItem> GetTechAndSalesListForEstimate(long franchiseeId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId)
                                && (x.RoleId == (long)RoleType.SalesRep || x.RoleId == (long)RoleType.Technician) && x.IsActive && x.Person.UserLogin != null
                                && !x.Person.UserLogin.IsLocked);


            return orgRoleUsers.Select(l => new MultiSelectListItem
            {
                Label = l.Person.FirstName + " " + l.Person.MiddleName + " " + l.Person.LastName + " " + "(" + l.Role.Name + ")",
                Id = l.Id,

            }).OrderBy(y => y.Label).ToArray();
        }
        public IEnumerable<DropdownListItem> GetDocumentTypeForFranchisee(long franchiseeId)
        {
            var documentIds = new List<long>();
            var documentTypeRepository = _unitOfWork.Repository<DocumentType>();
            var franchiseeDocumentTypeRepository = _unitOfWork.Repository<FranchiseeDocumentType>();
            var franchiseeDocumentTypsList = new List<FranchiseeDocumentType>();
            var isPerpetuityOn = false;
            if (franchiseeId != 0)
            {
                franchiseeDocumentTypsList = franchiseeDocumentTypeRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.IsActive
                                    ).OrderBy(x => x.DocumentType.Order).ToList();
                documentIds = franchiseeDocumentTypsList.Select(x => x.DocumentTypeId).ToList();
                isPerpetuityOn = franchiseeDocumentTypsList.Any(x => x.DocumentTypeId == (11) && x.IsPerpetuity);
            }
            else if (franchiseeId == 0)
            {
                franchiseeDocumentTypsList = franchiseeDocumentTypeRepository.Table.Where(x => x.IsActive
                                     ).OrderBy(x => x.DocumentType.Order).ToList();
                documentIds = franchiseeDocumentTypsList.Select(x => x.DocumentTypeId).ToList();
                isPerpetuityOn = franchiseeDocumentTypsList.Any(x => x.DocumentTypeId == (11) && x.IsPerpetuity);
            }
            var documentTypeList = documentTypeRepository.Table.Where(x => documentIds.Contains(x.Id)).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString(), Order = l.Order, IsPerpetuity = false }).OrderBy(x => x.Order).ToArray();

            if (documentTypeList.Any(x => x.Value == "11"))
            {
                documentTypeList.FirstOrDefault(x => x.Value == "11").IsPerpetuity = isPerpetuityOn;
            }
            return documentTypeList;
        }
        public IEnumerable<DropdownListItem> GetNationalTypeForFranchisee(long franchiseeId)
        {
            var documentIds = new List<long>();
            var documentTypeRepository = _unitOfWork.Repository<DocumentType>();
            var franchiseeDocumentTypeRepository = _unitOfWork.Repository<FranchiseeDocumentType>();

            if (franchiseeId != 0)
            {
                documentIds = franchiseeDocumentTypeRepository.Table.Where(x => (x.FranchiseeId == franchiseeId) && x.IsActive
                                           && x.DocumentType.CategoryId == (long)Core.Organizations.Enum.DocumentCategory.NationalAccountDocuments).Select(x => x.DocumentTypeId).ToList();

            }
            else if (franchiseeId == 0)
            {
                documentIds = franchiseeDocumentTypeRepository.Table.Where(x => x.IsActive
                                          && x.DocumentType.CategoryId == (long)Core.Organizations.Enum.DocumentCategory.NationalAccountDocuments).Select(x => x.DocumentTypeId).ToList();
            }
            return documentTypeRepository.Table.Where(x => documentIds.Contains(x.Id)).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString() }).OrderBy(y => y.Value).ToArray();
        }

        public IEnumerable<DropdownListItem> GetUserListForDocument(long franchiseeId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            //return orgRoleUserRepository.Table.Where(x => ((x.RoleId == (long)RoleType.Technician)
            //                                || (x.RoleId == (long)RoleType.SalesRep))
            //                   && (franchiseeId <= 0 || x.OrganizationId == franchiseeId) && x.IsActive && x.Person.UserLogin != null
            //                   && !x.Person.UserLogin.IsLocked)
            return orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId) && x.IsActive && x.Person.UserLogin != null
                               && !x.Person.UserLogin.IsLocked && x.RoleId != (long)(RoleType.Equipment))
               .Select(l => new DropdownListItem
               {
                   Display = l.Person.FirstName + " " + l.Person.MiddleName + " " + l.Person.LastName,
                   Id = l.UserId,
               }).Distinct().ToArray();
        }

        public IEnumerable<MultiSelectListItem> GetTechListForMeetingForUser(long franchiseeId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var orgUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId) && x.IsActive && x.Person.UserLogin != null
                               && !x.Person.UserLogin.IsLocked)
                               .Select(x => new { x.UserId, x.Person.FirstName, x.Person.MiddleName, x.Person.LastName })
                               .Distinct()
                               .OrderBy(x => x.UserId)
                               .ToArray();


            var orgUserList = orgUsers.Select(l => new MultiSelectListItem
            {
                Label = l.FirstName + " " + l.MiddleName + " " + l.LastName,
                Id = l.UserId,
            });

            return orgUserList;
        }


        public IEnumerable<MultiSelectListItem> GetAssigneeListForScheduler(long franchiseeId, long roleId, long userId, bool? status, string Role, long? isEmpty)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var equipmentUserDetailsRepository = _unitOfWork.Repository<EquipmentUserDetails>();
            var userLoginRepository = _unitOfWork.Repository<UserLogin>();
            var roleRepository = _unitOfWork.Repository<Role>();
            long userRoleId = roleRepository.Table.Where(x => x.Name == Role).Select(x => x.Id).FirstOrDefault();
            var orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId && x.IsActive));
            //bool? isUserLocked = Convert.ToBoolean(status);

            if (roleId == (long)RoleType.SalesRep || roleId == (long)RoleType.OperationsManager)
                orgRoleUsers = orgRoleUsers.Where(x => x.RoleId == (long)RoleType.Technician || x.UserId == userId);
            else
                orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId && x.IsActive == true)
                               && (x.RoleId == (long)RoleType.Technician || x.RoleId == (long)RoleType.SalesRep || x.RoleId == (long)RoleType.Equipment));

            var orgRoleUser = (from i in userLoginRepository.Table
                               join o in orgRoleUsers on i.Id equals o.UserId
                               where ((o.RoleId == userRoleId || (userRoleId == 0)))
                               select new MultiSelectListItem
                               {
                                   Label = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName,
                                   Id = o.Id,
                                   ColorCode = o.Role.Id == (long)(RoleType.Technician) ? o.ColorCode : o.ColorCodeSale,
                                   Alias = o.Person.FirstName.Substring(0, 1).ToUpper() + "" + o.Person.LastName.Substring(0, 1).ToUpper(),
                                   Role = o.Role.Name,
                                   UserId = o.UserId,
                                   isLocked = i.IsLocked
                               }).OrderBy(y => y.Label).ToArray();


            var userIds = orgRoleUser.Select(x => x.UserId).ToList();
            var equipmentUses = (from i in equipmentUserDetailsRepository.Table
                                 join o in orgRoleUsers on i.UserId equals o.UserId
                                 where ((userRoleId == 0 && o.RoleId == (long)RoleType.Equipment)
                                 && o.OrganizationId == franchiseeId
                                 && !userIds.Contains(o.UserId))
                                 select new MultiSelectListItem
                                 {
                                     Label = o.Person.FirstName + " " + o.Person.MiddleName + " " + o.Person.LastName,
                                     Id = o.Id,
                                     ColorCode = o.ColorCode,
                                     Alias = o.Person.FirstName.Substring(0, 1).ToUpper() + "" + o.Person.LastName.Substring(0, 1).ToUpper(),
                                     Role = o.Role.Name,
                                     UserId = o.UserId,
                                     isLocked = i.IsLock
                                 }).OrderBy(y => y.Label).ToArray();
            var a = orgRoleUser.Concat(equipmentUses);
            //a = a.Concat(a1);
            if (status == false)
            {
                a = a.Where(x => x.isLocked == false).OrderBy(x => x.Label);
            }
            return a.Distinct().OrderBy(x => x.Label);
        }


        public IEnumerable<DropdownListItem> GetFranchiseeNameValuePairByRole(long? userId, long? roleId)
        {
            var userList = _unitOfWork.Repository<OrganizationRoleUser>().Table.Where(x => x.UserId == userId).ToList();
            var franchiseeIds = userList.Select(x => x.OrganizationId).ToList();
            var franchiseeCollection = _unitOfWork.Repository<Franchisee>();
            if (roleId == (long)(RoleType.SuperAdmin) || roleId == (long)(RoleType.FrontOfficeExecutive))
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive).Select(f => new DropdownListItem
                { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
            else if (roleId == (long)(RoleType.FranchiseeAdmin))
            {
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive && franchiseeIds.Contains(x.Id)).Select(f => new DropdownListItem
                { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
            }
            else
            {
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive).Select(f => new DropdownListItem
                { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
            }
        }

        public IEnumerable<DropdownListItem> GetUserListForFA(long franchiseeId, long roleId, long userId)
        {
            var orgRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            var orgRoleUsers = orgRoleUserRepository.Table.Where(x => (franchiseeId <= 0 || x.OrganizationId == franchiseeId)
                                 && x.IsActive
                                && x.Person.UserLogin != null && !x.Person.UserLogin.IsLocked).
                                GroupBy(y => y.Person);

            if (roleId == (long)RoleType.SalesRep)
                orgRoleUsers = orgRoleUsers.Where(x => x.Key.Id == userId);

            return orgRoleUsers.Select(l => new DropdownListItem
            {
                Display = l.Key.FirstName + " " + l.Key.MiddleName + " " + l.Key.LastName,
                Value = l.Key.Id.ToString(),
            }).OrderBy(y => y.Display).ToArray();
        }


        public IEnumerable<DropdownListItem> GetFranchiseeListWithOut0ML()
        {
            var franchiseeCollection = _unitOfWork.Repository<Franchisee>();
            var orgUserId = _unitOfWork.Repository<OrganizationRoleUser>();
            return franchiseeCollection.Table.Where(x => x.Organization.IsActive && !x.Organization.Name.StartsWith("0")).
                    Select(f => new DropdownListItem { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
        }
        public IEnumerable<DropdownListItem> GetFranchiseeNameValuePairByRoleForFA(long? userId, long? roleId)
        {
            var userList = _unitOfWork.Repository<OrganizationRoleUser>().Table.Where(x => x.UserId == userId).ToList();
            var franchiseeIds = userList.Select(x => x.OrganizationId).ToList();
            var franchiseeCollection = _unitOfWork.Repository<Franchisee>();
            if (roleId == (long)(RoleType.SuperAdmin) || roleId == (long)(RoleType.FrontOfficeExecutive))
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive).Select(f => new DropdownListItem
                { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
            else if (roleId == (long)(RoleType.FranchiseeAdmin))
            {
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive && franchiseeIds.Contains(x.Id)).Select(f => new DropdownListItem
                { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
            }
            else if (roleId == (long)(RoleType.SalesRep) || roleId == (long)(RoleType.Technician) || roleId == (long)(RoleType.OperationsManager))
            {
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive && franchiseeIds.Contains(x.Id)).Select(f => new DropdownListItem
                { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
            }
            else
            {
                return franchiseeCollection.Table.Where(x => x.Organization.IsActive).Select(f => new DropdownListItem
                { Display = f.Organization.Name, Value = f.Id.ToString(), Id = f.Id }).OrderBy(x => x.Display).ToArray();
            }
        }


        public List<ServiceTypeGroupedListItem> GetServiceTypesNewOrder()
        {
            var serviceTypes = _unitOfWork.Repository<ServiceType>();
            var serviceTypesList = serviceTypes.Table.Where(x => x.SubCategoryId != null && x.IsActive).OrderBy(x => x.NewOrderBy).Select(s => new ServiceTypeListItem
            {
                Display = s.Name,
                Value = s.Id.ToString(),
                CategoryId = s.CategoryId.ToString(),
                CategoryName = s.Category.Name,
                Description = s.Description,
                SubCategoryId = s.SubCategoryId,
                SubCategoryName = s.SubCategory != null ? s.SubCategory.Name : "",
                NewOrderBy = s.NewOrderBy.Value,
                Synonyms = s.Alias
            }).ToList();

            var serviceTypesGroupedList = serviceTypesList.GroupBy(x => x.SubCategoryName).ToList();
            var serviceTypesGroupedViewModel = new ServiceTypeGroupedListItem();

            var serviceTypesGroupedViewModelList = new List<ServiceTypeGroupedListItem>();
            foreach (var serviceTypesGrouped in serviceTypesGroupedList)
            {
                serviceTypesGroupedViewModel = new ServiceTypeGroupedListItem();
                serviceTypesGroupedViewModel.GroupName = serviceTypesGrouped.Key;
                serviceTypesGroupedViewModel.Collection = serviceTypesGrouped.OrderBy(x => x.NewOrderBy).ToList();
                serviceTypesGroupedViewModelList.Add(serviceTypesGroupedViewModel);
            }
            return serviceTypesGroupedViewModelList;
        }

        public List<ServiceTypeGroupedListItem> GetMarketingClassNewOrder()
        {
            var marketingClassCollection = _unitOfWork.Repository<MarketingClass>();
            var serviceTypesList = marketingClassCollection.Table.Where(x => x.CategoryId != null).OrderBy(x => x.NewOrderBy).Select(s => new ServiceTypeListItem
            {
                Display = s.Name,
                Value = s.Id.ToString(),
                CategoryId = s.CategoryId.ToString(),
                CategoryName = s.Category.Name,
                Description = s.Description,
                SubCategoryId = s.CategoryId,
                SubCategoryName = s.Category != null ? s.Category.Name : "",
                NewOrderBy = s.NewOrderBy.Value,
               

            }).ToList();

            var serviceTypesGroupedList = serviceTypesList.GroupBy(x => x.SubCategoryName).ToList();
            var serviceTypesGroupedViewModel = new ServiceTypeGroupedListItem();

            var serviceTypesGroupedViewModelList = new List<ServiceTypeGroupedListItem>();
            
            foreach (var serviceTypesGrouped in serviceTypesGroupedList)
            {
                serviceTypesGroupedViewModel = new ServiceTypeGroupedListItem();
                serviceTypesGroupedViewModel.GroupName = serviceTypesGrouped.Key;
                serviceTypesGroupedViewModel.Order = serviceTypesGrouped.OrderBy(x => x.NewOrderBy).Count();
                serviceTypesGroupedViewModel.Collection = serviceTypesGrouped.OrderBy(x => x.NewOrderBy).ToList();
                serviceTypesGroupedViewModelList.Add(serviceTypesGroupedViewModel);
            }
            serviceTypesGroupedViewModelList = serviceTypesGroupedViewModelList.OrderByDescending(x => x.Order).ToList();
            return serviceTypesGroupedViewModelList;
        }
        public List<DropdownListItem> GetServiceTagCategories()
        {
            var lookUpCollection = _unitOfWork.Repository<Lookup>();
            var serviceTypesList = lookUpCollection.Table.Where(x => x.LookupTypeId == (long)LookUpTypeCategory.ListOfServiceTag && x.Id != 286).OrderBy(x => x.RelativeOrder).Select(s => new DropdownListItem
            {
                Display = s.Name,
                Value = s.Id.ToString(),
                Alias = s.Alias
            }).ToList();
            return serviceTypesList;
        }
        
        public List<DropdownListItem> GetListOfServices()
        {
            var lookUpCollection = _unitOfWork.Repository<ServicesTag>();
            var uniqueList = lookUpCollection.Table.Where(x=>x.IsActive).OrderBy(x => x.Service).Select(x => x.Service).Distinct();
            var serviceTypesList = uniqueList.Select(s => new DropdownListItem
            {
                Display = s,
                Value = s
            }).ToList();
            var servicesWithBundles = serviceTypesList.Where(x => x.Display.StartsWith("Bundle")).OrderBy(x=>x.Display);
            var serviceTypeIdsBundles = servicesWithBundles.Select(x => x.Display).ToList();
            var serviceTypeWithoutBundles = serviceTypesList.Where(x => !serviceTypeIdsBundles.Contains(x.Display)).OrderBy(x => x.Display).ToList();

            serviceTypesList = new List<DropdownListItem>();
            serviceTypesList.AddRange(servicesWithBundles);
            serviceTypesList.AddRange(serviceTypeWithoutBundles);

            return serviceTypesList;
        }
        public IEnumerable<ServiceTypeListItem> GetAllServicesList()
        {
            // List of IDs to filter
            List<long> ides = new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 11, 13, 15, 16, 17, 18, 19, 33, 34, 35, 37, 38, 40, 42, 43 };

            // Fetch service types filtered by ID
            var serviceTypes = _unitOfWork.Repository<ServiceType>().Table.Where(x => ides.Contains(x.Id)).ToList(); // Convert to List for compatibility

            // Find the specific item
            var otherMaterialType = serviceTypes.FirstOrDefault(x => x.Id == 37);
            if (otherMaterialType != null)
            {
                // Remove the object from the list
                serviceTypes.Remove(otherMaterialType);

                // Get the sorted list
                var sortedList = serviceTypes.OrderBy(x => x.Name != null ? x.Name : string.Empty).ToList();

                // Add the specific item to the end
                sortedList.Add(otherMaterialType);

                // Return the result
                return sortedList.Select(s => new ServiceTypeListItem
                {
                    Display = s.Name,
                    Value = s.Id.ToString(),
                    CategoryId = s.CategoryId.ToString(),
                    CategoryName = s.Category != null ? s.Category.Name : string.Empty // Handle null categories
                }).ToArray();
            }

            // If no specific item, return the ordered list
            return serviceTypes.OrderBy(x => x.OrderBy).Select(s => new ServiceTypeListItem
            {
                     Display = s.Name,
                     Value = s.Id.ToString(),
                     CategoryId = s.CategoryId.ToString(),
                     CategoryName = s.Category != null ? s.Category.Name : string.Empty
            })
            .ToArray();
        }

    }
}