using System.Linq;
using Core.Notification.Domain;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System.Collections.Generic;
using Core.Application.ViewModel;
using Core.Application.Domain;
using Core.Geo.Domain;
using Core.Sales.Domain;
using Core.Users.Domain;

namespace Core.Scheduler
{
    public interface IJobFactory
    {
        Job CreateDomain(JobEditModel model);
        JobViewModel CreateViewModel(JobScheduler domain,
            List<EstimateInvoice> estimateInvoiceList, List<JobScheduler> schedulerList, List<EstimateInvoiceService> serviceList,
            List<EstimateInvoiceAssignee> estimateInvoiceAssignees, List<EquipmentUserDetails> equipmentUserDetails,
            List<long?> customerSignatures, List<OrganizationRoleUser> organizationRoleUsers);

        JobViewModel CreateViewModelForListView(JobScheduler domain, List<EquipmentUserDetails> equipmentUserDetails);

        JobResourceEditModel CreateResouceModel(JobResource domain);
        JobCustomerEditModel CreateCustomerModel(JobCustomer domain);
        JobSchedulerEditModel CreateJobShedulerModel(JobScheduler domain, List<EstimateInvoiceAssignee> estimateInvoiceAssignees);
        JobCustomer CreateDomain(JobCustomerEditModel model);
        JobEstimate CreateDomain(JobEstimateEditModel model);
        JobEstimateEditModel CreateEstimateModel(JobEstimate domain, JobScheduler scheduler);
        JobScheduler CreateDomain(JobSchedulerEditModel model);
        JobEditModel CreateEditModel(Job job, JobScheduler jobScheduler, List<EstimateInvoiceAssignee> estimateInvoiceAssignees = null);
        JobResource CreateDomain(FileUploadModel model, long fileId);
        JobNote CreateDomain(SchedulerNoteModel model);
        JobViewModel CreateViewModel(Holiday domain, Franchisee franchisee);
        JobEstimateEditModel CreateVacationModel(JobScheduler domain);
        JobScheduler CreateSchedulerDomain(JobEstimateEditModel model);
        JobOccurenceEditModel CreateEditModel(JobScheduler domain);
        JobScheduler CreateDomain(JobOccurenceEditModel model);
        JobSchedulerEditModel CreateModel(JobEditModel model);
        JobScheduler CreateDomain(VacationRepeatEditModel model);
        Meeting CreatMeetingModel(JobEstimateEditModel domain);
        Meeting CreatMeetingModel(VacationRepeatEditModel domain);
        JobScheduler CreateMeetingDomain(JobEstimateEditModel model);
        JobScheduler CreateRepearMeetingDomain(VacationRepeatEditModel model);
        MeetingEditModel CreateMeetingModel(JobScheduler domain);

        JobScheduler CreateMeetingDomainForDeleting(JobEstimateEditModel model);
        JobScheduler CreateDomainForEstimate(JobOccurenceEditModel model);
        //EmailViewModel CreateMeetingDomainForMail(EmailTemplate template, List<EmailTemplate> templateList, List<long> modelList);
        EmailViewModel CreateMeetingDomainForMail(EmailTemplate template, List<EmailTemplate> templateList);
        JobEstimate CreateDomainOccurance(JobOccurenceEditModel model, JobEstimate jobEstimate);
        JobEstimateCategoryViewModel CreatePairingModel(JobEstimateImageCategory domain, JobEstimateCategoryViewModel imageParentChild);

        JobEstimateImageCategory CreateJobEstimateCategory(JobEstimateCategoryViewModel model);
        JobEstimateServices CreateJobEstimatePairing(JobEstimateServiceViewModel model, long categoryId, long? typeId);
        JobEstimateImageCategory CreateJobEstimateCategory(JobEstimateImageCategory domain, JobEstimateCategoryViewModel model);
        JobEstimateServices CreateJobEstimatePairing(JobEstimateServiceViewModel model, long categoryId, JobEstimateServices domain);
        JobEstimateServiceViewModel CreatePairingModel(JobEstimateServices domain);
        JobEstimateImage CreateJobEstimateImageModel(JobEstimateServiceViewModel model, long serviceId, long? typeId);
        JobEstimateServiceViewModel CreateImageModel(JobEstimateImage domain);
        FileModel CreateFileModel(JobEstimateImage domain);
        JobEstimateImage CreateJobEstimateImageDomain(JobEstimateServiceViewModel model, long categoryId, long? typeId, long? fileId);

        JobEstimateServiceViewModel CreateServiceViewModel(JobEstimateServices domain, IEnumerable<FileModel> files, bool? isFromBefore, long? userId);

        FileModel CreateServiceFileViewModel(File domain, Application.Domain.File thumbNailDomain, long? userId, List<BeforeAfterImages> beforeAfterImages = null);

        JobEstimateImage CreateJobEstimateImageEditDomain(JobEstimateServiceViewModel model, long categoryId, long? typeId, FileModel fileId);
        BeforeAfterImageMailAudit CreateBeforeAfterImageMailDomain(JobScheduler jobSchdeuler, long? notificationQueueId, long? fileId, long? franchiseeId, BeforeAfterImageSendMailViewModel model = null);

        FranchiseInfoModel CreateViewModel(Address domain);
        CustomerInfoModel CreateViewModelForCustomer(Address domain);

        BeforeAfterViewModel CreateBeforeAfterViewModel(JobEstimateImage jobEstimareAfter,
            JobEstimateServices jobEstimateService, List<JobEstimateImage> jobEstimateImageList, List<MarkbeforeAfterImagesHistry> markbeforeAfterImagesHistryList, int index, List<OrganizationRoleUser> orgRoleUser);

        BeforeAfterForImageViewModel CreateNeforeAfterViewModel(JobEstimateServices jobEstimateBeforesCategory,
                    JobEstimateServices jobEstimateAftereCateogy, List<JobEstimateImage> jobEstimateImagess, JobEstimateServices jobEstimateBeforesExteriorImages);

        BeforeAfterForImageViewModel CreateBeforeAfterViewModel(BeforeAfterImages jobEstimateBeforeCategory,
                BeforeAfterImages jobEstimateAfterCateogy, BeforeAfterImages jobEstimateBeforesExteriorImages
            , List<JobEstimateServices> jobEstimateService);

        JobEstimateImageCategory CreateJobEstimateCategory(ShiftJobEstimateViewModel model);
        JobEstimateImage CreateJobEstimateImageDomain(ShiftJobEstimateViewModel model, long categoryId, long? typeId, long? fileId);
    }

}
