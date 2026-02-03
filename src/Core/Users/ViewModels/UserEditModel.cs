using Core.Application.ViewModel;
using Core.Scheduler.ViewModel;
using System.Collections.Generic;

namespace Core.Users.ViewModels
{
    public class UserEditModel : EditModelBase
    {
        public UserLoginEditModel UserLoginEditModel { get; set; }
        public PersonEditModel PersonEditModel { get; set; }
        public long OrganizationId { get; set; }
        public long RoleId { get; set; }
        public bool CreateLogin { get; set; }
        public string Alias { get; set; }
        public string FileName { get; set; }
        public long? FileId { get; set; }
        public ICollection<long> RoleIds { get; set; }
        public FileUploadModel FileUploadModel { get; set; }
        public ICollection<long> info { get; set; }
        public ICollection<long> OrganizationIds { get; set; }
        public bool IsExecutive { get; set; }
        public long? UserId { get; set; }
        public bool IsChanged { get; set; }
        public string Css { get; set; }
        public bool IsDefault { get; set; }
        public bool? isImageChanged { get; set; }

        public FrabchiseeDocumentEditModel franchiseeDocument { get; set; }
        public UserEditModel()
        {
            UserLoginEditModel = new UserLoginEditModel();
            PersonEditModel = new PersonEditModel();
            IsExecutive = false;
            IsDefault = true;
            franchiseeDocument = new FrabchiseeDocumentEditModel();
        }
    }
}
