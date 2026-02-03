using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Users.Enum;
using System;
using System.IO;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeDocumentService : IFranchiseeDocumentService
    {
        private readonly IClock _clock;
        private readonly IFileService _fileService;
        private readonly ILogService _logService;
        private readonly IRepository<FranchiseDocument> _franchiseeDocumentRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IFranchiseeDocumentFactory _franchiseeDocumentFactory;
        private readonly IRepository<DocumentType> _documentTypeRepository;
        private readonly ISortingHelper _sortingHelper;
        private readonly IDocumentNotificationPollingAgent _documentNotificationPollingAgent;
        private readonly IRepository<FranchiseeDocumentType> _franchiseeDocumentTypeRepository;

        public FranchiseeDocumentService(IUnitOfWork unitOfWork, IClock clock, IFileService fileService, ILogService logService,
            IFranchiseeDocumentFactory franchiseeDocumentFactory, ISortingHelper sortingHelper, IDocumentNotificationPollingAgent documentNotificationPollingAgent)
        {
            _clock = clock;
            _fileService = fileService;
            _logService = logService;
            _franchiseeDocumentRepository = unitOfWork.Repository<FranchiseDocument>();
            _franchiseeDocumentFactory = franchiseeDocumentFactory;
            _sortingHelper = sortingHelper;
            _documentNotificationPollingAgent = documentNotificationPollingAgent;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _documentTypeRepository = unitOfWork.Repository<Domain.DocumentType>();
            _franchiseeDocumentTypeRepository = unitOfWork.Repository<Domain.FranchiseeDocumentType>();
        }
        public bool SaveDocuments(FranchiseeDocumentEditModel model)
        {
            try
            {
                if (model.IsFromUser)
                {
                    model.Id = 0;

                }
                if (model.Id != 0)
                {
                    var docuumentInfo = _franchiseeDocumentRepository.Get(model.Id);
                    foreach (var id in model.FranchiseeIds.Distinct())
                    {
                        model.FranchiseeId = id;
                    }
                    var document = _franchiseeDocumentFactory.CreateDomain(model, docuumentInfo.FileId);
                    document.Franchisee = _franchiseeRepository.Get(model.FranchiseeId);
                    document.DocumentType = _documentTypeRepository.Get(model.DocumentTypeId.GetValueOrDefault());
                    if (model.FileModel == null)
                    {
                        document.File = null;
                        document.FileId = null;
                    }
                    if (model.FileModel != null && model.FileModel.Id == 0)
                    {
                        var fileId = SaveFile(model);
                        var uploadedDocument = _franchiseeDocumentFactory.CreateDomain(model, fileId);
                        document.FileId = uploadedDocument.FileId;
                    }

                    document.DataRecorderMetaData = docuumentInfo.DataRecorderMetaData;
                    document.DataRecorderMetaDataId = docuumentInfo.DataRecorderMetaDataId;
                    document.Id = model.Id;

                    _franchiseeDocumentRepository.Save(document);

                    if (model.IsImportant && docuumentInfo.IsImportant != model.IsImportant)
                    {
                        _documentNotificationPollingAgent.CreateDocumentUploadNotification(model.FileModel.Caption, model.FranchiseeIds, model.DataRecorderMetaData.CreatedBy);
                    }
                }
                else
                {
                    var fileId = default(long?);
                    string timesFormat = DateTime.Now.ToString("ss");
                    if (model.FileModel != null)
                    {
                        model.FileModel.Caption = String.Format(model.FileModel.Caption + "__" + timesFormat);
                        fileId = SaveFile(model);
                    }
                    else
                    {
                        fileId = null;
                    }
                    foreach (var id in model.FranchiseeIds.Distinct())
                    {
                        model.FranchiseeId = id;
                        var document = _franchiseeDocumentFactory.CreateDomain(model, fileId);
                        _franchiseeDocumentRepository.Save(document);
                    }
                    if (model.IsImportant)
                    {
                        _documentNotificationPollingAgent.CreateDocumentUploadNotification(model.FileModel.Caption, model.FranchiseeIds, model.DataRecorderMetaData.CreatedBy);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error Occured while saving Document," + ex.StackTrace));
                return false;
            }
            return true;
        }

        public long SaveFile(FranchiseeDocumentEditModel model)
        {
            //string dateFormat = DateTime.Now.ToString("dd/MM/yy");

            var path = MediaLocationHelper.FilePath(model.FileModel.RelativeLocation, model.FileModel.Name).ToFullPath();
            var destination = MediaLocationHelper.GetFranchiseeDocumentLocation();
            //var destFileName = string.Format((model.FileModel.Caption.Length <= 20) ? model.FileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
            //                    : model.FileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));

            var destFileName = model.FileModel.Caption;
            var fileName = _fileService.MoveFile(path, destination, destFileName, model.FileModel.Extension);
            model.FileModel.Name = destFileName + model.FileModel.Extension;
            model.FileModel.RelativeLocation = Path.GetDirectoryName(fileName).ToRelativePath();
            var file = _fileService.SaveModel(model.FileModel);
            return file.Id;
        }
        public DocumentViewModel GetFranchiseeInfoById(long? docId)
        {
            var documentInfo = _franchiseeDocumentRepository.Get(docId.GetValueOrDefault());
            var franchiseeDocumentTypeDomain = _franchiseeDocumentTypeRepository.Table.FirstOrDefault(x => x.DocumentTypeId == 11 && x.FranchiseeId == documentInfo.FranchiseeId);
            var document = _franchiseeDocumentFactory.CreateDomain(documentInfo);
            document.IsPerpetuity = franchiseeDocumentTypeDomain.IsPerpetuity;
            return document;
        }
        public DocumentListModel GetFranchiseeDocument(DocumentListFilter filter, int pageNumber, int pageSize)
        {
            var collection = GetDocumentFilterList(filter);

            var finalcollection = collection.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new DocumentListModel
            {
                Collection = finalcollection.Select(_franchiseeDocumentFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, collection.Count())
            };
        }

        private IQueryable<FranchiseDocument> GetDocumentFilterList(DocumentListFilter filter)
        {

            var documentList = _franchiseeDocumentRepository.Table.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                                          && (string.IsNullOrEmpty(filter.Text) || x.File.Name.Contains(filter.Text))
                                                          && (filter.PeriodStartDate == null || x.ExpiryDate >= filter.PeriodStartDate)
                                                          && (filter.PeriodEndDate == null || x.ExpiryDate <= filter.PeriodEndDate)
                                                           && (filter.DocumentTypeId == 0 || x.DocumentTypeId == filter.DocumentTypeId)
                                                          && (string.IsNullOrEmpty(filter.IsImportant) || (x.IsImportant.ToString() == filter.IsImportant))
                                                          && (!filter.ShowToUser || x.ShowToUser)
                                                          && (filter.CategoryId == null || filter.CategoryId == x.DocumentType.CategoryId));
            if (filter.isSaleTech)
            {
                documentList = documentList.Where((x => (x.UserId == filter.loggedinUser) || x.DocumentTypeId != (long)Enum.DocumentType.NCA)).OrderByDescending(x => x.ExpiryDate);
            }
            documentList = _sortingHelper.ApplySorting(documentList, x => x.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);
            //documentList = _sortingHelper.ApplySorting(documentList, x => x.ExpiryDate, (long)SortingOrder.Asc);
            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        documentList = _sortingHelper.ApplySorting(documentList, x => x.Id, filter.SortingOrder);
                        break;
                    case "Name":
                        documentList = _sortingHelper.ApplySorting(documentList, x => x.File.Caption, filter.SortingOrder);
                        break;
                    case "ExpiryDate":
                        documentList = _sortingHelper.ApplySorting(documentList, x => x.ExpiryDate, filter.SortingOrder);
                        break;
                    case "UploadDate":
                        documentList = _sortingHelper.ApplySorting(documentList, x => x.DataRecorderMetaData.DateCreated, filter.SortingOrder);
                        break;
                    case "UploadedBy":
                        documentList = _sortingHelper.ApplySorting(documentList, x => x.DataRecorderMetaData.CreatedBy, filter.SortingOrder);
                        break;
                    case "Franchisee":
                        documentList = _sortingHelper.ApplySorting(documentList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "Type":
                        documentList = _sortingHelper.ApplySorting(documentList, x => x.DocumentType.Name, filter.SortingOrder);
                        break;
                }
            }
            return documentList;
        }

        public bool Delete(long id)
        {
            var doc = _franchiseeDocumentRepository.Get(id);
            if (doc == null)
                return false;
            _franchiseeDocumentRepository.Delete(id);
            return true;
        }

        public string IsExpiryValid(FranchiseeDocumentEditModel model)
        {
            long? franchiseeId = default(long?);
            foreach (var id in model.FranchiseeIds)
            {
                franchiseeId = id;
            }
            var docuumentInfo = _franchiseeDocumentRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.DocumentTypeId == model.DocumentTypeId).OrderByDescending(x => x.Id).FirstOrDefault();
            if (docuumentInfo != null)
            {
                if (docuumentInfo.ExpiryDate >= model.ExpiryDate)
                {
                    return (docuumentInfo.ExpiryDate).Value.ToShortDateString();
                }
            }
            return "";
        }
    }
}
