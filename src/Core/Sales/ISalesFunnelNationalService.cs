using Core.Sales.ViewModel;

namespace Core.Sales
{
    public interface ISalesFunnelNationalService
    {
        SalesFunnelNationalListModel GetSalesFunnelNationalList(SalesFunnelNationalListFilter filter);
        bool CreateExcelForNatioanlFunnel(SalesFunnelNationalListFilter filter, out string fileName);

        SalesFunnelNationalListModel GetSalesFunnelLocalList(SalesFunnelNationalListFilter filter);

        bool CreateExcelForLocalFunnel(SalesFunnelNationalListFilter filter, out string fileName);

        SalesFunnelLocalGraphListModel GenerateChartData(SalesFunnelNationalListFilter filter);

    }
}
