using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Review.ViewModel
{
   public class ReviewPushResponseViewModel
    {
        public string Name { get; set; }
        public long? Id { get; set; }
        public long? Location_id { get; set; }
        public long? Customer_id { get; set; }
        public long? FranchiseeId { get; set; }
        public string Franchise_name { get; set; }
        public string Rating { get; set; }
        public DateTime? Rp_date { get; set; }
        public DateTime? Db_date { get; set; }
        public string Url { get; set; }
        public string Review { get; set; }
        public string Email { get; set; }
        public long? CustomerId { get; set; }
        public string Taz_franchise_name { get; set; }
    }
}
