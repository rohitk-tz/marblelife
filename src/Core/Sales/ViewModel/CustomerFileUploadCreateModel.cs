using Core.Application.Attribute;
using Core.Application.ViewModel;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class CustomerFileUploadCreateModel : EditModelBase
    {
        public long Id { get; set; }
        public long FileId { get; set; }
        public long StatusId { get; set; }
        public FileModel File { get; set; }
        public string notes { get; set; }
    }
}
