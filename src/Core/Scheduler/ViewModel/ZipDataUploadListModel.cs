using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class ZipDataUploadListModel
    {
        public IEnumerable<ZipDataUploadViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public ZipDataListFilter Filter { get; set; }
    }
}
