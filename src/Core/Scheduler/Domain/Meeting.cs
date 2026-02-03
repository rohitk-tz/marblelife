using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class Meeting : DomainBase
    {
        //public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public long? ParentId { get; set; }
        public double? Offset { get; set; }
        public DateTime StartDateTimeString { get; set; }
        public DateTime EndDateTimeString { get; set; }
        [ForeignKey("ParentId")]
        public Meeting Parent{ get; set; }
        public bool IsEquipment { get; set; }
    }
}
