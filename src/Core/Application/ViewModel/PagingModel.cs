namespace Core.Application.ViewModel
{
    public class PagingModel
    {
        public int PageSize { get; private set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }

        public PagingModel()
            : this(ApplicationManager.Settings.PageSize)
        {
        }

        public PagingModel(int pageSize)
        {
            PageSize = pageSize;
        }

        public PagingModel(int currentPage, int pageSize, int totalRecords)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalRecords = totalRecords;
        }
    }
}
