using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Extensions;
using Core.Application.ViewModel;

namespace Core.Application.Impl
{
    [DefaultImplementation]
    public class FileService : IFileService
    {
        private readonly IClock _clock;
        private readonly IRepository<File> _fileRepository;

        public const long ContentTypeOther = 6;
        public FileService(IUnitOfWork unitOfWork, IClock clock)
        {
            _clock = clock;
            _fileRepository = unitOfWork.Repository<File>();
        }

        public File SaveFile(FileModel file, MediaLocation fileDestination, string fileNamePrefix)
        {
            string filePath = file.RelativeLocation.ToPath().ToFullPath();

            if (file.Id < 1)
                file.DataRecorderMetaData = new DataRecorderMetaData(_clock.UtcNow);

            return SaveModel(file);
        }

        public File SaveModel(FileModel fileModel)
        {
            var domain = new File
            {
                Caption = fileModel.Caption,
                Name = fileModel.Name,
                DataRecorderMetaData = fileModel.DataRecorderMetaData,
                Size = fileModel.Size,
                RelativeLocation = fileModel.RelativeLocation,
                MimeType = fileModel.MimeType,
                IsNew = fileModel.Id <= 0,
                css = fileModel.css,
                //FileReferenceId = fileModel.FileReferenceId
            };

            _fileRepository.Save(domain);

            fileModel.Id = domain.Id;
            return domain;
        }

        public FileModel Get(long fileId)
        {
            var domain = _fileRepository.Get(fileId);
            return Get(domain);
        }

        public FileModel Get(File domain)
        {
            var model = new FileModel
            {
                Id = domain.Id,
                Caption = domain.Caption,
                Name = domain.Name,
                DataRecorderMetaData = domain.DataRecorderMetaData,
                Size = domain.Size,
                RelativeLocation = (domain.RelativeLocation + "\\" + domain.Name).ToUrl()
            };

            return model;
        }

        public string MoveFile(string source, MediaLocation destination, string fileName, string ext)
        {
            var destFileName = destination.Path + "\\" + fileName + ext;
            System.IO.File.Move(source, destFileName);

            return destFileName;
        }
    }
}
