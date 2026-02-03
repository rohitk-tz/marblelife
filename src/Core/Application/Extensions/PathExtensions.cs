using Core.Application.Exceptions;

namespace Core.Application.Extensions
{
    public static class PathExtensions
    {
        public static string ToUrl(this string path)
        {
            return path.Replace("\\", "/");
        }

        public static string ToPath(this string url)
        {
            return url.Replace("/", "\\");
        }

        public static string ToRelativePath(this string path)
        {
            if (!path.ToLower().Contains(ApplicationManager.Settings.MediaRootPath.ToLower()))
                throw new InvalidDataProvidedException();

            return path.ToLower().Replace(ApplicationManager.Settings.MediaRootPath.ToLower(), "");
        }

        public static string ToFullPath(this string relPath)
        {
            return ApplicationManager.Settings.MediaRootPath + relPath;
        }
        public static string ToFullPathForHomeAdvisor(this string relPath)
        {
            return ApplicationManager.Settings.MediaRootPath +"\\HomeAdvisor"+ relPath;
        }
    }
}
