using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.AWS;
using Core.Reports.ViewModel;
using Core.Scheduler.Domain;
using System;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class BeforeAfterImagesUploadwithS3Bucket : IBeforeAfterImagesUploadwithS3Bucket
    {
        private readonly ILogService _logService;
        private readonly IAWSService _aWSService;
        private IUnitOfWork _unitOfWork;
        private ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
        private readonly IRepository<File> _fileRepository;
        public BeforeAfterImagesUploadwithS3Bucket(ILogService logService, IUnitOfWork unitOfWork, ISettings settings, IAWSService aWSService)
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _aWSService = aWSService;
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _beforeAfterImagesRepository = unitOfWork.Repository<BeforeAfterImages>();
            _fileRepository = unitOfWork.Repository<File>();
        }
        public async void UploadBeforeAfterImageswithS3Bucket()
        {
            try
            {
                var dateTime = _clock.ToUtc(new DateTime(2022, 01, 01));
                _logService.Info(string.Format("Start UploadBeforeAfterImageswithS3Bucket function for S3 Bucket  - {0}", _clock.UtcNow));
                var isFolderExist = false;
                var filePath = string.Empty;
                var beforeAfterImagesList = _beforeAfterImagesRepository.IncludeMultiple(x => x.File, x => x.ThumbFile, y => y.DataRecorderMetaData).Where(x => ((x.TypeId == ((long?)LookupTypes.BeforeWork)) || (x.TypeId == ((long?)LookupTypes.DuringWork)) || (x.TypeId == ((long?)LookupTypes.AfterWork)) || (x.TypeId == ((long?)LookupTypes.ExteriorBuilding))) && (x.DataRecorderMetaData.DateCreated >= dateTime || x.IsAddToLocalGallery) && x.S3BucketURL == null && x.S3BucketThumbURL == null && x.FileId != null && x.ThumbFileId != null).OrderByDescending(z => z.Id).ToList();
                _logService.Info(string.Format("Get BeforeAfterImages Data for S3 Bucket  - {0}", _clock.UtcNow));
                foreach (var image in beforeAfterImagesList)
                {
                    try
                    {
                        if (image.FileId == null)
                        {
                            continue;
                        }
                        filePath = image.File != null ? image.File.RelativeLocation + "\\" + image.File.Name : "";
                        if (image.JobId != null)
                        {
                            isFolderExist = await _aWSService.DoesFolderexists("Job_" + image.JobId.ToString());
                            if (!isFolderExist)
                            {
                                var isFolderCreated = await _aWSService.CreateSubFolderAsync("Job_" + image.JobId.ToString());
                            }
                            var isFileSave = await _aWSService.UploadPartRequestFileInSubFolder("Job_" + image.JobId.ToString(), image.FileId.ToString(), filePath);
                            _logService.Info(string.Format("Upload File In Subfolder For Job for S3 Bucket  - {0}", _clock.UtcNow));
                            if (isFileSave)
                            {
                                filePath = image.ThumbFile != null ? image.ThumbFile.RelativeLocation + "\\" + image.ThumbFile.Name : "";
                                string path = "Job_" + image.JobId.ToString() + "/" + image.FileId.ToString();
                                string aWSBucketURL = _settings.AWSBucketURL + path;
                                image.S3BucketURL = aWSBucketURL;
                                if (image.ThumbFileId != null)
                                {
                                    await _aWSService.UploadPartRequestFileInSubFolder("Job_" + image.JobId.ToString(), image.ThumbFileId.ToString() + "_Thumb", filePath);
                                    path = "Job_" + image.JobId.ToString() + "/" + image.ThumbFileId.ToString() + "_Thumb";
                                    string aWSBucketThumbURL = _settings.AWSBucketURL + path;
                                    image.S3BucketThumbURL = aWSBucketThumbURL;
                                }
                            }
                        }
                        else
                        {
                            if (image.FileId == null)
                            {
                                continue;
                            }
                            isFolderExist = await _aWSService.DoesFolderexists("Estimate_" + image.EstimateId.ToString());
                            if (!isFolderExist)
                            {
                                var isFolderCreated = await _aWSService.CreateSubFolderAsync("Estimate_" + image.EstimateId.ToString());
                            }
                            var isFileSave = await _aWSService.UploadPartRequestFileInSubFolder("Estimate_" + image.EstimateId.ToString(), image.FileId.ToString(), filePath);
                            _logService.Info(string.Format("Upload File In Subfolder For Estimate for S3 Bucket  - {0}", _clock.UtcNow));
                            if (isFileSave)
                            {
                                filePath = image.ThumbFile != null ? image.ThumbFile.RelativeLocation + "\\" + image.ThumbFile.Name : "";
                                string path = "Estimate_" + image.EstimateId.ToString() + "/" + image.FileId.ToString();
                                string aWSBucketURL = _settings.AWSBucketURL + path;
                                image.S3BucketURL = aWSBucketURL;
                                if (image.ThumbFileId != null)
                                {
                                    await _aWSService.UploadPartRequestFileInSubFolder("Estimate_" + image.EstimateId.ToString(), image.ThumbFileId.ToString() + "_Thumb", filePath);
                                    path = "Estimate_" + image.EstimateId.ToString() + "/" + image.ThumbFileId.ToString() + "_Thumb";
                                    string aWSBucketThumbURL = _settings.AWSBucketURL + path;
                                    image.S3BucketThumbURL = aWSBucketThumbURL;
                                }
                            }
                        }
                        _beforeAfterImagesRepository.Save(image);
                        _unitOfWork.SaveChanges();
                        _logService.Info(string.Format("Save The URL for S3 Bucket  - {0}", _clock.UtcNow));
                    }
                    catch (Exception ex)
                    {
                        _logService.Error("Error In S3 Bucket File: ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Error("Error Message: ", ex);
            }
        }


    }
}