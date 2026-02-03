using Core.Application.Domain;
using Core.Application.Exceptions;
using Core.Organizations;
using Core.Users.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Core.Users.Domain
{
    public class Phone : DomainBase
    {
        public long TypeId { get; set; }
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public string Extension { get; set; }
        public bool IsTransferable { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
        public virtual ICollection<Organization> Organizations { get; set; }

        [ForeignKey("TypeId")]
        public virtual Lookup Lookup { get; set; }
        public Phone()
        {
            CountryCode = "1";
        }

        public Phone(PhoneType type)
        {
            CountryCode = "1";
            TypeId = (long)type;
        }
        public Phone(string areaCode, string number, PhoneType type)
        {
            CountryCode = "1";
            AreaCode = areaCode;
            Number = number;
            TypeId = (long)type;
        }
        public static Phone Create(string phone, long phoneType, long id)
        {
            if (string.IsNullOrEmpty(phone)) return null;

            var obj = new Phone();

            string ext;
            if (ContainsExtension(ref phone, out ext))
            {
                obj.Extension = ext;
            }

            phone = SanitizePhoneString(phone);

            if (phone.Length < 10 || !IsNumeric(phone)) throw new InvalidDataProvidedException();

            obj.Number = phone.Substring(phone.Length - 7, 7);
            obj.AreaCode = phone.Substring(phone.Length - 10, 3);
            obj.TypeId = phoneType > 0 ? phoneType : (long)PhoneType.Cell;
            obj.Id = id;
            obj.IsNew = id < 1;
            return obj;
        }

        public static bool IsNumeric(string phone)
        {
            var regex = new Regex("^[0-9]+$");
            return regex.IsMatch(phone);
        }

        private static bool ContainsExtension(ref string phone, out string extension)
        {
            extension = "";
            var pos = phone.ToLower().IndexOf("x", System.StringComparison.Ordinal);

            if (pos < 0) return false;

            extension = phone.Substring(pos + 1, phone.Length - pos - 1);

            phone = phone.Substring(0, pos);

            if (!string.IsNullOrWhiteSpace(extension)) return true;

            return false;
        }

        private static string SanitizePhoneString(string phone)
        {
            return phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace("#", "").Replace(" ", "").Trim();
        }
    }
}
