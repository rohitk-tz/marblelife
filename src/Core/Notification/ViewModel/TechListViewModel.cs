
using System;

namespace Core.Notification.ViewModel
{
    public class TechListViewModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string TechName
        {
            get
            {
                return TechNameString();
            }
        }
        public string FullName {
            get
            {
                return FullNameString();
            }
        }
        public string src { get; set; }
        public long? fileId { get; set; }
        public string display { get; set; }
        private string FullNameString()
        {
            var name = string.Empty;
            if (!string.IsNullOrEmpty(FirstName))
            {
                name += FirstName;
            }
            if (!string.IsNullOrEmpty(MiddleName))
            {
                name += " " + MiddleName;
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                name += " " + LastName;
            }

            return name;
        }

        private string TechNameString()
        {
            var name = FullName;
            if (!string.IsNullOrEmpty(Role))
            {
                name += " (" + Role + ")";
            }
            return name;
        }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long? Id { get; set; }
    }
}