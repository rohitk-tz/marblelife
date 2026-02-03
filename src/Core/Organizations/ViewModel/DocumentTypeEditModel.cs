using Core.Application.Attribute;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class DocumentTypeEditModel
    {
        //public long Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public long CategoryId { get; set; }
        public bool isActive { get; set; }
        public long DocumentId { get; set; }
        public bool IsPerpetuity { get; set; }
    }
}
