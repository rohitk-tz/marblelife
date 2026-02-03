using Core.Application;
using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class DocumentNotificationPollingAgent : IDocumentNotificationPollingAgent
    {
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly ILogService _logService;
        private readonly ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<FranchiseDocument> _franchiseeDocumentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentNotificationPollingAgent(IUnitOfWork unitOfWork, IUserNotificationModelFactory userNotificationModelFactory, ILogService logService,
            ISettings settings, IClock clock)
        {
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _userNotificationModelFactory = userNotificationModelFactory;
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _unitOfWork = unitOfWork;
            _franchiseeDocumentRepository = unitOfWork.Repository<FranchiseDocument>();
        }
        public void CreateDocumentUploadNotification(string fileName, ICollection<long> franciseeIds, long? createdBy)
        {
            var organizationRoleUser = _organizationRoleUserRepository.Get(createdBy.Value);
            if (organizationRoleUser == null)
                return;
            if (organizationRoleUser.RoleId == (long)RoleType.SuperAdmin)
            {
                long defaultFranchiseeId = 2;
                foreach (var franchiseeId in franciseeIds.Where(x => x > defaultFranchiseeId).Distinct())
                {
                    try
                    {
                        var franchisee = _franchiseeRepository.Get(franchiseeId);
                        if (franchisee == null)
                            continue;
                        _userNotificationModelFactory.CreateDocumentUploadNotification(fileName, organizationRoleUser, franchisee);
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(ex.StackTrace);
                        continue;
                    }
                }
            }
            else
                _userNotificationModelFactory.CreateDocumentUploadNotification(fileName, organizationRoleUser, null);
        }

        public void SendExpiryNotification()
        {
            if (!_settings.SendExpiryNotification)
            {
                _logService.Info("Document Expiry notification is turned off!");
                return;
            }
            var currentDate = _clock.UtcNow;
            var dateToCompare = currentDate.AddDays(3);
            var listDocuments = _franchiseeDocumentRepository.Table.Where(x => x.IsImportant && x.ExpiryDate != null
                                                && x.ExpiryDate >= currentDate && x.ExpiryDate <= dateToCompare).ToList();

            _unitOfWork.StartTransaction();
            foreach (var doc in listDocuments)
            {
                try
                {
                    _logService.Info(string.Format("starting Notification for doc {0} ", doc.Id));
                    _userNotificationModelFactory.CreateDocumentExpiryNotification(doc);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Info("Exception" + ex.StackTrace);
                    _unitOfWork.Rollback();
                    continue;
                }
                finally
                {
                    _unitOfWork.ResetContext();
                }
            }
        }
    }
}
