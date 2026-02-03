
using Core.ToDo.Domain;
using Core.ToDo.ViewModel;
using System.Collections.Generic;

namespace Core.ToDo
{
   public interface IToDoFactory
    {
        ToDoFollowUpList CreateDomain(ToDoEditModel editModel);
        ToDoEditModel CreateViewModel(ToDoFollowUpList domain, List<ToDoFollowUpComment> toDoListWhole = null);
    }
}
