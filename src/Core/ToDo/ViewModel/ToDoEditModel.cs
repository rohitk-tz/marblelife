using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;

namespace Core.ToDo.ViewModel
{
    [NoValidatorRequired]
    public class ToDoEditModel : EditModelBase
    {
        public long? OccuranceType { get; set; }
        public DateTime ActualDate { get; set; }
        public DateTime Date { get; set; }
        public string PersonName { get; set; }
        public string FranchiseeName { get; set; }
        public string Comment { get; set; }
        public string Task { get; set; }
        public long StatusId { get; set; }
        public long Id { get; set; }
        public long? FranchiseeId { get; set; }
        public string StatusName { get; set; }
        public long? UserId { get; set; }
        public bool IsExpand { get; set; }
        public bool IsFranchiseeLevel { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsCustomerUse { get; set; }
        public long? TypeId { get; set; }
        public long? SchedulerId { get; set; }
        public long? TaskChoiceId { get; set; }
        public string TaskChoice { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public long? DataRecorderMetaDataId { get; set; }
        public string RedirectURL { get; set; }
        public string LinkedEstimateTitle { get; set; }
        public string SelectedFranchiseeId { get; set; }
        public string IsJobOrEstimate { get; set; }
    }




    public class CustomerNameModel
    {
        public string Name { get; set; }
        public long? CustomerId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    [NoValidatorRequired]
    public class CommentListModel
    {
        public List<CommentModel> CommentList { get; set; }
        public List<CommentModel> TotalCommentList { get; set; }
        public bool IsMoreThanFive { get; set; }
        public long? TodayToDoCount { get; set; }
    }

    [NoValidatorRequired]
    public class UserListModel
    {
        public long? UserId { get; set; }
        public long? RoleId { get; set; }
        public List<long?> CommentList { get; set; }
    }

    [NoValidatorRequired]
    public class CommentModel
    {
        public long? Id { get; set; }
        public string Comment { get; set; }
        public long? ToDoId { get; set; }
        public string UserName { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        public string ToDo { get; set; }
        public string CustomerName { get; set; }
        public string PersonName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public long? StatusId { get; set; }
        public long? UserId { get; set; }
        public long? FranchiseeId { get; set; }
        public long? SchedulerId { get; set; }
        public bool IsCustomerUse { get; set; }
        public bool IsVisible { get; set; }
        public long? TaskOptionId { get; set; }
        public string RedirectToFollowUp { get; set; }
    }
    public class ToDoModel
    {
        public string Name { get; set; }
        public long? ToDoId { get; set; }
    }
    public enum PresentPast
    {
        Past = 1,
        Present = 2,
        Future = 3,
    }
    [NoValidatorRequired]
    public class ToDoSchedulerModel
    {
        public string FranchiseeId { get; set; }
        public string CustomerName { get; set; }
        public long IsJobOrEstimate { get; set; }
    }

    [NoValidatorRequired]
    public class ToDoSchedulerViewModel
    {
        public ToDoSchedulerViewModel()
        {
            JobSchedulerForToDo = new List<JobSchedulerForToDo>();
        }
        public long IsJobOrScheduler { get; set; }
        public List<JobSchedulerForToDo> JobSchedulerForToDo { get; set; }
    }

    public class JobSchedulerForToDo
    {
        public long Id { get; set; }
        public long? EstimateId { get; set; }
        public long? JobId { get; set; }
        public long FranchiseeId { get; set; }
        public string Title { get; set; }
        public String SchedulerDateTime { get; set; }
    }
}
