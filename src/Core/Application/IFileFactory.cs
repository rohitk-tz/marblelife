using Core.Application.Domain;
using Core.Application.ViewModel;

namespace Core.Application
{
    public interface IFileFactory
    {
        File CreateDomain(FileModel model, File inPersistence);
        FileModel CreateModel(File file);
    }
}
