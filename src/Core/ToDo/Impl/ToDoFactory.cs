
using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.ToDo.Domain;
using Core.ToDo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.ToDo.Impl
{
    [DefaultImplementation]
    public class ToDoFactory : IToDoFactory
    {
        private readonly IClock _clock;
        private readonly ISettings _setting;
        public ToDoFactory(IClock clock, ISettings setting)
        {
            _clock = clock;
            _setting = setting;
        }
        public ToDoFollowUpList CreateDomain(ToDoEditModel editModel)
        {
            return new ToDoFollowUpList()
            {
                Comment = editModel.Comment,
                Date = (editModel.Date),
                StatusId = editModel.StatusId,
                Task = editModel.Task,
                Id = editModel.Id,
                IsNew = editModel.Id > 0 ? false : true,
                FranchiseeId = editModel.FranchiseeId,
                UserId = editModel.UserId,
                CustomerId = editModel.CustomerId,
                CustomerName = editModel.CustomerName,
                Email = editModel.Email,
                PhoneNumber = editModel.PhoneNumber,
                TypeId = editModel.TypeId,
                SchedulerId = editModel.SchedulerId,
                TaskChoiceId = editModel.TaskChoiceId,
                DataRecorderMetaData = editModel.Id > 0 && editModel.DataRecorderMetaDataId > 0 ? editModel.DataRecorderMetaData : new DataRecorderMetaData(),
            };
        }

        public ToDoEditModel CreateViewModel(ToDoFollowUpList domain, List<ToDoFollowUpComment> toDoListWhole = null)
        {
            var today = DateTime.Now.Date;
            var comment = "";
            if (toDoListWhole != null)
            {
                var toDoComment = toDoListWhole.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ToDoId == domain.Id);
                if (toDoComment != null)
                {
                    comment = toDoComment.Comment;
                }
                else
                {
                    comment = domain.Comment;
                }
            }
            else
            {
                comment = domain.Comment;
            }
            return new ToDoEditModel()
            {
                Comment = comment,
                Date = _clock.ToUtc(domain.Date),
                StatusId = domain.StatusId,
                Task = domain.Task,
                Id = domain.Id,
                FranchiseeId = domain.FranchiseeId,
                StatusName = domain.Lookup.Name,
                IsExpand = false,
                OccuranceType = (today == domain.Date) ? (long?)PresentPast.Present : (today > domain.Date) ? (long?)PresentPast.Past : (long?)PresentPast.Future,
                PersonName = domain.Person != null ? domain.Person.FirstName + " " + domain.Person.LastName : "",
                FranchiseeName = domain.Franchisee != null ? domain.Franchisee.Organization.Name : "",
                CustomerName = domain.CustomerName,
                Email = domain.Email,
                PhoneNumber = domain.PhoneNumber,
                UserId = domain.UserId,
                IsCustomerUse = domain.CustomerName != "" ? true : false,
                TaskChoiceId = domain.TaskChoiceId,
                TaskChoice = domain.TaskChoice != null ? domain.TaskChoice.Name : "",
                DateCreated = domain.DataRecorderMetaData != null ? _clock.ToLocal(domain.DataRecorderMetaData.DateCreated) : (DateTime?)null,
                DateModified = domain.DataRecorderMetaData != null && domain.DataRecorderMetaData.DateModified != null ? _clock.ToLocal((DateTime)domain.DataRecorderMetaData.DateModified) : (DateTime?)null,
                RedirectURL = domain.JobScheduler != null ? domain.JobScheduler.EstimateId != null ? _setting.RedirectToJobEstimation + domain.JobScheduler.EstimateId + "/manage/" + domain.JobScheduler.Id : _setting.RedirectToJobEstimation + domain.JobScheduler.JobId + "/edit/" + domain.JobScheduler.Id : "",
                LinkedEstimateTitle = domain.JobScheduler != null ? domain.JobScheduler.Title : ""
            };
        }
    }
}
