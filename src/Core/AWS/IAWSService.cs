using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AWS
{
    public interface IAWSService
    {
        Task<bool> CreateSubFolderAsync(string SubFolder);
        Task<bool> UploadPartRequestFileInSubFolder(string SubFolderName, string Key, string filePath);
        Task<bool> DoesFolderexists(string subFolderName);
    }
}
