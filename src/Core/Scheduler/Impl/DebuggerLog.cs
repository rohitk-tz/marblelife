using Core.Application;
using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class DebuggerLog : IDebuggerLog
    {
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        public DebuggerLog(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _fileRepository = unitOfWork.Repository<Application.Domain.File>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
        }

        public string CreateDebugger(JobEstimateServices jobEstimateServicesFromUI, List<JobEstimateServices> jobEstimateServicesFromDb, string text, out string debuggerLogs)
        {
            if (jobEstimateServicesFromDb == null)
            {
                if (text == "Before")
                {
                    debuggerLogs = "Adding Before Finish-Material-" + jobEstimateServicesFromUI.FinishMaterial + " , Adding Before Surface-Color-" + jobEstimateServicesFromUI.SurfaceColor
                                                              + " ,Adding Before Surface-Material-" + jobEstimateServicesFromUI.SurfaceMaterial + " ,Adding Before Surface-Type-" + jobEstimateServicesFromUI.SurfaceType
                                                              + " ,Adding Before Company-Name-" + jobEstimateServicesFromUI.CompanyName + " ,Adding Before MAIDJANITORIAL-" + jobEstimateServicesFromUI.MAIDJANITORIAL
                                                              + " ,Adding Before Maid-Service-" + jobEstimateServicesFromUI.MaidService + " ,Adding Before Property-Manager-" + jobEstimateServicesFromUI.PropertyManager
                                                              + " ,Adding Before Floor-Number-" + jobEstimateServicesFromUI.FloorNumber;
                }
                else
                {
                    debuggerLogs = "Adding After Finish-Material-" + jobEstimateServicesFromUI.FinishMaterial + " , Adding After Surface-Color-" + jobEstimateServicesFromUI.SurfaceColor
                                                              + " ,Adding After Surface-Material-" + jobEstimateServicesFromUI.SurfaceMaterial + " ,Adding After Surface-Type-" + jobEstimateServicesFromUI.SurfaceType
                                                              + " ,Adding After Company-Name-" + jobEstimateServicesFromUI.CompanyName + " ,Adding After MAIDJANITORIAL-" + jobEstimateServicesFromUI.MAIDJANITORIAL
                                                              + " ,Adding After Maid-Service-" + jobEstimateServicesFromUI.MaidService + " ,Adding After Property-Manager-" + jobEstimateServicesFromUI.PropertyManager
                                                              + " ,Adding After Floor-Number-" + jobEstimateServicesFromUI.FloorNumber;
                }
            }
            else
            {
                debuggerLogs = "";
                var jobEstimateServicesFromUILocal = jobEstimateServicesFromDb.FirstOrDefault(x => x.Id == jobEstimateServicesFromUI.Id);
                if (jobEstimateServicesFromUILocal != null)
                    debuggerLogs = GettingChangesInServices(jobEstimateServicesFromUI, jobEstimateServicesFromUILocal, text);
                else
                {
                    if (text == "Before")
                    {
                        debuggerLogs = "Adding Before Finish-Material-" + jobEstimateServicesFromUI.FinishMaterial + " , Adding Before Surface-Color-" + jobEstimateServicesFromUI.SurfaceColor
                                                                  + " ,Adding Before Surface-Material-" + jobEstimateServicesFromUI.SurfaceMaterial + " ,Adding Before Surface-Type-" + jobEstimateServicesFromUI.SurfaceType
                                                                  + " ,Adding Before Company-Name-" + jobEstimateServicesFromUI.CompanyName + " ,Adding Before MAIDJANITORIAL-" + jobEstimateServicesFromUI.MAIDJANITORIAL
                                                                  + " ,Adding Before Maid-Service-" + jobEstimateServicesFromUI.MaidService + " ,Adding Before Property-Manager-" + jobEstimateServicesFromUI.PropertyManager
                                                                  + " ,Adding Before Floor-Number-" + jobEstimateServicesFromUI.FloorNumber;
                    }
                    else
                    {
                        debuggerLogs = "Adding After Finish-Material-" + jobEstimateServicesFromUI.FinishMaterial + " , Adding After Surface-Color-" + jobEstimateServicesFromUI.SurfaceColor
                                                                  + " ,Adding After Surface-Material-" + jobEstimateServicesFromUI.SurfaceMaterial + " ,Adding After Surface-Type-" + jobEstimateServicesFromUI.SurfaceType
                                                                  + " ,Adding After Company-Name-" + jobEstimateServicesFromUI.CompanyName + " ,Adding After MAIDJANITORIAL-" + jobEstimateServicesFromUI.MAIDJANITORIAL
                                                                  + " ,Adding After Maid-Service-" + jobEstimateServicesFromUI.MaidService + " ,Adding After Property-Manager-" + jobEstimateServicesFromUI.PropertyManager
                                                                  + " ,Adding After Floor-Number-" + jobEstimateServicesFromUI.FloorNumber;
                    }
                }
            }
            return debuggerLogs;
        }

        public string CreateDebuggerForImage(JobEstimateImage jobEstimateServicesImageFromUI, List<JobEstimateImage> jobEstimateServicesFromDb, string text, out string debuggerLogs)
        {
            debuggerLogs = "";
            if (jobEstimateServicesFromDb == null || jobEstimateServicesFromDb.Count() == 0)
            {
                if (text == "Before")
                {
                    var fileDomain = _fileRepository.Get(jobEstimateServicesImageFromUI.FileId.Value);
                    debuggerLogs = "Adding Before ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                }
                else
                {
                    var fileDomain = _fileRepository.Get(jobEstimateServicesImageFromUI.FileId.Value);
                    debuggerLogs = "Adding After ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                }


            }
            else
            {
                var jobEstimateServicesImageFromLocal = jobEstimateServicesFromDb.FirstOrDefault(x => x.Id == jobEstimateServicesImageFromUI.Id);
                var fileDomain = _fileRepository.Get(jobEstimateServicesImageFromUI.FileId.Value);
                if (jobEstimateServicesImageFromLocal != null)
                {
                    if (jobEstimateServicesImageFromLocal.FileId != jobEstimateServicesImageFromUI.FileId)
                    {

                        if (text == "Before")
                        {
                            debuggerLogs = "Adding Before ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                        else
                        {
                            debuggerLogs = "Adding After ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                    }
                }
                else
                {
                    if (jobEstimateServicesImageFromUI.FileId != null)
                    {
                        if (text == "Before")
                        {
                            debuggerLogs = "Adding Before ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                        else
                        {
                            debuggerLogs = "Adding After ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                    }
                }
                if (jobEstimateServicesImageFromLocal != null)
                {


                    if (jobEstimateServicesImageFromLocal.IsBestImage != jobEstimateServicesImageFromUI.IsBestImage)
                    {
                        if (jobEstimateServicesImageFromUI.IsBestImage)
                        {
                            debuggerLogs += text + " Image Added as Best Image: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                        else
                        {
                            debuggerLogs += text + " Image Removed from Best Image: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                    }

                    if (jobEstimateServicesImageFromLocal.IsAddToLocalGallery != jobEstimateServicesImageFromUI.IsAddToLocalGallery)
                    {
                        if (jobEstimateServicesImageFromUI.IsAddToLocalGallery)
                        {
                            debuggerLogs += text + " Image Added  in Local Gallery: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                        else
                        {
                            debuggerLogs += text + " Image Removed  from Local Gallery: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                    }
                }
                else
                {
                        if (jobEstimateServicesImageFromUI.IsBestImage)
                        {
                            debuggerLogs += text + " Image Added as Best Image: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                        else
                        {
                            debuggerLogs += text + " Image Removed from Best Image: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }

                        if (jobEstimateServicesImageFromUI.IsAddToLocalGallery)
                        {
                            debuggerLogs += text + " Image Added  in Local Gallery: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                        else
                        {
                            debuggerLogs += text + " Image Removed  from Local Gallery: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                        }
                }
            }
            return debuggerLogs;
        }

        private string GettingChangesInServices(JobEstimateServices fromUi, JobEstimateServices fromDb, string text)
        {
            var description = "";
            if (fromUi.BuildingLocation != fromDb.BuildingLocation)
            {
                description += text + "Changes in Building Location from " + fromDb.BuildingLocation + " to " + fromUi.BuildingLocation;
            }

            if (fromUi.CompanyName != fromDb.CompanyName)
            {
                description += text + "Changes in Company Name from " + fromDb.CompanyName + " to " + fromUi.CompanyName;
            }

            if (fromUi.FinishMaterial != fromDb.FinishMaterial)
            {
                description += text + "Changes in Finsish Material from " + fromDb.FinishMaterial + " to " + fromUi.FinishMaterial;
            }

            if (fromUi.FloorNumber != fromDb.FloorNumber)
            {
                description += text + "Changes in Floor Number from " + fromDb.FloorNumber + " to " + fromUi.FloorNumber;
            }

            if (fromUi.MAIDJANITORIAL != fromDb.MAIDJANITORIAL)
            {
                description += text + "Changes in Maid Janitorial Number from " + fromDb.MAIDJANITORIAL + " to " + fromUi.MAIDJANITORIAL;
            }

            if (fromUi.MaidService != fromDb.MaidService)
            {
                description += text + "Changes in Maid Service Number from " + fromDb.MaidService + " to " + fromUi.MaidService;
            }

            if (fromUi.PropertyManager != fromDb.PropertyManager)
            {
                description += text + "Changes in Property Manager Number from " + fromDb.PropertyManager + " to " + fromUi.PropertyManager;
            }

            if (fromUi.SurfaceColor != fromDb.SurfaceColor)
            {
                description += text + "Changes in Surface Color Number from " + fromDb.SurfaceColor + " to " + fromUi.SurfaceColor;
            }

            if (fromUi.SurfaceMaterial != fromDb.SurfaceMaterial)
            {
                description += text + "Changes in Surface Material Number from " + fromDb.SurfaceMaterial + " to " + fromUi.SurfaceMaterial;
            }

            if (fromUi.SurfaceType != fromDb.SurfaceType)
            {
                description += text + "Changes in Surface Type  Number from " + fromDb.SurfaceType + " to " + fromUi.SurfaceType;
            }
            if (fromUi.ServiceTypeId != fromDb.ServiceTypeId)
            {
                var previousService = _serviceTypeRepository.Get(fromDb.ServiceTypeId.Value);
                var newService = _serviceTypeRepository.Get(fromUi.ServiceTypeId.Value);
                description += text + "Changes in Service Type  Number from " + previousService.Name + " to " + newService.Name;
            }
            return description;
        }
    }
}
