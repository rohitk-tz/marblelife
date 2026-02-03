using Core.Geo.ViewModel;

namespace Core.Notification.ViewModel
{
    public class EmailNotificationModelBase
    {
        public string CompanyLogoImageUrl { get; set; }
        public string CompanyName { get; set; }
        public string ApplicationName { get; set; }
        public AddressViewModel Address { get; set; }
        public string SiteRootUrl { get; set; }
        public string Designation { get; set; }
        public string Phone { get; set; }
        public string OwnerName { get; set; }
        public string SchedulingAppliation { get; set; }
        public string GetUrl(string relativeUrl)
        {
            return string.Format("{0}{1}", SiteRootUrl, relativeUrl);
        }
    }
}
