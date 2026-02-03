namespace Core.Application.ViewModel
{
    public class ModelValidationItem
    {
        public string Name { get; set; }

        public string Error { get; set; }

        public ModelValidationItem(string name, string error)
        {
            Name = name;
            Error = error;
        }
    }
}