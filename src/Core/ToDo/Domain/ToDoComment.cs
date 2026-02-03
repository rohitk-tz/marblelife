using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.ToDo.Domain
{
    public class ToDoFollowUpComment : DomainBase
    {
        public long ToDoId { get; set; }
        [ForeignKey("ToDoId")]
        public virtual ToDoFollowUpList ToDo { get; set; }

        public string Comment { get; set; }
        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    }
}
