using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobListModel
    {
        public IEnumerable<JobViewModel> Collection { get; set; } 
        public JobListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }

        public string DefaultView { get; set; }
        public decimal? TotalSum { get; set; }
    }
}
