using Core.Application;
using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeNotesFactory : IFranchiseeNotesFactory
    {
        private readonly IClock _clock;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        public FranchiseeNotesFactory(IClock clock, IOrganizationRoleUserInfoService organizationRoleUserInfoService)
        {
            _clock = clock;
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
        }
        public FranchiseeNotes CreateDomain(long franchiseeId, FranchiseeEditModel model)
        {
            var domain = new FranchiseeNotes
            {
                FranchiseeId = franchiseeId,
                Text = model.Text,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaDataId,
                IsNew = true
            };
            return domain;
        }

        public FranchiseeNotesViewModel CreateViewModel(FranchiseeNotes notes)
        {
            var createdBy = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(notes.DataRecorderMetaData.CreatedBy.Value);

            var viewModel = new FranchiseeNotesViewModel
            {
                Id = notes.Id,
                Text = notes.Text,
                CreatedOn = notes.DataRecorderMetaData.DateCreated,
                CreatedBy = createdBy.Email,
                CreatedByName = createdBy.Name.FullName
            };
            return viewModel;
        }
    }
}
