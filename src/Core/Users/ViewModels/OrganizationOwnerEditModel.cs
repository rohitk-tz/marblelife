using Core.Application.ValueType;
using Core.Application.ViewModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Users.ViewModels
{
    public class OrganizationOwnerEditModel : EditModelBase
    {
        public long OwnerId { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public bool SendUserLoginViaEmail { get; set; }
        public bool IsRecruitmentFeeApplicable { get; set; }

        public string Password { get; set; }
        [NotMapped]
        public Name OwnerName
        {
            get { return new Name(OwnerFirstName, OwnerLastName); }
            set { OwnerName = value; }
        }

        public OrganizationOwnerEditModel()
        {
            SendUserLoginViaEmail = true;
            IsRecruitmentFeeApplicable = false;
        }
    }
}
