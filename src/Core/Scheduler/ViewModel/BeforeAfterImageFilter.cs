using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class BeforeAfterImageFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ScrollClick { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public long ServiceTypeId { get; set; }
        public string BuildingLocation { get; set; }
        public string SurfaceTypeId { get; set; }
        public string SurfaceMaterial { get; set; }
        public string SurfaceColor { get; set; }
        public string FinishType { get; set; }
        public string BuildingType { get; set; }
        public string ManagementCompany { get; set; }
        public string MaidService { get; set; }
        public string FinishMaterial { get; set; }
        public long? FranchiseeId { get; set; }
        public long? MarketingClassId { get; set; }
        public long? UserId { get; set; }
        public long? LoggedUserId { get; set; }
        public long? LoggedInFranchiseeId { get; set; }
        public long? RoleId { get; set; }

        public List<long?> AlreadyOneUserId { get; set; }


    }

    //New Review Marketing Page Filter Model
    [NoValidatorRequired]
    public class ReviewMarketingFilter
    {
        public long UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? FranchiseeId { get; set; }
        public string SurfaceType { get; set; }
        public string SurfaceMaterial { get; set; }
        public string SurfaceColor { get; set; }
        public string FinishMaterial { get; set; }
        public string BuildingLocation { get; set; }
        public long SurfaceTypeId { get; set; }
        public long ServiceTypeId { get; set; }
        public string IsImagePairReviewed { get; set; }//
        public long MarketingClassId { get; set; }
        //public string ManagementCompany { get; set; }
        //public string MaidService { get; set; }
        public long LoggedUserId { get; set; }
        public long RoleId { get; set; }
        public long LoggedInFranchiseeId { get; set; }
        public bool IsDateFilter { get; set; }
        public bool IsFilter { get; set; }
    }
}