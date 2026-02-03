using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    public class DocumentListModel
    {
        public IEnumerable<DocumentViewModel> Collection { get; set; }
        public DocumentListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
