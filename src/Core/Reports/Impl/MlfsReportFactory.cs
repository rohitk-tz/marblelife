using Core.Application.Attribute;
using Core.Reports.ViewModel;
using Core.Sales.Domain;
using System;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class MlfsReportFactory : IMlfsReportFactory
    {
        public MLFSReportConfigurationViewModel CreateViewModel(MlfsConfigurationSetting domain)
        {
            return new MLFSReportConfigurationViewModel
            {
                IsActive = true,
                MaxValue =(domain.MaxValue),
                MinValue =(domain.MinValue),
                Name = domain.Status,
                Id = domain.Id,
                ColorCode = domain.ColorCode
            };
        }
        public MlfsConfigurationSetting CreateDomain(MLFSReportConfigurationViewModel viewModel)
        {
            return new MlfsConfigurationSetting
            {
                IsActive = true,
                MaxValue =(viewModel.MaxValue),
                MinValue = (viewModel.MinValue),
                Status = viewModel.Name,
                Id = viewModel.Id.GetValueOrDefault(),
                IsNew = viewModel.Id <= 0 ? true : false,
                ColorCode = viewModel.ColorCode

            };
        }
    }
}
