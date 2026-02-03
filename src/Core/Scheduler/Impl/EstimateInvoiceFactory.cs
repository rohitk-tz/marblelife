
using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Extensions;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class EstimateInvoiceFactory : IEstimateInvoiceFactory
    {
        private readonly IRepository<Lookup> _lookupRepository;
        private readonly IRepository<EstimateInvoiceDimension> _estimateInvoiceDimensionRepository;
        private readonly IRepository<EstimateInvoiceServiceImage> _estimateInvoiceServiceImageRepository;
        private readonly IRepository<ServicesTag> _servicesTagRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EstimateInvoiceFactory(IUnitOfWork unitOfWork)
        {
            _lookupRepository = unitOfWork.Repository<Lookup>();
            _estimateInvoiceDimensionRepository = unitOfWork.Repository<EstimateInvoiceDimension>();
            _servicesTagRepository = unitOfWork.Repository<ServicesTag>();
            _estimateInvoiceServiceImageRepository = unitOfWork.Repository<EstimateInvoiceServiceImage>();
            _unitOfWork = unitOfWork;
        }
        public EstimateInvoiceService CreateDomain(EstimateInvoiceServiceEditModel model)
        {
            string descriptionString = model.Description != null ? model.Description : "";
            string locationString = model.LocationIds.Count() > 0 ? string.Join(",", model.LocationIds.Select(x => x.Id)) : "";
            string serviceNameString = model.ServiceIds1 != null ? CreateStringList(model.ServiceIds1, true) : "";
            string typeOfStoneColorString = model.TypeOfStoneColor1 != null ? string.Join(",", model.TypeOfStoneColor1) : "";
            string typeOfStoneTypeString = model.TypeOfStoneType1 != null ? string.Join(",", model.TypeOfStoneType1) : "";
            string typeOfServiceString = model.TypeOfSurface1 != null ? string.Join(",", model.TypeOfSurface1) : "";
            string typeOfStoneType3String = model.typeOfStoneType3 != null ? string.Join(",", model.typeOfStoneType3) : "";
            var domain = new EstimateInvoiceService()
            {
                DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                Description = descriptionString,
                Location = locationString,
                ServiceName = serviceNameString,
                ServiceType = model.ServiceType,
                StoneColor = typeOfStoneColorString,
                StoneType = typeOfStoneTypeString,
                TypeOfService = typeOfServiceString,
                Option1 = string.Format("{0:0.##}", model.Option1),
                Option2 = string.Format("{0:0.##}", model.Option2),
                Option3 = string.Format("{0:0.##}", model.Option3),
                InvoiceNumber = model.InvoiceNumber,
                IsNew = model.Id == 0 ? true : false,
                Notes = model.Notes,
                StoneType2 = typeOfStoneType3String,
                PriceNotes = model.PriceNotes,
                IsCross = model.IsCross.GetValueOrDefault(),
                IsBundle = model.IsBundle.GetValueOrDefault(),
                BundleName = model.BundleName,
                IsActive = model.IsActive.GetValueOrDefault(),
                IsMainBundle = model.IsMainBundle.GetValueOrDefault(),
                Alias = model.Alias
            };
            return domain;
        }
        public EstimateInvoice CreateDomain(EstimateInvoiceEditModel model)
        {
            var domain = new EstimateInvoice()
            {
                DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                EstimateId = model.EstimateId,
                SchedulerId = model.SchedulerId,
                InvoiceCustomerId = model.InvoiceCustomerId,
                CustomerId = model.CustomerId,
                FranchiseeId = model.FranchiseeId,
                IsNew = model.Id == 0 ? true : false,
                NumberOfInvoices = model.NumberOfInvoices,
                Option = model.Option,
                PriceOfService = model.Price,
                LessDeposit = model.LessDeposit,
                Notes = model.Notes,
                Option1 = model.Option1,
                Option2 = model.Option2,
                Option3 = model.Option3,
                IsInvoiceChanged = true
            };
            return domain;
        }
        public EstimateInvoiceCustomer CreateDomainForCustomer(EstimateInvoiceEditModel model)
        {
            var domain = new EstimateInvoiceCustomer()
            {
                CityName = model.City,
                PhoneNumber1 = model.PhoneNumber1,
                PhoneNumber2 = model.PhoneNumber2,
                StateName = model.StateName,
                Email = model.Email,
                StreetAddress = model.Address,
                CustomerName = model.CustomerName,
                DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                IsNew = model.Id == 0 ? true : false,


            };
            return domain;
        }

        public EstimateInvoiceServiceEditMailModel CreateMailViewModel(EstimateInvoiceService estimateInvoiceService, List<EstimateInvoiceService> subList, List<TermsAndConditionFranchisee> termsList)
        {
            var template = "";
            var option1Splitted = (estimateInvoiceService.Option1).Split(' ');
            var option2Splitted = (estimateInvoiceService.Option2).Split(' ');
            var option3Splitted = (estimateInvoiceService.Option3).Split(' ');
            if (estimateInvoiceService.ServiceType.StartsWith("CONCRETE") || estimateInvoiceService.ServiceType.StartsWith("ENDURACRETE"))
            {
                template = termsList.FirstOrDefault(x => x.TyepeId == 240).TermAndCondition;
            }
            else
            {
                template = termsList.FirstOrDefault(x => x.TyepeId == 241).TermAndCondition;
            }

            var subItemSum = subList.Select(x => CreateViewModelForSubItem(x, null, null)).ToList();
            var subItemTotalSumOption1 = subItemSum.Sum(x => decimal.Parse(x.Option1));
            var subItemTotalSumOption2 = subItemSum.Sum(x => decimal.Parse(x.Option2));
            var subItemTotalSumOption3 = subItemSum.Sum(x => decimal.Parse(x.Option3));
            var locationSplitt = estimateInvoiceService.Location.Split(',').ToList();
            var locationids = estimateInvoiceService.Location != "" ? estimateInvoiceService.Location.Split(',').Select(x => CreateListModel(x)).ToList() : new List<ListViewModel>();
            var editMailModel = new EstimateInvoiceServiceEditMailModel()
            {
                Description = estimateInvoiceService.Description != "" ? estimateInvoiceService.Description : "",
                LocationName = string.Join(", ",locationSplitt),
                Option1 = option1Splitted[0].ToString(),
                Option2 = option2Splitted[0].ToString(),
                Option3 = option3Splitted[0].ToString(),
                TypeOfService = estimateInvoiceService.TypeOfService != "" ? estimateInvoiceService.TypeOfService : "",
                TypeOfStoneColor = estimateInvoiceService.StoneColor != "" ? estimateInvoiceService.StoneColor : "",
                TypeOfStoneType = estimateInvoiceService.StoneType != "" ? estimateInvoiceService.StoneType : "",
                typeOfStoneType3 = estimateInvoiceService.StoneType2 != "" ? estimateInvoiceService.StoneType2 : "",
                TypeOfSurface = estimateInvoiceService.TypeOfService != "" ? estimateInvoiceService.TypeOfService : "",
                ServiceIds = estimateInvoiceService.ServiceName != "" ? estimateInvoiceService.ServiceName : "",
                ServiceType = estimateInvoiceService.ServiceType != "" ? estimateInvoiceService.ServiceType : "",
                SubItem = subList.Select(x => CreateViewModelForSubItem(x, null, null)).ToList(),
                Template = template,
                Notes = estimateInvoiceService.PriceNotes != null ? estimateInvoiceService.PriceNotes : "",
                BackColor = estimateInvoiceService.IsCross ? "red" : "transparent",
                Option1Total = subItemSum.Sum(x => decimal.Parse(x.Option1)),
                Option2Total = subItemSum.Sum(x => decimal.Parse(x.Option2)),
                Option3Total = subItemSum.Sum(x => decimal.Parse(x.Option3)),
                LocationIds = locationids,
                SubItemTotalSumOption1 = subItemTotalSumOption1,
                SubItemTotalSumOption2 = subItemTotalSumOption2,
                SubItemTotalSumOption3 = subItemTotalSumOption3,
                SubItemNotesCount = subList.Where(x => !string.IsNullOrEmpty(x.PriceNotes)).Count()
            };
            return editMailModel;
        }

        public EstimateInvoiceServiceViewModel CreateViewModel(EstimateInvoiceService estimateInvoiceService,
                                                           List<EstimateInvoiceServiceDescriptionViewModel> estimateInvoiceServiceDescriptionViewModel,
                                                           EstimateInvoice estimateInvoice, List<EstimateInvoiceService> subItem, List<EstimateInvoiceDimension> estimateInvoiceDimensions, List<EstimateInvoiceAssignee> assigneeList, HoningMeasurement honingMeasurement, List<HoningMeasurement> honingMeasurementList, List<HoningMeasurementDefault> honingMeasurementDefaultList)
        {
            var honingMeasurementListLocal = honingMeasurementList.Where(x => x.EstimateInvoiceServiceId == estimateInvoiceService.Id).ToList();
            var honingMeasurementDefaultListLocal = honingMeasurementDefaultList.Where(x => x.EstimateInvoiceServiceId == estimateInvoiceService.Id).ToList();
            if (subItem.Where(x => !x.IsBundle).Count() > 0)
            {
                var serviceNameList = subItem.Where(x => !x.IsBundle).Select(x => x.ServiceName).ToList();
                string subItemString = subItem != null ? string.Join(",", serviceNameList) : "";
                estimateInvoiceService.ServiceName = estimateInvoiceService.ServiceName + "," + subItemString;
            }

            var serviceNameSplit = estimateInvoiceService.ServiceName != null ? estimateInvoiceService.ServiceName.Split(',').Select(x => CreateListModel(x)).ToList() : new List<ListViewModel>();
            var locationList = estimateInvoiceService.Location != "" ? estimateInvoiceService.Location : "";
            var stoneColorList = estimateInvoiceService.StoneColor != "" ? estimateInvoiceService.StoneColor : "";
            var stoneTypeList = estimateInvoiceService.StoneType != "" ? estimateInvoiceService.StoneType : "";
            var typeOfServiceList = estimateInvoiceService.TypeOfService != "" ? estimateInvoiceService.TypeOfService : "";
            var descriptionList = estimateInvoiceService.Description != "" ? new List<string>() { estimateInvoiceService.Description } : new List<string>();
            var stoneType2List = estimateInvoiceService.StoneType2 != "" ? estimateInvoiceService.StoneType2 : "";
            var locationids = estimateInvoiceService.Location != "" ? estimateInvoiceService.Location.Split(',').Select(x => CreateListModel(x)).ToList() : new List<ListViewModel>();
            var estimateInvoiceDimension = estimateInvoiceDimensions.Where(x => x.EstimateInvoiceServiceId == estimateInvoiceService.Id).ToList();
            var user = assigneeList.Where(x => x.InvoiceNumber == estimateInvoiceService.InvoiceNumber).Select(x => new InvoiceAssigneeModel()
            {
                Name = x.User.Person != null ? x.User.Person.FirstName + " " + x.User.Person.LastName : ""
            }).ToList();

            var images = _estimateInvoiceServiceImageRepository.Table.Where(x => x.EstimateInvoiceServiceId == estimateInvoiceService.Id).ToList();
            var image = images.Select(x => new EstimateInvoiceServiceImageModel()
            {
                Caption = x.File != null ? x.File.Name: "",
                RelativeLocation = x.File != null ? (x.File.RelativeLocation + "\\" + x.File.Name).ToUrl() : "",
                FileId = x.File != null ? x.File.Id : default(long),
                IsUploadedImage = true
            }).ToList();

            var serviceTypeId = GetMeasurementType(estimateInvoiceService);
            var invoiceService = new EstimateInvoiceServiceViewModel()
            {
                IsAlias = estimateInvoiceService.Alias != null && estimateInvoiceService.Alias != "" ? true : false,
                Alias = estimateInvoiceService.Alias != null ? estimateInvoiceService.Alias : "",
                ServiceIds = serviceNameSplit,
                LocationName = locationList,
                TypeOfStoneColor = stoneColorList,
                ServiceType = estimateInvoiceService.ServiceType,
                TypeOfStoneType = stoneTypeList,
                TypeOfSurface = typeOfServiceList,
                Id = estimateInvoiceService.Id,
                LessDeposit = estimateInvoice.LessDeposit,
                Option1 = estimateInvoiceService.Option1,
                Option2 = estimateInvoiceService.Option2,
                Option3 = estimateInvoiceService.Option3,
                InvoiceNumber = estimateInvoiceService.InvoiceNumber.ToString(),
                OldInvoiceNumber = estimateInvoiceService.InvoiceNumber.ToString(),
                Notes = estimateInvoiceService.Notes,
                TypeOfStoneType2 = stoneType2List,
                IsExpand = false,
                SubItem = subItem.Select(x => CreateViewModelForSubItem(x, honingMeasurementList, honingMeasurementDefaultList)).ToList(),
                Description = descriptionList.Count() > 0 ? descriptionList[0] : "",
                PriceNotes = estimateInvoiceService.PriceNotes,
                IsCross = estimateInvoiceService.IsCross,
                BorderColor = estimateInvoiceService.IsCross ? "red" : "transparent",
                IsBundle = estimateInvoiceService.IsBundle,
                IsActive = estimateInvoiceService.IsActive,
                BundleName = estimateInvoiceService.BundleName,
                IsMainBundle = estimateInvoiceService.IsMainBundle,
                LocationIds = locationids,
                Measurements = estimateInvoiceDimension.Select(x => CreateEstimateInvoiceDimensionViewModel(x)).ToList(),
                AssigneeName = String.Join(", ", user.Select(x => x.Name)),
                ServiceTypeId = serviceTypeId,
                UnitTypeId = serviceTypeId != null ? GetUnitTypeId(serviceTypeId) : null,
                UnitType = serviceTypeId != null ? GetUnitType(serviceTypeId) : "",
                HoningMeasurement = honingMeasurement != null ? CreateHoningViewModel(honingMeasurement, honingMeasurementDefaultListLocal.FirstOrDefault()) : null,
                ImageList = image,
                HoningMeasurementList = honingMeasurementListLocal.Count() > 0 ? honingMeasurementListLocal.Select(x => CreateHoningViewModel(x, honingMeasurementDefaultListLocal.FirstOrDefault(y=>y.HoningMeasurementId == x.Id))).ToList() : null
            };

            return invoiceService;
        }
        private ListViewModel CreateListModel(string value)
        {
            if (value == "")
            {
                return new ListViewModel();
            }
            return new ListViewModel()
            {
                Id = value
            };
        }

        private string CreateStringList(List<ListViewModel> serviceList, bool isTakeFirstElement)
        {
            string serviceString = "";
            foreach (var service in serviceList)
            {
                if (serviceString == "")
                {
                    serviceString = service.Id;

                }
                else
                {
                    serviceString += "," + service.Id;
                }
                if (isTakeFirstElement)
                {
                    break;
                }
            }
            return serviceString;

        }

        public SubItemEditModel CreateViewModelForSubItem(EstimateInvoiceService estimateInvoiceServices, List<HoningMeasurement> honingMeasurementList, List<HoningMeasurementDefault> honingMeasurementDefaultList)
        {
            var HoningMeasurementLocal = default(HoningMeasurement);
            var HoningMeasurementDefaultLocal = default(HoningMeasurementDefault);
            var serviceNameSplit = estimateInvoiceServices.ServiceName.Split(',').Select(x => CreateListModel(x)).ToList();

            var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceServiceId == estimateInvoiceServices.Id).ToList();
            var estimateInvoiceImages = _estimateInvoiceServiceImageRepository.Table.Where(x => x.EstimateInvoiceServiceId == estimateInvoiceServices.Id).ToList();

            var option1Splitted = (estimateInvoiceServices.Option1).Split(' ');
            var option2Splitted = (estimateInvoiceServices.Option2).Split(' ');
            var option3Splitted = (estimateInvoiceServices.Option3).Split(' ');
            var serviceTypeId = GetMeasurementType(estimateInvoiceServices);
            if (honingMeasurementList != null && honingMeasurementList.Count() > 0)
            {
                HoningMeasurementLocal = honingMeasurementList.FirstOrDefault(x => x.EstimateInvoiceServiceId == estimateInvoiceServices.Id);
            }
            if (honingMeasurementDefaultList != null && honingMeasurementDefaultList.Count() > 0)
            {
                HoningMeasurementDefaultLocal = honingMeasurementDefaultList.FirstOrDefault(x => x.EstimateInvoiceServiceId == estimateInvoiceServices.Id);
            }
            var subItem = new SubItemEditModel()
            {
                Alias = "",
                Description = estimateInvoiceServices.Description,
                Notes = estimateInvoiceServices.Notes,
                Option1 = option1Splitted[0].ToString(),
                Option2 = option2Splitted[0].ToString(),
                Option3 = option3Splitted[0].ToString(),
                Id = estimateInvoiceServices.Id,
                ServiceIds = serviceNameSplit.FirstOrDefault(),
                PriceNotes = estimateInvoiceServices.PriceNotes,
                IsCross = estimateInvoiceServices.IsCross,
                BorderColor = estimateInvoiceServices.IsCross ? "red" : "transparent",
                IsActive = estimateInvoiceServices.IsActive,
                IsBundle = estimateInvoiceServices.IsBundle,
                BundleName = estimateInvoiceServices.BundleName,
                IsMainBundle = estimateInvoiceServices.IsMainBundle,
                ServiceTypeId = serviceTypeId,
                UnitTypeId = GetUnitTypeId(serviceTypeId),
                Measurements = estimateInvoiceServiceMeasurements.Select(x => CreateEstimateInvoiceDimensionViewModel(x, GetUnitTypeId(serviceTypeId))).ToList(),
                ImageList = estimateInvoiceImages.Select(x => CreateEstimateInvoiceImageViewModel(x)).ToList(),
                UnitType = GetUnitType(serviceTypeId),
                HoningMeasurement = (honingMeasurementList != null && honingMeasurementList.Count() > 0 && HoningMeasurementLocal != null) ? CreateHoningViewModel(HoningMeasurementLocal, HoningMeasurementDefaultLocal) : null
            };
            return subItem;
        }

        public EstimateInvoiceDimensionViewModel CreateEstimateInvoiceDimensionViewModel(EstimateInvoiceDimension estimateInvoiceServiceDimension, long? unitTypeId = null)
        {
            var dimension = new EstimateInvoiceDimensionViewModel()
            {
                Length = estimateInvoiceServiceDimension.Length,
                Width = estimateInvoiceServiceDimension.Width,
                Area = estimateInvoiceServiceDimension.Area,
                AreaTime = estimateInvoiceServiceDimension.AreaTime,
                Description = estimateInvoiceServiceDimension.Description,
                UnitId = unitTypeId != null ? unitTypeId : null,
                UnitType = estimateInvoiceServiceDimension.UnitType != null ? estimateInvoiceServiceDimension.UnitType.Name : "",
                IncrementedPrice = estimateInvoiceServiceDimension.IncrementedPrice,
                SetPrice = estimateInvoiceServiceDimension.SetPrice,
                IsSaved = true,
                Dimension = estimateInvoiceServiceDimension.Dimension != null ? estimateInvoiceServiceDimension.Dimension.ToString() : null
            };
            return dimension;
        }

        public EstimateInvoiceService CreateDomainModel(EstimateInvoiceServiceEditModel estimateInvoiceEditServices, SubItemEditModel subItem)
        {
            string locationString = estimateInvoiceEditServices.LocationName1 != null ? string.Join(",", estimateInvoiceEditServices.LocationName1) : "";
            string serviceNameString = estimateInvoiceEditServices.ServiceIds1 != null ? CreateStringList(estimateInvoiceEditServices.ServiceIds1, false) : "";
            string typeOfStoneColorString = estimateInvoiceEditServices.TypeOfStoneColor1 != null ? string.Join(",", estimateInvoiceEditServices.TypeOfStoneColor1) : "";
            string typeOfStoneTypeString = estimateInvoiceEditServices.TypeOfStoneType1 != null ? string.Join(",", estimateInvoiceEditServices.TypeOfStoneType1) : "";
            string typeOfServiceString = estimateInvoiceEditServices.TypeOfSurface1 != null ? string.Join(",", estimateInvoiceEditServices.TypeOfSurface1) : "";
            string typeOfStoneType3String = estimateInvoiceEditServices.typeOfStoneType3 != null ? string.Join(",", estimateInvoiceEditServices.typeOfStoneType3) : "";

            var option1Splitted = (subItem.Option1).Split(' ');
            var option2Splitted = (subItem.Option2).Split(' ');
            var option3Splitted = (subItem.Option3).Split(' ');
            var estimateInvoiceService = new EstimateInvoiceService()
            {
                Id = subItem.Id.GetValueOrDefault(),
                DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                Description = subItem.Description,
                Option1 = option1Splitted[0].ToString(),
                Option2 = option2Splitted[0].ToString(),
                Option3 = option3Splitted[0].ToString(),
                InvoiceNumber = estimateInvoiceEditServices.InvoiceNumber,
                Location = locationString,
                ServiceName = subItem.ServiceIds.Id,
                ServiceType = estimateInvoiceEditServices.ServiceType,
                StoneColor = typeOfStoneColorString,
                StoneType = typeOfStoneTypeString,
                TypeOfService = typeOfServiceString,
                ParentId = estimateInvoiceEditServices.Id.GetValueOrDefault(),
                StoneType2 = typeOfStoneType3String,
                IsNew = subItem.Id == 0 || subItem.Id == null ? true : false,
                Notes = subItem.Notes,
                PriceNotes = subItem.PriceNotes,
                IsCross = subItem.IsCross.GetValueOrDefault(),
                IsBundle = subItem.IsBundle.GetValueOrDefault(),
                IsActive = subItem.IsActive.GetValueOrDefault(),
                BundleName = subItem.BundleName,
                IsMainBundle = subItem.IsMainBundle.GetValueOrDefault()
            };
            return estimateInvoiceService;

        }

        public TechnicianWorkOrderForInvoice CreateDomain(TechnicianWorkOrder model, long? estimateInvoiceId)
        {
            model.Name = model.Name.Replace(@"\", "");
            var domain = new TechnicianWorkOrderForInvoice()
            {
                TechnicianWorkOrderId = model.WorkOrderId,
                IsActive = false,
                EstimateinvoiceId = estimateInvoiceId,
                TechnicianWorkOrder = model,
                InvoiceNumber = 0
            };
            return domain;
        }

        public EstimateInvoiceDimension CreateDomain(EstimateInvoiceDimensionEditModel model, long? estimateInvoiceServiceId)
        {
            var domain = new EstimateInvoiceDimension()
            {
                EstimateInvoiceServiceId = estimateInvoiceServiceId,
                IsNew = true,
                Length = model.Length,
                Width = model.Width,
                Area = model.Area,
                AreaTime = model.AreaTime,
                Description = model.Description,
                IncrementedPrice = model.IncrementedPrice,
                SetPrice = model.SetPrice,
                UnitTypeId = model.UnitId,
                //UnitType = model.UnitType
                Dimension = long.TryParse(model.Dimension, out var result) ? (long?)result : null
        };
            return domain;
        }

        public ListViewModelForWorkOrder CreateViewModel(TechnicianWorkOrderForInvoice domain)
        {
            var value = domain.TechnicianWorkOrder.Name;
            if (value == "Colorseal-1 (Tile Only - De-emphasize)")
            {
                value = "Colorseal-1(Tile Only-" + "<span style='margin-left:30px!important'> De-(emphasize)</span>";
            }
            else if (value == "Brick Paver Cleaner (Phosphoric Acid)")
            {
                value = "Brick Paver Cleaner" + "<span style='margin-left:30px!important'> (Phosphoric Acid)</span>";
            }
            else if (value == "Floor Cleaner Concentrate Gallon")
            {
                value = "Floor Cleaner Concentrate" + "<span style='margin-left:30px!important'> Gallon</span>";
            }
            else if (value == "Mold & Mildew Stain Remover 32oz Spray")
            {
                value = "Mold & Mildew Stain Remover" + "<span style='margin-left:30px!important'> 32oz Spray</span>";
            }
            else if (value == "Mold & Mildew Stain Remover Gallon")
            {
                value = "Mold & Mildew Stain Remover" + "<span style='margin-left:30px!important'> Gallon</span>";
            }
            else if (value == "Stain Removal Kit (Poultice, Plastic)")
            {
                value = "Stain Removal Kit (Poultice," + "<span style='margin-left:30px!important'> Plastic</span>";
            }
            var viewModel = new ListViewModelForWorkOrder()
            {
                CategoryName = domain.TechnicianWorkOrder != null ? domain.TechnicianWorkOrder.WorkOrder.Name : "",
                Name = value,
                IsPresent = domain.IsActive,
                InvoiceNumber = domain.InvoiceNumber
            };
            return viewModel;
        }

        public class InvoiceAssigneeModel
        {
            public string Name { get; set; }
        }

        private long? GetMeasurementType(EstimateInvoiceService service)
        {
            var splittedServices = service.ServiceName.Split(',');
            var isBundle = service.ServiceName.StartsWith("Bundle");
            if (!isBundle)
            {
                var servicestag = _servicesTagRepository.Table.Where(x => x.ServiceType.Name == service.ServiceType && x.MaterialType == service.StoneType2 && splittedServices.Contains(x.Service)).FirstOrDefault();
                if ((service.ServiceType == "COUNTERLIFE" && service.StoneType2 == "Engineered Stone") && (splittedServices.Contains("Engineered Stone-Polish") || splittedServices.Contains("Engineered Stone-Scratch Removal")))
                {
                    return (long)MeasurementEnum.TIME;
                }
                else if (servicestag != null)
                    return servicestag.CategoryId;
                else
                    return (long)MeasurementEnum.AREA;
            }
            else
            {
                var servicestag = _servicesTagRepository.Table.Where(x => x.ServiceType.Name == service.ServiceType && x.MaterialType == service.StoneType2 && splittedServices.Contains(x.Service)).FirstOrDefault();
                if (servicestag != null)
                    return servicestag.CategoryId;
                else
                    return (long)MeasurementEnum.TIME;
            }



            //if ((service.ServiceType == "STONELIFE" || service.ServiceType == "COUNTERLIFE" || service.ServiceType == "CLEANSHIELD" || service.ServiceType == "GROUTLIFE" || service.ServiceType == "CONCRETE-REPAIR"
            //    || service.ServiceType == "ENDURACRETE"  || service.ServiceType == "CONCRETE-COUNTERTOPS" || service.ServiceType == "CONCRETE-OVERLAYMENTS" ||
            //    service.ServiceType == "CARPETLIFE" || service.ServiceType == "CLEANAIR" || service.ServiceType == "METALLIFE" || service.ServiceType == "TILEINSTALL" || service.ServiceType == "VINYLGUARD"
            //    || service.ServiceType == "WOODLIFE" || service.ServiceType == "WOODLIFE")
            //    && (service.StoneType2 == "Marble" || service.StoneType2 == "Granite" || service.StoneType2 == "Ceramic" || service.StoneType2 == "Concrete" || service.StoneType2 == "Corian" ||
            //    service.StoneType2 == "Engineered Stone" || service.StoneType2 == "material type" || service.StoneType2 == "Porcelain" || service.StoneType2 == "Terrazzo" || service.StoneType2 == "Travertine" || service.StoneType2 == "Vinyl")
            //    || (splittedServices.Contains("Cleaning") || splittedServices.Contains("Cleaning-Address Mold Stains") || splittedServices.Contains("Cleanshield") || splittedServices.Contains("Coating") || splittedServices.Contains("Colorseal-Hydroforce") ||
            //    splittedServices.Contains("Colorseal-Powerwash") || splittedServices.Contains("Corian-Polish") || splittedServices.Contains("Corian-Scratch Removal") || splittedServices.Contains("Engineered Stone-Polish")
            //    || splittedServices.Contains("Engineered Stone-Scratch Removal") || splittedServices.Contains("Grind") || splittedServices.Contains("Hardening") || splittedServices.Contains("Hydroforce Cleaning")
            //    || splittedServices.Contains("Mold Stain Removal") || splittedServices.Contains("Powerwash Cleaning") || splittedServices.Contains("Sealing") || splittedServices.Contains("Stripping Carpet Adhesive") || splittedServices.Contains("Stripping-Silicone")
            //    || splittedServices.Contains("Stripping-Unknown") || splittedServices.Contains("Stripping-Urethane") || splittedServices.Contains("Stripping-Wax") || splittedServices.Contains("Tilelok") || splittedServices.Contains("Traverfil")))
            //{
            //    return (long)MeasurementEnum.AREA;
            //}
            //else if ((service.ServiceType == "CARPETLIFE" || service.ServiceType == "CLEANAIR" || service.ServiceType == "CLEANSHIELD" ||  service.ServiceType == "CONCRETE-OVERLAYMENTS"
            //    || service.ServiceType == "CONCRETE-REPAIR" || service.ServiceType == "COUNTERLIFE" || service.ServiceType == "ENDURACRETE" || service.ServiceType == "GROUTLIFE" || service.ServiceType == "METALLIFE"
            //    || service.ServiceType == "Service Type" || service.ServiceType == "STONELIFE" || service.ServiceType == "TILEINSTALL") &&
            //    (service.StoneType2 == "Ceramic" || service.StoneType2 == "Concrete" || service.StoneType2 == "Corian" || service.StoneType2 == "Engineered Stone" || service.StoneType2 == "Granite" || service.StoneType2 == "Marble"
            //    || service.StoneType2 == "material type" || service.StoneType2 == "Porcelain" || service.StoneType2 == "Terrazzo" || service.StoneType2 == "Travertine" || service.StoneType2 == "Vinyl") ||
            //    (splittedServices.Contains("Chip Repair") || splittedServices.Contains("Concrete Chip Repair") || splittedServices.Contains("Concrete Crack Repair") || splittedServices.Contains("Concrete Spot Stain Removal") ||
            //     splittedServices.Contains("Corian-Chip Repair") || splittedServices.Contains("Corian-Crack Repair") || splittedServices.Contains("Crack Repair") || splittedServices.Contains("Endurachip-Fast") || splittedServices.Contains("Endurachip-Light")
            //     || splittedServices.Contains("Hard Water Removal") || splittedServices.Contains("Large Concrete Area Repairs") || splittedServices.Contains("Other") || splittedServices.Contains("Seam Repair") || splittedServices.Contains("Stain Removal")
            //     || splittedServices.Contains("Stripping Carpet Adhesive") || splittedServices.Contains("Terrazzo Chip Repair") || splittedServices.Contains("Terrazzo Crack Repair") || splittedServices.Contains("Terrazzo Large Area Repair") || splittedServices.Contains("Tile Rebond")
            //     || splittedServices.Contains("Tile Removal") || splittedServices.Contains("Tile Replacement")))
            //{
            //    return (long)MeasurementEnum.EVENT;
            //}

            //else if ((service.ServiceType == "CONCRETE-REPAIR" || service.ServiceType == "COUNTERLIFE" || service.ServiceType == "GROUTLIFE" || service.ServiceType == "STONELIFE") &&
            //    (service.StoneType2 == "Ceramic" || service.StoneType2 == "Concrete" || service.StoneType2 == "Corian" || service.StoneType2 == "Granite" || service.StoneType2 == "Marble"
            //    || service.StoneType2 == "Porcelain" || service.StoneType2 == "Terrazzo" || service.StoneType2 == "Travertine" || service.StoneType2 == "Vinyl") ||
            //    (splittedServices.Contains("Caulking") || splittedServices.Contains("Expansion Joint Repairs") || splittedServices.Contains("Expansion Joint Sealing") || splittedServices.Contains("Grout Repair")
            //    || splittedServices.Contains("Grout Replacement") || splittedServices.Contains("Stripping Carpet Adhesive")))
            //{
            //    return (long)MeasurementEnum.LINEARFT;
            //}
            //else if ((service.ServiceType == "MAINTENANCE:BI-MONTHLY" || service.ServiceType == "MAINTENANCE:QUARTERLY" || service.ServiceType == "STONELIFE") &&
            //    (service.StoneType2 == "Ceramic" || service.StoneType2 == "Concrete" || service.StoneType2 == "Corian" || service.StoneType2 == "Granite" || service.StoneType2 == "Marble"
            //    || service.StoneType2 == "Porcelain" || service.StoneType2 == "Terrazzo" || service.StoneType2 == "Travertine" || service.StoneType2 == "Vinyl")
            //    || (splittedServices.Contains("MAINTENANCE (Bi-Monthly)") || splittedServices.Contains("MAINTENANCE (Monthly)") || splittedServices.Contains("MAINTENANCE (Other Frequency)") || splittedServices.Contains("MAINTENANCE (Quarterly)")
            //    || splittedServices.Contains("Stripping Carpet Adhesive"))
            //    )
            //{
            //    return (long)MeasurementEnum.MAINTENANCE;
            //}
            //else if ((service.ServiceType == "STONELIFE" || service.ServiceType == "PRODUCT") &&
            //    (service.StoneType2 == "Ceramic" || service.StoneType2 == "Concrete" || service.StoneType2 == "Corian" || service.StoneType2 == "Granite"
            //    || service.StoneType2 == "Marble" || service.StoneType2 == "Porcelain" || service.StoneType2 == "Terrazzo" || service.StoneType2 == "Travertine" || service.StoneType2 == "Vinyl") ||
            //    (splittedServices.Contains("Floor Cleaner Concentrate Gallon") || splittedServices.Contains("Granite Cleaner 32 oz Spray") || splittedServices.Contains("Granite Cleaner Refill Gallon")
            //    || splittedServices.Contains("Granite Gloss conditioner 8oz") || splittedServices.Contains("Granite Sealer 4 oz Spray") || splittedServices.Contains("Grout Sealer 16 oz") || splittedServices.Contains("Marble Cleaner 32 oz Spray")
            //    || splittedServices.Contains("Marble cleaner Conc. 32oz") || splittedServices.Contains("Marble cleaner Refill Gallon") || splittedServices.Contains("Marble Gloss conditioner 16oz") || splittedServices.Contains("Marble Polish 16 oz") ||
            //    splittedServices.Contains("MaxOut Concentrate Gallon") || splittedServices.Contains("MaxOut Grout Cleaner 32 oz") || splittedServices.Contains("Mold & Mildew Stain Remover 32oz Spray") || splittedServices.Contains("Mold & Mildew Stain Remover Gallon")
            //    || splittedServices.Contains("Soap Scum Remover 16oz") || splittedServices.Contains("Stone Sealer 16 oz") || splittedServices.Contains("Stripping Carpet Adhesive") || splittedServices.Contains("Tile Cleaner 32oz Spray") || splittedServices.Contains("Tile Cleaner Concentrate 32 oz")
            //    || splittedServices.Contains("Tile Cleaner Refill Gallon")))
            //{
            //    return (long)MeasurementEnum.PRODUCTPRICE;
            //}

            //else if ((service.ServiceType == "CLEANSHIELD" || service.ServiceType == "CONCRETE-COATINGS" || service.ServiceType == "CONCRETE-OVERLAYMENTS"
            //    || service.ServiceType == "COUNTERLIFE" || service.ServiceType == "ENDURACRETE" || service.ServiceType == "METALLIFE" || service.ServiceType == "STONELIFE" || service.ServiceType == "WOODLIFE") &&
            //    (service.StoneType2 == "Ceramic" || service.StoneType2 == "Concrete" || service.StoneType2 == "Corian" || service.StoneType2 == "Granite" || service.StoneType2 == "Marble" || service.StoneType2 == "Porcelain"
            //    || service.StoneType2 == "Terrazzo" || service.StoneType2 == "Travertine" || service.StoneType2 == "Vinyl" || service.StoneType2 == "Repair Chip" || service.StoneType2 == "Repair Crack" || service.StoneType2 == "Hone" || service.StoneType2 == "Seal") ||
            //    (splittedServices.Contains("Concrete Preparations") || splittedServices.Contains("Hone") || splittedServices.Contains("Hone Surface Prep") ||
            //    splittedServices.Contains("Honing") || splittedServices.Contains("Metal Scratch Removal") || splittedServices.Contains("Polishing") || splittedServices.Contains("Scratch & Wear Removal (Sanding)")
            //   || splittedServices.Contains("Stripping Carpet Adhesive") || splittedServices.Contains("Polish")))
            //{
            //    return (long)MeasurementEnum.TIME;
            //}

            //else if ((service.ServiceType == "SALESTAX" && (service.StoneType2 == "Ceramic" || service.StoneType2 == "Concrete" || service.StoneType2 == "Corian" || service.StoneType2 == "Granite"
            //    || service.StoneType2 == "Marble" || service.StoneType2 == "Porcelain" || service.StoneType2 == "Terrazzo" || service.StoneType2 == "Travertine" || service.StoneType2 == "Vinyl") &&
            //    (splittedServices.Contains("Products") || splittedServices.Contains("Service") || splittedServices.Contains("Stripping Carpet Adhesive"))))
            //{
            //    return (long)MeasurementEnum.TAXRATE;
            //}
            //return (long)MeasurementEnum.AREA;
        }


        public long? GetUnitTypeId(long? unitTypeId)
        {
            if (unitTypeId == (long?)MeasurementEnum.AREA)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "AREA").Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.EVENT)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "EVENT").Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.LINEARFT)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "LINEARFT").Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.MAINTENANCE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "MAINTENANCE").Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.PRODUCTPRICE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "PRODUCTPRICE").Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.TAXRATE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "TAXRATE").Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.TIME)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "TIME").Id;
            }
            return null;
        }


        private string GetUnitType(long? unitTypeId)
        {
            if (unitTypeId == (long?)MeasurementEnum.AREA)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "AREA").Name;
            }
            else if (unitTypeId == (long?)MeasurementEnum.EVENT)
            {
                //return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "EVENT").Name;
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "AREA").Name;
            }
            else if (unitTypeId == (long?)MeasurementEnum.LINEARFT)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "LINEARFT").Name;
            }
            else if (unitTypeId == (long?)MeasurementEnum.MAINTENANCE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "MAINTENANCE").Name;
            }
            else if (unitTypeId == (long?)MeasurementEnum.PRODUCTPRICE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "PRODUCTPRICE").Name;
            }
            else if (unitTypeId == (long?)MeasurementEnum.TAXRATE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "TAXRATE").Name;
            }
            else if (unitTypeId == (long?)MeasurementEnum.TIME)
            {
                //return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "TIME").Name;
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == "AREA").Name;
            }
            return "";
        }

        public EstimateInvoiceDimension CreateDomain(EstimateInvoiceDimensionViewModel model, long? estimateInvoiceServiceId)
        {
            var domain = new EstimateInvoiceDimension()
            {
                EstimateInvoiceServiceId = estimateInvoiceServiceId,
                IsNew = true,
                Length = model.Length,
                Width = model.Width,
                Area = model.Area,
                AreaTime = model.AreaTime,
                Description = model.Description
            };
            return domain;
        }

        public HoningMeasurementViewModel CreateHoningViewModel(HoningMeasurement honingMeasurement, HoningMeasurementDefault honingMeasurementDefault)
        {
            var HasOriginalValues = false;
            if (honingMeasurementDefault != default(HoningMeasurementDefault))
            {
                HasOriginalValues = true;
            }
            var honingMeasurementDomain = new HoningMeasurementViewModel();
            if (honingMeasurementDefault != null)
            {
                honingMeasurementDomain = new HoningMeasurementViewModel()
                {
                    SeventeenBase = honingMeasurement.SeventeenBase,
                    TotalArea = honingMeasurement.TotalArea,
                    Area = honingMeasurement.Area,
                    Length = honingMeasurement.Length,
                    Width = honingMeasurement.Width,
                    Caco = honingMeasurement.Caco,
                    Dimension = honingMeasurement.Dimension.ToString(),
                    EightHundred = honingMeasurement.EightHundred,
                    ElevenThousand = honingMeasurement.ElevenThousand,
                    EightThousand = honingMeasurement.EightThousand,
                    FifteenHundred = honingMeasurement.FifteenHundred,
                    ThreeThousand = honingMeasurement.ThreeThousand,
                    Fifty = honingMeasurement.Fifty,
                    FourHundred = honingMeasurement.FourHundred,
                    Hundred = honingMeasurement.Hundred,
                    Ihg = honingMeasurement.Ihg,
                    MinRestoration = honingMeasurement.MinRestoration,
                    ProdutivityRate = honingMeasurement.ProdutivityRate,
                    Sections = honingMeasurement.Sections,
                    Thirty = honingMeasurement.Thirty,
                    TotalAreaInHour = honingMeasurement.TotalAreaInHour,
                    TotalAreaInShift = honingMeasurement.TotalAreaInShift,
                    TotalCostPerSquare = honingMeasurement.TotalCostPerSquare,
                    TotalCost = honingMeasurement.TotalCost,
                    TotalMinute = honingMeasurement.TotalMinute,
                    TwoHundred = honingMeasurement.TwoHundred,
                    UGC = honingMeasurement.UGC,
                    ShiftPrice = honingMeasurement.ShiftPrice,
                    ShiftName = honingMeasurement.ShiftName,
                    StartingPointTechShiftEstimates = honingMeasurement.StartingPointTechShiftEstimates,
                    RowDescription = honingMeasurement.RowDescription,
                    IsSaved = true,
                    HasOriginalValues = HasOriginalValues,
                    SectionsOriginal = honingMeasurementDefault.SectionsDefault,
                    UGCOriginal = honingMeasurementDefault.UGCDefault,
                    ThirtyOriginal = honingMeasurementDefault.ThirtyDefault,
                    FiftyOriginal = honingMeasurementDefault.FiftyDefault,
                    HundredOriginal = honingMeasurementDefault.HundredDefault,
                    TwoHundredOriginal = honingMeasurementDefault.TwoHundredDefault,
                    FourHundredOriginal = honingMeasurementDefault.FourHundredDefault,
                    EightHundredOriginal = honingMeasurementDefault.EightHundredDefault,
                    FifteenHundredOriginal = honingMeasurementDefault.FifteenHundredDefault,
                    ThreeThousandOriginal = honingMeasurementDefault.ThreeThousandDefault,
                    EightThousandOriginal = honingMeasurementDefault.EightThousandDefault,
                    ElevenThousandOriginal = honingMeasurementDefault.ElevenThousandDefault,
                    CacoOriginal = honingMeasurementDefault.CacoDefault,
                    IhgOriginal = honingMeasurementDefault.IhgDefault,
                    ProdutivityRateOriginal = honingMeasurementDefault.ProdutivityRateDefault,
                    TotalMinuteOriginal = honingMeasurementDefault.TotalMinuteDefault,
                    TotalAreaInHourOriginal = honingMeasurementDefault.TotalAreaInHourDefault,
                    TotalAreaInShiftOriginal = honingMeasurementDefault.TotalAreaInShiftDefault,
                    SeventeenBaseOriginal = honingMeasurementDefault.SeventeenBaseDefault,
                    TotalAreaOriginal = honingMeasurementDefault.TotalAreaDefault,
                    TotalCostOriginal = honingMeasurementDefault.TotalCostDefault,
                    TotalCostPerSquareOriginal = honingMeasurementDefault.TotalCostPerSquareDefault,
                    MinRestorationOriginal = honingMeasurementDefault.MinRestorationDefault
                };
            }
            else
            {
                honingMeasurementDomain = new HoningMeasurementViewModel()
                {
                    SeventeenBase = honingMeasurement.SeventeenBase,
                    TotalArea = honingMeasurement.TotalArea,
                    Area = honingMeasurement.Area,
                    Length = honingMeasurement.Length,
                    Width = honingMeasurement.Width,
                    Caco = honingMeasurement.Caco,
                    Dimension = honingMeasurement.Dimension.ToString(),
                    EightHundred = honingMeasurement.EightHundred,
                    ElevenThousand = honingMeasurement.ElevenThousand,
                    EightThousand = honingMeasurement.EightThousand,
                    FifteenHundred = honingMeasurement.FifteenHundred,
                    ThreeThousand = honingMeasurement.ThreeThousand,
                    Fifty = honingMeasurement.Fifty,
                    FourHundred = honingMeasurement.FourHundred,
                    Hundred = honingMeasurement.Hundred,
                    Ihg = honingMeasurement.Ihg,
                    MinRestoration = honingMeasurement.MinRestoration,
                    ProdutivityRate = honingMeasurement.ProdutivityRate,
                    Sections = honingMeasurement.Sections,
                    Thirty = honingMeasurement.Thirty,
                    TotalAreaInHour = honingMeasurement.TotalAreaInHour,
                    TotalAreaInShift = honingMeasurement.TotalAreaInShift,
                    TotalCostPerSquare = honingMeasurement.TotalCostPerSquare,
                    TotalCost = honingMeasurement.TotalCost,
                    TotalMinute = honingMeasurement.TotalMinute,
                    TwoHundred = honingMeasurement.TwoHundred,
                    UGC = honingMeasurement.UGC,
                    ShiftPrice = honingMeasurement.ShiftPrice,
                    ShiftName = honingMeasurement.ShiftName,
                    StartingPointTechShiftEstimates = honingMeasurement.StartingPointTechShiftEstimates,
                    RowDescription = honingMeasurement.RowDescription,
                    IsSaved = true,
                    HasOriginalValues = HasOriginalValues
                };
            }
            return honingMeasurementDomain;
        }


        public MaintenanceViewModelCharges CreateMaintanceChargesViewModel(MaintenanceCharges honingMeasurement)
        {
            var MAINTENANCEViewModelCharges = new MaintenanceViewModelCharges()
            {
                High = honingMeasurement.High,
                Low = honingMeasurement.Low,
                Material = honingMeasurement.Material
            };
            return MAINTENANCEViewModelCharges;
        }


        public EstimateInvoiceImageEditModel CreateEstimateInvoiceImageViewModel(EstimateInvoiceServiceImage estimateInvoiceServiceDimension, long? unitTypeId = null)
        {
            var image = new EstimateInvoiceImageEditModel()
            {
                Caption = estimateInvoiceServiceDimension.File != null ? estimateInvoiceServiceDimension.File.Name : "",
                FileId = estimateInvoiceServiceDimension.File != null ? estimateInvoiceServiceDimension.File.Id : default(long),
                RelativeLocation = estimateInvoiceServiceDimension.File != null ? (estimateInvoiceServiceDimension.File.RelativeLocation + "\\" + estimateInvoiceServiceDimension.File.Name).ToUrl() : "",
                IsUploadedImage = true
            };
            return image;
        }

    }
}