using Api.Areas.Application.ViewModel;
using System.Collections.Generic;

namespace API.Areas.Application.ViewModel
{
    public class ServiceTypeListItem : DropdownListItem
    {
        public string  CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public long? SubCategoryId { get; set; }
        public string SubCategoryName{ get; set; }
        public int NewOrderBy { get; set; }
        public string Synonyms { get; set; }
    }

    public class ServiceTypeGroupedListItem
    {
        public string GroupName { get; set; }
        public int Order { get; set; }
        public List<ServiceTypeListItem> Collection { get; set; }
    }
}