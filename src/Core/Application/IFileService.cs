using Core.Application.Domain;
using Core.Application.Impl;
using Core.Application.ViewModel;

namespace Core.Application
{
    public interface IFileService
    {
        File SaveModel(FileModel fileModel);
        FileModel Get(long fileId);
        FileModel Get(File domain);
        File SaveFile(FileModel file, MediaLocation fileDestination, string fileNamePrefix);
        string MoveFile(string source, MediaLocation destination, string fileNamePrefix, string ext);
    }
}
