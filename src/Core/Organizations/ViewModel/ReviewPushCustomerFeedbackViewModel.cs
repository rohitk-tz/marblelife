using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
   public class ReviewPushCustomerFeedbackViewModel
    {
        public long? Id { get; set; }
        public long? RP_ID { get; set; }
        public string Franchisee_name { get; set; }
        public string Franchisee_site { get; set; }
        public string Name_first { get; set; }
        public string Name_last { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? Review_id { get; set; }
        public string Date { get; set; }
        public string Response { get; set; }
        public int FromQA { get; set; }
    }


    public class ReviewPushCustomerGoogleFeedbackViewModel
    {
        public string source { get; set; }
        public string source_id { get; set; }
        public string reviewType { get; set; }
        public string reviewTitle { get; set; }
        public string reviewBody { get; set; }
        public string ratingValue { get; set; }
        public DateTime datePublished { get; set; }
        public string author { get; set; }
        public string reviewLink { get; set; }
    }

}
