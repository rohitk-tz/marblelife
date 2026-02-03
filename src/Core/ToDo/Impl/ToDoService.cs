using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using Core.ToDo.Domain;
using Core.ToDo.Enum;
using Core.ToDo.ViewModel;
using Core.Users.Domain;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.ToDo.Impl
{
    [DefaultImplementation]
    public class ToDoService : IToDoService
    {
        private readonly IToDoFactory _todofactory;
        private readonly IRepository<ToDoFollowUpList> _toDoListRepository;
        private readonly IRepository<Person> _personsRepository;
        private readonly IRepository<ToDoFollowUpComment> _toDoCommentRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<JobCustomer> _customerRepository;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<DataRecorderMetaData> _dataRecorderMetaDataRepository;
        private readonly IClock _clock;
        private readonly ISettings _setting;
        public ToDoService(IToDoFactory todofactory, IUnitOfWork unitOfWork, IClock clock, ISettings setting)
        {
            _todofactory = todofactory;
            _toDoListRepository = unitOfWork.Repository<ToDoFollowUpList>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _clock = clock;
            _setting = setting;
            _customerRepository = unitOfWork.Repository<JobCustomer>();
            _toDoCommentRepository = unitOfWork.Repository<ToDoFollowUpComment>();
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _dataRecorderMetaDataRepository = unitOfWork.Repository<DataRecorderMetaData>();
            _personsRepository = unitOfWork.Repository<Person>();
        }
        public ToDoListModel Get(ToDoFilter filter)
        {
            var orgRoleUserList = _organizationRoleUserRepository.Table.ToList();
            var orgRoleUserDomain = _organizationRoleUserRepository.Get(filter.OrgUserId.GetValueOrDefault());
            var franchiseeIds = orgRoleUserList.Where(x => x.UserId == orgRoleUserDomain.UserId).Select(x => x.OrganizationId).ToList();
            var userIdForParticularFranchisee = _organizationRoleUserRepository.Table.Where(x => x.OrganizationId == orgRoleUserDomain.OrganizationId)
                                                                .Select(x => x.UserId).ToList();
            var userIds = _organizationRoleUserRepository.Table.Where(x => franchiseeIds.Contains(x.OrganizationId)).Select(x => x.UserId).ToList();
            var toDoListWhole = _toDoCommentRepository.Table.ToList();

            var todayDate = DateTime.UtcNow.Date;
            var toDoListTotal = _toDoListRepository.Table.ToList();
            var toDoListComment = _toDoCommentRepository.Table.ToList();
            var toDoList = toDoListTotal.Where(x =>
             (filter.StartDate == null ||
             (filter.StartDate <= x.Date && filter.EndDate >= x.Date))
             && (filter.StatusId == null || filter.StatusId == x.StatusId)).OrderBy(x1 => x1.Date).ToList();

            var expiredToDoList = toDoList.Where(x => x.Date < todayDate && x.Date > todayDate.AddYears(-1)).OrderByDescending(z => z.Id).ToList();

            toDoList = toDoList.Where(x => filter.StartDate == null ? x.Date >= todayDate : true).ToList();

            if (filter.RoleId == (long?)(RoleType.FranchiseeAdmin))
            {
                toDoList = toDoList.Where(x => userIds.Contains(x.UserId.GetValueOrDefault())).ToList();
                expiredToDoList = expiredToDoList.Where(x => userIds.Contains(x.UserId.GetValueOrDefault())).ToList();
            }
            else if (filter.RoleId == (long?)(RoleType.SalesRep) || filter.RoleId == (long?)(RoleType.Technician))
            {
                toDoList = toDoList.Where(x => filter.UserId == (x.UserId.GetValueOrDefault())).ToList();
                expiredToDoList = expiredToDoList.Where(x => filter.UserId == (x.UserId.GetValueOrDefault())).ToList();
            }

            if (filter.IsFranchiseeLevel.GetValueOrDefault())
            {
                toDoList = toDoList.Where(x => x.FranchiseeId == filter.FranchiseeId).ToList();
                expiredToDoList = expiredToDoList.Where(x => x.FranchiseeId == filter.FranchiseeId).ToList();
            }
            var toDoListToday = toDoList.Where(x => x.Date.Date == todayDate.Date && x.UserId == filter.UserId).OrderBy(x => x.Lookup.Id).ToList();
            if (filter.IsFranchiseeLevel.GetValueOrDefault())
            {
                toDoListToday = toDoList.Where(x => x.FranchiseeId == filter.FranchiseeId).ToList();
            }
            if (filter.RoleId == (long?)(RoleType.SuperAdmin) || filter.RoleId == (long?)(RoleType.FrontOfficeExecutive))
            {
                toDoListToday = toDoList.Where(x => x.Date.Date == todayDate.Date).OrderBy(x => x.Lookup.Id).ToList();
            }

            if (filter.RoleId == (long?)(RoleType.FranchiseeAdmin))
            {
                toDoListToday = toDoList.Where(x => x.Date.Date == todayDate.Date && userIds.Contains(x.UserId.Value)).ToList();
                if (toDoListToday.Count() > 0)
                {
                    toDoListToday = toDoListToday.OrderBy(x => x.Id).ToList();
                }
            }
            var todayListId = toDoListToday.Select(x => x.Id);
            toDoList = toDoList.Where(x => !todayListId.Contains(x.Id)).ToList();
            var totolCount = toDoList.Count();
            toDoList = toDoList.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
            toDoList = toDoList.OrderBy(x => x.Lookup.Id).ToList();
            return new ToDoListModel()
            {
                AllCollection = toDoList.Select(x => _todofactory.CreateViewModel(x, toDoListComment)).ToList(),
                TotalCount = totolCount,
                TodaysCollection = toDoListToday.Select(x => _todofactory.CreateViewModel(x, toDoListComment)).ToList(),
                TodayToDoCount = toDoListToday.Where(x => x.StatusId == (long?)(ToDoStatusEnum.OPEN) || x.StatusId == (long?)(ToDoStatusEnum.INPROGRESS)).Count(),
                ExpiredToDoCollection = expiredToDoList.Select(x => _todofactory.CreateViewModel(x, toDoListComment)).ToList(),
                ExpiredToDoCount = expiredToDoList.Count(),
                IsFranchiseeAdmin = filter.RoleId != (long?)RoleType.SuperAdmin && filter.RoleId != (long?)RoleType.FrontOfficeExecutive ? true : false,
                FranchiseeId = filter.LoggedInFranchiseeId
            };
        }

        public bool SaveToDoFollowUp(ToDoEditModel model, long? franchiseeId)
        {
            var toDoId = default(long?);
            var jobScheduler = _jobSchedulerRepository.Get(model.SchedulerId.GetValueOrDefault());
            var isPresent = false;
            var isNew = false;
            var toDoComment = new ToDoFollowUpComment();
            var id = default(long);
            var dataRecordrId = default(long);
            if (model.Id > 0)
            {
                isNew = false;
                toDoId = model.Id;
                toDoComment = _toDoCommentRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ToDoId == model.Id);
                if (toDoComment != null)
                {
                    id = toDoComment.Id;
                    dataRecordrId = toDoComment.DataRecorderMetaDataId;
                }
            }
            else
            {
                isNew = true;
            }
            model.Date = model.ActualDate;
            var domain = _todofactory.CreateDomain(model);
            domain.FranchiseeId = model.FranchiseeId != null && model.FranchiseeId > 0 ? model.FranchiseeId : franchiseeId;
            if (model.SelectedFranchiseeId != "" && model.SelectedFranchiseeId != null)
            {
                var frId = long.Parse(model.SelectedFranchiseeId);
                domain.FranchiseeId = frId;
            }
            if (domain.FranchiseeId == 0 || domain.FranchiseeId == 1)
            {
                domain.FranchiseeId = null;
            }
            if (jobScheduler != null)
            {
                domain.CustomerId = jobScheduler.Job != null ? jobScheduler.Job.CustomerId : jobScheduler.Estimate.CustomerId;
            }
            if (!domain.IsNew && domain.DataRecorderMetaData.Id > 0)
            {
                domain.DataRecorderMetaDataId = domain.DataRecorderMetaData.Id;
            }
            _toDoListRepository.Save(domain);
            if (domain.Comment != null)
            {
                if (toDoComment != null)
                {
                    isPresent = toDoComment.Comment == domain.Comment;
                }
                else
                {
                    isNew = true;
                }
                var toDoCommentDomain = new ToDoFollowUpComment()
                {
                    ToDoId = domain.Id,
                    Comment = domain.Comment,
                    DataRecorderMetaData = new DataRecorderMetaData(),
                    IsNew = true
                };

                if (isNew)
                {
                    _toDoCommentRepository.Save(toDoCommentDomain);
                }
                else
                {
                    if (!isPresent)
                    {
                        toDoCommentDomain.Id = id;
                        toDoCommentDomain.IsNew = false;
                        toDoCommentDomain.DataRecorderMetaDataId = dataRecordrId;
                        _toDoCommentRepository.Save(toDoCommentDomain);
                    }
                }
            }
            return true;
        }
        public ToDoEditModel GetToDoById(long? id)
        {
            var comment = "";
            var domain = _toDoListRepository.Table.FirstOrDefault(x => x.Id == id);
            var toDoComment = _toDoCommentRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ToDoId == id);
            if (toDoComment != null)
            {
                comment = toDoComment.Comment;
            }
            else
            {
                comment = domain.Comment;
            }
            return new ToDoEditModel()
            {
                Comment = comment,
                Id = domain.Id,
                StatusName = domain.Lookup.Name,
                FranchiseeId = domain.FranchiseeId,
                Task = domain.Task,
                Date = domain.Date,
                StatusId = domain.StatusId,
                IsCustomerUse = domain.CustomerName != "" ? true : false,
                CustomerName = domain.CustomerName,
                PhoneNumber = domain.PhoneNumber,
                Email = domain.Email,
                TypeId = domain.TypeId,
                TaskChoiceId = domain.TaskChoiceId,
                TaskChoice = domain.TaskChoice != null ? domain.TaskChoice.Name : "",
                DataRecorderMetaData = domain.DataRecorderMetaData != null ? domain.DataRecorderMetaData : null,
                IsJobOrEstimate = domain.JobScheduler != null ? domain.JobScheduler.EstimateId != null ? "Estimate" : "Job" : "",
                SchedulerId = domain.JobScheduler != null ? domain.JobScheduler.Id : default(long)
            };
        }


        public IEnumerable<CustomerNameModel> GetCustomerList(string text)
        {
            var stateList = _customerRepository.Fetch(x => x.CustomerName.StartsWith(text.Trim())).Distinct().ToList();
            return stateList.Select(x => new CustomerNameModel() { Name = x.CustomerName, CustomerId = x.Id });
        }
        public CustomerNameModel GetCustomerInfo(string text)
        {
            var customer = _customerRepository.Table.FirstOrDefault(x => x.CustomerName == (text.Trim()));
            return new CustomerNameModel()
            {
                PhoneNumber = customer != null ? customer.PhoneNumber : "",
                Email = customer != null && customer.Email != null ? customer.Email : ""
            };
        }

        public bool SaveCommentInfo(CommentModel model)
        {
            var toDoCommentDomain = new ToDoFollowUpComment()
            {
                ToDoId = model.ToDoId.GetValueOrDefault(),
                Comment = model.Comment,
                DataRecorderMetaData = new DataRecorderMetaData(),
                IsNew = true
            };
            _toDoCommentRepository.Save(toDoCommentDomain);
            return true;
        }

        public CommentListModel GetCommentInfo(long? toDoId)
        {
            var commentList = _toDoCommentRepository.Table.Where(x => x.ToDoId == toDoId).OrderByDescending(x => x.Id).ToList();
            var commentViewModel = commentList.Select(x => CreateViewModel(x)).ToList();
            return new CommentListModel()
            {
                CommentList = commentViewModel
            };
        }
        private CommentModel CreateViewModel(ToDoFollowUpComment domain)
        {
            var orgRoleUser = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.Id == domain.DataRecorderMetaData.CreatedBy);

            return new CommentModel()
            {
                ToDoId = domain.ToDoId,
                Comment = domain.Comment,
                UserName = orgRoleUser != null ? orgRoleUser.Person.FirstName + " " + orgRoleUser.Person.LastName : "",
                Date = domain.DataRecorderMetaData.DateCreated,
                Id = domain.Id
            };
        }
        private CommentModel CreateViewModel(ToDoFollowUpList domain)
        {
            var person = _personsRepository.Table.FirstOrDefault(x => x.Id == domain.UserId);
            return new CommentModel()
            {
                ToDoId = domain.Id,
                Comment = domain.Comment,
                Id = domain.Id,
                Date = domain.Date,
                UserName = person.FirstName + " " + person.LastName,
                Status = domain.Lookup.Name,
                ToDo = domain.Task,
                CustomerName = domain.CustomerName,
                PhoneNumber = domain.PhoneNumber,
                Email = domain.Email,
                StatusId = domain.StatusId,
                UserId = domain.UserId,
                FranchiseeId = domain.FranchiseeId,
                SchedulerId = domain.SchedulerId,
                IsVisible = true,
                RedirectToFollowUp = _setting.RedirectToFollowUp
            };
        }
        public CommentListModel GetCommentToDoForScheduler(long? toDoId, long? userId, long? roleId)
        {
            var currentDate = DateTime.UtcNow.Date;
            var todayFollowUpToDo = default(int);
            var toDoList = _toDoListRepository.Table.Where(x => x.SchedulerId == toDoId &&
            ((roleId == (long?)RoleType.SuperAdmin || roleId == (long?)RoleType.FranchiseeAdmin
            || roleId == (long?)RoleType.FrontOfficeExecutive) ? true : (x.UserId == userId))).ToList();
            var commentViewModel = toDoList.Select(x => CreateViewModel(x)).ToList();
            var toDoListTopFive = commentViewModel.Take(5).ToList();
            if (roleId == (long?)RoleType.SuperAdmin || roleId == (long?)RoleType.FrontOfficeExecutive)
            {
                todayFollowUpToDo = _toDoListRepository.Table.Where(x => x.Date == currentDate && (x.StatusId == (long?)(ToDoStatusEnum.OPEN) || x.StatusId == (long?)(ToDoStatusEnum.INPROGRESS))).ToList().Count();
            }
            else if (roleId == (long?)RoleType.FranchiseeAdmin)
            {
                var franchiseeIds = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).ToList();
                var userIds = _organizationRoleUserRepository.Table.Where(x => franchiseeIds.Contains(x.OrganizationId)).Select(x => x.UserId).ToList();
                todayFollowUpToDo = _toDoListRepository.Table.Where(x => userIds.Contains(x.UserId.Value) && x.Date == currentDate && (x.StatusId == (long?)(ToDoStatusEnum.OPEN) || x.StatusId == (long?)(ToDoStatusEnum.INPROGRESS))).ToList().Count();
            }
            else if (roleId == (long?)RoleType.SalesRep || roleId == (long?)RoleType.Technician)
            {
                todayFollowUpToDo = _toDoListRepository.Table.Where(x => x.UserId == userId && x.Date == currentDate && (x.StatusId == (long?)(ToDoStatusEnum.OPEN) || x.StatusId == (long?)(ToDoStatusEnum.INPROGRESS))).ToList().Count();
            }
            return new CommentListModel()
            {
                CommentList = toDoListTopFive,
                TotalCommentList = commentViewModel,
                IsMoreThanFive = commentViewModel.Count() > 5 ? true : false,
                TodayToDoCount = todayFollowUpToDo
            };
        }


        public CommentListModel GetCommentToDoForSchedulerScreen(UserListModel model)
        {
            if ((model.RoleId == (long?)(RoleType.Technician)|| model.RoleId == (long?)(RoleType.SalesRep)) && model.CommentList.Count() == 0)
            {
                model.CommentList.Add(model.UserId);
            }
            var currentDate = DateTime.UtcNow.Date;
            var userList = model.CommentList.ToList();
            var userIds = _organizationRoleUserRepository.Table.Where(x => userList.Contains(x.Id)).Select(x => x.UserId).ToList();
            var toDoList = _toDoListRepository.Table.Where(x => userIds.Contains(x.UserId.Value)
            && x.Date == currentDate.Date).OrderBy(x => x.StatusId).ToList();
            var commentViewModel = toDoList.Select(x => CreateViewModel(x)).ToList();
            return new CommentListModel()
            {
                CommentList = commentViewModel,
            };
        }
        public bool SaveToDoByStatus(ToDoEditModel model)
        {
            var todo = _toDoListRepository.Table.FirstOrDefault(x => x.Id == model.Id);
            todo.StatusId = model.StatusId;
            todo.IsNew = false;
            //model.Date = model.ActualDate;
            //var domain = _todofactory.CreateDomainForToDoStatus(model);
            _toDoListRepository.Save(todo);
            return true;
        }

        public bool DeleteToDo(long? id)
        {
            var todoDomamin = _toDoListRepository.Get(id.Value);
            _toDoListRepository.Delete(todoDomamin);
            return true;
        }
        public IEnumerable<ToDoModel> GetToDoList(string text, long userId)
        {
            var toDoList = _toDoListRepository.Fetch(x => x.Task.ToUpper().StartsWith(text.ToUpper().Trim()) && x.UserId == userId).Distinct().ToList();
            var toDoListModel = toDoList.Select(x => new ToDoModel() { Name = x.Task }).Distinct().ToList();
            toDoListModel.Add(new ToDoModel() { Name = "Cold-Calling" });
            toDoListModel.Add(new ToDoModel() { Name = "Follow-Up" });
            var toDoListModelGrouped = toDoListModel.GroupBy(x => x.Name).Select(x => new ToDoModel() { Name = x.Key }).ToList();
            toDoListModel = toDoListModelGrouped.Where(x => x.Name.ToUpper().StartsWith(text.ToUpper().Trim())).Distinct().ToList();
            return toDoListModel;
        }
        public ToDoEditModel GetToDoInfo(string text, long userId)
        {
            var toDoDomain = _toDoListRepository.Table.FirstOrDefault(x => x.Task == text.Trim() && x.UserId == userId);
            if (toDoDomain != null)
            {
                var toDoEditModel = GetToDoById(toDoDomain.Id);
                toDoEditModel.Id = 0;
                return toDoEditModel != null ? toDoEditModel : default(ToDoEditModel);
            }
            return default(ToDoEditModel);
        }

        public ToDoSchedulerViewModel GetSchedulerForToDo(ToDoSchedulerModel model)
        {
            var isJob = false;
            if (model.IsJobOrEstimate == 0)
            {
                isJob = true;
            }
            var franchiseeId = long.Parse(model.FranchiseeId);

            var list = new ToDoSchedulerViewModel();
            List<JobScheduler> jobSchedulerList = new List<JobScheduler>();

            list.IsJobOrScheduler = model.IsJobOrEstimate;

            //var jobScheduler = isJob == true ? _jobSchedulerRepository.IncludeMultiple(x => x.Person).Where(x => x.JobId != null && x.FranchiseeId == franchiseeId && x.Person.FirstName.Contains(model.CustomerName)).ToList() : _jobSchedulerRepository.IncludeMultiple(x => x.Person).Where(x => x.EstimateId != null && x.FranchiseeId == franchiseeId && x.Person.FirstName.Contains(model.CustomerName)).ToList();
            
            if (isJob)
            {
                jobSchedulerList = //_jobSchedulerRepository.Table.Where(x => x.JobId != null && x.FranchiseeId == franchiseeId).ToList();
                jobSchedulerList = _jobSchedulerRepository.IncludeMultiple(x => x.Job).Where(x => x.JobId != null && x.FranchiseeId == franchiseeId && x.Job.JobCustomer.CustomerName.Contains(model.CustomerName)).ToList();
            }
            else
            {
                jobSchedulerList = //_jobSchedulerRepository.Table.Where(x => x.EstimateId != null && x.FranchiseeId == franchiseeId).ToList();
                jobSchedulerList = _jobSchedulerRepository.IncludeMultiple(x => x.Estimate).Where(x => x.EstimateId != null && x.FranchiseeId == franchiseeId && x.Estimate.JobCustomer.CustomerName.Contains(model.CustomerName)).ToList();
            }

            foreach (var item in jobSchedulerList)
            {
                JobSchedulerForToDo jobScheduler = new JobSchedulerForToDo();

                jobScheduler.Id = item.Id;
                jobScheduler.EstimateId = item.EstimateId != null ? item.EstimateId : null;
                jobScheduler.JobId = item.JobId != null ? item.JobId : null;
                jobScheduler.Title = item.Title;
                jobScheduler.SchedulerDateTime = item.ActualStartDate.ToString("MM/dd/yyyy hh:mm tt");
                list.JobSchedulerForToDo.Add(jobScheduler);
            }

            return list;
        }
    }
}
