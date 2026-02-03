namespace Core.Application.Impl
{
    public class MediaLocation
    {
        private readonly string _relativeFolderPath;

        public MediaLocation(string relativeFolderPath)
        {
            _relativeFolderPath = relativeFolderPath;
            Path = ApplicationManager.Settings.MediaRootPath + "\\" + relativeFolderPath;
        }

        public string Path { get; private set; }

        public string GetRelativeFolderPath()
        {
            return _relativeFolderPath;
        }
    }
}
