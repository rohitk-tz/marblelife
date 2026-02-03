using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.MarketingLead.Domain;
using Core.MarketingLead.Enum;
using Core.MarketingLead.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Threading.Tasks;
using RestSharp;
using Core.Scheduler.Domain;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class MarketingLeadsService : IMarketingLeadsService
    {
        private readonly ILogService _logService;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;
        private IClock _clock;
        private readonly IMarketingLeadsFactory _marketingLeadsFactory;
        private readonly IRepository<Lookup> _lookupRepository;
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadCallDetailRepository;
        private readonly IRepository<MarketingLeadCallDetailV2> _marketingLeadCallDetailV2Repository;
        private readonly IRepository<MarketingLeadCallDetailV3> _marketingLeadCallDetailV3Repository;
        private readonly IRepository<MarketingLeadCallDetailV4> _marketingLeadCallDetailV4Repository;
        private readonly IRepository<MarketingLeadCallDetailV5> _marketingLeadCallDetailV5Repository;
        private readonly IRepository<WebLead> _webLeadRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<ZipCode> _zipCodeRepository;
        private readonly IRepository<County> _countyRepository;
        private readonly IRepository<CallDetailsReportNotes> _calldetailsreportnotes;
        public MarketingLeadsService(IUnitOfWork unitOfWork, ISettings settings, ILogService logService, IClock clock, IMarketingLeadsFactory marketingLeadsFactory)
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _marketingLeadsFactory = marketingLeadsFactory;
            _lookupRepository = unitOfWork.Repository<Lookup>();
            _marketingLeadCallDetailRepository = unitOfWork.Repository<MarketingLeadCallDetail>();
            _webLeadRepository = unitOfWork.Repository<WebLead>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _marketingLeadCallDetailV2Repository = unitOfWork.Repository<MarketingLeadCallDetailV2>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _marketingLeadCallDetailV3Repository = unitOfWork.Repository<MarketingLeadCallDetailV3>();
            _marketingLeadCallDetailV4Repository = unitOfWork.Repository<MarketingLeadCallDetailV4>();
            _marketingLeadCallDetailV5Repository = unitOfWork.Repository<MarketingLeadCallDetailV5>();
            _zipCodeRepository = unitOfWork.Repository<ZipCode>();
            _countyRepository = unitOfWork.Repository<County>();
            _calldetailsreportnotes = unitOfWork.Repository<CallDetailsReportNotes>();

        }

        public void GetMarketingLeads()
        {
            _logService.Info(string.Format("Web Lead Started At: - {0}", _clock.UtcNow));
            if (_settings.GetWebLeads)
            {
                _logService.Info(string.Format("Getting WebLeads - {0}", _clock.UtcNow));
                GetWebLeads();
            }
            if (_settings.GetCallDetails)
            {
                _logService.Info(string.Format("Geting Call Details form API V2! - {0}", _clock.UtcNow));
                GetCallDetailsV3();
            }
            if (_settings.GetCallDetails)
            {
                _logService.Info(string.Format("Geting Call Details form Invoca API! - {0}", _clock.UtcNow));
                GetCallDetailsInvoca();
                //SyncOldCallDetailsNotes();
                SyncMarketingLeadCallDetailsIdWithCallNotes();
            }
            _logService.Info(string.Format("Web Lead End At: - {0}", _clock.UtcNow));
        }

        #region WebLeads
        private void GetWebLeads()
        {
            var apiKey = _settings.WebLeadsAPIkey;

            if (string.IsNullOrEmpty(apiKey))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }

            var currentDate = _clock.UtcNow;
            var previousDate = currentDate.AddDays(-6);

            //if (_settings.GetHistoryData)
            //{
            //    previousDate = _settings.WebLeadStartDate;
            //    currentDate = _settings.WebLeadEndDate;
            //}
            const string Success = "SUCCESS";
            const string Error = "ERROR";

            for (DateTime date = previousDate; date <= currentDate; date = date.AddDays(1))
            {
                var result = GetWebLeadsfromAPI(date.AddDays(1), date, apiKey);
                var lenght = result.Length;
                if (lenght <= 33)
                {
                    continue;
                }
                var response = (new JavaScriptSerializer()).Deserialize<WebLeadListModel>(result);
                if (response.result.ToUpper().Equals(Success.ToUpper()) && response.info.Count() > 0)
                {
                    SaveWebLeads(response);
                }
                else if (response.result.ToUpper().Equals(Error.ToUpper()))
                {
                    _logService.Error(string.Format("Error In Webleads API : {0}", response.code));
                }
            }
        }
        private void SaveWebLeads(WebLeadListModel list)
        {
            var inDbWebLeads = _webLeadRepository.Table.ToList();
            var franchiseeList = _franchiseeRepository.Table.ToList();
            var webLeadFranchiseeList = CreateList(franchiseeList);
            //list.info = list.info.Take(100).ToList();
            foreach (var item in list.info)
            {
                if (!string.IsNullOrEmpty(item.name_first) && !string.IsNullOrEmpty(item.name_last) && item.Id > 0)
                {
                    try
                    {
                        _logService.Error(string.Format("Parsing Data : {0}", item.name_first));
                        var webleads = inDbWebLeads.FirstOrDefault(x => x.WebLeadId == item.Id);
                        //if (!webleads.Any())
                        //{
                        var domain = _marketingLeadsFactory.CreateDomain(item);
                        var franchisee = franchiseeList.FirstOrDefault(x => x.Id == domain.FranchiseeId);

                        //if (franchisee != null && franchisee.FranchiseeId > 0)
                        //    domain.FranchiseeId = franchisee.FranchiseeId;
                        if (franchisee == null)
                        {
                            domain.FranchiseeId = null;
                        }
                        domain.IsNew = webleads != null ? false : true;
                        if (!domain.IsNew)
                        {
                            domain.InvoiceId = webleads.InvoiceId;
                        }
                        domain.Id = webleads != null ? webleads.Id : 0;
                        if (webleads!=null && domain.FranchiseeId == 61 && webleads.Id == 0)
                        {
                            domain.FranchiseeId = 95;
                        }
                        _webLeadRepository.Save(domain);
                        //}
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Error saving WebLead - {0}", ex.Message));
                        continue;
                    }
                }
            }
            _unitOfWork.SaveChanges();
        }

        private IList<WebLeadFranchiseeList> CreateList(IList<Franchisee> list)
        {
            var Idlist = new List<WebLeadFranchiseeList>();
            foreach (var item in list)
            {
                var model = new WebLeadFranchiseeList();
                var stringIds = item.WebLeadFranchiseeId;
                Idlist.Add(model);
            }
            return Idlist;
        }

        private string GetWebLeadsfromAPI(DateTime currentDate, DateTime previousDate, string apiKey)
        {
            var fromDate = string.Format("{0:yyyy-MM-dd}", previousDate);
            var toDate = string.Format("{0:yyyy-MM-dd}", currentDate);
            string url = string.Format("http://marblelife.com/ziplocator/API/index/token/{0}/sdate/{1}/edate/{2}", apiKey, fromDate, toDate);

            var result = string.Empty;
            using (var client = new WebClient())
            {
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Web Exception :", ex.Message));
                }
            }
            return result;
        }

        #endregion

        #region CallDetails DialogTech

        private void GetCallDetailsV3()
        {
            var access_key = _settings.AccessKeyV2;
            var secret_key = _settings.SecretKeyV2;

            if (string.IsNullOrEmpty(access_key) || string.IsNullOrEmpty(secret_key))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }

            //var previousDate = DateTime.Now.AddMonths(-4);
            //var currentDate = (DateTime.Now);
            var previousDate = DateTime.Now.AddDays(-6);
            var currentDate = (DateTime.Now);

            _logService.Info(string.Format(" Current  Time for Marketing Class - {0}", currentDate));
            _logService.Info(string.Format(" Previous Time for Marketing Class - {0}", previousDate));
            //CallDetailList list = new CallDetailList();
            for (DateTime date = previousDate; date <= currentDate; date = date.AddHours(4))
            {
                _logService.Info(string.Format(" Start Time for Marketing Class - {0}", date.AddHours(-4)));
                _logService.Info(string.Format(" End Time for Marketing Class - {0}", date));

                var result = GetCallDetailFromAPIV3(date, date.AddHours(-4), access_key, secret_key);
                var list = new CallDetailListV3();
                var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate };
                try
                {
                    list = (new JavaScriptSerializer()).Deserialize<CallDetailListV3>(result);
                    //_logService.Info(string.Format(" Total Records - {0}", list.response.Count()));
                }
                catch (Exception e1)
                {
                    continue;
                }


                if (list != null)
                {
                    _logService.Info(string.Format(" Total Records - {0}", list.response.Count()));
                }

                if (list != null)
                    SaveCallDetailsV3(list.response, date);
            }
        }
        private string GetCallDetailFromAPIV3(DateTime currentDate, DateTime previousDate, string access_key, string secret_key)
        {
            var key = access_key + ":" + secret_key;
            var fromdate = previousDate.ToString("yyyy-MM-ddTHH:mm" + ":00");
            var toDate = currentDate.ToString("yyyy-MM-ddTHH:mm" + ":00");
            string url = string.Format("https://dialogtechapis.com/calls/v2/?start_date={0}&end_date={1}&page_size=100", fromdate, toDate);

            var result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, "a6ff7c66529eef96af8516807a86a065742b0ff04cc8c2b08de46ef41c4310ed:fea78783fd69ba0611b59330a68e601c09cfa50113d15bf15a70a1773d48da6f");
                client.Headers.Add("x-api-key", "WGeOstjp9818CWpEkzm2EaHlo8MPdyem8nVoSXDk");
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Web Exception :", ex.Message));
                }
            }
            return result;
        }

        private void SaveCallDetailsV3(CallRetailRecordV2 item, long? id, DateTime callDate)
        {
            if (!string.IsNullOrEmpty(item.sid))
            {
                try
                {
                    var callDetailInDB = _marketingLeadCallDetailV3Repository.Table.Where(x => x.Sid == item.sid).ToList();
                    var domain = _marketingLeadsFactory.CreateModelForNewAPI3(item.call_details, item.sid, callDate);
                    if (!callDetailInDB.Any())
                    {
                        domain.IsNew = true;
                    }
                    else
                    {
                        domain.IsNew = false;
                        domain.Id = callDetailInDB.FirstOrDefault().Id;
                    }

                    _logService.Info(string.Format(" Saving Marketing Lead Class Detail Verion 3 For Phone Lable - {0}", domain.Sid));
                    domain.MarketingLeadCallDetailId = id;
                    _marketingLeadCallDetailV3Repository.Save(domain);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Verson 3 - {0}", ex.InnerException.StackTrace));
                    //continue;
                }
            }
            _unitOfWork.SaveChanges();
        }

        private void SaveCallDetailsV3(List<CallRetailRecordV2> list, DateTime callDate)
        {
            foreach (var item in list)
            {
                var callDetailInDB = _marketingLeadCallDetailRepository.Table.Where(x => x.SessionId == item.sid).ToList();
                if (!string.IsNullOrEmpty(item.call_details.caller_id))
                {
                    try
                    {
                        var callTypeId = GetCallType(item.call_details.call_type);

                        item.call_details.SessionId = item.sid;
                        var model = _marketingLeadsFactory.CreateModelForNewApi(item.call_details, callTypeId);
                        var domain = _marketingLeadsFactory.CreateDomain(model);

                        if (!callDetailInDB.Any())
                        {
                            domain.IsNew = true;
                        }
                        else
                        {
                            domain.IsNew = false;
                            domain.Id = callDetailInDB.FirstOrDefault().Id;
                        }

                        _logService.Info(string.Format(" Saving Marketing Lead Class Detail Verion 2 For Phone Lable - {0}", domain.PhoneLabel));
                        domain.IsFromNewAPI = true;
                        //domain.DateAdded = callDate;
                        if (item.call_details != null && item.call_details.advanced_routing != null && item.call_details.advanced_routing.callflow != null)
                        {
                            if (item.call_details.advanced_routing.callflow.callflow_destination != null)
                            {
                                var franchiseeName = item.call_details.advanced_routing.callflow.callflow_destination;
                                var organization = _organizationRepository.Table.FirstOrDefault(x => x.Name == franchiseeName);
                                if (organization != null)
                                {
                                    domain.FranchiseeId = organization.Id;
                                    domain.TagId = (long)TagType.FranchiseDirect;
                                }
                            }
                        }
                        _marketingLeadCallDetailRepository.Save(domain);
                        _unitOfWork.SaveChanges();

                        SaveCallDetailsV2(item, domain.Id, callDate);
                        SaveCallDetailsV3(item, domain.Id, callDate);
                        SaveCallDetailsV4(item, domain.Id, callDate);
                        SaveCallDetailsV5(item, domain.Id, callDate);
                        _unitOfWork.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Verson 2 - {0}", ex.InnerException.StackTrace));
                        continue;
                    }
                }
            }
            _unitOfWork.SaveChanges();
        }

        private void SaveCallDetailsV2(CallRetailRecordV2 item, long? id, DateTime callDate)
        {
            if (!string.IsNullOrEmpty(item.sid))
            {
                try
                {
                    var callDetailInDB = _marketingLeadCallDetailV2Repository.Table.Where(x => x.Sid == item.sid).ToList();
                    var domain = _marketingLeadsFactory.CreateModelForNewAPI(item.call_details, item.sid, callDate);
                    if (!callDetailInDB.Any())
                    {
                        domain.IsNew = true;
                    }
                    else
                    {
                        domain.IsNew = false;
                        domain.Id = callDetailInDB.FirstOrDefault().Id;
                    }

                    _logService.Info(string.Format(" Saving Marketing Lead Class Detail Verion 2 For Phone Lable - {0}", domain.PhoneLabel));
                    domain.MarketingLeadCallDetailId = id;
                    _marketingLeadCallDetailV2Repository.Save(domain);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Verson 2 - {0}", ex.InnerException.StackTrace));
                    //continue;
                }
            }
            _unitOfWork.SaveChanges();
        }

        private void SaveCallDetailsV4(CallRetailRecordV2 item, long? id, DateTime callDate)
        {
            if (!string.IsNullOrEmpty(item.sid))
            {
                try
                {
                    var callDetailInDB = _marketingLeadCallDetailV4Repository.Table.Where(x => x.Sid == item.sid).ToList();
                    var domain = _marketingLeadsFactory.CreateModelForNewAPI4(item, item.sid, callDate);
                    if (!callDetailInDB.Any())
                    {
                        domain.IsNew = true;
                    }
                    else
                    {
                        domain.IsNew = false;
                        domain.Id = callDetailInDB.FirstOrDefault().Id;
                    }

                    _logService.Info(string.Format(" Saving Marketing Lead Class Detail Verion 3 For Phone Lable - {0}", domain.Sid));
                    domain.MarketingLeadCallDetailId = id;
                    _marketingLeadCallDetailV4Repository.Save(domain);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Verson 3 - {0}", ex.InnerException.StackTrace));
                    //continue;
                }
            }
            _unitOfWork.SaveChanges();
        }

        private void SaveCallDetailsV5(CallRetailRecordV2 item, long? id, DateTime callDate)
        {
            if (!string.IsNullOrEmpty(item.sid))
            {
                try
                {
                    var callDetailInDB = _marketingLeadCallDetailV5Repository.Table.Where(x => x.Sid == item.sid).ToList();
                    var domain = _marketingLeadsFactory.CreateModelForNewAPI5(item, item.sid, callDate);
                    if (!callDetailInDB.Any())
                    {
                        domain.IsNew = true;
                    }
                    else
                    {
                        domain.IsNew = false;
                        domain.Id = callDetailInDB.FirstOrDefault().Id;
                    }

                    _logService.Info(string.Format(" Saving Marketing Lead Class Detail Verion 3 For Phone Lable - {0}", domain.Sid));
                    domain.MarketingLeadCallDetailId = id;
                    _marketingLeadCallDetailV5Repository.Save(domain);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Verson 3 - {0}", ex.InnerException.StackTrace));
                    //continue;
                }
            }
            _unitOfWork.SaveChanges();
        }

        private long GetCallType(string callType)
        {
            long defaultCallType = (long)CallType.Inbound;
            var type = callType.Trim().ToLower();
            var callTypeid = _lookupRepository.Table.Where(x => x.LookupTypeId == (long)LookupTypes.CallType && x.Name.ToLower().Equals(callType)).Select(y => y.Id).FirstOrDefault();
            if (callTypeid > 0)
                defaultCallType = callTypeid;
            return defaultCallType;
        }
        #endregion

        #region CallDetails Invoca
        private void GetCallDetailsInvoca()
        {
            //var fromDate = DateTime.Now.AddDays(-1).ToString("yyyy-mm-dd");
            //var toDate = DateTime.Now.AddDays(-1).ToString("yyyy-mm-dd");
            var marketingCallDetailListFromInvoca = _marketingLeadCallDetailRepository.Table.Where(x => x.IsFromInvoca).OrderByDescending(x => x.Id).ToList();
            string startAfterId = string.Empty;
            if (marketingCallDetailListFromInvoca.Count > 0)
            {
                startAfterId = marketingCallDetailListFromInvoca.FirstOrDefault().SessionId;
                //startAfterId = "5909-8C22E83077D7";
            }
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;
            //var fromDate = new DateTime(2025, 9, 18);
            //var toDate = new DateTime(2025, 9, 28);

            _logService.Info(string.Format("Getting API data For from Time for Marketing Class for dates - {0} and {1}", fromDate, toDate));
            var result = GetCallDetailFromAPIInvoca(fromDate, toDate, startAfterId);
            var list = new List<InvocaCallDetails>();
            var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate };
            try
            {
                //list = (new JavaScriptSerializer()).Deserialize<List<InvocaCallDetails>>(result);
                list = JsonConvert.DeserializeObject<List<InvocaCallDetails>>(result, settings);
            }
            catch (Exception e1)
            {
                _logService.Error("Error in GetCallDetailsInvoca Function: " + e1);
            }
            if (list != null)
            {
                _logService.Info(string.Format(" Total Records - {0}", list.Count()));
            }
            if (list != null)
                SaveCallDetailsInvoca(list);
        }

        private void SaveCallDetailsInvoca(List<InvocaCallDetails> list)
        {
            if (!list.Any())
            {
                return;
            }
            var startTime = Convert.ToDateTime(list.FirstOrDefault().start_time_local).Date;
            var endTime = startTime.AddDays(1);
            var franchiseeNotes = _calldetailsreportnotes.Table.Where(x => x.Timestamp >= startTime && x.Timestamp <= endTime && x.MarketingLeadId == null).ToList();
            foreach (var item in list)
            {
                var callDetailInDB = _marketingLeadCallDetailRepository.Table.Where(x => x.SessionId == item.complete_call_id).ToList();
                if (!string.IsNullOrEmpty(item.calling_phone_number))
                {
                    try
                    {
                        var callTypeId = GetCallType(item.mobile);
                        item.SessionId = item.complete_call_id;
                        var model = _marketingLeadsFactory.CreateModelForInvoca(item, callTypeId);
                        var domain = _marketingLeadsFactory.CreateDomain(model);

                        if (!callDetailInDB.Any())
                        {
                            domain.IsNew = true;
                        }
                        else
                        {
                            domain.IsNew = false;
                            domain.Id = callDetailInDB.FirstOrDefault().Id;
                        }

                        _logService.Info(string.Format(" Saving Marketing Lead Class Detail Verion 2 For Phone Label - {0}", domain.PhoneLabel));
                        domain.IsFromNewAPI = true;
                        domain.IsFromInvoca = true;
                        //domain.DateAdded = callDate;
                        if (item.office != null)
                        {
                            var franchiseeName = item.office;
                            var organization = _organizationRepository.Table.FirstOrDefault(x => x.Name.ToLower() == franchiseeName.ToLower());
                            if (organization != null)
                            {
                                domain.FranchiseeId = organization.Id;
                                domain.TagId = (long)TagType.FranchiseDirect;
                            }
                        }
                        _marketingLeadCallDetailRepository.Save(domain);
                        _unitOfWork.SaveChanges();
                        SaveCallDetailsInvocaV2(item, domain.Id, franchiseeNotes);
                        SaveCallDetailsInvocaV3(item, domain.Id);
                        SaveCallDetailsInvocaV4(item, domain.Id);
                        SaveCallDetailsInvocaV5(item, domain.Id);
                        _unitOfWork.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Verson 2 - {0}", ex.InnerException.StackTrace));
                        continue;
                    }
                }
            }
            _unitOfWork.SaveChanges();
        }

        private void SaveCallDetailsInvocaV2(InvocaCallDetails item, long? id, List<CallDetailsReportNotes> noteList)
        {
            if (!string.IsNullOrEmpty(item.complete_call_id))
            {
                try
                {
                    var callDetailInDB = _marketingLeadCallDetailV2Repository.Table.Where(x => x.Sid == item.complete_call_id).ToList();
                    var zipCounty = new ZipCode();
                    if (item.caller_zip != null)
                    {
                        zipCounty = _zipCodeRepository.Table.Where(x => x.CountyId != null && x.Zip == item.caller_zip).OrderByDescending(x => x.Id).FirstOrDefault();
                    }
                        
                    var county = zipCounty != null && zipCounty != default(ZipCode) ? _countyRepository.Table.Where(x => x.Id == zipCounty.CountyId).FirstOrDefault() : new County();
                    var route = county != null ? county.FranchiseeName : "";
                    var domain = _marketingLeadsFactory.CreateModelForInvocaAPI(item, item.complete_call_id, route);
                    if (!callDetailInDB.Any())
                    {
                        domain.IsNew = true;
                    }
                    else
                    {
                        domain.IsNew = false;
                        domain.Id = callDetailInDB.FirstOrDefault().Id;
                    }

                    _logService.Info(string.Format(" Saving Marketing Lead Class Detail Version 2 For Phone Label - {0}", domain.PhoneLabel));
                    domain.MarketingLeadCallDetailId = id;
                    _marketingLeadCallDetailV2Repository.Save(domain);
                    SaveFranchiseeNotes(domain, noteList);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Version 2 - {0}", ex.InnerException.StackTrace));
                    //continue;
                }
            }
            _unitOfWork.SaveChanges();
        }

        private void SaveCallDetailsInvocaV3(InvocaCallDetails item, long? id)
        {
            if (!string.IsNullOrEmpty(item.complete_call_id))
            {
                try
                {
                    var callDetailInDB = _marketingLeadCallDetailV3Repository.Table.Where(x => x.Sid == item.complete_call_id).ToList();
                    var domain = _marketingLeadsFactory.CreateModelForInvocaAPI3(item, item.complete_call_id);
                    if (!callDetailInDB.Any())
                    {
                        domain.IsNew = true;
                    }
                    else
                    {
                        domain.IsNew = false;
                        domain.Id = callDetailInDB.FirstOrDefault().Id;
                    }

                    _logService.Info(string.Format(" Saving Marketing Lead Class Detail Version 3 For Phone Label - {0}", domain.Sid));
                    domain.MarketingLeadCallDetailId = id;
                    _marketingLeadCallDetailV3Repository.Save(domain);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Version 3 - {0}", ex.InnerException.StackTrace));
                    //continue;
                }
            }
            _unitOfWork.SaveChanges();
        }

        private void SaveCallDetailsInvocaV4(InvocaCallDetails item, long? id)
        {
            if (!string.IsNullOrEmpty(item.complete_call_id))
            {
                try
                {
                    var callDetailInDB = _marketingLeadCallDetailV4Repository.Table.Where(x => x.Sid == item.complete_call_id).ToList();
                    var domain = _marketingLeadsFactory.CreateModelForInvocaAPI4(item, item.complete_call_id);
                    if (!callDetailInDB.Any())
                    {
                        domain.IsNew = true;
                    }
                    else
                    {
                        domain.IsNew = false;
                        domain.Id = callDetailInDB.FirstOrDefault().Id;
                    }
                    _logService.Info(string.Format("Saving Marketing Lead Class Detail Version 3 For Phone Label - {0}", domain.Sid));
                    domain.MarketingLeadCallDetailId = id;
                    _marketingLeadCallDetailV4Repository.Save(domain);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Version 3 - {0}", ex.InnerException.StackTrace));
                    //continue;
                }
            }
            _unitOfWork.SaveChanges();
        }

        private void SaveCallDetailsInvocaV5(InvocaCallDetails item, long? id)
        {
            if (!string.IsNullOrEmpty(item.complete_call_id))
            {
                try
                {
                    var callDetailInDB = _marketingLeadCallDetailV5Repository.Table.Where(x => x.Sid == item.complete_call_id).ToList();
                    var domain = _marketingLeadsFactory.CreateModelForInvocaAPI5(item, item.complete_call_id);
                    if (!callDetailInDB.Any())
                    {
                        domain.IsNew = true;
                    }
                    else
                    {
                        domain.IsNew = false;
                        domain.Id = callDetailInDB.FirstOrDefault().Id;
                    }

                    _logService.Info(string.Format(" Saving Marketing Lead Class Detail Verion 3 For Phone Lable - {0}", domain.Sid));
                    domain.MarketingLeadCallDetailId = id;
                    _marketingLeadCallDetailV5Repository.Save(domain);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error Saving Marketing Lead Class Detail For Verson 3 - {0}", ex.InnerException.StackTrace));
                    //continue;
                }
            }
            _unitOfWork.SaveChanges();
        }

        private string GetCallDetailFromAPIInvoca(DateTime fromDate, DateTime toDate, string startAfterId)
        {
            string fromDateString = fromDate.ToString("yyyy-MM-dd");
            string toDateString = toDate.ToString("yyyy-MM-dd");
            // string url = string.Format("https://marblelife.invoca.net/api/2020-10-01/networks/transactions/2423.json?from={0}&to={1}&oauth_token=AgpaEpKjQKtYpgqyPcwP8LnBkNBVINZ5", fromDateString, toDateString);
            string url = string.Format("https://marblelife.invoca.net/api/2020-10-01/networks/transactions/2423.json?include_columns=$invoca_custom_columns,$invoca_default_columns&from={0}&to={1}&oauth_token=AgpaEpKjQKtYpgqyPcwP8LnBkNBVINZ5&start_after_transaction_id={2}", fromDateString, toDateString, startAfterId);
            var result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add("oauth_token", "AgpaEpKjQKtYpgqyPcwP8LnBkNBVINZ5");
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Web Exception :", ex.Message));
                }
            }
            return result;
        }
        private bool SaveFranchiseeNotes(MarketingLeadCallDetailV2 domain, List<CallDetailsReportNotes> noteList)
        {
            noteList = noteList.Where(x => x.MarketingLeadId == null).ToList();
            var callerId = domain.CallerId.Replace("-", "");
            var callerIdDomain = noteList.FirstOrDefault(x => x.CallerId == callerId);
            if(callerIdDomain != null)
            {
                callerIdDomain.MarketingLeadId = domain.Id;
                _calldetailsreportnotes.Save(callerIdDomain);
            }
            return true;
        }

        #endregion
        private bool SyncOldCallDetailsNotes()
        {
            var oldNotes = _calldetailsreportnotes.Table.Where(x => x.MarketingLeadId == null).ToArray();
            var callDetails = _marketingLeadCallDetailV2Repository.Table.OrderByDescending(x => x.Id).ToArray();
            foreach(var callNote in oldNotes)
            {
                var isPresent = callDetails.Any(x => x.CallerId == callNote.CallerId);
                if (isPresent)
                {
                    var marketingCallId = callDetails.FirstOrDefault(x => x.CallerId == callNote.CallerId).Id;
                    callNote.MarketingLeadId = marketingCallId;
                    _calldetailsreportnotes.Save(callNote);
                    _unitOfWork.SaveChanges();
                }
            }
            return true;
        }
        private bool SyncMarketingLeadCallDetailsIdWithCallNotes()
        {
            var oldNotes = _calldetailsreportnotes.Table.Where(x => x.MarketingLeadIdFromCallDetailsReport == null).ToArray();
            var callDetails = _marketingLeadCallDetailRepository.Table.OrderByDescending(x => x.Id).ToArray();
            foreach (var callNote in oldNotes)
            {
                var isPresent = callDetails.Any(x => x.CallerId == callNote.CallerId);
                if (isPresent)
                {
                    var marketingCallId = callDetails.FirstOrDefault(x => x.CallerId == callNote.CallerId).Id;
                    callNote.MarketingLeadIdFromCallDetailsReport = marketingCallId;
                    _calldetailsreportnotes.Save(callNote);
                    _unitOfWork.SaveChanges();
                }
            }
            return true;
        }
    }
}
