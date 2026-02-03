using Core.Application.ValueType;
using Core.Application.ViewModel;
using Core.Geo.ViewModel;
using System.Collections.Generic;

namespace Core.Users.ViewModels
{
    public class PersonEditModel : EditModelBase
    {
        public string ColorCodeSale { get; set; }
        public string Color { get; set; }
        public long Id { get; set; }
        public long PersonId { get; set; }
        public Name Name { get; set; }
        public string Email { get; set; }
        public long? FileId { get; set; }

        public string FileName { get; set; }
        public IEnumerable<AddressEditModel> Address { get; set; }
        public IEnumerable<PhoneEditModel> PhoneNumbers { get; set; }
        public bool IsActive { get; set; }
        public bool IsRecruitmentFeeApplicable { get; set; }

        public PersonEditModel()
        {
            PhoneNumbers = new List<PhoneEditModel>();
            Address = new List<AddressEditModel>();
            Name = new Name();
            IsActive = true;
            IsRecruitmentFeeApplicable = false;
        }
    }
}
