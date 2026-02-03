namespace API.Areas.Application.ViewModel
{
    public class MultiSelectListItem
    {
        public string Label { get; set; }
        public string Alias { get; set; }
        public string ColorCode { get; set; } 
        public long Id { get; set; }
        public string Role { get; set; }
        public long UserId { get; set; }
        public bool? isLocked { get; set; }
    }
}