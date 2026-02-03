
namespace Core.Organizations.Domain
{
  public  class ReviewPushAPILocation : DomainBase
    {
        public string Name { get; set; }
        public long Location_Id { get; set; }
        public long Rp_ID { get; set; }
        public string NewRp_ID { get; set; }
    }

}
