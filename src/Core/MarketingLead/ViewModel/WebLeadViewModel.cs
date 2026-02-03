namespace Core.MarketingLead.ViewModel
{
    public class WebLeadViewModel
    {
        public long Id { get; set; }
        public long franchise_id { get; set; }
        public string name_first { get; set; }
        public string name_last { get; set; }
        public string franchise_name { get; set; }
        public string franchise_email { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string County { get; set; }
        public string zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Addr { get; set; }
        public string Addr2 { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string type_property { get; set; }
        public string type_surface { get; set; }
        public string surface_desc { get; set; }
        public string source_contact { get; set; }
        public string AddEmail { get; set; }
        public string source_url { get; set; }
        public int? status { get; set; }
        public string date_created { get; set; }
        public string femail { get; set; }
    }
}
