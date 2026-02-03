using Core.Application.Attribute;
using Core.Notification.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Impl
{
    [DefaultImplementation]
    public class PdfFileService : IPdfFileService
    {
        public WkHtmltoPdfSwitches Switches { get; set; }
        public long? jobschedulerId;
        private Process _process;
        private string _sourcePath;
        private string _destinationPath;
        private string _fileName;
        private readonly ISettings _settings;
        public PdfFileService(ISettings settings)
        {
            _settings = settings;
        }
        public string GeneratePdfFromTemplateAndModel(object model, string destinationFolder, string generatedFileName, 
            string templateFilePath)
        {
            {
                var fileName = generatedFileName;
                var viewPath = templateFilePath;
                var viewText = System.IO.File.ReadAllText(viewPath);
                var view = NotificationServiceHelper.FormatContent(viewText, model);
                var pdfConverterPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\bin");

                if(pdfConverterPath==null)
                {
                    pdfConverterPath = _settings.TemplateRootPathBin;
                }
                var wkHtmltoPdfSwitches = new WkHtmltoPdfSwitches
                {
                    MarginTop = 5,
                    MarginBottom = 5,
                    MarginLeft = 5,
                    MarginRight = 5,
                    PageHeight = 500,
                    PageWidth = 200,
                    PageSize = "A4",
                    Orientation = "Portrait",
                    // FooterUrl = footerUrl
                };

                SetDefaultSwitch(wkHtmltoPdfSwitches);

                var filePath = Generate(new StringBuilder(view), destinationFolder, pdfConverterPath, fileName);
                return !string.IsNullOrEmpty(filePath) ? filePath : null;
            }
        }

        private void SetDefaultSwitch(WkHtmltoPdfSwitches defaultSwitches)
        {
            Switches = new WkHtmltoPdfSwitches();
            Switches = defaultSwitches;
        }

        private string Generate(StringBuilder htmlStream, string destinationPath, string pdfConverterPath = "", string fileName = "")
        {
            _process = new Process();
            _destinationPath = destinationPath;

            if (!Directory.Exists(_destinationPath))
                Directory.CreateDirectory(_destinationPath);

            _fileName = string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid() + ".pdf" : fileName;
            var logger = ApplicationManager.DependencyInjection.Resolve<ILogService>();

            var startInfo = new ProcessStartInfo
            {
                FileName = string.IsNullOrWhiteSpace(pdfConverterPath) ? @"wkhtmltopdf.exe" : pdfConverterPath + @"\wkhtmltopdf.exe",
                Arguments = Switches + " - " + _destinationPath + _fileName,
                UseShellExecute = false, // needs to be false in order to redirect output
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true, // redirect all 3, as it should be all 3 or none
                WorkingDirectory = destinationPath
            };
            _process.StartInfo = startInfo;
            _process.Start();

            var streamWriter = _process.StandardInput;
            streamWriter.Write(htmlStream.ToString());
            streamWriter.Close();

            // read the output here...
            //string output = _process.StandardOutput.ReadToEnd();
            string error = _process.StandardError.ReadToEnd();
            //logger.Info(output);
            logger.Error(error);

            // ...then wait n milliseconds for exit (as after exit, it can't read the output)
            _process.WaitForExit(30000);

            int returnCode = _process.ExitCode;

            _process.Close();

            // Need to check it later 
            if (returnCode == 0)
            {
                return _destinationPath + _fileName;
            }
            if(_destinationPath!=null && _fileName!=null )
            {
                return _destinationPath + _fileName;
            }
            if (!string.IsNullOrWhiteSpace(error) && error.Trim().ToLower().StartsWith("qt") &&
                error.Trim().ToLower().EndsWith("done") && System.IO.File.Exists(_destinationPath + "\\" + _fileName))
            {
                return _destinationPath + _fileName;
            }

            return null;

            // if 0, it worked
        }
    }
}
