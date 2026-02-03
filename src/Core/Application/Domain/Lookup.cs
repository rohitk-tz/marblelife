namespace Core.Application.Domain
{
    public class Lookup : DomainBase
    {
        public long LookupTypeId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public byte? RelativeOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
