using Core.Scheduler.Domain;
using Core.ToDo.ViewModel;
using System.Collections.Generic;

namespace Core.ToDo
{
   public interface IToDoService
    {
        ToDoListModel Get(ToDoFilter filter);
        bool SaveToDoFollowUp(ToDoEditModel model, long? franchiseeId);
        ToDoEditModel GetToDoById(long? Id);
        IEnumerable<CustomerNameModel> GetCustomerList(string text);
        CustomerNameModel GetCustomerInfo(string text);
        bool SaveCommentInfo(CommentModel model);

        CommentListModel GetCommentInfo(long? toDoId);
        CommentListModel GetCommentToDoForScheduler(long? toDoId,long? userId,long? roleId);
        CommentListModel GetCommentToDoForSchedulerScreen(UserListModel model);
        bool SaveToDoByStatus(ToDoEditModel model);
        bool DeleteToDo(long? id);

        IEnumerable<ToDoModel> GetToDoList(string text,long userId);
        ToDoEditModel GetToDoInfo(string text, long userId);
        ToDoSchedulerViewModel GetSchedulerForToDo(ToDoSchedulerModel model);
    }
}
