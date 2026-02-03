using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.ViewModel
{
    public class MLFSReportListModel
    {
        public List<List<decimal>> TotalServiceSum { get; set; }
        public string EndDate { get; set; }
        public List<FranchiseeGroupModel> FranchiseeGroupModel { get; set; }
        public List<QuarterClass> QuarterList { get; set; }
        public List<int> YearList { get; set; }
        public List<int> YearWiseSum { get; set; }
        public List<ColorStatusClass> ColorStatusWithYearList { get; set; }
        public int CollectionCount { get; set; }
        public List<MLFSReportConfigurationViewModel> StatusList { get; set; }
        public List<decimal> TotalSum { get; set; }
        public List<decimal> InternationalFranchiseeSum { get; set; }
        public List<decimal> LocalFranchiseeSum { get; set; }
        public List<decimal> ActiveFranchiseeSum { get; set; }
        public List<decimal> ActiveFranchiseeSumWithoutNew { get; set; }
        public List<decimal> ActiveLocalFranchiseeSumWithNew { get; set; }
        public List<decimal> ActiveLocalFranchiseeSumWithoutNew { get; set; }

    }
    public class QuarterClass
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
    }
    public class ColorStatusClass
    {
        public int Year { get; set; }
        public decimal Value { get; set; }
        public string Color { get; set; }
        public string BackGroundColor { get; set; }
        public bool IsParsed { get; set; }
    }
}
