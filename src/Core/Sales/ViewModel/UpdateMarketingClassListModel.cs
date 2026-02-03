using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;
using System;
using Core.Scheduler.ViewModel;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class UpdateMarketingClassListModel
    {
        public IEnumerable<UpdateMarketingClassViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public UpdateMarketingClassInfoListFilter Filter { get; set; }
    }
}
