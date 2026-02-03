using Core.Application.Domain;

namespace Core.Application.ViewModel
{
    public class EditModelBase 
    {
        public DataRecorderMetaData DataRecorderMetaData { get; set; }

        public EditModelBase()
        {
            DataRecorderMetaData = new DataRecorderMetaData();
        }
    }
}
