using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class LocalMarketingReviewFilter
    {
        public string UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? FranchiseeId { get; set; }
        public string SurfaceType { get; set; }
        public string SurfaceMaterial { get; set; }
        public string SurfaceColor { get; set; }
        public string FinishMaterial { get; set; }
        public string BuildingLocation { get; set; }
        public string SurfaceTypeId { get; set; }
        public string ServiceTypeId { get; set; }
        public string IsImagePairReviewed { get; set; }
        public bool IsDateFilter { get; set; }
        public bool IsFilter { get; set; }
        public long LoggedInFranchiseeId { get; set; }
        public long LoggedUserId { get; set; }
        public long RoleId { get; set; }
        public long MarketingClassId { get; set; }
        public bool PendingToAddInLocalSiteGallery { get; set; }
    }
}
