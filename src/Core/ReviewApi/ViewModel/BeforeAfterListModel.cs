using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
   public class BeforeAfterListModel
    {
        public List<BeforeAfterMainModel> Data { get; set; }
        public List<BeforeAfterViewModel> BeforeAfterList { get; set; }
        public string Status { get; set; }
        public int count { get; set; }
    }

    [NoValidatorRequired]
    public class BeforeAfterFilterModel
    {
        public long? FranchiseeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
