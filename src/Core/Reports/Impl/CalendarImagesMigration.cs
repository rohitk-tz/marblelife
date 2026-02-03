using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Core.Application;
using Core.Application.Attribute;
using Core.Application.Enum;
using Core.AWS;
using Core.Scheduler.Domain;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using Core.Application.Impl;
using Core.Application.Extensions;
using System.Collections.Generic;
using Core.MarketingLead.Domain;
using Newtonsoft.Json;
using Core.Sales.Domain;
using Core.Organizations.Domain;
using System.Data;
using Core.Application.ViewModel;
using Core.Sales;
using Core.Sales.Enum;
using Core.Geo;
using Core.Billing.Domain;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class CalendarImagesMigration : ICalendarImagesMigration
    {
        private readonly ILogService _logService;
        private readonly IAWSService _aWSService;
        private IUnitOfWork _unitOfWork;
        private ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadCallDetailRepository;
        private readonly IRepository<MarketingLeadCallDetailV2> _marketingLeadCallDetailV2Repository;
        private readonly IRepository<ZipCode> _zipCodeRepository;
        private readonly IRepository<County> _countyRepository;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<InvoicePayment> _invoicePaymentRepository;
        private readonly IRepository<PaymentItem> _paymentItemRepository;

        public CalendarImagesMigration(
            ILogService logService,
            IUnitOfWork unitOfWork, ISettings settings,
            IAWSService aWSService
            )
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _aWSService = aWSService;
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _beforeAfterImagesRepository = unitOfWork.Repository<BeforeAfterImages>();
            _fileRepository = unitOfWork.Repository<Application.Domain.File>();
            _marketingLeadCallDetailRepository = unitOfWork.Repository<MarketingLeadCallDetail>();
            _marketingLeadCallDetailV2Repository = unitOfWork.Repository<MarketingLeadCallDetailV2>();
            _zipCodeRepository = unitOfWork.Repository<ZipCode>();
            _countyRepository = unitOfWork.Repository<County>();
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _invoicePaymentRepository = unitOfWork.Repository<InvoicePayment>();
            _paymentItemRepository = unitOfWork.Repository<PaymentItem>();
        }
        public void CalendarImagesMigrationToNewApplication()
        {
            _logService.Info(string.Format("Calender Images Migration Job Starting"));
            //var updatedIds = UpdateSalesDataClassAndServices();
            //var idList = UpdateS3BucketURL();
            //ImagesMigration();
            //ThumbUpload();
            //OverrideThumbAndUpload(idList);

            //UpdateIMaxCallRecords();

            _logService.Info(string.Format("Calender Images Migration Job End"));
            //var d = updatedIds.Distinct().ToList();
            //_logService.Info(string.Format("Total Records Updated for Service Type for Year 2025:" + d));
        }
        private List<long> UpdateSalesDataClassAndServices()
        {
            var ManageUploadIds = new List<long>();
            try
            {
                DateTime year = new DateTime(2025, 1, 1);
                var allSalesData = _salesDataUploadRepository.Table.Where(x => x.PeriodStartDate >= year && x.StatusId == (long)SalesDataUploadStatus.Parsed).OrderByDescending(x => x.Id).ToList();

                foreach (var salesData in allSalesData)
                {
                    try
                    {
                        var excelData = GetExcelData(salesData);
                        if(excelData != null)
                        {
                            var getDBDta = GetDBDta(excelData, salesData, ManageUploadIds);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Error(string.Format("Error in internal UpdateSalesDataClassAndServices function1:" + ex));
                        return ManageUploadIds;
                    }
                }
                return ManageUploadIds;
            }
            catch(Exception ex)
            {
                _logService.Error(string.Format("Error in internal UpdateSalesDataClassAndServices function2:" + ex));
                return ManageUploadIds;
            }
        }

        private IList<ParsedFileParentModel> GetExcelData(SalesDataUpload salesData)
        {
            IList<ParsedFileParentModel> collection = new List<ParsedFileParentModel>();
            try
            {
                var salesDataUpload = GetFileToParse(salesData.Id);
                if (salesDataUpload == null)
                {
                    return collection;
                }

                DataTable data;
                
                try
                {
                    var filePath = MediaLocationHelper.FilePath(salesDataUpload.File.RelativeLocation, salesDataUpload.File.Name).ToFullPath();
                    data = ExcelFileParser.ReadExcel(filePath);
                    var salesDataFileParser = ApplicationManager.DependencyInjection.Resolve<ISalesDataFileParser>();
                    salesDataFileParser.PrepareHeaderIndex(data);
                    string message;
                    if (!salesDataFileParser.CheckForValidHeader(data, out message))
                    {
                        return collection;
                    }
                    string result;
                    //if (!salesDataFileParser.CheckForValidClassName(data, out result))
                    //{
                    //    return collection;
                    //}
                    collection = salesDataFileParser.PrepareDomainFromDataTable(data);

                    return collection;
                    
                }
                catch (Exception ex)
                {
                    return collection;
                }
            }
            catch (Exception ex)
            {
                return collection;
            }
        }
        private List<long> GetDBDta(IList<ParsedFileParentModel> excelCollection, SalesDataUpload salesData, List<long> ManageUploadIds)
        {
            try
            {
                long serviceIdinExcel = 0;

                var allSalesData = _franchiseeSalesRepository.Table.Where(x => x.SalesDataUploadId == salesData.Id).ToList();
                foreach (var excelItem in excelCollection)
                {
                    try
                    {
                        if(excelItem.Invoice.InvoiceItems.Count > 1 || excelItem.Invoice.Payments.Count > 1)
                        {
                            continue;
                            var hasDifferent = excelItem.Invoice.InvoiceItems.ToList();
                            if (hasDifferent.Select(x => x.ItemId).Distinct().Count() > 1)
                            {
                                var allSalesQBItem1 = allSalesData.FirstOrDefault(x => x.QbInvoiceNumber == excelItem.QbIdentifier);
                                if(allSalesQBItem1 == null)
                                {
                                    continue;
                                }
                                var invoiceItem1 = _invoiceItemRepository.Table.Where(x => x.InvoiceId == allSalesQBItem1.InvoiceId).ToList();
                                if(invoiceItem1 != null)
                                {
                                    var excelItemList = excelItem.Invoice.InvoiceItems.ToList();
                                    foreach(var eItem in excelItemList)
                                    {
                                        var dItem = invoiceItem1.Where(x => x.Amount == eItem.Amount && x.Rate == eItem.Rate && x.Description == eItem.Description).ToList();
                                        if(dItem != null && dItem.Count > 0)
                                        {
                                            if(dItem.Count > 1)
                                            {
                                                foreach(var entry in dItem)
                                                {
                                                    if(eItem.ItemId != -1 && entry.ItemId != eItem.ItemId)
                                                    {
                                                        //Update InvoiceItem
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if(eItem.ItemId != -1 && dItem[0].ItemId != eItem.ItemId)
                                                {
                                                    //Update InvoiceItem
                                                }
                                            }
                                        }
                                    }
                                }
                                continue;
                            }
                        }
                        if(excelItem.ServiceTypeId == -1)
                        {
                            continue;
                        }
                        serviceIdinExcel = excelItem.ServiceTypeId;
                        var allSalesQBItem = allSalesData.FirstOrDefault(x => x.QbInvoiceNumber == excelItem.QbIdentifier);

                        if(allSalesQBItem == null)
                        {
                            continue;
                        }
                        //Service Type in Invoice Detail Invoice Item
                        var invoiceItem = _invoiceItemRepository.Table.Where(x => x.InvoiceId == allSalesQBItem.InvoiceId).ToList();

                        var invoicePaymentIds = _invoicePaymentRepository.Table.Where(x => x.InvoiceId == allSalesQBItem.InvoiceId).Select(x => x.PaymentId).ToList();
                        
                        //Service Type in Invoice Detail Invoice Item
                        var paymentItem = _paymentItemRepository.Table.Where(x => invoicePaymentIds.Contains(x.PaymentId)).ToList();

                        foreach(var invoice in invoiceItem)
                        {
                            if(invoice.ItemId != serviceIdinExcel)
                            {
                                //Update Service For Invoice Item
                                UpdateService("invoiceItem", invoice.Id, serviceIdinExcel, ManageUploadIds, salesData.Id);
                            }
                        }

                        foreach(var payment in paymentItem)
                        {
                            if(payment.ItemId != serviceIdinExcel)
                            {
                                //Update Service For Payment Item
                                UpdateService("paymentItem", payment.Id, serviceIdinExcel, ManageUploadIds, salesData.Id);
                            }
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        _logService.Error(string.Format("Error in internal UpdateSalesDataClassAndServices function3:" + ex));
                        return ManageUploadIds;
                    }
                }
                return ManageUploadIds;
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error in internal UpdateSalesDataClassAndServices function4:" + ex));
                return ManageUploadIds;
            }
        }
        private SalesDataUpload GetFileToParse(long id)
        {
            return _salesDataUploadRepository.Table.Where(x => x.Id == id && x.IsActive).FirstOrDefault();
        }
        
        private bool UpdateService(string table, long tableId, long serviceId, List<long> ManageUploadIds, long salesDataId)
        {
            try
            {
                if (serviceId == -1)
                {
                    return true;
                }
                else if(table == "invoiceItem")
                {
                    var invoiceItem = _invoiceItemRepository.Table.FirstOrDefault(x => x.Id == tableId);
                    invoiceItem.ItemId = serviceId;
                    invoiceItem.IsNew = false;
                    _invoiceItemRepository.Save(invoiceItem);
                    _unitOfWork.SaveChanges();
                    ManageUploadIds.Add(salesDataId);
                }
                else if(table == "paymentItem")
                {
                    var paymentItem = _paymentItemRepository.Table.FirstOrDefault(x => x.Id == tableId);
                    paymentItem.ItemId = serviceId;
                    paymentItem.IsNew = false;
                    _paymentItemRepository.Save(paymentItem);
                    _unitOfWork.SaveChanges();
                    ManageUploadIds.Add(salesDataId);
                }
                return true;
            }
            catch(Exception ex)
            {
                _logService.Error(string.Format("Error in internal UpdateSalesDataClassAndServices function5:" + ex));
                return false;
            }
        }
        


        private List<long> UpdateS3BucketURL()
        {
            List<long> idList = new List<long>();
            try
            {
                var beforeAfterImages = _beforeAfterImagesRepository.IncludeMultiple(x => x.File, x => x.ThumbFile, y => y.DataRecorderMetaData).Where(x => x.Id > 0 && x.FileId != null && x.S3BucketURL.Contains("https://marblelife-qa.s3.amazonaws.com") && ((x.TypeId == ((long?)LookupTypes.BeforeWork)) || (x.TypeId == ((long?)LookupTypes.AfterWork)) || (x.TypeId == ((long?)LookupTypes.DuringWork)) || (x.TypeId == ((long?)LookupTypes.ExteriorBuilding)))).OrderBy(z => z.Id).ToList();
                foreach (var image in beforeAfterImages)
                {
                    try
                    {
                        
                        if (image.FileId != null && image.ThumbFileId != null && image.S3BucketURL != null && image.S3BucketThumbURL != null)
                        {
                            string s3URL = image.S3BucketURL;
                            image.S3BucketURL = s3URL.Replace("marblelife-qa", "marblelife-prod");

                            string s3URLThumb = image.S3BucketThumbURL;
                            image.S3BucketThumbURL = s3URLThumb.Replace("marblelife-qa", "marblelife-prod");
                            image.IsNew = false;

                            _beforeAfterImagesRepository.Save(image);
                            _unitOfWork.SaveChanges();
                            idList.Add(image.Id);
                            _logService.Info(string.Format("UpdateS3BucketURL Finished"));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Error(string.Format("Error in internal UpdateS3BucketURL function:" + ex));
                    }
                }
                _logService.Info(string.Format("UpdateS3BucketURL Job Finished"));
                
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error in UpdateS3BucketURL function:" + ex));
                
            }
            return idList;
        }
        private bool ImagesMigration()
        {
            try
            {
                var beforeAfterImages = _beforeAfterImagesRepository.IncludeMultiple(x => x.File, x => x.ThumbFile, y => y.DataRecorderMetaData).Where(x => x.Id > 0 && x.FileId != null && ((x.TypeId == ((long?)LookupTypes.BeforeWork)) || (x.TypeId == ((long?)LookupTypes.AfterWork)) || (x.TypeId == ((long?)LookupTypes.DuringWork)) || (x.TypeId == ((long?)LookupTypes.ExteriorBuilding)))).OrderBy(z => z.Id).ToList();
                foreach (var image in beforeAfterImages) 
                {
                    try
                    {
                        if(image.FileId != null && image.ThumbFileId != null && image.S3BucketURL == null && image.S3BucketThumbURL == null)
                        {
                            ImageUploadOnS3Bucket(image);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Error(string.Format("Error in CreateThumbForImages function:" + ex));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error in CreateThumbForImages function:" + ex));
                return false;
            }
        }

        private bool ThumbUpload()
        {
            try
            {
                var beforeAfterImages = _beforeAfterImagesRepository.IncludeMultiple(x => x.File, x => x.ThumbFile, y => y.DataRecorderMetaData).Where(x => x.Id > 0 && x.FileId != null && ((x.TypeId == ((long?)LookupTypes.BeforeWork)) || (x.TypeId == ((long?)LookupTypes.AfterWork)) || (x.TypeId == ((long?)LookupTypes.DuringWork)) || (x.TypeId == ((long?)LookupTypes.ExteriorBuilding)))).OrderBy(z => z.Id).ToList();
                foreach (var image in beforeAfterImages)    
                {
                    try
                    {
                        if (image.FileId != null && image.ThumbFileId != null && image.S3BucketURL != null && image.S3BucketThumbURL == null)
                        {
                            UploadThumbImageOnlyInS3(image);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Error(string.Format("Error in CreateThumbForImages function:" + ex));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error in CreateThumbForImages function:" + ex));
                return false;
            }
        }

        private bool OverrideThumbAndUpload(List<long> idList)
        {
            try
            {
                var beforeAfterImages = _beforeAfterImagesRepository.IncludeMultiple(x => x.File, x => x.ThumbFile, y => y.DataRecorderMetaData).Where(x => x.Id > 0 && x.FileId != null && idList.Contains(x.Id) && ((x.TypeId == ((long?)LookupTypes.BeforeWork)) || (x.TypeId == ((long?)LookupTypes.AfterWork)) || (x.TypeId == ((long?)LookupTypes.DuringWork)) || (x.TypeId == ((long?)LookupTypes.ExteriorBuilding)))).OrderBy(z => z.Id).ToList();
                foreach (var image in beforeAfterImages)
                {
                    try
                    {
                        if (image.FileId != null && image.ThumbFileId != null && image.S3BucketURL != null && image.S3BucketThumbURL != null)
                        {
                            string originalImagePath = "";
                            string thumbnailPath = "";
                            var file = image.File;
                            var thumbFile = image.ThumbFile;

                            originalImagePath = System.IO.Path.GetFullPath(Path.Combine(file.RelativeLocation, file.Name));
                            thumbnailPath = System.IO.Path.GetFullPath(Path.Combine(thumbFile.RelativeLocation, thumbFile.Name));

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
                    }
                    catch (Exception ex)
                    {
                        _logService.Error(string.Format("Error in CreateThumbForImages function:" + ex));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error in CreateThumbForImages function:" + ex));
                return false;
            }
        }



        // Condition-1
        //private FileViewModel CreateImageThumbNail(BeforeAfterImages images)
        //{
        //    var file = images.File;
        //    string css = "";

        //    var destimationNameModified = "";
        //    var fileName = file.RelativeLocation + "\\" + file.Name;
        //    var extension = GetExtension(fileName);

        //    var destimationName = MediaLocationHelper.GetThiumbMediaLocation().Path + "\\" + file.Name;

        //    if (extension != ".png" && extension != ".jpeg" && extension != ".jpg")
        //    {
        //        Bitmap srcBmp = new Bitmap(fileName);

        //        float ratio = srcBmp.Width / srcBmp.Height;

        //        if (ratio == 0)
        //        {
        //            ratio = 1;
        //        }

        //        Image imgOriginal = Image.FromFile(fileName);
        //        // Finds height and width of original image
        //        float OriginalHeight = imgOriginal.Height;
        //        float OriginalWidth = imgOriginal.Width;
        //        // Finds height and width of resized image
        //        int ThumbnailWidth = 500;
        //        int ThumbnailHeight = 500;
        //        int ThumbnailMax = 500;

        //        if (OriginalHeight > OriginalWidth)
        //        {
        //            ThumbnailHeight = ThumbnailMax;
        //            ThumbnailWidth = (int)((OriginalWidth / OriginalHeight) * (float)ThumbnailMax);
        //        }
        //        else
        //        {
        //            ThumbnailWidth = ThumbnailMax;
        //            ThumbnailHeight = (int)((OriginalHeight / OriginalWidth) * (float)ThumbnailMax);
        //        }

        //        // Create new bitmap that will be used for thumbnail
        //        srcBmp.Dispose();
        //        Bitmap ThumbnailBitmap = new Bitmap(ThumbnailWidth, ThumbnailHeight);
        //        //ThumbnailBitmap.Dispose();
        //        Graphics ResizedImage = Graphics.FromImage(ThumbnailBitmap);
        //        // Resized image will have best possible quality
        //        ResizedImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        ResizedImage.CompositingQuality = CompositingQuality.HighQuality;
        //        ResizedImage.SmoothingMode = SmoothingMode.HighQuality;
        //        ResizedImage.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        //        // Draw resized image
        //        ResizedImage.DrawImage(imgOriginal, 0, 0, ThumbnailWidth, ThumbnailHeight);
        //        ResizedImage.Dispose();
        //        imgOriginal.Dispose();
        //        // Save thumbnail to file
        //        ThumbnailBitmap.Save(destimationName);
        //        ThumbnailBitmap.Dispose();

        //    }
        //    else
        //    {
        //        Image image = Image.FromFile(fileName);
        //        var thumb = GetThumbnailImage(fileName, 500, 500);
        //        image.Dispose();
        //        //thumb.Dispose();
        //        var destinationNameSplitted = destimationName.Split('.');
        //        destimationNameModified = destinationNameSplitted[0] + extension;
        //        var extensionSpliited = extension.Split('.');
        //        thumb.Save(destimationName);
        //        thumb.Dispose();
        //    }
        //    var fileNameSplitted = destimationNameModified.Split('.');
        //    var fileNameSplittedModified = fileNameSplitted[0] + extension;

        //    var mainFileNameSplitted = file.Name.Split('.');
        //    var mailFileNameSplittedModified = mainFileNameSplitted[0] + extension;
        //    var fileDomain = new Core.Application.Domain.File()
        //    {
        //        Caption = file.Caption,
        //        Name = mailFileNameSplittedModified,
        //        DataRecorderMetaData = new DataRecorderMetaData(),
        //        IsFileToBeDeleted = false,
        //        RelativeLocation = MediaLocationHelper.GetThiumbMediaLocation().Path,
        //        css = css == "" ? (file.css == "" || file.css == null) ? "" : file.css : css,
        //        Size = (int)new System.IO.FileInfo(destimationName).Length,
        //        MimeType = GetMimeType(destimationName),
        //        IsNew = true
        //    };

        //    _fileRepository.Save(fileDomain);
        //    _unitOfWork.SaveChanges();

        //    var base64String = GetBase64String(fileDomain);
        //    return new FileViewModel()
        //    {
        //        FileId = fileDomain.Id,
        //        Base64String = base64String,
        //        File = fileDomain,
        //        FullFilePath = fileDomain != null ? fileDomain.RelativeLocation + "\\" + fileDomain.Name : ""
        //    };
        //}
        //private string GetExtension(string fileName)
        //{
        //    string mimeType = "application/unknown";
        //    string ext = System.IO.Path.GetExtension(fileName).ToLower();
        //    return ext;
        //}
        //private string GetMimeType(string fileName)
        //{
        //    string mimeType = "application/unknown";
        //    string ext = System.IO.Path.GetExtension(fileName).ToLower();
        //    Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
        //    if (regKey != null && regKey.GetValue("Content Type") != null)
        //        mimeType = regKey.GetValue("Content Type").ToString();
        //    return mimeType;
        //}
        //private string GetBase64String(Application.Domain.File file)
        //{
        //    if (file == null)
        //    {
        //        return "";
        //    }
        //    try
        //    {
        //        var filePath = (file.RelativeLocation + @"\" + file.Name).ToPath();
        //        byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
        //        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
        //        base64ImageRepresentation = "data:image/png;base64," + base64ImageRepresentation;
        //        return base64ImageRepresentation;
        //    }
        //    catch (Exception e1)
        //    {
        //        return "";
        //    }
        //}
        //private Image GetThumbnailImage(string imagePath, int width, int height)
        //{
        //    using (Image originalImage = Image.FromFile(imagePath))
        //    {
        //        // Correct the orientation based on the EXIF data
        //        if (Array.IndexOf(originalImage.PropertyIdList, 0x0112) > -1)
        //        {
        //            var orientation = (int)originalImage.GetPropertyItem(0x0112).Value[0];
        //            switch (orientation)
        //            {
        //                case 1: // No rotation needed
        //                    break;
        //                case 2: // Flip horizontal
        //                    originalImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
        //                    break;
        //                case 3: // Rotate 180
        //                    originalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
        //                    break;
        //                case 4: // Flip vertical
        //                    originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
        //                    break;
        //                case 5: // Rotate 90 and flip horizontal
        //                    originalImage.RotateFlip(RotateFlipType.Rotate90FlipX);
        //                    break;
        //                case 6: // Rotate 90
        //                    originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
        //                    break;
        //                case 7: // Rotate 270 and flip horizontal
        //                    originalImage.RotateFlip(RotateFlipType.Rotate270FlipX);
        //                    break;
        //                case 8: // Rotate 270
        //                    originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
        //                    break;
        //            }
        //        }
        //        // Create thumbnail
        //        Image thumbnailImage = originalImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
        //        return thumbnailImage;
        //    }
        //}


        // Condition-2


        private async void ImageUploadOnS3Bucket(BeforeAfterImages image)
        {
            try
            {
                var isFolderExist = false;
                var filePath = string.Empty;
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
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        // Condition-3
        private async void UploadThumbImageOnlyInS3(BeforeAfterImages image)
        {
            try
            {
                var isFolderExist = false;
                var filePath = string.Empty;
                filePath = image.File != null ? image.File.RelativeLocation + "\\" + image.File.Name : "";
                if (image.JobId != null)
                {
                    isFolderExist = await _aWSService.DoesFolderexists("Job_" + image.JobId.ToString());
                    if (!isFolderExist)
                    {
                        var isFolderCreated = await _aWSService.CreateSubFolderAsync("Job_" + image.JobId.ToString());
                    }
                    //var isFileSave = await _aWSService.UploadPartRequestFileInSubFolder("Job_" + image.JobId.ToString(), image.FileId.ToString(), filePath);
                    var isFileSave = image.S3BucketURL != null ? true : false;
                    _logService.Info(string.Format("Upload File In Subfolder For Job for S3 Bucket  - {0}", _clock.UtcNow));
                    if (isFileSave)
                    {
                        filePath = image.ThumbFile != null ? image.ThumbFile.RelativeLocation + "\\" + image.ThumbFile.Name : "";
                        string path = "Job_" + image.JobId.ToString() + "/" + image.FileId.ToString();
                        //string aWSBucketURL = _settings.AWSBucketURL + path;
                        //image.S3BucketURL = aWSBucketURL;
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
                    isFolderExist = await _aWSService.DoesFolderexists("Estimate_" + image.EstimateId.ToString());
                    if (!isFolderExist)
                    {
                        var isFolderCreated = await _aWSService.CreateSubFolderAsync("Estimate_" + image.EstimateId.ToString());
                    }
                    //var isFileSave = await _aWSService.UploadPartRequestFileInSubFolder("Estimate_" + image.EstimateId.ToString(), image.FileId.ToString(), filePath);
                    var isFileSave = image.S3BucketURL != null ? true : false;
                    _logService.Info(string.Format("Upload File In Subfolder For Estimate for S3 Bucket  - {0}", _clock.UtcNow));
                    if (isFileSave)
                    {
                        filePath = image.ThumbFile != null ? image.ThumbFile.RelativeLocation + "\\" + image.ThumbFile.Name : "";
                        string path = "Estimate_" + image.EstimateId.ToString() + "/" + image.FileId.ToString();
                        //string aWSBucketURL = _settings.AWSBucketURL + path;
                        //image.S3BucketURL = aWSBucketURL;
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
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        // Condition-4
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

        public class FileViewModel
        {
            public Core.Application.Domain.File File { get; set; }
            public long? FileId { get; set; }
            public string Base64String { get; set; }
            public string FullFilePath { get; set; }
        }        

        private bool UpdateIMaxCallRecords()
        {
            try
            {
                _logService.Info(string.Format("Update I-Max Records Job Starting"));
                var i_MEX_recordsList = _marketingLeadCallDetailV2Repository.Table.Where(x => x.CallRoute == "I-MEX-CHIHUAHUA").ToList();
                var i_MEX_records = _marketingLeadCallDetailV2Repository.Table.Where(x => x.CallRoute == "I-MEX-CHIHUAHUA").Select(x => x.MarketingLeadCallDetailId).ToList();
                if(i_MEX_records.Count > 0)
                {
                    var marketingleadList1 = _marketingLeadCallDetailRepository.Table.Where(x => i_MEX_records.Contains(x.Id)).ToList();
                    if(marketingleadList1.Count > 0)
                    {
                        foreach(var marketinglead in marketingleadList1)
                        {
                            try
                            {
                                var invocaRecords = GetInvocaRecords(marketinglead.DateAdded, marketinglead.CallerId);

                                if (invocaRecords != null)
                                {
                                    if (invocaRecords.ZipCode != null)
                                    {
                                        var zipCounty = new ZipCode();
                                        zipCounty = _zipCodeRepository.Table.Where(x => x.CountyId != null && x.Zip == invocaRecords.ZipCode).OrderByDescending(x => x.Id).FirstOrDefault();
                                        var county = zipCounty != null && zipCounty != default(ZipCode) ? _countyRepository.Table.Where(x => x.Id == zipCounty.CountyId).FirstOrDefault() : new County();
                                        var route = county != null ? county.FranchiseeName : "";

                                        var marketingLeadCallDetailV2 = i_MEX_recordsList.FirstOrDefault(x => x.MarketingLeadCallDetailId == marketinglead.Id);
                                        marketingLeadCallDetailV2.CallRoute = route != null ? route : "ZIP Not Matched: " + invocaRecords.ZipCode;
                                        _marketingLeadCallDetailV2Repository.Save(marketingLeadCallDetailV2);
                                        _unitOfWork.SaveChanges();
                                    }
                                    else
                                    {
                                        var marketingLeadCallDetailV2 = i_MEX_recordsList.FirstOrDefault(x => x.MarketingLeadCallDetailId == marketinglead.Id);
                                        marketingLeadCallDetailV2.CallRoute = null;
                                        _marketingLeadCallDetailV2Repository.Save(marketingLeadCallDetailV2);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logService.Error(string.Format("Error in UpdateIMaxCallRecords For Called Id: " + marketinglead.CallerId));
                            }
                        }
                    }
                }

                _logService.Info(string.Format("Update I-Max Records Job End"));
                return true;
            }
            catch(Exception ex)
            {
                _logService.Error(string.Format("Error in UpdateIMaxCallRecords function:" + ex));
                return false;
            }
        }

        private CallsDetails GetInvocaRecords(DateTime? dateTime, string callerId)
        {
            CallsDetails callsDetails = new CallsDetails();
            try
            {
                if(dateTime != null)
                {
                    string startAfterId = "5909-8C22E83077D7";
                    var fromDate = dateTime?.AddDays(-1);
                    var toDate = dateTime?.AddDays(1);
                    _logService.Info(string.Format("Getting API data For from Time for Marketing Class for dates - {0} and {1}", fromDate, toDate));
                    var result = GetCallDetailFromAPIInvoca(fromDate.Value.Date, toDate.Value.Date, startAfterId);

                    List<CallRecord> records = JsonConvert.DeserializeObject<List<CallRecord>>(result);
                    if(records != null)
                    {
                        callsDetails.FranchiseeName = records.FirstOrDefault(x => x.calling_phone_number.Replace("-", "") == callerId).office;
                        callsDetails.ZipCode = records.FirstOrDefault(x => x.calling_phone_number.Replace("-", "") == callerId).caller_zip;
                        callsDetails.CallerId = records.FirstOrDefault(x => x.calling_phone_number.Replace("-", "") == callerId).calling_phone_number;
                    }
                }
                return callsDetails;
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error in GetInvocaRecords function:" + ex));
                return callsDetails;
            }
        }

        private string GetCallDetailFromAPIInvoca(DateTime fromDate, DateTime toDate, string startAfterId)
        {
            string fromDateString = fromDate.ToString("yyyy-MM-dd");
            string toDateString = toDate.ToString("yyyy-MM-dd");
            //string url = string.Format("https://marblelife.invoca.net/api/2020-10-01/networks/transactions/2423.json?from={0}&to={1}&oauth_token=AgpaEpKjQKtYpgqyPcwP8LnBkNBVINZ5", fromDateString, toDateString);
            //string url = string.Format("https://marblelife.invoca.net/api/2020-10-01/networks/transactions/2423.json?include_columns=$invoca_custom_columns,$invoca_default_columns&from={0}&to={1}&oauth_token=AgpaEpKjQKtYpgqyPcwP8LnBkNBVINZ5&start_after_transaction_id={2}", fromDateString, toDateString, startAfterId);

            string url = string.Format("https://marblelife.invoca.net/api/2020-10-01/networks/transactions/2423.json?include_columns=$invoca_custom_columns,$invoca_default_columns&from={0}&to={1}&oauth_token=AgpaEpKjQKtYpgqyPcwP8LnBkNBVINZ5&call_in_progress=false", fromDateString, toDateString);
            var result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add("oauth_token", "AgpaEpKjQKtYpgqyPcwP8LnBkNBVINZ5");
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Web Exception :", ex.Message));
                }
            }
            return result;
        }

        private class CallRecord
        {
            public string office { get; set; }
            public string caller_zip { get; set; }
            public string calling_phone_number { get; set; }
        }

        private class CallsDetails
        {
            public string FranchiseeName { get; set; }
            public string ZipCode { get; set; }
            public string CallerId { get; set; }
        }
    }
}
