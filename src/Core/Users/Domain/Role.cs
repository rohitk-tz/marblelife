namespace Core.Users.Domain
{
    public class Role: DomainBase
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public int AccessOrder { get; set; } 
    }
}
