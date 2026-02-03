using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Scheduler;
using Core.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeDocumentFactory : IFranchiseeDocumentFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        private readonly IJobFactory _jobFactory;
        private readonly IClock _clock;
        IRepository<DocumentType> _documentTypeRepository;
        IRepository<Organization> _organizationRepository;
        IRepository<LookupType> _lookupTypeRepository;
        IRepository<FranchiseeDocumentType> _franchiseeDocumentTypeRepository;
        public FranchiseeDocumentFactory(IOrganizationRoleUserInfoService organizationRoleUserInfoService, IClock clock, IUnitOfWork unitOfWork, IJobFactory jobFactory)
        {
            _unitOfWork = unitOfWork;
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _clock = clock;
            _documentTypeRepository = unitOfWork.Repository<DocumentType>();
            _lookupTypeRepository = unitOfWork.Repository<LookupType>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _franchiseeDocumentTypeRepository = unitOfWork.Repository<FranchiseeDocumentType>();
            _jobFactory = jobFactory;
        }

        public FranchiseDocument CreateDomain(FranchiseeDocumentEditModel model, long? fileId)
        {
            var domain = new FranchiseDocument
            {
                FileId = fileId,
                ExpiryDate = _clock.ToUtc(model.ExpiryDate.GetValueOrDefault()),
                FranchiseeId = model.FranchiseeId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsImportant = model.IsImportant,
                ShowToUser = model.ShowToUser,
                DocumentTypeId = model.DocumentTypeId,
                IsNew = model.Id == 0 ? true : false,
                UserId = model.UserId,
                UploadFor = model.UploadFor,
                IsPerpetuity = model.IsPerpetuity,
                IsRejected = model.IsRejected

            };
            return domain;
        }

        public DocumentViewModel CreateViewModel(FranchiseDocument document)
        {
            var userInfo = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(document.DataRecorderMetaData.CreatedBy.Value);
            var uploadedBy = userInfo.Email;
            var uploadedbyRole = _organizationRoleUserInfoService.GetRoleName(document.DataRecorderMetaData.CreatedBy.Value);
            var uploadedbyUserId = _organizationRoleUserInfoService.GetUserIdFromOrganizationRoleUserId(document.DataRecorderMetaData.CreatedBy != null ? document.DataRecorderMetaData.CreatedBy.GetValueOrDefault() : document.DataRecorderMetaData.ModifiedBy.GetValueOrDefault());
            var index = document.File != null ? document.File.Caption.LastIndexOf("__") : -1;
            if (index > -1)
            {
                document.File.Caption = document.File.Caption.Substring(0, index);
            }
            var model = new DocumentViewModel
            {
                Id = document.Id,
                ExpiryDate = document.ExpiryDate != null ? (document.ExpiryDate.GetValueOrDefault()) : default(DateTime?),
                FileName = document.File != null ? document.File.Name : document.IsRejected ? "Declined Document" : "",
                Caption = document.File != null ? document.File.Caption : document.IsRejected ? "Declined Document" : "",
                FranchiseeName = document.Franchisee.Organization.Name,
                FranchiseeId = document.FranchiseeId,
                FileId = document.FileId != null ? document.FileId : null,
                UploadedOn = document.DataRecorderMetaData.DateCreated,
                UploadedBy = uploadedBy,
                Type = document.File != null ? document.File.MimeType : "",
                IsImportant = document.IsImportant,
                IsExpired = document.ExpiryDate < _clock.UtcNow,
                DocumentType = document.DocumentType != null ? document.DocumentType.Name : null,
                DocumentTypeId = document.DocumentTypeId,
                IsUploadedBySuperAdmin = uploadedbyRole.Name.ToLower() == "super admin" ? true : false,
                uploadedByUserId = uploadedbyUserId != null ? uploadedbyUserId.UserId : 0,
                UploadFor = document.UploadFor,
                IsDateDefault = (document.ExpiryDate != default(DateTime)) ? false : true,
                IsRejected = document.IsRejected,
                IsPerpetuity = document.IsPerpetuity
            };
            return model;
        }
        public IEnumerable<DocumentTypeEditModel> CreateEditModelForDocument(IEnumerable<FranchiseeDocumentType> documentList)
        {
            if (documentList == null || documentList.Count() < 1)
                return PrepareListModel();
            IList<DocumentTypeEditModel> list = new List<DocumentTypeEditModel>();
            foreach (var document in documentList)
            {
                var model = new DocumentTypeEditModel();
                model.DocumentId = document.DocumentTypeId;
                model.Name = document.DocumentType.Name;
                model.CategoryId = document.DocumentType.Id;
                model.isActive = document.IsActive;
                model.IsPerpetuity = document.IsPerpetuity;
                list.Add(model);
            }
            return list;
        }
        private ICollection<DocumentTypeEditModel> PrepareListModel()
        {
            var services = _documentTypeRepository.Table.Where(s => s.CategoryId != (long)Enum.DocumentType.LoanAgreement && s.CategoryId != (long)Enum.DocumentType.AnnualTaxFilling && s.CategoryId != (long)Enum.DocumentType.FranchiseeContract).ToList();
            IList<DocumentTypeEditModel> list = new List<DocumentTypeEditModel>();
            foreach (var service in services)
            {
                var model = PrepareModel(service);
                list.Add(model);
            }
            return list;
        }
        private DocumentTypeEditModel PrepareModel(DocumentType service)
        {

            var model = new DocumentTypeEditModel();
            if (service.Id == (long)Enum.DocumentType.RESALECERTIFICATE || service.Id == (long)Enum.DocumentType.LICENSE)
            {
                model.isActive = false;

            }
            else
            {
                model.isActive = true;
            }
            model.Name = service.Name;
            model.CategoryId = service.CategoryId;
            model.DocumentId = service.Id;
            return model;
        }
        public List<FranchiseeDocumentType> CreateDocumentDomain(IEnumerable<DocumentTypeEditModel> model, long franchiseeId)
        {
            var servicesInDb = new List<FranchiseeDocumentType>();
            var index = 0;
            if (franchiseeId > 0)
            {
                servicesInDb = _franchiseeDocumentTypeRepository.Fetch(x => x.FranchiseeId == franchiseeId).ToList();
            }
            foreach (var value in model)
            {
                FranchiseeDocumentType obj = null;
                if (servicesInDb.Count < index + 1)
                {
                    obj = new FranchiseeDocumentType();
                    obj.IsNew = true;
                    obj.Franchisee = _organizationRepository.Table.Where(x => x.Id == franchiseeId).Select(x => x).FirstOrDefault();
                    obj.DocumentType = _documentTypeRepository.Table.Where(x => x.Id == value.DocumentId).Select(x => x).FirstOrDefault();

                    servicesInDb.Add(obj);
                }
                else
                {
                    obj = servicesInDb.ElementAt(index);
                }
                obj.IsPerpetuity = value.IsPerpetuity;
                obj.IsActive = value.isActive;
                obj.FranchiseeId = obj.FranchiseeId;
                index++;
            }
            while (index < servicesInDb.Count)
            {
                servicesInDb.RemoveAt(index);
            }
            return servicesInDb;
        }

        public DocumentViewModel CreateDomain(FranchiseDocument model)
        {
            var domain = new DocumentViewModel
            {
                ExpiryDate = model.ExpiryDate,
                FranchiseeId = model.FranchiseeId,
                IsImportant = model.IsImportant,
                DocumentTypeId = model.DocumentTypeId,
                showToUser = model.ShowToUser,
                TypeId = model.DocumentType.Id,
                userId = model.UserId,
                UploadFor = model.UploadFor,
                IsPerpetuity = model.IsPerpetuity,
                IsRejected = model.IsRejected,
                FileModel = model.FileId != null ? _jobFactory.CreateServiceFileViewModel(model.File,null, model.UserId) : null
            };
            return domain;
        }

        public FrabchiseeDocumentEditModel CreateDomainForUser(FranchiseDocument model)
        {
            var domain = new FrabchiseeDocumentEditModel
            {
                FileId = model.FileId,
                FileName = model.File.Name,
                documentId = model.Id
            };
            return domain;
        }

    }
}
