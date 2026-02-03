using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.AWS;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class S3BucketSync : IS3BucketSync
    {
        private readonly ILogService _logService;
        private readonly IAWSService _aWSService;
        private IUnitOfWork _unitOfWork;
        private ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
        private readonly IRepository<File> _fileRepository;
        public S3BucketSync(ILogService logService, IUnitOfWork unitOfWork, ISettings settings, IAWSService aWSService)
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _aWSService = aWSService;
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _beforeAfterImagesRepository = unitOfWork.Repository<BeforeAfterImages>();
            _fileRepository = unitOfWork.Repository<File>();
        }
        public void S3BucketSyncInEvery2min()
        {
            _logService.Info(string.Format("S3 Bucket Auto Syncing Job Start"));
            CreateThumbForImages();
            //SyncS3Bucket();

            AutoSyncWithBulkUpload();
            _logService.Info(string.Format("S3 Bucket Auto Syncing Job end"));
        }
        public void CreateThumbForImages()
        {
            try
            {
                var beforAfterImagesList = _beforeAfterImagesRepository.Table.Where(x => x.Id > 0 && x.FileId != null && x.ThumbFileId == null && ((x.TypeId == (long)LookupTypes.BeforeWork) || (x.TypeId == (long)LookupTypes.AfterWork) || (x.TypeId == (long)LookupTypes.ExteriorBuilding))).OrderByDescending(x => x.Id).ToList();
                foreach (var image in beforAfterImagesList)
                {
                    try
                    {
                        var thumb = CreateThumb(image);
                        image.ThumbFile = thumb.File;
                        image.ThumbFileId = thumb.FileId;
                        _beforeAfterImagesRepository.Save(image);
                        _unitOfWork.SaveChanges();
                        _logService.Info(string.Format("Thumb Is Created For Id - {0}", image.Id));
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Error In Create Thumb In Job - {0} and {1}", ex, image.Id));
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Error("Error In Create Thumb In Job: ", ex);
            }
        }
        private FileViewModel CreateThumb(BeforeAfterImages images)
        {
            var file = images.File;
            string css = "";

            var destimationNameModified = "";
            var fileName = file.RelativeLocation + "\\" + file.Name;
            var extension = GetExtension(fileName);

            var destimationName = MediaLocationHelper.GetThiumbMediaLocation().Path + "\\" + file.Name;

            //System.IO.File.WriteAllText(destimationName,"textMessage");
            //return default(FileViewModel);
            if (extension != ".png" && extension != ".jpeg" && extension != ".jpg")
            {
                Bitmap srcBmp = new Bitmap(fileName);

                float ratio = srcBmp.Width / srcBmp.Height;

                if (ratio == 0)
                {
                    ratio = 1;
                }

                Image imgOriginal = Image.FromFile(fileName);
                // Finds height and width of original image
                float OriginalHeight = imgOriginal.Height;
                float OriginalWidth = imgOriginal.Width;
                // Finds height and width of resized image
                int ThumbnailWidth = 500;
                int ThumbnailHeight = 500;
                int ThumbnailMax = 500;

                if (OriginalHeight > OriginalWidth)
                {
                    ThumbnailHeight = ThumbnailMax;
                    ThumbnailWidth = (int)((OriginalWidth / OriginalHeight) * (float)ThumbnailMax);
                }
                else
                {
                    ThumbnailWidth = ThumbnailMax;
                    ThumbnailHeight = (int)((OriginalHeight / OriginalWidth) * (float)ThumbnailMax);
                }

                // Create new bitmap that will be used for thumbnail
                srcBmp.Dispose();
                Bitmap ThumbnailBitmap = new Bitmap(ThumbnailWidth, ThumbnailHeight);
                //ThumbnailBitmap.Dispose();
                Graphics ResizedImage = Graphics.FromImage(ThumbnailBitmap);
                // Resized image will have best possible quality
                ResizedImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
                ResizedImage.CompositingQuality = CompositingQuality.HighQuality;
                ResizedImage.SmoothingMode = SmoothingMode.HighQuality;
                ResizedImage.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                // Draw resized image
                ResizedImage.DrawImage(imgOriginal, 0, 0, ThumbnailWidth, ThumbnailHeight);
                ResizedImage.Dispose();
                imgOriginal.Dispose();
                // Save thumbnail to file
                ThumbnailBitmap.Save(destimationName);
                ThumbnailBitmap.Dispose();

            }
            else
            {
                Image image = Image.FromFile(fileName);
                //Image thumb = image.GetThumbnailImage(500, 500, () => false, IntPtr.Zero);
                var thumb = GetThumbnailImage(fileName, 500, 500);
                image.Dispose();
                //thumb.Dispose();
                var destinationNameSplitted = destimationName.Split('.');
                destimationNameModified = destinationNameSplitted[0] + extension;
                var extensionSpliited = extension.Split('.');
                thumb.Save(destimationName);
                thumb.Dispose();

                //thumb.Save(fileName);
            }
            var fileNameSplitted = destimationNameModified.Split('.');
            var fileNameSplittedModified = fileNameSplitted[0] + extension;

            var mainFileNameSplitted = file.Name.Split('.');
            var mailFileNameSplittedModified = mainFileNameSplitted[0] + extension;
            var fileDomain = new File()
            {
                Caption = file.Caption,
                Name = mailFileNameSplittedModified,
                DataRecorderMetaData = new DataRecorderMetaData(),
                IsFileToBeDeleted = false,
                RelativeLocation = MediaLocationHelper.GetThiumbMediaLocation().Path,
                css = css == "" ? (file.css == "" || file.css == null) ? "" : file.css : css,
                Size = (int)new System.IO.FileInfo(destimationName).Length,
                MimeType = GetMimeType(destimationName),
                IsNew = true
            };

            _fileRepository.Save(fileDomain);
            _unitOfWork.SaveChanges();

            var base64String = GetBase64String(fileDomain);
            return new FileViewModel()
            {
                FileId = fileDomain.Id,
                Base64String = base64String,
                File = fileDomain,
                FullFilePath = fileDomain != null ? fileDomain.RelativeLocation + "\\" + fileDomain.Name : ""
            };
            //return fileDomain;
            //return true;
        }
        private string GetExtension(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return ext;
        }
        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        private string GetBase64String(Application.Domain.File file)
        {

            if (file == null)
            {
                return "";
            }
            try
            {
                var filePath = (file.RelativeLocation + @"\" + file.Name).ToPath();
                byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                base64ImageRepresentation = "data:image/png;base64," + base64ImageRepresentation;
                return base64ImageRepresentation;
            }
            catch (Exception e1)
            {
                return "";
            }
        }
        public class FileViewModel
        {
            public File File { get; set; }
            public long? FileId { get; set; }
            public string Base64String { get; set; }
            public string FullFilePath { get; set; }
        }


        public async void SyncS3Bucket()
        {
            try
            {
                var dateTime = _clock.ToUtc(new DateTime(2022, 01, 01));
                _logService.Info(string.Format("Start UploadBeforeAfterImageswithS3Bucket function for S3 Bucket  - {0}", _clock.UtcNow));
                var isFolderExist = false;
                var filePath = string.Empty;
                var beforeAfterImagesList = _beforeAfterImagesRepository.IncludeMultiple(x => x.File, x => x.ThumbFile, y => y.DataRecorderMetaData).Where(x => ((x.TypeId == ((long?)LookupTypes.BeforeWork)) || (x.TypeId == ((long?)LookupTypes.DuringWork)) || (x.TypeId == ((long?)LookupTypes.AfterWork)) || (x.TypeId == ((long?)LookupTypes.ExteriorBuilding))) && (x.DataRecorderMetaData.DateCreated >= dateTime || x.IsAddToLocalGallery) && x.S3BucketURL == null && x.S3BucketThumbURL == null).OrderByDescending(z => z.Id).ToList();
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

        private Image GetThumbnailImage(string imagePath, int width, int height)
        {
            using (Image originalImage = Image.FromFile(imagePath))
            {
                // Correct the orientation based on the EXIF data
                if (Array.IndexOf(originalImage.PropertyIdList, 0x0112) > -1)
                {
                    var orientation = (int)originalImage.GetPropertyItem(0x0112).Value[0];
                    switch (orientation)
                    {
                        case 1: // No rotation needed
                            break;
                        case 2: // Flip horizontal
                            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3: // Rotate 180
                            originalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4: // Flip vertical
                            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            break;
                        case 5: // Rotate 90 and flip horizontal
                            originalImage.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6: // Rotate 90
                            originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7: // Rotate 270 and flip horizontal
                            originalImage.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 8: // Rotate 270
                            originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }
                }

                // Create thumbnail
                Image thumbnailImage = originalImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
                return thumbnailImage;
            }
        }

        private void AutoSyncWithBulkUpload()
        {
            try
            {
                var beforeAfterImageList = _beforeAfterImagesRepository.Table.Where(x => x.IsImageUpdated == false).ToList();
                if (beforeAfterImageList != null)
                {
                    foreach (var image in beforeAfterImageList)
                    {
                        if (image.FileId != null && image.S3BucketURL != null && image.S3BucketURL != "" && image.S3BucketThumbURL != null && image.S3BucketThumbURL != "")
                        {
                            var thumb = ImproveTheQualityOfThumb(image);
                            if (thumb)
                            {
                                image.IsImageUpdated = true;
                                _beforeAfterImagesRepository.Save(image);
                                _unitOfWork.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Error("Error In AutoSyncWithBulkUpload In Job: ", ex);
            }
        }

        private bool ImproveTheQualityOfThumb(BeforeAfterImages image)
        {
            try
            {
                if (image.FileId != null && image.ThumbFileId != null && image.S3BucketURL != null && image.S3BucketThumbURL != null)
                {
                    string originalImagePath = "";
                    string thumbnailPath = "";
                    var file = image.File;
                    var thumbFile = image.ThumbFile;

                    originalImagePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(file.RelativeLocation, file.Name));
                    thumbnailPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(thumbFile.RelativeLocation, thumbFile.Name));

                    int thumbWidth = 500;
                    int thumbHeight = 500;
                    _logService.Info(string.Format("CreateThumbForImages Started"));
                    if (originalImagePath != null && originalImagePath != "" && originalImagePath != string.Empty && thumbnailPath != null && thumbnailPath != "" && thumbnailPath != string.Empty)
                    {
                        var overrideThumb = OverrideThumb(originalImagePath, thumbnailPath, thumbWidth, thumbHeight);
                        if (overrideThumb == true)
                        {
                            var imageUploadInS3 = UpdateImageThumbInS3Bucket(image);
                        }
                    }
                    _logService.Info(string.Format("CreateThumbForImages Finished"));
                }

                return true;
            }
            catch (Exception ex)
            {
                _logService.Error("Error In ImproveTheQualityOfThumb In Job: ", ex);
                return false;
            }
        }
        private bool OverrideThumb(string originalImagePath, string thumbnailPath, int thumbWidth, int thumbHeight)
        {
            try
            {
                // Load the original image
                using (Image originalImage = Image.FromFile(originalImagePath))
                {
                    // Fix the orientation based on EXIF data if necessary
                    RotateImageIfRequired(originalImage);

                    // Calculate the resized dimensions while maintaining the aspect ratio
                    int originalWidth = originalImage.Width;
                    int originalHeight = originalImage.Height;
                    float ratioX = (float)thumbWidth / originalWidth;
                    float ratioY = (float)thumbHeight / originalHeight;
                    float ratio = Math.Max(ratioX, ratioY); // Make sure it covers the whole thumbnail area

                    int newWidth = (int)(originalWidth * ratio);
                    int newHeight = (int)(originalHeight * ratio);

                    // Create a new bitmap with the fixed thumbnail size (500x500)
                    using (Bitmap thumbnailBitmap = new Bitmap(thumbWidth, thumbHeight))
                    {
                        using (Graphics g = Graphics.FromImage(thumbnailBitmap))
                        {
                            // Set background color (white)
                            g.Clear(Color.White);

                            // Set high-quality settings
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            // Calculate position to center the resized image on the canvas
                            int posX = (thumbWidth - newWidth) / 2;
                            int posY = (thumbHeight - newHeight) / 2;

                            // Draw the resized image onto the fixed-size canvas
                            g.DrawImage(originalImage, posX, posY, newWidth, newHeight);

                            // Save the thumbnail to the specified path
                            thumbnailBitmap.Save(thumbnailPath, ImageFormat.Jpeg);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error in CreateThumb function:" + ex));
                return false;
            }
        }
        private static void RotateImageIfRequired(Image img)
        {
            const int ExifOrientationId = 0x112; // Property tag for EXIF orientation
            if (Array.IndexOf(img.PropertyIdList, ExifOrientationId) > -1)
            {
                var prop = img.GetPropertyItem(ExifOrientationId);
                int orientationValue = BitConverter.ToUInt16(prop.Value, 0);

                RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;

                switch (orientationValue)
                {
                    case 3:
                        rotateFlipType = RotateFlipType.Rotate180FlipNone;
                        break;
                    case 6:
                        rotateFlipType = RotateFlipType.Rotate90FlipNone;
                        break;
                    case 8:
                        rotateFlipType = RotateFlipType.Rotate270FlipNone;
                        break;
                }

                if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
                {
                    img.RotateFlip(rotateFlipType);
                }
            }
        }
        public bool UpdateImageThumbInS3Bucket(BeforeAfterImages image)
        {
            try
            {
                if (image.ThumbFileId != null && !string.IsNullOrEmpty(image.S3BucketThumbURL))
                {
                    //string thumbnailPath = @"D:\Projects\makalu\docs\Media\ThumbImages\Epping Forest Mansio_2024-10-25_10-23-35.jpg";
                    string thumbnailPath = image.ThumbFile != null ? image.ThumbFile.RelativeLocation + "\\" + image.ThumbFile.Name : "";

                    // Call the method synchronously
                    bool isUpload = ReplaceImageInS3(image.S3BucketThumbURL, thumbnailPath);
                    if (isUpload)
                    {
                        // Append timestamp to URL to bypass any potential caching
                        Console.WriteLine($"Updated S3 URL: {image.S3BucketThumbURL}?timestamp={DateTime.UtcNow.Ticks}");
                    }
                    return isUpload;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in UpdateImageInS3Bucket: " + ex.Message);
                return false;
            }
        }
        public bool ReplaceImageInS3(string bucketUrl, string localImagePath)
        {
            string awsAccessKeyId = _settings.AWSAccessKey;
            string awsSecretAccessKey = _settings.AWSSecreatKey;

            var parsedResult = ParseS3Url(bucketUrl);
            string bucketName = parsedResult.Item1;
            string objectKey = parsedResult.Item2;

            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(objectKey))
            {
                Console.WriteLine("Invalid S3 URL or path parsing issue");
                return false;
            }

            try
            {
                using (var s3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.USEast1))
                {
                    try
                    {
                        var putRequest = new PutObjectRequest
                        {
                            BucketName = bucketName,
                            Key = objectKey,
                            FilePath = localImagePath,
                            ContentType = "image/jpeg"
                        };

                        Console.WriteLine($"Uploading local image {localImagePath} to S3 bucket {bucketName} at key {objectKey}");
                        PutObjectResponse response = s3Client.PutObject(putRequest); // Synchronous call

                        if (response != null && response.HttpStatusCode == HttpStatusCode.OK)
                        {
                            Console.WriteLine("Image successfully updated in S3 with status code: " + response.HttpStatusCode);
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Failed to update image in S3. Response status code: " + response.HttpStatusCode);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("General Exception during S3 PutObject: " + ex.Message);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ReplaceImageInS3: " + ex.Message);
                return false;
            }
        }
        private static Tuple<string, string> ParseS3Url(string s3Url)
        {
            Uri uri = new Uri(s3Url);
            string host = uri.Host;

            string[] hostParts = host.Split('.');
            if (hostParts.Length < 3)
            {
                Console.WriteLine("Error parsing bucket name from host");
                return new Tuple<string, string>(null, null);
            }

            string bucketName = hostParts[0];
            string objectKey = uri.AbsolutePath.TrimStart('/');
            Console.WriteLine("Parsed Bucket Name: " + bucketName);
            Console.WriteLine("Parsed Object Key: " + objectKey);

            return new Tuple<string, string>(bucketName, objectKey);
        }
    }
}
