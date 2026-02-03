using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Scheduler.ViewModel;
using System.Collections.Generic;

namespace Core.Users.ViewModels
{
    [NoValidatorRequired]
    public class UserSignatureListEditModel
    {
        public List<UserSignatureEditModel> UserSignatureEditModel { get; set; }
        public UserSignatureListEditModel()
        {
            UserSignatureEditModel = new List<UserSignatureEditModel>();
        }
    }
    [NoValidatorRequired]
    public class UserSignatureEditModel
    {
        public long? UserId { get; set; }
        public long? OrganizationRoleUserId { get; set; }
        public string SignatureName { get; set; }
        public string Signature { get; set; }
        public bool? IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
    [NoValidatorRequired]
    public class UserSignatureSaveModel{
        public string SignatureName { get; set; }
        public string Signature { get; set; }
        public bool IsDefault { get; set; }
    }
    [NoValidatorRequired]
    public class UserSignatureListSaveModel
    {
        public List<UserSignatureSaveModel> UserSignatureSaveModel { get; set; }
        public UserSignatureListSaveModel()
        {
            UserSignatureSaveModel = new List<UserSignatureSaveModel>();
        }
    }
}
