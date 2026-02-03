using Core.Sales.Domain;
using Core.Sales.ViewModel;

namespace Core.Sales
{
   public interface ISalesFunnelFactory
    {
        SalesFunnelNationalExcelViewModel CreateListModel(SalesFunnelNationalViewModel domain);
        SalesFunnelLocalExcelViewModel CreateListModel(SalesFunnelLocalViewModel domain);
    }
}
