namespace Core.Application.Domain
{
    public class ContentType : DomainBase
    {
        public virtual string Name { get; set; }

        public virtual string MimeType { get; set; }
    }
}
