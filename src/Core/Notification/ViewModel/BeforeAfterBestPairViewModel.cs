using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification.ViewModel
{
    public class BeforeAfterBestPairViewModel
    {
        public string BeforeImageUrl { get; set; }
        public string AfterImageUrl { get; set; }
        public string PersonName { get; set; }
        public string BestImageMarkedOn { get; set; }
        public string FranchiseeName { get; set; }
        public string AfterImageCss { get; set; }
        public string BeforeImageCss { get; set; }
    }
    public class BeforeAfterBestPairListModel
    {
        public BeforeAfterBestPairListModel()
        {
            BeforeAfterBestPairViewModel = new List<BeforeAfterBestPairViewModel>();
        }
       public List<BeforeAfterBestPairViewModel> BeforeAfterBestPairViewModel { get; set; }
    }

    public class BeforeAfterBestPairGroupedViewModel
    {
        public BeforeAfterBestPairGroupedViewModel()
        {
            BeforeAfterBestPairViewModel = new List<BeforeAfterBestPairViewModel>();
        }
        public string FranchiseeName { get; set; }
        public List<BeforeAfterBestPairViewModel> BeforeAfterBestPairViewModel { get; set; }
    }

    public class BeforeAfterBestPairGroupedListModel
    {
        public BeforeAfterBestPairGroupedListModel()
        {
            BeforeAfterBestPairGroupedViewModel = new List<BeforeAfterBestPairGroupedViewModel>();
        }
        public List<BeforeAfterBestPairGroupedViewModel> BeforeAfterBestPairGroupedViewModel { get; set; }
    }
}
