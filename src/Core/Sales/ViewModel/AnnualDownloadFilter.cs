using Core.Application.Attribute;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class AnnualDownloadFilter
    {
        public string PropName { get; set; }
        public long Order { get; set; }
        public long ReportTypeId { get; set; }
    }
}
