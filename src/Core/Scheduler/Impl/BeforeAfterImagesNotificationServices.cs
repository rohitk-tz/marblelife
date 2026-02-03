using Core.Application;
using Core.Application.Attribute;
using System;
using System.Linq;
using Core.MarketingLead.Domain;
using Core.Organizations;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class BeforeAfterImagesNotificationServices : IBeforeAfterImagesNotificationServices
    {
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<MarketingLeadCallDetailV2> _marketingLeadCallDetailV2Repository;
        private ILogService _logService;
        private IUnitOfWork _unitOfWork;
        public BeforeAfterImagesNotificationServices(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock, IJobFactory jobFactory, IFileService fileService, IBeforeAfterThumbNailService beforeAfterThumbNameService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _marketingLeadCallDetailV2Repository = unitOfWork.Repository<MarketingLeadCallDetailV2>();
            _organizationRepository = unitOfWork.Repository<Organization>();
        }

        public void ProcessRecords()
        {
            //AddingFileThumbMigration();
            SendBeforeAfterImages();
        }

        private void SendBeforeAfterImages()
        {
            var previousDate = DateTime.Now.AddMonths(-13);
            var currentDate = (DateTime.Now).AddMonths(1);
            _logService.Info(string.Format(" Current  Time for Updating  Marketing Class - {0}", currentDate));
            _logService.Info(string.Format(" Previous Time for Updating Marketing Class - {0}", previousDate));
            _logService.Info(string.Format(" Start Time for Updating Marketing Class - {0}", previousDate));
            _logService.Info(string.Format(" End Time for Updating Marketing Class - {0}", currentDate));

            var collection = _marketingLeadCallDetailV2Repository.IncludeMultiple(x => x.MarketingLeadCallDetail).Where(x => (x.MarketingLeadCallDetail != null) && (x.MarketingLeadCallDetail.DateAdded >= previousDate && x.MarketingLeadCallDetail.DateAdded <= currentDate && x.MarketingLeadCallDetail.CalledFranchiseeId == null)).OrderBy(x=>x.Id).ToList();
            //&& x.MarketingLeadCallDetail.CalledFranchiseeId != null
            foreach (var marketingCall in collection)
            {
                try
                {
                    var callRoute = marketingCall.CallRoute;
                    var phoneCall = marketingCall.PhoneLabel;
                    if (callRoute != null && (callRoute.Contains("-Line") || callRoute.Contains("- Line")))
                    {
                        var franchiseeId = GetCalledFranchiseeIdByCallRoute(callRoute);
                        if (franchiseeId != null && franchiseeId != 1)
                        {
                            marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeId;
                            _marketingLeadCallDetailV2Repository.Save(marketingCall);
                            _unitOfWork.SaveChanges();
                        }
                    }
                    else
                    {
                        var franchiseeId = GetCalledFranchiseeIdByZipCode(marketingCall.EnteredZipCode != null ? marketingCall.EnteredZipCode : marketingCall.ZipCode);

                        if (franchiseeId != null && franchiseeId != 1)
                        {
                            marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeId;
                            _marketingLeadCallDetailV2Repository.Save(marketingCall);
                            _unitOfWork.SaveChanges();
                        }
                        else if (phoneCall != null && phoneCall.Contains("Local"))
                        {
                            var franchiseeIdLocal = GetCalledFranchiseeIdByPhoneLabel(phoneCall);
                            if (franchiseeIdLocal != null && franchiseeIdLocal != 1)
                            {
                                marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeIdLocal;
                                _marketingLeadCallDetailV2Repository.Save(marketingCall);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else if (phoneCall != null && phoneCall.Contains("$$"))
                        {
                            var franchiseeIdLocal = GetCalledFranchiseeIdByDollar(phoneCall);
                            if (franchiseeIdLocal != null && franchiseeIdLocal != 1)
                            {
                                marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeIdLocal;
                                _marketingLeadCallDetailV2Repository.Save(marketingCall);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else if (phoneCall != null && phoneCall.Contains("IVR Bridge"))
                        {
                            var franchiseeIdLocal = GetCalledFranchiseeIdByLocalBridge(phoneCall);
                            if (franchiseeIdLocal != null && franchiseeIdLocal != 1)
                            {
                                marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeIdLocal;
                                _marketingLeadCallDetailV2Repository.Save(marketingCall);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else if (phoneCall != null && phoneCall.Contains("CC-SE-"))
                        {
                            var franchiseeIdLocal = GetCalledFranchiseeIdByCC(phoneCall);
                            if (franchiseeIdLocal != null && franchiseeIdLocal != 1)
                            {
                                marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeIdLocal;
                                _marketingLeadCallDetailV2Repository.Save(marketingCall);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else if (phoneCall != null && phoneCall.Contains("CC"))
                        {
                            var franchiseeIdLocal = GetCalledFranchiseeIdByPhoneLabel(phoneCall);
                            if (franchiseeIdLocal != null && franchiseeIdLocal != 1)
                            {
                                marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeIdLocal;
                                _marketingLeadCallDetailV2Repository.Save(marketingCall);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else if (phoneCall != null && phoneCall.Contains("Line One"))
                        {
                            var franchiseeIdLocal = GetCalledFranchiseeIdByLineOne(phoneCall);
                            if (franchiseeIdLocal != null && franchiseeIdLocal != 1)
                            {
                                marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeIdLocal;
                                _marketingLeadCallDetailV2Repository.Save(marketingCall);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else if (phoneCall != null && phoneCall.Contains("MarbleLife |"))
                        {
                            var franchiseeIdLocal = GetCalledFranchiseeIdByMarbleLife(phoneCall);
                            if (franchiseeIdLocal != null && franchiseeIdLocal != 1)
                            {
                                marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeIdLocal;
                                _marketingLeadCallDetailV2Repository.Save(marketingCall);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else if (callRoute != null && callRoute.Contains("IVR Bridge"))
                        {
                            var franchiseeIdLocal = GetCalledFranchiseeIdByLocalBridge(phoneCall);
                            if (franchiseeIdLocal != null && franchiseeIdLocal != 1)
                            {
                                marketingCall.MarketingLeadCallDetail.CalledFranchiseeId = franchiseeIdLocal;
                                _marketingLeadCallDetailV2Repository.Save(marketingCall);
                                _unitOfWork.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception e1)
                {
                    _logService.Error("Error Parsing Phone Call For: " + marketingCall.PhoneLabel + " having errro " + e1.InnerException);
                    continue;
                }
            }
        }

        private long? GetCalledFranchiseeIdByPhoneLabel(string phoneLabel)
        {
            var franchiseeNameFromPhoneLabel = phoneLabel.Split('-');
            var franchiseeName = franchiseeNameFromPhoneLabel.Length > 1 ? franchiseeNameFromPhoneLabel[1] : "";
            franchiseeName = franchiseeName.TrimEnd();
            franchiseeName = franchiseeName.TrimStart();

            var franchiseeId = GetFranchiseeForFranchiseeName(franchiseeName);
            if (franchiseeId != null)
            {
                return franchiseeId;
            }

            //franchiseeName = franchiseeName.Replace(" ", string.Empty);
            var franchiseeNameWithOutSpace = franchiseeName.Replace(" ", string.Empty);
            var franchisee = _organizationRepository.Table.FirstOrDefault(x => x.Name == franchiseeName || x.Name.Contains(franchiseeName) || x.Name.Contains(franchiseeNameWithOutSpace));
            if (franchisee != null)
            {
                return franchisee.Id;
            }
            return default(long?);
        }

        private long? GetCalledFranchiseeIdByDollar(string phoneLabel)
        {
            var franchiseeNameFromPhoneLabel = phoneLabel.Split(new[] { "$$" }, StringSplitOptions.None);
            var franchiseeName = franchiseeNameFromPhoneLabel.Length > 1 ? franchiseeNameFromPhoneLabel[1] : "";
            if (franchiseeName.Contains("Local Website"))
            {
                var franchiseeNameList = franchiseeName.Split(new[] { "Local Website" }, StringSplitOptions.None);
                franchiseeName = franchiseeNameList[1];
            }
            else if (franchiseeName.Contains("Internet -"))
            {
                var franchiseeNameList = franchiseeName.Split(new[] { "Internet -" }, StringSplitOptions.None);
                franchiseeName = franchiseeNameList[1];
            }
            franchiseeName = franchiseeName.TrimEnd();
            franchiseeName = franchiseeName.TrimStart();

            var franchiseeId = GetFranchiseeForFranchiseeName(franchiseeName);
            if (franchiseeId != null)
            {
                return franchiseeId;
            }

            //franchiseeName = franchiseeName.Replace(" ", string.Empty);
            var franchiseeNameWithOutSpace = franchiseeName.Replace(" ", string.Empty);
            var franchisee = _organizationRepository.Table.FirstOrDefault(x => x.Name == franchiseeName || x.Name.Contains(franchiseeName) || x.Name.Contains(franchiseeNameWithOutSpace));
            if (franchisee != null)
            {
                return franchisee.Id;
            }
            return default(long?);
        }

        private long? GetCalledFranchiseeIdByLocalBridge(string phoneLabel)
        {
            var franchiseeNameFromPhoneLabel = phoneLabel.Split(new[] { "- IVR Bridge" }, StringSplitOptions.None);
            var franchiseeName = franchiseeNameFromPhoneLabel.Length > 1 ? franchiseeNameFromPhoneLabel[0] : "";
            franchiseeName = franchiseeName.TrimEnd();
            franchiseeName = franchiseeName.TrimStart();

            var franchiseeId = GetFranchiseeForFranchiseeName(franchiseeName);
            if (franchiseeId != null)
            {
                return franchiseeId;
            }
            var franchiseeNameWithOutSpace = franchiseeName.Replace(" ", string.Empty);
            var franchisee = _organizationRepository.Table.FirstOrDefault(x => x.Name == franchiseeName || x.Name.Contains(franchiseeName) || x.Name.Contains(franchiseeNameWithOutSpace));
            if (franchisee != null)
            {
                return franchisee.Id;
            }
            return default(long?);
        }

        private long? GetCalledFranchiseeIdByCallRoute(string callRoute)
        {
            var franchiseeNameFromPhoneLabel = callRoute.Split('-');
            var franchiseeName = franchiseeNameFromPhoneLabel.Length > 1 ? franchiseeNameFromPhoneLabel[1] : "";
            franchiseeName = franchiseeName.TrimEnd();
            franchiseeName = franchiseeName.TrimStart();

            var franchiseeId = GetFranchiseeForFranchiseeName(franchiseeName);
            if (franchiseeId != null)
            {
                return franchiseeId;
            }

            var franchiseeNameWithOutSpace = franchiseeName.Replace(" ", string.Empty);
            var franchisee = _organizationRepository.Table.FirstOrDefault(x => x.Name == franchiseeName || x.Name.Contains(franchiseeName) || x.Name.Contains(franchiseeNameWithOutSpace));
            if (franchisee != null)
            {
                return franchisee.Id;
            }
            return default(long?);

        }

        private long? GetCalledFranchiseeIdByZipCode(string zipCode)
        {
            var organizationList = _organizationRepository.Table.ToList();
            foreach (var organization in organizationList)
            {
                if (organization.Address.Count() > 0)
                {
                    var address = organization.Address.FirstOrDefault();
                    if (zipCode == address.ZipCode)
                    {
                        return organization.Id;
                    }
                    else
                    {
                        if (address.Zip != null && zipCode == address.Zip.Code)
                        {
                            return organization.Id;
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
                else
                {
                    continue;
                }
            }
            return default(long?);
        }

        private long? GetFranchiseeForFranchiseeName(string franchiseeName)
        {
            if (franchiseeName == "St Louis" || franchiseeName == "St Louis ")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "MO-St.Louis");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Virginia Beach" || franchiseeName == "Virginia Beach ")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "VA-Hampton Roads");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Boca Raton" || franchiseeName == "Boca Raton ")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "FL-West Palm");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Springfield" || franchiseeName == "Springfield ")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "FL-Orlando");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Orange County")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "CA-Orange");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Northeast Florida")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "FL-Pensacola");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Boca Raton, FL")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "FL-West Palm");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "St Louis")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "MO-St.Louis");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "North Texas")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "TX-Dallas");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "The Carolinas")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "SC-Greenville");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Huntsville, AL")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "AL-Huntsville");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "San Diego, CA")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "CA-San Diego");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Southwest Alabama")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "AL-Mobile");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Orange County")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "CA-Orange");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Seattle, WA")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "WA-Seattle");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "York Canada")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "I-CANADA (York)");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Carolinas")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "SC-Greenville");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Northwest FL")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "FL-Pensacola");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Salt Lake City, Utah")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "UT-Salt Lake City");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "UT-Salt Lake City")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "UT-Salt Lake City");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Western Mass")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "MA-Western");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Richmond, VA")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "VA-Richmond");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Central Florida")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "FL-Orlando");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Northern Chicago")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "IL-Chicago");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Gainesville FL")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "FL-Gainesville");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Portland, OR")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "OR-Portland");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Southeast Michigan")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "MI-Detroit");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Omaha, NE")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "NE-Omaha");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Orange County")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "CA-Orange");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Western MI")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "MI-Grand Rapids");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "central georgia")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "GA-Columbus");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Vancouver")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "I-CANADA (Vancouver)");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "West Michigan")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "MI-Grand Rapids");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Pittsburgh")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "PA-Pittsburgh");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Michigan")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "MI-Detroit");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "FL-Boca Raton")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "FL-West Palm");
                return franchiseeLouis.Id;
            }
            else if (franchiseeName == "Detroit")
            {
                var franchiseeLouis = _organizationRepository.Table.FirstOrDefault(x => x.Name == "MI-Detroit");
                return franchiseeLouis.Id;
            }

            else
            {
                return null;
            }

        }
        private long? GetCalledFranchiseeIdByLineOne(string phoneLabel)
        {

            var franchiseeNameFromPhoneLabel = phoneLabel.Split(new[] { "-Line One" }, StringSplitOptions.None);
            var franchiseeName = franchiseeNameFromPhoneLabel.Length > 1 ? franchiseeNameFromPhoneLabel[0] : "";
            franchiseeName = franchiseeName.TrimEnd();
            franchiseeName = franchiseeName.TrimStart();

            var franchiseeId = GetFranchiseeForFranchiseeName(franchiseeName);
            if (franchiseeId != null)
            {
                return franchiseeId;
            }

            //franchiseeName = franchiseeName.Replace(" ", string.Empty);
            var franchiseeNameWithOutSpace = franchiseeName.Replace(" ", string.Empty);
            var franchisee = _organizationRepository.Table.FirstOrDefault(x => x.Name == franchiseeName || x.Name.Contains(franchiseeName) || x.Name.Contains(franchiseeNameWithOutSpace));
            if (franchisee != null)
            {
                return franchisee.Id;
            }
            return default(long?);
        }

        private long? GetCalledFranchiseeIdByCC(string phoneLabel)
        {

            var franchiseeNameFromPhoneLabel = phoneLabel.Split(new[] { "CC-SE-" }, StringSplitOptions.None);
            var franchiseeName = franchiseeNameFromPhoneLabel.Length > 1 ? franchiseeNameFromPhoneLabel[1] : "";
            franchiseeName = franchiseeName.TrimEnd();
            franchiseeName = franchiseeName.TrimStart();

            var franchiseeId = GetFranchiseeForFranchiseeName(franchiseeName);
            if (franchiseeId != null)
            {
                return franchiseeId;
            }

            //franchiseeName = franchiseeName.Replace(" ", string.Empty);
            var franchiseeNameWithOutSpace = franchiseeName.Replace(" ", string.Empty);
            var franchisee = _organizationRepository.Table.FirstOrDefault(x => x.Name == franchiseeName || x.Name.Contains(franchiseeName) || x.Name.Contains(franchiseeNameWithOutSpace));
            if (franchisee != null)
            {
                return franchisee.Id;
            }
            return default(long?);
        }

        private long? GetCalledFranchiseeIdByMarbleLife(string phoneLabel)
        {

            var franchiseeNameFromPhoneLabel = phoneLabel.Split(new[] { "|" }, StringSplitOptions.None);

            var franchiseeName = franchiseeNameFromPhoneLabel.Length > 1 ? franchiseeNameFromPhoneLabel[1] : "";
            franchiseeName = franchiseeName.TrimEnd();
            franchiseeName = franchiseeName.TrimStart();

            var franchiseeId = GetFranchiseeForFranchiseeName(franchiseeName);
            if (franchiseeId != null)
            {
                return franchiseeId;
            }

            //franchiseeName = franchiseeName.Replace(" ", string.Empty);
            var franchiseeNameWithOutSpace = franchiseeName.Replace(" ", string.Empty);
            var franchisee = _organizationRepository.Table.FirstOrDefault(x => x.Name == franchiseeName || x.Name.Contains(franchiseeName) || x.Name.Contains(franchiseeNameWithOutSpace));
            if (franchisee != null)
            {
                return franchisee.Id;
            }
            return default(long?);
        }
    }
}
