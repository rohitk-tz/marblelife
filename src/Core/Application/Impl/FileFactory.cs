using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Domain;
using Core.Application.ViewModel;
using Core.Application.Impl;

namespace Core.Application.Impl
{
    [DefaultImplementation]
    public class FileFactory : IFileFactory
    {
        public File CreateDomain(FileModel model, File inPersistence)
        {
            var domain = new File
            {
                Caption = model.Caption,
                DataRecorderMetaData = model.DataRecorderMetaData,
                Id = model.Id,
                Name = model.Name,
                IsNew = model.Id <= 0,
                Size = model.Size,
                MimeType = model.MimeType,
                RelativeLocation = model.RelativeLocation
            };

            return domain;
        }

        public FileModel CreateModel(File file)
        {
            var model = new FileModel
            {
                Id = file.Id,
                Caption = file.Caption,
                DataRecorderMetaData = file.DataRecorderMetaData,
                Name = file.Name,
                Size = file.Size,
                MimeType = file.MimeType,
                RelativeLocation = file.RelativeLocation
            };
            return model;
        }
    }
}
