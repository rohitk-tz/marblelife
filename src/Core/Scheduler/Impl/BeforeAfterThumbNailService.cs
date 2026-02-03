using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Extensions;
using Core.Application.Impl;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using File = Core.Application.Domain.File;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class BeforeAfterThumbNailService : IBeforeAfterThumbNailService
    {
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private IUnitOfWork _unitOfWork;

        public BeforeAfterThumbNailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _fileRepository = _unitOfWork.Repository<File>();
        }
        public FileViewModel CreateImageThumb(Application.Domain.File file, string css = "")
        {
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
                //css = css == "" ? (file.css == "" || file.css == null) ? "" : file.css : css,
                css = "",
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

    }

    public class FileViewModel
    {
        public File File { get; set; }
        public long? FileId { get; set; }
        public string Base64String { get; set; }
        public string FullFilePath { get; set; }
    }
}
