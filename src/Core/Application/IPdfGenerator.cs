using System.Text;
using Core.Application.Impl;

namespace Core.Application
{
    public interface IPdfGenerator
    {
        void SetDefaultSwitch(WkHtmltoPdfSwitches defaultSwitches);
        string Generate(string sourcePath, string destinationPath, string pdfConverterPath = "", string fileName = "");
        string Generate(StringBuilder htmlStream, string destinationPath, string pdfConverterPath = "", string fileName = "");
    }
}
