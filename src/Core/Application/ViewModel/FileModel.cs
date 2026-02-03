using Core.Application.Attribute;
using System;

namespace Core.Application.ViewModel
{
    [NoValidatorRequired]
    public class FileModel : EditModelBase
    {
        public long UserId { get; set; }
        public long? OriginalId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public decimal Size { get; set; }
        public string RelativeLocation { get; set; }
        public string MimeType { get; set; }
        public string Extension { get; set; }
        public string css { get; set; }
        public string ImageUrl { get; set; }
        public string S3BucketImageUrl { get; set; }
        public string Url { get; set; }

        public long? FileId { get; set; }
        public string CreatedBy { get; set; }

        public long? FileReferenceId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsIFrame { get; set; }
        public string IFrameUrl { get; set; }
        public long? UploadByRoleId { get; set; }
        public string ThumbImageUrl { get; set; }
        public string S3BucketThumbImageUrl { get; set; }

        public long? ThumbFileId { get; set; }
        public bool ImageSavedByUser { get; set; }
        public DateTime? CreatedOnForImage { get; set; }
        public string CreatedByForImage { get; set; }
        public long? BeforeAfterId { get; set; }
        public bool? IsImageCropped { get; set; }
        public string CroppedImage { get; set; }
        public string CroppedImageThumb { get; set; }
        public string CroppedImageUrl { get; set; }
        public long? CroppedImageFileId { get; set; }
        public long? CroppedImageThumbFileId { get; set; }
    }
}