using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using System.Collections.Generic;

namespace Core.Scheduler
{
    public interface IEstimateInvoiceFactory
    {
        EstimateInvoiceServiceViewModel CreateViewModel(EstimateInvoiceService estimateInvoiceService,
                                                            List<EstimateInvoiceServiceDescriptionViewModel> estimateInvoiceServiceDescriptionViewModel,
                                                            EstimateInvoice estimateInvoice, List<EstimateInvoiceService> subItem, List<EstimateInvoiceDimension> estimateInvoiceDimensions, List<EstimateInvoiceAssignee> assignees,HoningMeasurement honingMeasurement, List<HoningMeasurement> honingMeasurementList, List<HoningMeasurementDefault> honingMeasurementDefaultList);

        EstimateInvoice CreateDomain(EstimateInvoiceEditModel model );
        EstimateInvoiceCustomer CreateDomainForCustomer(EstimateInvoiceEditModel model);
        EstimateInvoiceService CreateDomain(EstimateInvoiceServiceEditModel model);

        EstimateInvoiceServiceEditMailModel CreateMailViewModel(EstimateInvoiceService estimateInvoiceService, List<EstimateInvoiceService> subList,List<TermsAndConditionFranchisee> termsList);
        SubItemEditModel CreateViewModelForSubItem(EstimateInvoiceService estimateInvoiceServices, List<HoningMeasurement> honingMeasurementList, List<HoningMeasurementDefault> honingMeasurementDefaultList);
        EstimateInvoiceService CreateDomainModel(EstimateInvoiceServiceEditModel estimateInvoiceEditServices, SubItemEditModel subItem);

        TechnicianWorkOrderForInvoice CreateDomain(TechnicianWorkOrder model,long? estimateInvoiceId);
        ListViewModelForWorkOrder CreateViewModel(TechnicianWorkOrderForInvoice model);
        EstimateInvoiceDimension CreateDomain(EstimateInvoiceDimensionEditModel model, long? estimateInvoiceServiceId);
        EstimateInvoiceDimensionViewModel CreateEstimateInvoiceDimensionViewModel(EstimateInvoiceDimension estimateInvoiceServiceDimension,long? unitTypeId=null);
        EstimateInvoiceDimension CreateDomain(EstimateInvoiceDimensionViewModel model, long? estimateInvoiceServiceId);
        MaintenanceViewModelCharges CreateMaintanceChargesViewModel(MaintenanceCharges honingMeasurement);
    }
}
