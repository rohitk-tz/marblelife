using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using System.Collections.Generic;
using static Core.Organizations.Impl.FranchiseeInfoService;

namespace Core.Organizations
{
    public interface IFranchiseeInfoService
    {
        FranchiseeEditModel Get(long userId);
        void Save(FranchiseeEditModel franchiseeViewModel);
        bool Delete(long franchiseeId);
        FranchiseeListModel GetFranchiseeCollection(FranchiseeListFilter filter, int pageNumber, int pageSize);
        FeeProfileViewModel GetFranchiseeFeeProfile(long franchiseeId);
        IEnumerable<FranchiseeNotesViewModel> GetFranchiseeNotes(long franchiseeId);
        bool DeactivateFranchisee(long franchiseeId, string deactivationNote);
        bool ActivateFranchisee(long franchiseeId);
        bool IsUniqueBusinessId(long? businessId, long id = 0);
        bool GetGeoCode(long franchiseeId);
        bool DownloadFranchisee(FranchiseeListFilter filter, out string fileName);
        bool DownloadFranchiseeDirectory(FranchiseeListFilter filter, out string fileName);
        FranchiseeResignListModel GetFranchiseeResignList(FranchiseeListFilter filter);
        bool DownloadFileFranchiseeDirectory(FranchiseeListFilter filter, out string fileName);
        bool DownloadFileFranchiseeDirectoryRedesign(FranchiseeListFilter filter, out string fileName);

        string GetFranchiseeDeactivationNote(long franchiseeId);
        IEnumerable<DropdownListItem> GetFranchiseeRPID(long? franchiseeId);

        bool OneTimeProjectChangeStatus(OneTimeProjectFilter filter);
        decimal GetMinimumRoyalityForFranchisee(long franchiseeId);
        FranchiseeDocumentListModel GetFranchiseeDocumentReport(FranchiseeDocumentFilter filter);
        IEnumerable<DropdownListItem> GetFranchiseeDocumentList();
        bool DownloadTaxReport(FranchiseeDocumentFilter filter, out string fileName);
        bool SEOChargesChangeStatus(OneTimeProjectFilter filter);
    }
}
