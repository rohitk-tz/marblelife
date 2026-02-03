using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    public class BestPairImageSendToMarketingSiteModel
    {
        public BestPairImageSendToMarketingSiteModel()
        {
            Collection = new List<BestPairImageForMarketingSiteModel>();
        }
        public List<BestPairImageForMarketingSiteModel> Collection { get; set; }
    }

    public class BestPairImageForMarketingSiteModel
    {
        public BestPairImageForMarketingSiteModel()
        {
            ExteriorImageList = new List<ExteriorImagesModelForMarketingSite>();
        }
        public long? Id { get; set; }
        public long? FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public long BeforeImageId { get; set; }
        public string BeforeImageS3BucketURL { get; set; }
        public string BeforeImageS3BucketThumbURL { get; set; }
        public string CroppedBeforeImageS3BucketURL { get; set; }
        public string BeforeImageCSS { get; set; }
        public long AfterImageId { get; set; }
        public string AfterImageS3BucketURL { get; set; }
        public string AfterImageS3BucketThumbURL { get; set; }
        public string CroppedAfterImageS3BucketURL { get; set; }
        public string AfterImageCSS { get; set; }
        public string SurfaceMaterial { get; set; }
        public string ServiceType { get; set; }
        public string SurfaceType { get; set; }
        public string SurfaceColor { get; set; }
        public string SurfaceFinish { get; set; }
        public string BuildingLocation { get; set; }
        public string MarketingClass { get; set; }
        public bool IsBestPair { get; set; }
        public bool IsSpecialPair { get; set; }
        public string FromSource { get; set; }
        public List<ExteriorImagesModelForMarketingSite> ExteriorImageList { get; set; }
    }

    public class ExteriorImagesModelForMarketingSite
    {
        public long? Id { get; set; }
        public long ExteriorImageId { get; set; }
        public string ExteriorImageS3BucketURL { get; set; }
        public string CroppedExteriorImageS3BucketURL { get; set; }
        public string ExteriorImageCSS { get; set; }
        public string ExteriorThumbImageS3BucketURL { get; set; }
    }
}
