
using Core.Application.Attribute;
using Core.Application.ViewModel;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class SaveBeforeAfterImageFilter 
    {
        public long? BeforeServiceId { get; set; }
        public long? AfterServiceId { get; set; }
        public bool IsAddToGalary { get; set; }
        public bool IsSelected { get; set; }
    }


    [NoValidatorRequired]
    public class EstimateWorthModel
    {
        public long? JobSchedulerId { get; set; }
        public decimal? Worth { get; set; }
        
    }
}
