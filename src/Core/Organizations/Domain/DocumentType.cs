namespace Core.Organizations.Domain
{
    public class DocumentType : DomainBase
    {
        public string Name { get; set; }
        public long CategoryId { get; set; }
        public long Order { get; set; }
    }
}
