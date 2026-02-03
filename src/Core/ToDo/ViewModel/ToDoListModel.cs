using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ToDo.ViewModel
{
    public class ToDoListModel
    {
        public int TotalCount { get; set; }
        public List<ToDoEditModel> AllCollection { get; set; }
        public List<ToDoEditModel> TodaysCollection { get; set; }
        public int TodayToDoCount { get; set; }
        public bool IsFranchiseeAdmin { get; set; }
        public long? FranchiseeId { get; set; }
        public List<ToDoEditModel> ExpiredToDoCollection { get; set; }
        public int ExpiredToDoCount { get; set; }
    }
}
