using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class PriceEstimateDataUploadListModel
    {
        public IEnumerable<PriceEstimateDataUploadViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public PriceEstimateDataListFilter Filter { get; set; }
    }

    [NoValidatorRequired]
    public class PriceEstimateDataUploadViewModel
    {
        public long Id { get; set; }
        public DateTime UploadedOn { get; set; }
        public string Status { get; set; }
        public long StatusId { get; set; }
        public string UploadedBy { get; set; }
        public string Notes { get; set; }
        public bool IsEditable { get; set; }
        public string TempNotes { get; set; }
        public long FileId { get; set; }
        public long LogFileId { get; set; }
    }

    [NoValidatorRequired]
    public class PriceEstimateDataListFilter
    {
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
    }
}
