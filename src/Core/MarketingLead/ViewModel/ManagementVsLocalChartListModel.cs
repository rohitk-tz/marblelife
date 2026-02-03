using System;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class ManagementVsLocalChartListModel
    {
        public decimal TotalCalls { get; set; }
        public decimal TotalMissedCalls { get; set; }
        
        public List<ManagementAndLocalChartGraphViewModel> ManagementVsLocalTableData { get; set; }
        public IList<ManagementAndLocalChartViewModel> ManagementVsLocalAverageData { get; set; }

        public decimal TotalCallsForDayForZeros { get; set; }
        public decimal TotalMissedCallsForZeros { get; set; }
        public decimal TotalMissedCallsForTheDayForZeros { get; set; }
        public decimal TotalCallsReceivedForDayForZeros { get; set; }


    }
}
