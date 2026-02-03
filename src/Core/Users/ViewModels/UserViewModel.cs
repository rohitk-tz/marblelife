using Core.Application.Attribute;
using Core.Application.ValueType;
using Core.Geo.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Users.ViewModels
{
    [NoValidatorRequired]
    public class UserViewModel
    {
        public long UserId { get; set; }
        public long OrganizationRoleUserId { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public Name Name { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public AddressViewModel Address { get; set; }
        public IEnumerable<object> PhoneNumbers { get; set; }
        public bool IsLocked { get; set; }
        public string FranchiseeName { get; set; }
        public long RoleId { get; set; }
        public bool IsActive { get; set; }

        public long? FranchiseeId { get; set; }
    }
}
