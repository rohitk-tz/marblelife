using Core.Application.ValueType;
using System;

namespace Core.Organizations.ViewModel
{
    public class FranchiseeNotesViewModel
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}
