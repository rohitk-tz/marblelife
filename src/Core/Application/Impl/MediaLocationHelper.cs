using System.IO;

namespace Core.Application.Impl
{
    public static class MediaLocationHelper
    {
        public static MediaLocation GetTempMediaLocation()
        {
            const string TempFolderName = "Temp";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }
        public static MediaLocation GetAttachmentMediaLocation() 
        {
            const string TempFolderName = "Attachment";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static MediaLocation GetJobMediaLocation()
        {
            const string TempFolderName = "JobMedia"; 
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static MediaLocation GetFranchiseeDocumentLocation()
        {
            const string FolderName = "Document";
            var location = new MediaLocation(FolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static MediaLocation GetCalendarMediaLocation() 
        {
            const string TempFolderName = "CalendarMedia";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static MediaLocation GetZipMediaLocation()
        {
            const string TempFolderName = "Zip";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }
        public static MediaLocation GetSalesMediaLocation()
        {
            const string TempFolderName = "Sales";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static MediaLocation GetMediaLocationForLogs()
        {
            const string TempFolderName = @"Sales\Logs\";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static string TempPathForExcel()
        {
            return GetSalesMediaLocation().Path + "\\" + System.Guid.NewGuid() + ".xlsx";
        }
        public static MediaLocation GetTempImageLocation()
        {
            const string TempFolderName = "Images";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static string FilePath(string relativeFolderpath, string fileName)
        {
            return relativeFolderpath + "\\" + fileName;
        }

        public static MediaLocation GetDocumentImageLocation()
        {
            const string TempFolderName = "DocumentImages";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }
        public static MediaLocation GetZipImageLocation()
        {
            const string TempFolderName = "ZipBeforeAfter";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }
        public static MediaLocation GetACustomerInvoiceLocation()
        {
            const string TempFolderName = "CustomerInvoice";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static MediaLocation GetInvoiceLocation()
        {
            const string TempFolderName = "InvoiceUpdate";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }

        public static MediaLocation GetThiumbMediaLocation()
        {
            const string TempFolderName = "ThumbImages";
            var location = new MediaLocation(TempFolderName);
            if (!Directory.Exists(location.Path)) Directory.CreateDirectory(location.Path);
            return location;
        }
    }
}
