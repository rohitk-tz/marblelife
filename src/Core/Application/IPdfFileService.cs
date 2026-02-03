namespace Core.Application
{
    public interface IPdfFileService
    {
        string GeneratePdfFromTemplateAndModel(object model, string destinationFolder, string generatedFileName, string templateFilePath);
    }
}
