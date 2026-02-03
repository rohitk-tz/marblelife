// Copyright 2019 Zone Defense LLC
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application;

namespace Infrastructure.Application.Impl
{
    [DefaultImplementation]
    public class PdfGenerator : IPdfGenerator
    {
        private Process _process;
        private string _sourcePath;
        private string _destinationPath;

        public WkHtmltoPdfSwitches Switches { get; set; }

        public bool AllowLoadingJavascriptbeforePdfGenerate { get; set; }
        private string _fileName;

        public PdfGenerator(WkHtmltoPdfSwitches defaultSwitches)
        {
            Switches = defaultSwitches;
            AllowLoadingJavascriptbeforePdfGenerate = false;
        }
        public PdfGenerator()
        {
            Switches = new WkHtmltoPdfSwitches();
        }

        public void SetDefaultSwitch(WkHtmltoPdfSwitches defaultSwitches)
        {
            Switches = defaultSwitches;
        }

        public string Generate(string sourcePath, string destinationPath, string pdfConverterPath = "", string fileName = "")
        {
            _process = new Process();
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
            _fileName = string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid() + ".pdf" : fileName;


            var startInfo = new ProcessStartInfo
            {
                FileName = string.IsNullOrWhiteSpace(pdfConverterPath) ? @"wkhtmltopdf.exe" : pdfConverterPath + @"/wkhtmltopdf.exe",
                Arguments = Switches + " \"" + _sourcePath + "\" " + _destinationPath + "\\" + _fileName,
                UseShellExecute = false, // needs to be false in order to redirect output
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true, // redirect all 3, as it should be all 3 or none
                WorkingDirectory = destinationPath
            };

            _process.StartInfo = startInfo;
            _process.Start();
            _process.WaitForExit(50000);

            if (_process.HasExited == false)
                _process.Kill();

            int returnCode = _process.ExitCode;

            var errorText = "";
            if (returnCode != 0)
            {
                errorText = _process.StandardError.ReadToEnd();
            }

            _process.Dispose();
            _process.Close();
            if (returnCode == 0) return _fileName;
            if (!string.IsNullOrWhiteSpace(errorText) && errorText.Trim().ToLower().StartsWith("qt") &&
                errorText.Trim().ToLower().EndsWith("done") && File.Exists(_destinationPath + "\\" + _fileName))
            {
                return _fileName;
            }

            return null;

            // if 0, it worked
        }

        public string Generate(StringBuilder htmlStream, string destinationPath, string pdfConverterPath = "", string fileName = "")
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
            if (!string.IsNullOrWhiteSpace(error) && error.Trim().ToLower().StartsWith("qt") &&
                error.Trim().ToLower().EndsWith("done") && File.Exists(_destinationPath + "\\" + _fileName))
            {
                return _destinationPath + _fileName;
            }

            return null;

            // if 0, it worked
        }
    }
}
