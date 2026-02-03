using Core.Reports.ViewModel;
using Core.Sales.Domain;

namespace Core.Reports
{
   public interface IMlfsReportFactory
    {
        MLFSReportConfigurationViewModel CreateViewModel(MlfsConfigurationSetting domain);
        MlfsConfigurationSetting CreateDomain(MLFSReportConfigurationViewModel viewModel);
    }
}
