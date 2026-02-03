namespace Api.Areas.Application.ViewModel
{
    public class DropdownListItem
    {
        public string Display { get; set; }
        public string Value { get; set; }
        public string Alias { get; set; }
        public long Order { get; set; }
        public bool? IsPerpetuity { get; set; }
        public long Id { get; set; }
        public DropdownListItem()
        {
        }

        public DropdownListItem(string text, string value)
        {
            Display = text;
            Value = value;
        }
        public DropdownListItem(string text, string value, string alias)
        {
            Display = text;
            Value = value;
            Alias = alias;
        }
        public DropdownListItem(string text, long id, string alias)
        {
            Display = text;
            Id = id;
            Alias = alias;
        }
    }
}