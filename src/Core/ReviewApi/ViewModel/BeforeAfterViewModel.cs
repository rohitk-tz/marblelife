using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
    public class BeforeAfterMainModel
    {
        public string FranchiseeName { get; set; }
        public long? FranchiseeId { get; set; }
        public List<BeforeAfterViewModel> List { get; set; }
        public int OrderBy { get; set; }
    }
    public class BeforeAfterViewModel
    {
        public long Id { get; set; }
        public string SurfaceMaterial { get; set; }
        public string ServiceType { get; set; }
        public string SurfaceType { get; set; }
        public string SurfaceColor { get; set; }
        public string FinishType { get; set; }
        public string BuildingLocation { get; set; }
        public string FranchiseeName { get; set; }

        public string BeforeImageUrl { get; set; }
        public string AfterImageUrl { get; set; }
        public long? ServiceId { get; set; }
        public string FranchiseeUrl { get; set; }
        public long? FranchiseeId { get; set; }
        public long? ImageId { get; set; }
        public string MarketingClass { get; set; }
        public string ExteriorImageUrl { get; set; }
        public string BeforeImageUrlThumb { get; set; }
        public string AfterImageUrlThumb { get; set; }
        public string ExteriorImageUrlThumb { get; set; }
    }
}
