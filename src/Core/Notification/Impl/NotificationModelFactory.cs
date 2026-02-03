using Core.Application;
using Core.Application.Attribute;
using Core.Geo;
using Core.Geo.ViewModel;
using Core.Notification.ViewModel;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class NotificationModelFactory : INotificationModelFactory
    {
        private readonly ISettings _settings;
        private readonly IAddressFactory _addressFactory;

        public NotificationModelFactory(ISettings settings, IUnitOfWork unitOfWork, IAddressFactory addressFactory)
        {
            _settings = settings;
            _addressFactory = addressFactory;
        }
        public EmailNotificationModelBase CreateBase(long organizationId)
        {
            var addressViewModel = new AddressViewModel();

            var model = new EmailNotificationModelBase
            {
                SiteRootUrl = _settings.SiteRootUrl,
                CompanyLogoImageUrl = _settings.LogoImage,
                Address = addressViewModel,
            };

            return model;
        }
        public EmailNotificationModelBase CreateBaseDefault()
        {
            var model = new EmailNotificationModelBase
            {
                SiteRootUrl = _settings.SiteRootUrl,
                CompanyLogoImageUrl = _settings.LogoImage,
                CompanyName = _settings.CompanyName,
                OwnerName = _settings.OwnerName,
                Designation = _settings.Designation,
                Phone = _settings.OwnerPhone,
                ApplicationName = _settings.ApplicationName,
                SchedulingAppliation = _settings.SchedulingAppliation,
                Address = new AddressViewModel()
            };
            return model;
        }
    }
}
