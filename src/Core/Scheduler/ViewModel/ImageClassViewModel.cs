using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{

    [NoValidatorRequired]
    public class JobEstimateServiceViewModel : EditModelBase
    {
        public JobEstimateServiceViewModel()
        {
            ImagesInfo = new List<FileModel>();
            FilesList = new List<long?>();
        }
        public int OriginalImageIndex { get; set; }
        public int SelectedIndex { get; set; }
        public int ImageIndex { get; set; }
        public bool IsSwapped { get; set; }
        public long? OriginalId { get; set; }
        public long? Id { get; set; }
        public bool IsGroutilife { get; set; }
        public string SurfaceMaterial { get; set; }
        public string SurfaceType { get; set; }
        public string SurfaceColor { get; set; }
        public string FinishMaterial { get; set; }
        public string CreatedBy { get; set; }
        public string EmailId { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public long? TypeId { get; set; }
        public long? FileId { get; set; }
        public long? ThumbFileId { get; set; }
        public List<long?> FilesList { get; set; }
        public long? CategoryId { get; set; }
        public string BuildingLocation { get; set; }
        public long? RowId { get; set; }
        public long? ServiceTypeId { get; set; }
        public string Text { get; set; }
        public long? PairId { get; set; }
        public bool IsBeforeImage { get; set; }
        public string CompamyName { get; set; }
        public string MaidService { get; set; }
        public string PropertyManager { get; set; }
        public string MaidJanitorial { get; set; }
        public List<FileModel> ImagesInfo { get; set; }
        public bool isDisable { get; set; }
        public string Designation { get; set; }
        public string FileSize { get; set; }
        public int FloorNumber { get; set; }

        public bool IsFromEstimate { get; set; }
        public bool IsFromCurrentScheduler { get; set; }
        public string Css { get; set; }
        public List<long> InvoiceNumber { get; set; }
        public List<FileMappedToInvoice> FileMapped { get; set; }
        public bool ImageSavedByUser { get; set; }
        public bool IsBestPairMarkedImage { get; set; }
        public bool IsAddTpLocalGalleryImage { get; set; }
    }

    public class FileMappedToInvoice{

        public long File { get; set; }
        public long InvoiceNumber { get; set; }
    }

}
