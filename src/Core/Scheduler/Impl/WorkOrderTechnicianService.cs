using Core.Application.Attribute;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class WorkOrderTechnicianService : IWorkOrderTechnicianService
    {
        private readonly IEstimateInvoiceFactory _estimateInvoiceFactory;
        public WorkOrderTechnicianService(IEstimateInvoiceFactory estimateInvoiceFactory)
        {
            _estimateInvoiceFactory = estimateInvoiceFactory;
        }
        public List<TechnicianWorkOrderForInvoice> CreateTechnicianWorkOrderInvoice(EstimateInvoiceServiceEditModel invoice, List<TechnicianWorkOrderForInvoice> technicianWorkOrderInvoice, long? estimateInvoiceId)
        {
            var materialType = invoice.typeOfStoneType3.Count() > 0 ? invoice.typeOfStoneType3[0] : "";
            var serviceTypeId = invoice.ServiceIds1.Select(x => x.Id).ToList();
            var subServiceTypeList = invoice.SubItem.Select(x => x.ServiceIds).ToList();
            var defaultValue = new List<ListViewModel>();
            defaultValue.Add(new ListViewModel() { Id = "0", Notes = "" });

            var subServiceTypeId = subServiceTypeList.Count() > 0 ? subServiceTypeList.Select(x => x.Id).ToList() : defaultValue.Select(x => x.Id).ToList();
            
            if (invoice.ServiceType == "STONELIFE" && materialType == "Marble")
            {
                if (serviceTypeId.Contains("Honing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Honing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "50").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "100").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "200").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "400").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "3000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "8000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "11000").IsActive = true;
                    //technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-1").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-2").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-3").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-4").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping Carpet Adhesive") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping Carpet Adhesive") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "Carpet Adhesive Stripper").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Silicone") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping-Silicone") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "PLC-40 - Silicone Stripper").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Urethane") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping-Urethane") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "JASCO/MECL - Urethane Stripper").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Wax") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping-Wax") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "Butyl-Wax Stripper").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Unknown") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping-Unknown") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "Butyl-Wax Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "JASCO/MECL - Urethane Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "PLC-40 - Silicone Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "Carpet Adhesive Stripper").IsActive = true;
                }
                if (serviceTypeId.Contains("Cleaning") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Cleaning") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-White").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Marble Cleaner Concentrate").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Pre-Care").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Maxout").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Soap & Scum Remover").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Mold & Mildew Cleaner").IsActive = true;
                }
                if (serviceTypeId.Contains("Lippage Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Lippage Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "50").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "100").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "200").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "3000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "8000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "11000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-2").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-3").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-4").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Lippage Removal (UGC, UGP)").IsActive = true;
                }
                if (serviceTypeId.Contains("Polishing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Polishing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel 7 inch FLUFFY").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel 22 inch FLUFFY").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "CaCO/Gold Plus").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "IHG").IsActive = true;
                }
                if (serviceTypeId.Contains("Coating") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Coating") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Coating Applicator").IsActive = true;
                }
                if (serviceTypeId.Contains("Stain Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stain Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Stain Removal Kit (Poultice, Plastic)").IsActive = true;
                }
                if (serviceTypeId.Contains("Caulking") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Caulking") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Caulk Gun (Kit)").IsActive = true;
                }
                if (serviceTypeId.Contains("Sealing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Sealing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.SEALER && x.TechnicianWorkOrder.Name == "Penetrating Sealer").IsActive = true;
                }
                if (serviceTypeId.Contains("Chip Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Chip Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Granite Polishing Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Crack Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Crack Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Tile Replacement") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Tile Replacement") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Tile Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Grout Replacement") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Grout Replacement") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Grout Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Hard Water Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Hard Water Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Razor Blades").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Brick Paver Cleaner (Phosphoric Acid)").IsActive = true;
                }
                if (serviceTypeId.Contains("Mold Stain Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Mold Stain Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Mold & Mildew Cleaner").IsActive = true;
                }
                if (serviceTypeId.Contains("Traverfil") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Traverfil") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Traverfil").IsActive = true;
                }
            }

            if (invoice.ServiceType == "STONELIFE" && materialType == "Granite")
            {
                if (serviceTypeId.Contains("Stripping Carpet Adhesive") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping Carpet Adhesive") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Silicone") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping-Silicone") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Urethane") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping-Urethane") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Wax") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping-Wax") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Unknown") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stripping-Unknown") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Honing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Honing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "3000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "8000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "3000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "11000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-2").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-3").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "Cleancare Step-4").IsActive = true;
                }
                if (serviceTypeId.Contains("Polishing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Polishing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+ Blk").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+wh").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "IHG").IsActive = true;
                }
                if (serviceTypeId.Contains("Sealing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Sealing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.SEALER && x.TechnicianWorkOrder.Name == "Penetrating Sealer").IsActive = true;
                }
                if (serviceTypeId.Contains("Lippage Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Lippage Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Lippage Removal (UGC, UGP)").IsActive = true;
                }
                if (serviceTypeId.Contains("Caulking") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Caulking") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Caulk Gun (Kit)").IsActive = true;
                }
                if (serviceTypeId.Contains("Chip Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Chip Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Crack Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Crack Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Hard Water Removal ") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Hard Water Removal ") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Razor Blades").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Brick Paver Cleaner (Phosphoric Acid)").IsActive = true;
                }
                if (serviceTypeId.Contains("Tile Replacement") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Tile Replacementl") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Tile Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Grout Replacement") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Grout Replacement") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Grout Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Cleaning") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Cleaning") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Marble Cleaner Concentrate").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Granite Cleaner").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Pre-Care").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Grout Pro PLUS (Acidic)").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Maxout").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Soap & Scum Remover").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Mold & Mildew Cleaner").IsActive = true;
                }
                if (serviceTypeId.Contains("Stain Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stain Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Stain Removal Kit (Poultice, Plastic)").IsActive = true;
                }
            }


            if (invoice.ServiceType == "COUNTERLIFE" && materialType == "Marble")
            {
                if (serviceTypeId.Contains("Cleaning") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Cleaning") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Marble Cleaner Concentrate").IsActive = true;
                }
                if (serviceTypeId.Contains("Chip Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Chip Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Crack Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Crack Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Tile Replacement") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Tile Replacement") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Tile Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Grout Replacement") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Grout Replacement") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Grout Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Hard Water Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Hard Water Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Razor Blades").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Brick Paver Cleaner (Phosphoric Acid)").IsActive = true;
                }
                if (serviceTypeId.Contains("Mold Stain Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Mold Stain Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Mold & Mildew Cleaner").IsActive = true;
                }
                if (serviceTypeId.Contains("Lippage Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Lippage Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Razor Blades").IsActive = true;
                }
                if (serviceTypeId.Contains("Polishing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Polishing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel-7 inch-FLAT").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "CaCO/Gold Plus").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "IHG").IsActive = true;
                }
                if (serviceTypeId.Contains("Honing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Honing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "50").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "100").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "400").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "200").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "3000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "8000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "11000").IsActive = true;
                }
                if (serviceTypeId.Contains("Sealing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Sealing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.SEALER && x.TechnicianWorkOrder.Name == "Penetrating Sealer").IsActive = true;
                }
                if (serviceTypeId.Contains("Stain Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stain Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Stain Removal Kit (Poultice, Plastic)").IsActive = true;
                }
                if (serviceTypeId.Contains("Caulking") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Caulking") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Caulk Gun (Kit)").IsActive = true;
                }
                if (serviceTypeId.Contains("Coating") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Coating") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Coating Applicator").IsActive = true;
                }
            }

            if (invoice.ServiceType == "COUNTERLIFE" && materialType == "Granite")
            {
                if (serviceTypeId.Contains("Cleaning") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Cleaning") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Granite Cleaner").IsActive = true;
                }
                if (serviceTypeId.Contains("Cleaning - Address Mold Stains") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Cleaning - Address Mold Stains") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Mold & Mildew Cleaner").IsActive = true;
                }
                if (serviceTypeId.Contains("Chip Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Chip Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Brick Paver Cleaner (Phosphoric Acid)").IsActive = true;
                }
                if (serviceTypeId.Contains("Crack Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Crack Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Hard Water Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Hard Water Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Razor Blades").IsActive = true;
                }
                if (serviceTypeId.Contains("Tile Replacement") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Tile Replacement") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Tile Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Grout Replacement") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Grout Replacement") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Grout Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Traverfil") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Traverfil") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Traverfil").IsActive = true;
                }
                if (serviceTypeId.Contains("Stain Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Stain Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Stain Removal Kit (Poultice, Plastic)").IsActive = true;
                }
                if (serviceTypeId.Contains("Seam Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Seam Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Honing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Honing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "3000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "8000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "11000").IsActive = true;
                }
                if (serviceTypeId.Contains("Lippage Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Lippage Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Lippage Removal (UGC, UGP)").IsActive = true;
                }
                if (serviceTypeId.Contains("Polishing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Polishing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel 7 inch FLUFFY").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel-7 inch-FLAT").IsActive = true;
                }
                if (serviceTypeId.Contains("Sealing") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Sealing") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.SEALER && x.TechnicianWorkOrder.Name == "Penetrating Sealer").IsActive = true;
                }
                if (serviceTypeId.Contains("Caulking") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Caulking") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Caulk Gun (Kit)").IsActive = true;
                }
                if (serviceTypeId.Contains("Coating") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Coating") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Coating Applicator").IsActive = true;
                }
            }

            if (invoice.ServiceType == "COUNTERLIFE" && materialType == "Corian")
            {
                if (serviceTypeId.Contains("Corian-Scratch Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Corian-Scratch Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "3000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "8000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "11000").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
                }
                if (serviceTypeId.Contains("Corian-Polish") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Corian-Polish") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "3M Polishing Powder").IsActive = true;
                }
                if (serviceTypeId.Contains("Corian-Chip Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Corian-Chip Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Corian-Crack Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Corian-Crack Repair") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Corian Crack Repair Kit").IsActive = true;
                }
            }

            //if (invoice.ServiceType == "COUNTERLIFE" && materialType == "Corian")
            //{
            //    if (serviceTypeId.Contains("Corian-Scratch Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Corian-Scratch Removal") : true))
            //    {
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "400").IsActive = true;
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "3000").IsActive = true;
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "8000").IsActive = true;
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "11000").IsActive = true;
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
            //    }
            //    if (serviceTypeId.Contains("Corian-Polish") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Corian-Polish") : true))
            //    {
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "3M Polishing Powder").IsActive = true;
            //    }
            //    if (serviceTypeId.Contains("Corian-Chip Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Corian-Chip Repair") : true))
            //    {
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
            //    }
            //    if (serviceTypeId.Contains("Corian-Crack Repair") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Corian-Crack Repair") : true))
            //    {
            //        technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Corian Crack Repair Kit").IsActive = true;
            //    }
            //}

            if (invoice.ServiceType == "COUNTERLIFE" && materialType == "Engineered Stone")
            {
                if (serviceTypeId.Contains("Engineered Stone - Scratch Removal") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Engineered Stone - Scratch Removal") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Soap & Scum Remover").IsActive = true;
                }
                if (serviceTypeId.Contains("Engineered Stone - Polish") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Engineered Stone - Polish") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Soap & Scum Remover").IsActive = true;
                }
            }
            if (invoice.ServiceType == "COUNTERLIFE" && materialType == "Engineered Stone")
            {
                if (serviceTypeId.Contains("Hone") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Hone") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "200").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "400").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
                }
                if (serviceTypeId.Contains("Cleanshield") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Cleanshield") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "CleanShield Kit (Light & Resin)").IsActive = true;
                }
            }

            if (invoice.ServiceType == "CLEANSHIELD")
            {
                if (serviceTypeId.Contains("Hone") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Hone") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "200").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "400").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "800").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.HANDDIAMOND && x.TechnicianWorkOrder.Name == "1800").IsActive = true;
                }
                if (serviceTypeId.Contains("Cleanshield") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Cleanshield") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "CleanShield Kit (Light & Resin)").IsActive = true;
                }
            }
            if (invoice.ServiceType == "GROUTLIFE/TILELOK" && (materialType == "Ceramic" || materialType == "Porcelain" ||
            materialType == "Concrete" || materialType == "Terrazzo"))
            {
                if (serviceTypeId.Contains("Tilelok"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Tilelok").IsActive = true;
                }
            }
            if ((invoice.ServiceType == "CONCRETE-REPAIR" || invoice.ServiceType == "CONCRETE-REPAIR\r") && (materialType == "Concrete" || materialType == "Terrazzo"))
            {
                if (serviceTypeId.Contains("Concrete Chip Repair"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Concrete Crack Repair").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Concrete Crack Repair").IsActive = true;
                }
                if (serviceTypeId.Contains("Concrete Crack Repair"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Concrete Crack Repair").IsActive = true;
                }
                if (serviceTypeId.Contains("Lippage Removal"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Lippage Removal (UGC, UGP)").IsActive = true;
                }
                if (serviceTypeId.Contains("Terrazzo Chip Repair"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Terrazzo Crack Repair"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Expansion Joint Repairs"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Expansion Joint Repair").IsActive = true;
                }
                if (serviceTypeId.Contains("Expansion Joint Sealing"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Expansion Joint Repair").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping Carpet Adhesive"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Chip Repair Kit").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Silicone"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "PLC-40 - Silicone Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Urethane"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "JASCO/MECL-Urethane Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Wax"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "Butyl-Wax Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
                if (serviceTypeId.Contains("Stripping-Unknown") || serviceTypeId.Contains("Stripping-Unknown/r"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "Butyl-Wax Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "JASCO/MECL - Urethane Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "PLC-40 - Silicone Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "Carpet Adhesive Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.BRUSHES && x.TechnicianWorkOrder.Name == "Brush-Orange").IsActive = true;
                }
            }
            if (invoice.ServiceType == "CONCRETE-COATINGS" && (materialType == "Concrete" || materialType == "Terrazzo"))
            {
                if (serviceTypeId.Contains("Concrete Chip Repair"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Mold & Mildew Cleaner").IsActive = true;
                }
                if (serviceTypeId.Contains("Hone Surface Prep"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.FLOORDIAMOND && x.TechnicianWorkOrder.Name == "200").IsActive = true;
                }
                if (serviceTypeId.Contains("MOISTURE VAPOR BARRIER"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Moisture Vapro Barrier").IsActive = true;
                }
                if (serviceTypeId.Contains("Epoxy Base"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Epoxy Concrete Base Coat").IsActive = true;
                }
                if (serviceTypeId.Contains("Polyspartic Base"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Polyspartic Top Coat").IsActive = true;
                }
                if (serviceTypeId.Contains("Vinyl Chips"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CHIPS && x.TechnicianWorkOrder.Name == "Endurachip (Vinyl)").IsActive = true;
                }
                if (serviceTypeId.Contains("Epoxy Top Coat"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Epoxy Concrete Top Coat").IsActive = true;
                }
                if (serviceTypeId.Contains("MARBLIZED COATING"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Polyspartic Top Coat").IsActive = true;
                }
                if (serviceTypeId.Contains("Anti-Spauling Driveway Sealing"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.SEALER && x.TechnicianWorkOrder.Name == "Concrete Sealing (Anti-Spauling)").IsActive = true;
                }
                if (serviceTypeId.Contains("Anti-Spauling Sideway Sealing"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.SEALER && x.TechnicianWorkOrder.Name == "Concrete Sealing (Anti-Spauling)").IsActive = true;
                }
            }
            if (invoice.ServiceType == "CONCRETE-COUNTERTOPS")
            {
                if (serviceTypeId.Contains("Marblized Top Coat Installation"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Polyspartic Top Coat").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Coating Applicator").IsActive = true;
                }
            }

            if (invoice.ServiceType == "CARPETLIFE")
            {
                if (serviceTypeId.Contains("Carpet Cleaning"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Carpet Cleaning Cleaner").IsActive = true;
                }
                if (serviceTypeId.Contains("Carpet Guard"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Carpet Stain Guard").IsActive = true;
                }
            }

            if (invoice.ServiceType == "CLEANAIR")
            {
                if (serviceTypeId.Contains("Clean Air Purification Treatments"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "CleanAIR Kit").IsActive = true;
                }
            }

            if (invoice.ServiceType == "TILEINSTALL")
            {
                if (serviceTypeId.Contains("Tile Removal"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Tile Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Tile Installation"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Tile Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Grouting"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.KITS && x.TechnicianWorkOrder.Name == "Grout Replacement Kit").IsActive = true;
                }
                if (serviceTypeId.Contains("Grout Haze Removal"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-White").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Pre-Care").IsActive = true;
                }
                if (serviceTypeId.Contains("Grout Sealing-Clear"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Pre-Care").IsActive = true;
                }
                if (serviceTypeId.Contains("Grout & Tile Sealing"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.SEALER && x.TechnicianWorkOrder.Name == "Penetrating Sealer").IsActive = true;
                }
            }

            if (invoice.ServiceType == "VINYLGUARD")
            {
                if (serviceTypeId.Contains("Stripping Wax"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.STRIPPING && x.TechnicianWorkOrder.Name == "Butyl-Wax Stripper").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Black").IsActive = true;
                }
                if (serviceTypeId.Contains("Waxing"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Wax").IsActive = true;
                }
                if (serviceTypeId.Contains("Cleaning and Pre-Coat Preparations"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Granite Cleaner").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Denatured Alcohol").IsActive = true;
                }
                if (serviceTypeId.Contains("VinylGuard Installation"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "VinylGuard").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Coating Applicator").IsActive = true;
                }
            }

            if (invoice.ServiceType == "WOODLIFE")
            {
                if (serviceTypeId.Contains("Staining"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Wood Stain").IsActive = true;
                }
                if (serviceTypeId.Contains("Top Coat"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.COATING && x.TechnicianWorkOrder.Name == "Wood Urethane Re-Coat").IsActive = true;
                }
            }

            if (invoice.ServiceType == "PRODUCT")
            {
                if (serviceTypeId.Contains("Granite Cleaner 32 oz Spray"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Granite Cleaner").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Granite Cleaner 32 oz Spray").IsActive = true;
                }
                if (serviceTypeId.Contains("Granite Cleaner Refill Gallon"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Granite Cleaner").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Granite Cleaner Refill Gallon").IsActive = true;
                }
                if (serviceTypeId.Contains("Granite Gloss conditioner  8oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Granite Gloss conditioner  8oz").IsActive = true;
                }
                if (serviceTypeId.Contains("Granite Sealer 4 oz Spray"))
                {
                    var careProducts = technicianWorkOrderInvoice.Where(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS).ToList();
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Granite Sealer 4 oz Spray").IsActive = true;
                }
                if (serviceTypeId.Contains("Marble Cleaner 32 oz Spray"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Marble Cleaner 32 oz Spray").IsActive = true;
                }
                if (serviceTypeId.Contains("Marble cleaner Refill Gallon"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Marble cleaner Refill Gallon").IsActive = true;
                }
                if (serviceTypeId.Contains("Marble cleaner Conc. 32oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Marble cleaner Conc. 32oz").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Marble Cleaner Concentrate").IsActive = true;
                }
                if (serviceTypeId.Contains("Marble Polish 16 oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Marble Polish 16 oz").IsActive = true;
                }
                if (serviceTypeId.Contains("Marble Gloss conditioner 16oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Marble Gloss conditioner 16oz").IsActive = true;
                }
                if (serviceTypeId.Contains("Stone Sealer 16 oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Stone Sealer 16 oz").IsActive = true;
                }
                if (serviceTypeId.Contains("Tile Cleaner 32oz Spray"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Tile Cleaner 32oz Spray").IsActive = true;
                }
                if (serviceTypeId.Contains("Tile Cleaner Refill Gallon"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Tile Cleaner Refill Gallon").IsActive = true;
                }
                if (serviceTypeId.Contains("Tile Cleaner Concentrate 32 oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Tile Cleaner Concentrate 32 oz").IsActive = true;
                }
                if (serviceTypeId.Contains("MaxOut Grout Cleaner 32 oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "MaxOut Grout Cleaner 32 oz").IsActive = true;
                }
                if (serviceTypeId.Contains("MaxOut Concentrate Gallon"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "MaxOut Concentrate Gallon").IsActive = true;
                }
                if (serviceTypeId.Contains("Grout Sealer 16 oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Grout Sealer 16 oz").IsActive = true;
                }
                if (serviceTypeId.Contains("Floor Cleaner Concentrate Gallon"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Floor Cleaner Concentrate Gallon").IsActive = true;
                }
                if (serviceTypeId.Contains("Mold & Mildew Stain Remover 32oz Spray"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Mold & Mildew Stain Remover 32oz Spray").IsActive = true;
                }
                if (serviceTypeId.Contains("Mold & Mildew Stain Remover Gallon"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Mold & Mildew Stain Remover Gallon").IsActive = true;
                }
                if (serviceTypeId.Contains("Soap Scum Remover 16oz"))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CAREPRODUCTS && x.TechnicianWorkOrder.Name == "Soap Scum Remover 16oz").IsActive = true;
                }
            }

            if (invoice.ServiceType == "MAINTENANCE" || invoice.ServiceType == "MAINTAINCE:MONTHLY" ||
                invoice.ServiceType == "MAINTAINCE:QUARTERLY" || invoice.ServiceType == "MAINTAINCE:OTHER" || invoice.ServiceType == "MAINTAINCE:BI-MONTHLY")
            {
                if (serviceTypeId.Contains("MAINTAINCE:Monthly") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("MAINTAINCE:Monthly") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel-22 inch FLAT").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel 22 inch FLUFFY").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "CaCO/Gold Plus").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+ Blk").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+wh").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "IHG").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Pre-Care").IsActive = true;
                }
                if (serviceTypeId.Contains("MAINTAINCE:Bi-Monthly") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("MAINTAINCE:Bi-Monthly") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel-22 inch FLAT").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel 22 inch FLUFFY").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "CaCO/Gold Plus").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+ Blk").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+wh").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "IHG").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Pre-Care").IsActive = true;
                }
                if (serviceTypeId.Contains("MAINTAINCE:Quarterly") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("MAINTAINCE:Quarterly") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel-22 inch FLAT").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel 22 inch FLUFFY").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "CaCO/Gold Plus").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+ Blk").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+wh").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "IHG").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Pre-Care").IsActive = true;
                }
                if (serviceTypeId.Contains("Maintenance: (Other)") || (subServiceTypeId.Count() > 0 ? subServiceTypeId.Contains("Maintenance: (Other)") : true))
                {
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Pad-Tan").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel-22 inch FLAT").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.PADS && x.TechnicianWorkOrder.Name == "Steel 22 inch FLUFFY").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "CaCO/Gold Plus").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+ Blk").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "Pt+wh").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.POLISH && x.TechnicianWorkOrder.Name == "IHG").IsActive = true;
                    technicianWorkOrderInvoice.FirstOrDefault(x => x.TechnicianWorkOrder.WorkOrderId == (long)TechnicianWorkOrderInvoiceType.CLEANER && x.TechnicianWorkOrder.Name == "Pre-Care").IsActive = true;
                }
            }


            return technicianWorkOrderInvoice;

        }


    }
}
