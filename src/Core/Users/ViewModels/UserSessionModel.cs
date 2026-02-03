using Core.Application.ValueType;
using System;
using System.Xml.Serialization;

namespace Core.Users.ViewModels
{
    public class UserSessionModel
    {
        public Name Name { get; set; }

        public long UserId { get; set; }

        public long OrganizationRoleUserId { get; set; }
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public long OrganizationType { get; set; }

        public DateTime? LastLoginAt { get; set; }

        [XmlIgnore]
        public long RoleId { get; set; }

        [XmlIgnore]
        public int AccessOrder { get; set; }

        public string RoleAlias { get; set; }
        public string RoleName { get; set; }

        public string CurrentTime { get; set; }
        public string TimeZoneId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        public bool IsTodayCurrencyExchangeRate { get; set; }
        public bool IsDefault { get; set; }
        public long? LoggedInOrganizationId { get; set; }
        public string FileName { get; set; }
        public string TeamFileName { get; set; }
        public string Css { get; set; }
        public long TodayToDoCount { get; set; }

        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }

        public long EstimateInvoiceId { get; set; }
        public long EstimateCustomerId { get; set; }

        public string Signature { get; set; }
        public bool IsSigned { get; set; }
        public long? SchedulerId { get; set; }

        public long? JobOrginialSchedulerId { get; set; }

        public bool? IsPostSignature { get; set; }
        public long? TypeId { get; set; }
    }
}
