using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Application.Enum;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using Core.Scheduler.Enum;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Core.Application.Domain;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class AttachingInvoicesServices : IAttachingInvoicesServices
    {
        private ILogService _logService;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly IJobFactory _jobFactory;
        private readonly IPdfFileService _pdfFileService;
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private readonly IEstimateInvoiceFactory _estimateInvoiceFactory;
        private readonly IRepository<EstimateServiceInvoiceNotes> _estimateServiceInvoiceNotesRepository;
        private readonly IRepository<JobEstimateServices> _jobEstimateServices;
        private readonly IRepository<JobEstimateImage> _jobEstimateImage;
        private readonly IRepository<JobEstimateImageCategory> _jobEstimateImageCategory;
        private readonly IRepository<EstimateInvoiceAssignee> _estimateInvoiceAssigneeRepository;
        private readonly IRepository<EstimateInvoiceDimension> _estimateInvoiceDimensionRepository;
        private readonly IRepository<TermsAndConditionFranchisee> _termsAndConditionFranchiseeRepository;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<CustomerSignature> _customerSignatureRepository;
        private readonly IRepository<EstimateInvoice> _estimateInvoiceRepository;
        private readonly IRepository<EstimateInvoiceService> _estimateInvoiceServiceRepository;
        private readonly List<string> fileNameList = new List<string>();

        public AttachingInvoicesServices(IUnitOfWork unitOfWork, ILogService logService, IClock clock, IJobFactory jobFactory, IEstimateInvoiceFactory estimateInvoiceFactory, IPdfFileService pdfFileService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _clock = clock;
            _pdfFileService = pdfFileService;
            _jobFactory = jobFactory;
            _estimateInvoiceFactory = estimateInvoiceFactory;
            _fileRepository = unitOfWork.Repository<Application.Domain.File>();
            _estimateServiceInvoiceNotesRepository = unitOfWork.Repository<EstimateServiceInvoiceNotes>();
            _jobEstimateImageCategory = unitOfWork.Repository<JobEstimateImageCategory>();
            _jobEstimateImage = unitOfWork.Repository<JobEstimateImage>();
            _jobEstimateServices = unitOfWork.Repository<JobEstimateServices>();
            _estimateInvoiceDimensionRepository = unitOfWork.Repository<EstimateInvoiceDimension>();
            _estimateInvoiceAssigneeRepository = unitOfWork.Repository<EstimateInvoiceAssignee>();
            _termsAndConditionFranchiseeRepository = unitOfWork.Repository<TermsAndConditionFranchisee>();
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _jobRepository = unitOfWork.Repository<Job>();
            _customerSignatureRepository = unitOfWork.Repository<CustomerSignature>();
            _estimateInvoiceServiceRepository = unitOfWork.Repository<EstimateInvoiceService>();
            _estimateInvoiceRepository = unitOfWork.Repository<EstimateInvoice>();
        }
        public void AttachInvoice()
        {
            try
            {
                InvoiceEstimate();
                _unitOfWork.ResetContext();
                InvoiceJob();
                _unitOfWork.ResetContext();
                InvoiceJobChanges();
                
            }
            catch (Exception e)
            {
                _logService.Error("Error in attaching invoice", e);
            }

        }

        private void ChangeInvoicesForJob(long estimateInvoiceId)
        {
            var estimateInvoice = _estimateInvoiceRepository.Get(estimateInvoiceId);
            var estimateServices = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoiceId).ToList();
            if (estimateInvoice == null)
                return;
            var jobs = _jobRepository.Table.FirstOrDefault(x => x.EstimateId == estimateInvoice.EstimateId);
            var assignees = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId && x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            if (jobs == null)
                return;
            foreach (var scheduler in jobs.JobScheduler)
            {
                _logService.Info("Attaching changed Invoice for scheduler Id: " + scheduler.Id);
                var services = _jobEstimateServices.Table.Where(x => x.JobEstimateImageCategory.SchedulerId == scheduler.Id).ToList();
                foreach (var service in services)
                {
                    var images = _jobEstimateImage.Table.Where(x => x.ServiceId == service.Id).ToList();
                    var estimateService = estimateServices.FirstOrDefault(x => x.InvoiceNumber == service.InvoiceNumber);
                    var assigneesForInvoice = assignees.Where(x => x.InvoiceNumber == service.InvoiceNumber && x.SchedulerId == scheduler.Id).ToList();
                    foreach (var image in images)
                    {
                        if (estimateService == null)
                        {
                            image.IsDeleted = true;
                            _jobEstimateImage.Delete(image);
                            foreach (var assignee in assigneesForInvoice)
                            {
                                _estimateInvoiceAssigneeRepository.Delete(assignee);
                            }
                        }
                        else
                        {
                            if (estimateService.JobEstimateImage != null)
                            {
                                image.FileId = estimateService.JobEstimateImage.FileId;
                                image.IsNew = false;
                                _jobEstimateImage.Save(image);
                            }
                        }
                    }
                }
            }
        }

        private void AttachInvoicesForJob(long jobschedulerId)
        {
            var jobscheduler = _jobSchedulerRepository.Table.FirstOrDefault(x => x.Id == jobschedulerId);
            var categoryId = default(long?);
            var assignees = _estimateInvoiceAssigneeRepository.Table.Where(x => x.SchedulerId == jobscheduler.Id && x.AssigneeId == jobscheduler.AssigneeId).ToList();
            var job = _jobRepository.Table.Where(x => x.Id == jobscheduler.JobId).FirstOrDefault();
            var jobestimate = _jobSchedulerRepository.Table.FirstOrDefault(x => x.EstimateId == job.EstimateId);

            if(jobestimate == null)
            {
                return;
            }

            var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == jobestimate.Id && x.EstimateId == job.EstimateId);
            if (estimateInvoice == null)
            {
                return;
            }
            var estimateServices = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            var jobEstimateImageCategory = _jobEstimateImageCategory.Table.Where(x => x.EstimateId == job.EstimateId).ToList();
            var jobEstimateImageCategoryDomain = jobEstimateImageCategory.FirstOrDefault(x => x.SchedulerId == jobschedulerId);
            if (jobEstimateImageCategoryDomain == null)
            {
                var estimateCategory = new JobEstimateImageCategory()
                {
                    EstimateId = job.EstimateId,
                    SchedulerId = jobschedulerId,
                    JobId = jobscheduler.JobId,
                    IsNew = true
                };
                _jobEstimateImageCategory.Save(estimateCategory);
                _unitOfWork.SaveChanges();
                categoryId = estimateCategory.Id;
            }
            else
            {
                categoryId = jobEstimateImageCategoryDomain.Id;
            }
            var inDbServicesIds = _jobEstimateServices.IncludeMultiple(x => x.DataRecorderMetaData).Where(x => x.CategoryId == categoryId
              && x.TypeId == (long)BeforeAfterImagesType.Invoice
              && x.IsFromInvoiceAttach.Value && x.IsInvoiceForJob == true && (x.JobEstimateImageCategory.SchedulerId == jobschedulerId)
              && !x.IsFromEstimate).ToList();
            foreach (var fileIdsDelete in inDbServicesIds)
            {
                _jobEstimateServices.Delete(fileIdsDelete);
                _unitOfWork.SaveChanges();
            }
            foreach (var assignee in assignees)
            {
                var jobEstimateBeforeService = new JobEstimateServices
                {
                    IsFromInvoiceAttach = true,
                    IsInvoiceForJob = true,
                    CategoryId = categoryId.GetValueOrDefault(),
                    TypeId = (long?)LookupTypes.InvoiceImages,
                    InvoiceNumber = assignee.InvoiceNumber,
                    DataRecorderMetaData = new DataRecorderMetaData(),
                    IsNew = true
                };
                _jobEstimateServices.Save(jobEstimateBeforeService);
                _unitOfWork.SaveChanges();

                var estimateService = estimateServices.FirstOrDefault(x => x.InvoiceNumber == assignee.InvoiceNumber);
                JobEstimateServiceViewModel model = new JobEstimateServiceViewModel();
                model.Id = 0;
                var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                if (estimateService != null && estimateService.JobEstimateImage != null)
                {
                    var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(model, buildingExteriorServiceId, (long?)LookupTypes.InvoiceImages, estimateService.JobEstimateImage.FileId);
                    jobEstimateBeforeServiceImage.IsNew = true;
                    _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                }
            }
        }

        private bool AttachInvoicesForEstimate(long estimateInvoiceId)
        {
            var estimateInvoice = _estimateInvoiceRepository.Get(estimateInvoiceId);
            var estimateServices = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoiceId).ToList();
            if (estimateInvoice == null)
                return false;

            var assignees = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId && x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            if (estimateInvoice.SchedulerId == null)
                return false;
            _logService.Info("Attaching changed Invoice for scheduler Id: " + (estimateInvoice.SchedulerId));
            try
            {
                return AddInvoiceToEstimate(estimateInvoice.SchedulerId.GetValueOrDefault(), false, null, null);
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool AddInvoiceToEstimate(long schedulerId, bool isInvoiceForJob, List<long?> invoiceNumbers = null, long? jobId = null, long? scheduleJobId = null)
        {
            var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);
            var sliderDomain = new JobEstimateServiceViewModel();
            if (estimateInvoice == null)
            {
                return false;
            }

            var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).OrderByDescending(x => x.Id);
            var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();
            if (estimateInvoiceServicesGroupedData.Count() == 0)
            {
                return false;
            }
            try
            {
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";
                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : "";
                estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : "";
                estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : "";
                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).Select(x => x.Id).ToList();
                if (jobId != null)
                {
                    jobForEstimateList = jobForEstimateList.Where(x => x.Equals(jobId)).ToList();
                }
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                int index = 1;
                var fileDomain = new List<Application.Domain.File>();
                var invoiceMapping = new List<FileMappedToInvoice>();

                var destinationFolderForAttachment = MediaLocationHelper.GetDocumentImageLocation().Path + "\\";
                var destinationFolder = MediaLocationHelper.GetAttachmentMediaLocation().Path + "\\";
                var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\");
                var viewPath = destinationFolder + "Templates\\" + "cutomer_invoice.cshtml";

                //var destinationFolder = MediaLocationHelper.GetDocumentImageLocation().Path + "\\";
                //var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + "cutomer_invoice.cshtml");
                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                if (invoiceNumbers != null)
                {
                    estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => invoiceNumbers.Contains(x.Key)).ToList();

                }
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();

                estimateInvoiceEditModel.OfficeName = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization!= null && estimateInvoice.Franchisee.Organization.Name!= null ?estimateInvoice.Franchisee.Organization.Name : "";
                estimateInvoiceEditModel.OfficeAddressLine1 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1 : "";
                estimateInvoiceEditModel.OfficeAddressLine2 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2 : "";
                estimateInvoiceEditModel.OfficeAddressCity = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName : "";
                estimateInvoiceEditModel.OfficeAddressState = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName : "";
                estimateInvoiceEditModel.OfficeAddressCountry = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country) != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name : ""; ;
                estimateInvoiceEditModel.OfficeAddressZipCode = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode : ""; ;

                estimateInvoiceEditModel.EstimateDate = estimateInvoice.JobScheduler.StartDateTimeString.ToString();
                var invoiceNumberCount = 1;
                var invoiceItemCount = estimateInvoiceServicesGroupedData.Count();
                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    
                    estimateInvoiceEditModel.Option1Total = 0;
                    estimateInvoiceEditModel.Option2Total = 0;
                    estimateInvoiceEditModel.Option3Total = 0;
                    var list = estimateInvoiceServicesLocal.ToList();

                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);
                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    estimateInvoiceEditModel.LessDepositPer = (int)estimateInvoice.Franchisee.LessDeposit;
                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption1.ToString()));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption2.ToString()));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption3.ToString()));
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);

                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));

                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";

                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";


                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }

                    var firstName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var LastName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), true);
                    fileFullName = fileFullName.Replace(".", "");
                    fileFullName = fileFullName.Replace(":", "");
                    var fileName = fileFullName + ".pdf";

                    var signatureViewModel = GetSignature(estimateInvoice.Id, estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.CustomerSignature = signatureViewModel.PreSignature;
                    estimateInvoiceEditModel.SignDateTime = signatureViewModel.PreSignatureDate;
                    estimateInvoiceEditModel.CustomerPostSignature = signatureViewModel.PostSignature;
                    estimateInvoiceEditModel.PostSignDateTime = signatureViewModel.PostSignatureDate;
                    estimateInvoiceEditModel.Technician = signatureViewModel.Technician;
                    if(invoiceItemCount == 1)
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id.ToString();
                    }
                    else
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id + "-" + invoiceNumberCount;
                        invoiceNumberCount += 1;
                    }

                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolderForAttachment, fileName, viewPath);
                    var fileModel = GetFileModel(file);
                    fileDomain.Add(fileModel);
                    var invoiceMap = new FileMappedToInvoice()
                    {
                        File = fileModel.Id,
                        InvoiceNumber = estimateInvoiceServicesLocal.Key
                    };
                    invoiceMapping.Add(invoiceMap);
                    foreach (var assignee in assignees)
                    {
                        assignee.Label = fileName;
                        assignee.IsNew = false;
                        _estimateInvoiceAssigneeRepository.Save(assignee);
                    }
                    index += 1;
                }
                var categoryId = default(long?);
                var jobEstimateImageCategory = _jobEstimateImageCategory.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();
                var jobEstimateImageCategoryDomain = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);
                if (jobEstimateImageCategoryDomain == null)
                {
                    var estimateCategory = new JobEstimateImageCategory()
                    {
                        EstimateId = jobScheduler.EstimateId,
                        SchedulerId = jobScheduler.Id,
                        JobId = jobScheduler.JobId,
                        IsNew = true
                    };
                    _jobEstimateImageCategory.Save(estimateCategory);
                    categoryId = estimateCategory.Id;
                }
                else
                {
                    categoryId = jobEstimateImageCategoryDomain.Id;
                }
                var filesId = fileDomain.Select(x => (long?)x.Id).ToList();
                sliderDomain = new JobEstimateServiceViewModel()
                {
                    FilesList = fileDomain.Select(x => (long?)x.Id).ToList(),
                    RowId = estimateInvoice.Id,
                };
                SaveBuildingImages(sliderDomain, categoryId.GetValueOrDefault(), isInvoiceForJob, jobScheduler.Id, invoiceMapping, scheduleJobId);
                _unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception e1)
            {
                _logService.Info("Exception in Attatching media, "+ e1);
                return false;
            }
        }

        private string FormatPhoneNumber(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            value = new System.Text.RegularExpressions.Regex(@"\D")
                .Replace(value, string.Empty);
            value = value.TrimStart('1');
            if (value.Length == 7)
                return Convert.ToInt64(value).ToString("###-####");
            if (value.Length == 10)
                return Convert.ToInt64(value).ToString("###-###-####");
            if (value.Length > 10)
                return Convert.ToInt64(value)
                    .ToString("###-###-#### " + new String('#', (value.Length - 10)));
            return value;
        }

        private string GetTechnicianName(List<EstimateInvoiceAssignee> assigneeList)
        {
            string techName = "";
            List<string> techNames = new List<string>();
            foreach (var jobScheduler in assigneeList)
            {
                techNames.Add(jobScheduler.User.Person != null ? jobScheduler.User.Person.FirstName + " " + jobScheduler.User.Person.LastName : "");
            }
            if (techNames.Count == 0)
            {
                techName = "-";
            }
            techName = string.Join(", ", techNames);
            return techName;
        }

        private List<EstimateInvoiceDimensionTableViewModel> GetMeasurements(List<EstimateInvoiceDimension> dimensionList, List<EstimateInvoiceService> invoiceService)
        {
            var estimateInvoiceDimensionTableViewModel = new EstimateInvoiceDimensionTableViewModel();
            var estimateInvoiceDimensionTableViewModelList = new List<EstimateInvoiceDimensionTableViewModel>();
            var estimateInvoiceDimensionEditModelList = new List<EstimateInvoiceDimensionViewModel>();
            var estimateInvoiceDimensionEditModel = new EstimateInvoiceDimensionViewModel();
            var x1 = 0;
            foreach (EstimateInvoiceService service in invoiceService)
            {
                estimateInvoiceDimensionEditModel = new EstimateInvoiceDimensionViewModel();
                estimateInvoiceDimensionTableViewModel = new EstimateInvoiceDimensionTableViewModel();
                x1++;
                estimateInvoiceDimensionTableViewModel.InvoiceLine = x1;
                if (dimensionList.Any(x => x.EstimateInvoiceServiceId == service.Id))
                {
                    var dimension = dimensionList.Where(x => x.EstimateInvoiceServiceId == service.Id).ToList();
                    estimateInvoiceDimensionTableViewModel.DimensionList = dimension.Select(x => _estimateInvoiceFactory.CreateEstimateInvoiceDimensionViewModel(x)).ToList();
                }
                else
                {
                    estimateInvoiceDimensionTableViewModel.DimensionList = new List<EstimateInvoiceDimensionViewModel>();
                }
                estimateInvoiceDimensionTableViewModelList.Add(estimateInvoiceDimensionTableViewModel);
            }

            return estimateInvoiceDimensionTableViewModelList;
        }

        private SignatureViewModel GetSignature(long? estimateInvoiceId, long? invoiceNumber)
        {
            var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoiceId).OrderByDescending(x => x.Id).ToList();

            SignatureViewModel signatureViewModel = new SignatureViewModel();
            if (customerSignature != null && customerSignature.Count() > 0)
            {
                var customerSignaturePreSigned = customerSignature.Where(x => x.TypeId == (long)SignatureType.PRECOMPLETION).OrderByDescending(x => x.Id).ToList();
                var customerSignatureForInvoice = customerSignaturePreSigned.FirstOrDefault(x => x.InvoiceNumber == invoiceNumber);
                if (customerSignaturePreSigned != null && customerSignatureForInvoice != null)
                {
                    signatureViewModel.PreSignature = customerSignatureForInvoice.Signature;
                    signatureViewModel.IsSigned = "none";
                    signatureViewModel.PreSignatureDate = customerSignatureForInvoice.SignedDateTime != null && customerSignatureForInvoice.SignedDateTime != default(DateTime?)
                                                                                    ? _clock.ToLocal(customerSignatureForInvoice.SignedDateTime.GetValueOrDefault()).ToString("MM/dd/yyyy") : "";

                }
                else
                {
                    if (customerSignaturePreSigned == null)
                    {
                        signatureViewModel.IsSigned = "block";
                    }
                }

                var customerSignaturePostSigned = customerSignature.Where(x => x.TypeId == (long)SignatureType.POSTCOMPLETION).OrderByDescending(x => x.Id).ToList();
                customerSignatureForInvoice = customerSignaturePostSigned.FirstOrDefault(x => x.InvoiceNumber == invoiceNumber);
                if (customerSignaturePostSigned != null && customerSignatureForInvoice != null)
                {
                    signatureViewModel.PostSignature = customerSignatureForInvoice.Signature;
                    signatureViewModel.IsSigned = "none";
                    signatureViewModel.PostSignatureDate = customerSignatureForInvoice.SignedDateTime != null && customerSignatureForInvoice.SignedDateTime != default(DateTime?)
                                                                                    ? _clock.ToLocal(customerSignatureForInvoice.SignedDateTime.GetValueOrDefault()).ToString("MM/dd/yyyy") : "";
                    signatureViewModel.Technician = customerSignatureForInvoice != null && customerSignatureForInvoice.JobScheduler != null ? customerSignatureForInvoice.JobScheduler.Person.FirstName + " " + customerSignatureForInvoice.JobScheduler.Person.LastName : " ";
                }
            }
            if (customerSignature.Count() == 0)
            {
                signatureViewModel.IsSigned = "block";
            }

            return signatureViewModel;
        }

        private string GetFileNameName(string customerName, EstimateInvoiceService invoiceService, bool isCustomerInvoice)
        {
            customerName = RemoveInvalidChars(customerName);
            var customerSplittedName = customerName.Split(' ');
            var locationJoined = "";
            var locationSplittedName = invoiceService.Location.Split(',');
            if (locationSplittedName.Length > 2)
            {
                locationSplittedName[0] = locationSplittedName[0].Replace(" ", "");
                locationSplittedName[1] = locationSplittedName[1].Replace(" ", "");
                locationJoined = locationSplittedName[0] + "_" + locationSplittedName[1];
            }
            else
            {
                var locationSplittedNameLocal = new List<string>();
                foreach (var locationName in locationSplittedName)
                {
                    locationSplittedNameLocal.Add(locationName.Replace(" ", ""));
                }
                locationJoined = String.Join("_", locationSplittedNameLocal);
            }

            var serviceType = invoiceService != null ? invoiceService.ServiceType : "";
            var fileName = "";
            foreach (var name in customerSplittedName.Reverse())
            {
                fileName += name + "_";
            }


            if (serviceType == "CONCRETE-COATINGS" || serviceType == "ENDURACRETE")
            {
                if (isCustomerInvoice)
                    fileName = fileName + "_" + "CustomerConcreteOrder";
                else
                    fileName = fileName + "_" + "InternalConcreteOrder";
            }
            else
            {
                if (isCustomerInvoice)
                    fileName = fileName + "_" + "CustomerOrder";
                else
                    fileName = fileName + "_" + "InternalOrder";
            }
            if (!string.IsNullOrEmpty(locationJoined))
                fileName = fileName + "_" + locationJoined;
            var count = fileNameList.Where(s => s.StartsWith(fileName)).Count();
            if (!fileNameList.Contains(fileName))
                fileNameList.Add(fileName);
            else
            {
                fileName = fileName + "_" + count;
                fileNameList.Add(fileName);
            }
            return fileName;
        }

        private Application.Domain.File GetFileModel(string localFileName)
        {

            var fileModel = new FileModel
            {
                Name = Path.GetFileName(localFileName),
                Caption = Path.GetFileNameWithoutExtension(localFileName),
                RelativeLocation = Path.GetDirectoryName(localFileName),
                MimeType = "application/pdf",
                Size = new FileInfo(localFileName).Length,
                Extension = Path.GetExtension(localFileName)
            };
            var file = new Application.Domain.File
            {
                Name = fileModel.Name,
                Caption = fileModel.Caption,
                RelativeLocation = fileModel.RelativeLocation,
                MimeType = fileModel.MimeType,
                Size = fileModel.Size,
                IsNew = true,
                DataRecorderMetaData = fileModel.DataRecorderMetaData
            };
            _ = file.DataRecorderMetaData.CreatedBy == null ? file.DataRecorderMetaData.CreatedBy = 1027 : file.DataRecorderMetaData.CreatedBy = file.DataRecorderMetaData.CreatedBy;
            //file.DataRecorderMetaData.IsNew = true;
            _fileRepository.Save(file);
            return file;
        }

        private void SaveBuildingImages(JobEstimateServiceViewModel sliderDomain, long categoryId, bool isInvoiceForJob, long schedulerId, List<FileMappedToInvoice> fileMappedToInvoices, long? jobSchedulerId = null)
        {
            var inDbServicesIds = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long)BeforeAfterImagesType.Invoice && x.IsFromInvoiceAttach.Value && x.IsInvoiceForJob == isInvoiceForJob && (x.JobEstimateImageCategory.SchedulerId == schedulerId) && !x.IsFromEstimate).Select(x => x.Id).ToList();
            foreach (var fileIdsDelete in inDbServicesIds)
            {
                var jobEstimateServices = _jobEstimateServices.Get(fileIdsDelete);

                var jobEstimateImages = _jobEstimateImage.IncludeMultiple(x => x.DataRecorderMetaData).FirstOrDefault(x => x.ServiceId == jobEstimateServices.Id);
                if (jobEstimateImages != null && jobEstimateImages.File != null)
                {
                    jobEstimateImages.File.IsFileToBeDeleted = true;
                    _jobEstimateImage.Save(jobEstimateImages);
                    _unitOfWork.SaveChanges();
                    _jobEstimateServices.Delete(jobEstimateServices);
                    _unitOfWork.SaveChanges();
                }
            }

            //if (isInvoiceForJob)
            //{
            //    var servicesIdsForJob = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long)BeforeAfterImagesType.Invoice && !x.IsFromEstimate && (x.JobEstimateImageCategory.SchedulerId == jobSchedulerId)).Select(x => x.Id).ToList();
            //    foreach (var fileIdsDelete in servicesIdsForJob)
            //    {
            //        var jobEstimateServices = _jobEstimateServices.Get(fileIdsDelete);

            //        var jobEstimateImages = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == jobEstimateServices.Id);

            //        jobEstimateImages.File.IsFileToBeDeleted = true;
            //        _jobEstimateImage.Save(jobEstimateImages);
            //        _jobEstimateServices.Delete(jobEstimateServices);
            //    }

            //}

            var estimateServices = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == sliderDomain.RowId).ToList();
            foreach (var fileId in sliderDomain.FilesList)
            {
                var map = fileMappedToInvoices.FirstOrDefault(x => x.File == fileId);
                var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, categoryId, (long?)LookupTypes.InvoiceImages);
                jobEstimateBeforeService.IsFromInvoiceAttach = true;
                jobEstimateBeforeService.IsInvoiceForJob = isInvoiceForJob;
                jobEstimateBeforeService.InvoiceNumber = map.InvoiceNumber;
                jobEstimateBeforeService.IsNew = true;
                _jobEstimateServices.Save(jobEstimateBeforeService);
                var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(sliderDomain, buildingExteriorServiceId, (long?)LookupTypes.InvoiceImages, fileId);
                jobEstimateBeforeServiceImage.IsNew = true;
                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                var services = estimateServices.Where(x => x.InvoiceNumber == map.InvoiceNumber).ToList();
                foreach (var service in services)
                {
                    if (service == null)
                    {
                        continue;
                    }
                    var file = service.JobEstimateImage != null ? service.JobEstimateImage.File : null;
                    if (file != null)
                    {
                        file.IsFileToBeDeleted = true;
                        _fileRepository.Save(file);
                    }
                    service.InvoiceImageId = jobEstimateBeforeServiceImage.Id;
                    service.IsNew = false;
                    _estimateInvoiceServiceRepository.Save(service);
                }
            }
        }


        private void InvoiceEstimate()
        {
            _unitOfWork.StartTransaction();
            var estimateInvoicesForEstimate = _estimateInvoiceRepository.Table.Where(x => x.IsInvoiceChanged).ToList();
            foreach (var estimateInvoice in estimateInvoicesForEstimate)
            {

                _logService.Info("Attacting Invoice For Estimate for EstimateInvoiceId: " + estimateInvoice.Id);

                var isFileAttached = AttachInvoicesForEstimate(estimateInvoice.Id);

                if (isFileAttached)
                {
                    estimateInvoice.IsInvoiceChanged = false;
                    estimateInvoice.IsInvoiceParsing = false;
                    _estimateInvoiceRepository.Save(estimateInvoice);

                }
            }
            _unitOfWork.SaveChanges();
        }

        private void InvoiceJob()
        {
            _unitOfWork.StartTransaction();

            var jobSchedulers = _jobSchedulerRepository.Table.Where(x => x.IsJobConverted).OrderByDescending(y => y.Id).ToList();
            foreach (var jobscheduler in jobSchedulers)
            {
                try
                {
                    _logService.Info("Attaching Invoice for scheduler Id: " + jobscheduler.Id);
                    AttachInvoicesForJob(jobscheduler.Id);
                    jobscheduler.IsJobConverted = false;
                    jobscheduler.IsNew = false;
                    _jobSchedulerRepository.Save(jobscheduler);
                }
                catch(Exception ex)
                {
                    _logService.Error("Error in attaching invoice for Job", ex);
                }
               
            }
            _unitOfWork.SaveChanges();
        }

        private void InvoiceJobChanges()
        {
            _unitOfWork.StartTransaction();
            var estimateInvoices = _estimateInvoiceRepository.Table.Where(x => x.IsInvoiceChanged && !x.IsInvoiceParsing).ToList();
            foreach (var invoice in estimateInvoices)
            {
                _logService.Info("Changing Invoice for estimate invoice Id: " + invoice.Id);
                ChangeInvoicesForJob(invoice.Id);
                invoice.IsInvoiceChanged = false;
                invoice.IsInvoiceParsing = false;
                _estimateInvoiceRepository.Save(invoice);

            }
            _unitOfWork.SaveChanges();
        }

        public string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
