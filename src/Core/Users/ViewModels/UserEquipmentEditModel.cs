using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Users.ViewModels
{
    [Core.Application.Attribute.NoValidatorRequired]
  public class UserEquipmentEditModel : EditModelBase
    {
        public PersonEquipmentEditModel PersonEditModel { get; set; }
        public long OrganizationId { get; set; }
        public long RoleId { get; set; }
        public bool CreateLogin { get; set; }
        public string Alias { get; set; }
        public string FileName { get; set; }
        public long? FileId { get; set; }
        public ICollection<long> RoleIds { get; set; }
        public ICollection<long> info { get; set; }
        public ICollection<long> OrganizationIds { get; set; }
        public bool IsExecutive { get; set; }
        public long? UserId { get; set; }
        public bool IsChanged { get; set; }
        public string Css { get; set; }
        public bool IsDefault { get; set; }

        public UserEquipmentEditModel()
        {
            PersonEditModel = new PersonEquipmentEditModel();
            IsExecutive = false;
            IsDefault = true;
        }
    }
}
