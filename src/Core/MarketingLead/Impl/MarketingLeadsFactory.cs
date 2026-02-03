using Core.Application;
using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.MarketingLead.Domain;
using Core.MarketingLead.Enum;
using Core.MarketingLead.ViewModel;
using Core.Sales;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class MarketingLeadsFactory : IMarketingLeadsFactory
    {
        private readonly ICustomerService _customerService;
        private readonly IClock _clock;
        private static string dateTimeString = "";
        public MarketingLeadsFactory(ICustomerService customerService, IClock clock)
        {
            _customerService = customerService;
            _clock = clock;
        }
        public MarketingLeadCallDetailViewModel CreateModel(CallRetailRecord record, long callTypeId)
        {
            DateTime dateAdded = DateTime.ParseExact(record.DateAdded, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var model = new MarketingLeadCallDetailViewModel
            {
                SessionId = record.SessionId,
                CallDuration = record.CallDuration,
                CallerId = record.CallerId,
                CallTypeId = callTypeId,
                ClickDescription = record.ClickDescription,
                DateAdded = _clock.ToUtc(dateAdded),
                DialedNumber = record.DialedNumber,
                PhoneLabel = record.PhoneLabel,
                TransferToNumber = record.TransferToNumber,
                TransferType = record.TransferType
            };
            return model;
        }

        public MarketingLeadCallDetail CreateDomain(MarketingLeadCallDetailViewModel model)
        {
            var domain = new MarketingLeadCallDetail
            {
                CallTransferType = model.TransferType,
                CallDuration = model.CallDuration,
                CallerId = model.CallerId,
                CallTypeId = model.CallTypeId,
                ClickDescription = model.ClickDescription,
                DateAdded = model.DateAdded,
                DialedNumber = model.DialedNumber,
                PhoneLabel = model.PhoneLabel != null ? model.PhoneLabel : "",
                TransferToNumber = model.TransferToNumber,
                SessionId = model.SessionId,
                TagId = (long)TagType.National,
                IsNew = true,
            };
            return domain;
        }

        public RoutingNumber CreateDomain(string phoneNumber, string phoneLabel)
        {
            var domain = new RoutingNumber
            {
                PhoneLabel = phoneLabel,
                PhoneNumber = phoneNumber,
                IsNew = true
            };
            return domain;
        }

        public WebLead CreateDomain(WebLeadViewModel model)
        {
            DateTime createdDate = DateTime.UtcNow;
            model.date_created = model.date_created.Split(',')[0];
            if (model.date_created != null)
                createdDate = DateTime.ParseExact(model.date_created, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var phone = Regex.Replace(model.Phone, @"[\s()-]", "");
            var domain = new WebLead
            {
                WebLeadId = model.Id,
                WebLeadFranchiseeId = model.franchise_id,
                Firstname = model.name_first,
                LastName = model.name_last,
                Country = model.Country,
                ProvinceName = model.Province,
                County = model.County,
                ZipCode = model.zip,
                Email = model.Email,
                Phone = phone,
                StreetAddress = model.Addr + " " + model.Addr2,
                SuiteNumber = model.Suite,
                City = model.City,
                PropertyType = model.type_property,
                SurfaceType = model.type_surface,
                ServiceDesc = model.surface_desc,
                Contact = model.source_contact,
                AddEmail = model.AddEmail == null || model.AddEmail == "off" ? false : true,
                URL = model.source_url,
                Status = model.status.Value,
                CreatedDate = createdDate,
                FEmail = model.franchise_email,
                IsNew = true,
                FranchiseeId = model.franchise_id
            };
            return domain;
        }

        public CallDetailViewModel CreateViewModel(MarketingLeadCallDetail domain)
        {
            var model = new CallDetailViewModel
            {
                Id = domain.Id,
                Dnis = domain.DialedNumber,
                Ani = domain.CallerId,
                CallType = domain.CallType != null ? domain.CallType.Name : null,
                PhoneLabel = domain.PhoneLabel,
                TransferToNumber = domain.TransferToNumber,
                TransferType = domain.CallTransferType,
                DateOfCall = Convert.ToDateTime(_clock.ToLocal(domain.DateAdded.GetValueOrDefault())),
                CallDuration = domain.CallDuration,
                CallTypeId = domain.CallTypeId,
                Franchisee = domain.Franchisee != null ? domain.Franchisee.Organization.Name : null,
                Tag = domain.Tag != null ? domain.Tag.Name : null,
                InvoiceId = domain.Invoice != null ? (domain.InvoiceId != null ? domain.InvoiceId.Value.ToString() : "") : "",
                DataFromNewAPI = domain.IsFromNewAPI == true ? "Yes" : "No",
                DataFromInvoca = domain.IsFromInvoca == true ? "Yes" : "No"
            };
            return model;
        }

        public RoutingNumberViewModel CreateViewModel(RoutingNumber domain)
        {
            var model = new RoutingNumberViewModel
            {
                Id = domain.Id,
                PhoneLabel = domain.PhoneLabel,
                PhoneNumber = domain.PhoneNumber,
                Franchisee = domain.Franchisee != null ? domain.Franchisee.Organization.Name : null,
                FranchiseeId = domain.FranchiseeId.ToString(),
                Tag = domain.Tag != null ? domain.Tag.Name : null,
                TagId = domain.TagId.ToString()
            };
            return model;
        }

        public WebLeadInfoModel CreateViewModel(WebLead domain)
        {
            try
            {
                var franchiseeName = "";
                if (domain.WebLeadFranchiseeId == 57 || domain.WebLeadFranchiseeId == 88)
                {
                    if (domain.WebLeadFranchiseeId == 57 && domain.Country == "SouthAfrica")
                    {
                        franchiseeName = "I-SOUTH AFRICA (Johannesburgh-Pretoria)";
                    }
                    else if (domain.WebLeadFranchiseeId == 88 && domain.Country == "UnitedArabEmirates")
                    {
                        franchiseeName = "I-UAE";
                    }
                }
                else
                {
                    franchiseeName = domain.Franchisee != null ? domain.Franchisee.Organization.Name : null;
                }
                //var model = new WebLeadInfoModel
                //{
                //    Id = domain.Id,
                //    WebLeadId = domain.WebLeadId,
                //    Name = domain.Firstname + " " + domain.LastName,
                //    Email = domain.Email,
                //    Phone = domain.Phone,
                //    StreetAddress = domain.StreetAddress,
                //    SuiteNumber = !string.IsNullOrEmpty(domain.SuiteNumber) ? domain.SuiteNumber : null,
                //    City = domain.City,
                //    Province = domain.ProvinceName,
                //    County = domain.County,
                //    Country = domain.Country,
                //    PropertyType = domain.PropertyType,
                //    SurfaceType = domain.SurfaceType,
                //    Contact = domain.Contact,
                //    FranchiseeEmail = domain.FEmail,
                //    ServiceDescription = domain.ServiceDesc,
                //    FranchiseeId = domain.FranchiseeId,
                //    Zip = domain.ZipCode,
                //    Date = domain.CreatedDate,
                //    Franchisee = franchiseeName,
                //    InvoiceId = domain.Invoice != null ? (domain.InvoiceId != null ? domain.InvoiceId.Value : 0) : 0,
                //    Url = domain.URL
                //};
                //return model;
                var model = new WebLeadInfoModel
                {
                    Id = domain != null ? domain.Id : 0,
                    Date = domain != null ? domain.CreatedDate : DateTime.MinValue,
                    Name = CleanString((domain != null && domain.Firstname != null ? domain.Firstname : "") + " " + (domain != null && domain.LastName != null ? domain.LastName : "")),
                    Email = CleanString(domain != null && domain.Email != null ? domain.Email : ""),
                    Phone = CleanString(domain != null && domain.Phone != null ? domain.Phone : ""),
                    StreetAddress = CleanString(domain != null && domain.StreetAddress != null ? domain.StreetAddress : ""),
                    SuiteNumber = CleanString(domain != null && !string.IsNullOrWhiteSpace(domain.SuiteNumber) ? domain.SuiteNumber : ""),
                    City = CleanString(domain != null && domain.City != null ? domain.City : ""),
                    Province = CleanString(domain != null && domain.ProvinceName != null ? domain.ProvinceName : ""),
                    County = CleanString(domain != null && domain.County != null ? domain.County : ""),
                    Country = CleanString(domain != null && domain.Country != null ? domain.Country : ""),
                    PropertyType = CleanString(domain != null && domain.PropertyType != null ? domain.PropertyType : ""),
                    SurfaceType = CleanString(domain != null && domain.SurfaceType != null ? domain.SurfaceType : ""),
                    Contact = CleanString(domain != null && domain.Contact != null ? domain.Contact : ""),
                    FranchiseeEmail = CleanString(domain != null && domain.FEmail != null ? domain.FEmail : ""),
                    ServiceDescription = CleanString(domain != null && domain.ServiceDesc != null ? domain.ServiceDesc : ""),
                    FranchiseeId = domain != null ? domain.FranchiseeId : null,
                    Zip = CleanString(domain != null && domain.ZipCode != null ? domain.ZipCode : ""),
                    Franchisee = CleanString(franchiseeName != null ? franchiseeName : ""),
                    InvoiceId = (domain != null && domain.Invoice != null) ? (domain.InvoiceId.HasValue ? domain.InvoiceId.Value : 0) : 0,
                    WebLeadId = domain != null ? domain.WebLeadId : 0,
                    Url = CleanString(domain != null && domain.URL != null ? domain.URL : "")
                };

                return model;
            }
            catch (Exception ex)
            {
                return new WebLeadInfoModel();
            }
        }

        public static string CleanString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            var sb = new StringBuilder(text.Length);

            foreach (char ch in text)
            {
                // Valid XML chars (Excel compatible)
                if (ch == 0x9 || ch == 0xA || ch == 0xD ||
                   (ch >= 0x20 && ch <= 0xD7FF) ||
                   (ch >= 0xE000 && ch <= 0xFFFD))
                {
                    sb.Append(ch);
                }
                // 0x00 and other control characters will be skipped
            }

            return sb.ToString().Trim();
        }


        public MarketingLeadChartViewModel CreateViewModel(MarketingLeadChartViewModel viewModel, int? reportType)
        {
            var model = new MarketingLeadChartViewModel();

            if (reportType != 2)
            {
                model = new MarketingLeadChartViewModel
                {
                    Date = (viewModel.Date),
                    Local = viewModel.Local <= 0 ? 0 : Math.Round((viewModel.LocalCount / viewModel.Local) * 100, 2),
                    National = viewModel.Total <= 0 ? 0 : Math.Round((viewModel.TotalCount / viewModel.Total) * 100, 2),
                    Total = reportType != 1 && reportType != 5 && reportType != 8 ? viewModel.Local : viewModel.LocalCount,
                    DayOfWeek = viewModel.Date.DayOfWeek.ToString(),
                    DateString = (viewModel.Date).ToShortDateString(),
                    LastYearDateString = (viewModel.Date.AddDays(-6)).ToShortDateString() + "-" + (viewModel.Date).ToShortDateString()
                };
            }
            else
            {

                model = new MarketingLeadChartViewModel
                {
                    Date = (viewModel.Date),
                    Local = viewModel.Local <= 0 ? 0 : viewModel.LocalCount,
                    National = viewModel.Total <= 0 ? 0 : viewModel.TotalCount,
                    Total = viewModel.Total,
                    DayOfWeek = viewModel.Date.DayOfWeek.ToString(),
                    DateString = (viewModel.Date).ToShortDateString(),
                    LastYearDateString = (viewModel.DateForGraph.AddDays(-6)).ToShortDateString() + "-" + (viewModel.DateForGraph).ToShortDateString()
                };
            }
            if (reportType == 1)
            {

                if (model.DayOfWeek == "Saturday" || model.DayOfWeek == "Sunday")
                {
                    model.Color = "grey";
                }
                else
                {
                    model.Color = "blue";
                }
            }
            else if (reportType == 2)
            {
                int times = Convert.ToInt32(model.Date.TimeOfDay.Hours);
                if (model.Date.TimeOfDay.Hours < 8 || model.Date.TimeOfDay.Hours > 17)
                {
                    model.Color = "grey";
                }
                else
                {
                    model.Color = "blue";
                }
            }
            return model;
        }

        public HomeAdvisorParentModel CreateViewModelForHomeAdvisor(HomeAdvisor domain)
        {
            return new HomeAdvisorParentModel()
            {
                CityName = domain.City != null ? domain.City.Name : domain.CityName,
                StateName = domain.State != null ? domain.State.Name : domain.StateName,
                CompanyName = domain.CompanyName,
                HAAccount = domain.HAAccount,
                LeadType = domain.LeadType,
                NetLeadDollar = domain.NetLeadDollar,
                SRID = domain.SRID,
                SRSubmittedDate = domain.SRSubmittedDate,
                Task = domain.Task,
                ZipCode = domain.ZipCode,
                Id = domain.Id,
                FranchiseeName = domain.Franchisee != null ? domain.Franchisee.Organization.Name : ""
            };
        }

        public MarketingLeadCallDetailV2 CreateModel(CallDetailV2 record)
        {
            var domain = new MarketingLeadCallDetailV2
            {
                APPState = record.APP_state,
                CallDuration = record.call_duration,
                CallerId = record.caller_id,
                CallRoute = record.call_route,
                CallRouteQualified = record.call_route_qualified,
                CallStatus = record.call_status,
                City = record.city,
                Destination = record.destination,
                EnteredZipCode = record.entered_zip,
                FirstName = record.first_name,
                LastName = record.last_name,
                IvrResults = record.ivr_results,
                PhoneLabel = record.phone_label,
                RepeaSourceCaller = record.repeat_source_caller,
                CallDate = record.call_date,
                Reroute = record.reroute,
                SetName = record.set_name,
                Sid = record.sid,
                Source = record.source,
                SourceCap = record.source_cap,
                SourceNumber = record.source_number,
                SourceQualified = record.source_qualified,
                State = record.state,
                StreetAddress = record.street_address,
                TalkMintues = record.talk_minutes,
                TalkSeconds = record.talk_seconds,
                TotalMintues = record.total_minutes,
                TotalSeconds = record.total_seconds,
                TransferNumber = record.transfer_number,
                ZipCode = record.zipcode


            };
            return domain;
        }

        public CallDetailViewModelV2 CreateViewModel(MarketingLeadCallDetailV2 domain)
        {
            var model = new CallDetailViewModelV2
            {
                Id = domain.Id,
                APPState = domain.APPState,
                CallDuration = domain.CallDuration,
                CallerId = domain.CallerId,
                CallRoute = domain.CallRoute,
                CallRouteQualified = domain.CallRouteQualified,
                CallStatus = domain.CallStatus,
                City = domain.City,
                Destination = domain.Destination,
                FirstName = domain.FirstName,
                RepeaSourceCaller = domain.RepeaSourceCaller,
                EnteredZipCode = domain.EnteredZipCode,
                FranchiseeName = domain.Franchisee != null ? domain.Franchisee.Organization.Name : "",
                IvrResults = domain.IvrResults,
                PhoneLabel = domain.PhoneLabel,
                Reroute = domain.Reroute,
                LastName = domain.LastName,
                SourceNumber = domain.SourceNumber,
                SetName = domain.SetName,
                TalkMintues = domain.TalkMintues,
                TalkSeconds = domain.TalkSeconds,
                Sid = domain.Sid,
                StreetAddress = domain.StreetAddress,
                TransferNumber = domain.TransferNumber,
                ZipCode = domain.ZipCode,
                SourceQualified = domain.SourceQualified,
                State = domain.APPState,
                SourceCap = domain.SourceCap,
                TotalSeconds = domain.TotalSeconds,
                TotalMintues = domain.TotalMintues,
                Source = domain.Source,
                DateOfCall = domain.MarketingLeadCallDetail != null ? Convert.ToDateTime(_clock.ToLocal(domain.MarketingLeadCallDetail.DateAdded.GetValueOrDefault()))
                : DateTime.MinValue,
                DataFromNewAPI = domain.MarketingLeadCallDetail != null ? domain.MarketingLeadCallDetail.IsFromNewAPI == true ? "Yes" : "No" : "No",
                DataFromInvoca = domain.MarketingLeadCallDetail != null ? domain.MarketingLeadCallDetail.IsFromInvoca == true ? "Yes" : "No" : "No"

            };
            return model;
        }

        public MarketingLeadCallDetailViewModel CreateModelForNewApi(CallRetailRecordV3 record, long callTypeId)
        {
            var dateTimeString = DateTime.Now;
            try
            {
                var dateTimeSplitted = record.start_time.Split('+');
                var dateTimeSpl = dateTimeSplitted[0].Split('T');
                var dateString = dateTimeSpl[0];
                var timeString = dateTimeSpl[1];
                dateTimeString = DateTime.Parse(dateString + " " + timeString);
                //var dateAdded2= dateAddedDate.ToString("yyyy-MM-dd HH: mm:ss");
            }
            catch (Exception e1)
            {
                dateTimeString = Convert.ToDateTime(record.start_time);
            }
            var dateTime = Convert.ToDateTime(record.start_time);
            var dateTime2 = DateTime.Parse(record.start_time);

            //var dateTime3 = DateTime.ParseExact(record.start_time);
            var model = new MarketingLeadCallDetailViewModel
            {
                SessionId = record.SessionId,
                CallDuration = Convert.ToInt32(record.call_duration_rounded_minutes),
                CallerId = record.caller_id,
                CallTypeId = callTypeId,
                ClickDescription = record.click_description,
                //DateAdded = _clock.ToUtc(dateAdded),
                DialedNumber = record.dialed_number,
                PhoneLabel = record.phone_label,
                TransferToNumber = record.call_transfer.transfer_to_number,
                TransferType = record.call_transfer.transfer_type,
                DateAdded = (dateTimeString),
            };
            return model;
        }


        public MarketingLeadCallDetailV2 CreateModelForNewAPI(CallRetailRecordV3 record, string sid, DateTime callDate)
        {
            //var dateTimeSplitted = record.start_time.Split('+');
            //var dateTimeSpl = dateTimeSplitted[0].Split('T');
            //var dateString = dateTimeSpl[0];
            //var timeString = dateTimeSpl[1];
            //var dateTimeString = DateTime.Parse(dateString + " " + timeString);

            var dateTime = Convert.ToDateTime(record.start_time);
            var domain = new MarketingLeadCallDetailV2
            {
                CallDuration = record.call_duration_rounded_minutes.ToString(),
                CallRoute = record.advanced_routing.callflow.callflow_call_route,
                CallRouteQualified = record.advanced_routing.leadflow.leadflow_affiliate_qualified,
                CallStatus = GetStatus(record.call_transfer),
                Destination = record.advanced_routing.callflow.callflow_destination,
                EnteredZipCode = record.advanced_routing.leadflow.leadflow_entered_zip,
                PhoneLabel = record.phone_label,
                CallDate = _clock.ToUtc(dateTime),
                Reroute = record.advanced_routing.callflow.callflow_reroute,
                SetName = record.advanced_routing.leadflow.leadflow_set_name,
                Sid = sid,
                Source = record.advanced_routing.callflow.callflow_source,
                SourceNumber = record.advanced_routing.callflow.callflow_call_route_qualified,
                SourceQualified = record.advanced_routing.callflow.callflow_call_route_qualified,
                TransferNumber = record.call_transfer.transfer_to_number,
                CallerId = record.caller_id
            };
            return domain;
        }


        public MarketingLeadCallDetailV3 CreateModelForNewAPI3(CallRetailRecordV3 record, string sid, DateTime callDate)
        {

            var dateTime = Convert.ToDateTime(record.start_time);
            var domain = new MarketingLeadCallDetailV3
            {
                CallflowDestination = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_destination : "",
                CallflowDestinationId = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_destination_id : "",
                CallflowEnteredZip = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_entered_zip : "",
                CallflowRepeatSourceCaller = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_repeat_source_caller : "",
                CallflowReroute = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_reroute : "",
                CallflowSetId = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_set_id : "",
                CallflowSetName = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_set_name : "",
                CallflowSource = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_source : "",
                CallflowSourceCap = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_source_cap : "",
                CallflowSourceId = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_source_id : "",
                CallflowSourceQualified = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_source_qualified : "",
                CallflowSourceRoute = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_call_route : "",
                CallflowSourceRouteId = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_call_route_id : "",
                CallflowSourceRouteQualified = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_call_route_qualified : "",
                CallflowState = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_state : "",
                CallNote_CallAnalytics = (record.call_analytics != null) ? record.call_analytics.call_note : "",
                CallTransferStatus_CallFlow = (record.call_transfer != null) ? record.call_transfer.call_transfer_status : "",
                City_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].current_address.city : "",
                StateCode_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].current_address.state_code : "",
                FirstName_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].first_name : "",
                LastName_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].last_name : "",
                GeoLookupAttempt_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].geo_lookup_attempt : "",
                GeoLookupResult_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].geo_lookup_result : "",
                KeywordGroups_CallAnalytics = record.call_analytics != null ? string.Join(",", record.call_analytics.keyword_groups) : "",
                KeywordSpottingComplete_CallAnalytics = record.call_analytics != null ? record.call_analytics.keyword_spotting_complete : "",
                PostalArea_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].current_address.postal_code : "",
                StreetLine1_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].current_address.street_line_1 : "",
                RingCount_CallFlow = (record.call_transfer != null) ? record.call_transfer.ring_count : default(decimal),
                RingSeconds_CallFlow = (record.call_transfer != null) ? record.call_transfer.ring_seconds : default(decimal),
                Sid = sid

            };
            return domain;
        }


        public MarketingLeadCallDetailV4 CreateModelForNewAPI4(CallRetailRecordV2 record, string sid, DateTime callDate)
        {

            var domain = new MarketingLeadCallDetailV4
            {
                Sid = sid,
                ActivityKeyword_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.activity_keyword : "",
                ActivityTag_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.activity_tag : "",
                ActivityTypeId_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.activity_type_id : "",
                ActivityValue_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.activity_value : "",
                Bid_Attribution = (record.Attribution.visitor_data != null) ? record.Attribution.Bid : "",
                BrowserVersion_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.browser_version : "",
                Browser_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.Browser : "",
                CallActivities = (record.Attribution.visitor_data != null) ? string.Join(",", record.call_details.call_activities) : "",
                Campaign_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.campaign : "",
                Channel_Attribution = (record.Attribution.visitor_data != null) ? record.Attribution.Channel : "",
                Device_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.Browser : "",
                DocumentPath_FirstTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.first_touch.document_path : "",
                DocumentTimeStamp_FirstTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.first_touch.document_timestamp : "",
                DocumentTitle_FirstTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.first_touch.document_title : "",
                DocumentUrl_FirstTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.first_touch.document_url : "",
                DocumentPath_LastTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.last_touch.document_path : "",
                DocumentTitle_LastTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.last_touch.document_title : "",
                DocumentTimeStamp_LastTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.last_touch.document_timestamp : "",
                DocumentUrl_LastTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.last_touch.document_url : "",
                GClid_VisitorData = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.visitor_data.gclid : "",
                GoogleUaClientId_VisitorData = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.visitor_data.google_ua_client_id : "",
                IPAddress_VisitorData = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.visitor_data.ip_address : "",
                MissedCall_CallMetrics = (record.call_details != null && record.call_details.call_metrics != null) ? record.call_details.call_metrics.MissedCall : "",
                OsVersion_VisitorData = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.visitor_data.os_version : "",
                Os_VisitorData = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.visitor_data.Os : "",
                Pid_Attribution = (record.Attribution != null) ? record.Attribution.Pid : "",
                Platform_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.platform : "",
                Rank_Attribution = (record.Attribution.visitor_data != null) ? record.Attribution.Rank : "",
                SearchTerm_VisitorData = (record.Attribution != null && record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.search_term : "",
                Status_Attribution = record.Attribution != null ? record.Attribution.Status : "",
                SourceGuard_VisitorData = (record.Attribution != null && record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.sourceguard : "",
                VisitorLogUrl_VisitorData = (record.Attribution != null && record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.visitor_log_url : "",

            };
            return domain;
        }


        public MarketingLeadCallDetailV5 CreateModelForNewAPI5(CallRetailRecordV2 record, string sid, DateTime callDate)
        {

            var domain = new MarketingLeadCallDetailV5
            {
                Sid = sid,
                AdgroupId_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.adgroup_id : "",
                Adgroup_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.Adgroup : "",
                AdId_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.ad_id : "",
                Ads_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? "" : "",
                CallInlyFlag_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.call_only_flag : "",
                CampaignId_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.campaign_id : "",
                Campaign_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.Campaign : "",
                ClickId_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.click_id : "",
                CustomId_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.custom_id : "",
                CustomValue_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.custom_value : "",
                DomainSetId_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.domain_set_id : "",
                DomainSetName_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.domain_set_name : "",
                KeywordId_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.keyword_id : "",
                KeyWordMatchType_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.keyword_id : "",
                Keywords_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? "" : "",
                LocationName_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.location_name : "",
                PoolName_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.pool_name : "",
                Type_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.Type : "",
                UtmCampaign_DefaultUtmParameters = (record.Attribution != null && record.Attribution.visitor_data != null && record.Attribution.visitor_data.default_utm_parameters != null) ? record.Attribution.visitor_data.default_utm_parameters.utm_campaign : "",
                UtmContent_DefaultUtmParameters = (record.Attribution != null && record.Attribution.visitor_data != null && record.Attribution.visitor_data.default_utm_parameters != null) ? record.Attribution.visitor_data.default_utm_parameters.utm_content : "",
                UtmMedium_DefaultUtmParameters = (record.Attribution != null && record.Attribution.visitor_data != null && record.Attribution.visitor_data.default_utm_parameters != null) ? record.Attribution.visitor_data.default_utm_parameters.utm_medium : "",
                UtmSource_DefaultUtmParameters = (record.Attribution != null && record.Attribution.visitor_data != null && record.Attribution.visitor_data.default_utm_parameters != null) ? record.Attribution.visitor_data.default_utm_parameters.utm_source : "",
                UtmTerm_DefaultUtmParameters = (record.Attribution != null && record.Attribution.visitor_data != null && record.Attribution.visitor_data.default_utm_parameters != null) ? record.Attribution.visitor_data.default_utm_parameters.utm_term : "",
                VtAceid_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_aceid : "",
                VtAdposition_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_adposition : "",
                VtAdType_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_adtype : "",
                VtCreative_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_creative : "",
                VtDeviceModel_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_devicemodel : "",
                VtDevice_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_device : "",
                VtKeyword_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_keyword : "",
                VtMatchType_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_matchtype : "",
                VtNetwork_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_network : "",
                VtParam1_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_param1 : "",
                VtParam2_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_param2 : "",
                VtPlacement_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_placement : "",
                VtProductTargetId_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_producttargetid : "",
                VtRandom_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_random : "",
                VtTarget_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_target : "",


            };
            return domain;
        }
        private string GetStatus(CallTransfer callTransfer)
        {
            if (callTransfer == null)
            {
                return "Not Connected";
            }
            else if (callTransfer.call_transfer_status == "ANSWER")
            {
                return "Connected";
            }
            else
            {
                return "Not Connected";
            }
        }

        public PhoneCallViewModel CreatePhoneViewModel(FranchiseeTechMailEmail domain, Phonechargesfee phonechargesfee)
        {

            var phoneCallViewModel = new PhoneCallViewModel()
            {
                ChargesForPhone = domain.ChargesForPhone,
                DateOfChange = domain.DateForCharges,
                PersonName = domain.Person.Name.FirstName + " " + domain.Person.Name.LastName,
                CallCount = domain.CallCount,
                TotalCost = domain.CallCount != null ? (double)(domain.ChargesForPhone * domain.CallCount) : 0,
                Month = domain.DateForCharges != null ? domain.DateForCharges.Value.Date.ToString("MMMM") + "," + domain.DateForCharges.Value.Date.ToString("yyyy") : "",
                Id = domain.Id,
                IsEdit = true,
                IsEditCall = true,
                IsExpand = false,
                FranchiseeId = domain.FranchiseeId,
                IsEditDate = false,
                DateOfChangeString = domain.DateForCharges != null ? (domain.DateForCharges).Value.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'") : "",
                IsInvoiceGenerated = phonechargesfee != null ? phonechargesfee.IsInvoiceGenerated : false,
                IsInvoiceGeneratedFromAPI = phonechargesfee != null ? phonechargesfee.IsInvoiceGenerated : false,
                IsInvoiceInQueue = phonechargesfee != null ? phonechargesfee.IsInvoiceInQueue : false,
                FranchiseeName = (domain != null) ? domain.Franchisee.Organization.Name : "",
                CallCountOld = domain.CallCount,
                DateOfChangeOld = domain.DateForCharges,
                ChargesForPhoneOld = domain.ChargesForPhone,
                InvoiceId = phonechargesfee != null ? phonechargesfee.InvoiceItemId.GetValueOrDefault().ToString() : "",
                PhonechargesfeeId = phonechargesfee != null ? phonechargesfee.Id : default(long?),
                MonthForDataRecorder = domain.DataRecorderMetaData.DateModified != null ? domain.DataRecorderMetaData.DateModified.Value.Date.ToString("MMMM") + "," + domain.DateForCharges.Value.Date.ToString("yyyy") : domain.DataRecorderMetaData.DateCreated.Date.ToString("MMMM") + "," + domain.DataRecorderMetaData.DateCreated.Date.ToString("yyyy"),
                CurrentDate = domain.DataRecorderMetaData.DateModified != null ? domain.DataRecorderMetaData.DateModified.Value.Date : domain.DataRecorderMetaData.DateCreated.Date

            };
            return phoneCallViewModel;
        }


        public PhoneCallVInvoiceiewModel CreatePhoneInvoiceViewModel(Phonechargesfee phonechargesfee)
        {
            var phoneCallViewModel = new PhoneCallVInvoiceiewModel()
            {
                Amount = (double)phonechargesfee.Amount,
                DateCreated = phonechargesfee.DateCreated,
                InvoiceItemId = phonechargesfee.InvoiceItemId,
                IsInvoiceGenerated = phonechargesfee.IsInvoiceGenerated,
                IsInvoiceInQueue = phonechargesfee.IsInvoiceInQueue,
                Id = phonechargesfee.Id,
                PhonechargesfeeId = phonechargesfee.Id



            };
            return phoneCallViewModel;
        }


        public CallDetailViewModel CreateNewViewModel(MarketingLeadCallDetail domain, MarketingLeadCallDetailV3 domain3,
            MarketingLeadCallDetailV4 domain4, MarketingLeadCallDetailV5 domain5, MarketingLeadCallDetailV2 domain2, List<CallDetailsReportNotes> callNotes)
        {
            try
            {
                var note = string.Empty;
                var preferredContactNumber = default(long?);
                var firstName = string.Empty;
                var lastName = string.Empty;
                var company = string.Empty;
                var office = string.Empty;
                var zipCode = string.Empty;
                var resultingAction = string.Empty;
                var houseNumber = string.Empty;
                var street = string.Empty;
                var city = string.Empty;
                var state = string.Empty;
                var country = string.Empty;
                var email = string.Empty;
                if (callNotes.Count > 0)
                {
                    //var callnote = callNotes.LastOrDefault(x => x.Timestamp.AddMinutes(2) >= domain.DateAdded && x.Timestamp.AddMinutes(-2) <= domain.DateAdded);

                    var callnote = domain2 != null ? callNotes.LastOrDefault(x => x.MarketingLeadId != null && x.MarketingLeadId == domain2.Id) : null;
                    note = callnote != null ? callnote.Notes : "";
                    preferredContactNumber = callnote != null ? callnote.PreferredContactNumber : null;
                    firstName = callnote != null ? callnote.FirstName : null;
                    lastName = callnote != null ? callnote.LastName : null;
                    company = callnote != null ? callnote.Company : null;
                    office = callnote != null ? callnote.Office : null;
                    zipCode = callnote != null ? callnote.ZipCode : null;
                    resultingAction = callnote != null ? callnote.ResultingAction : null;
                    houseNumber = callnote != null ? callnote.HouseNumber : null;
                    street = callnote != null ? callnote.Street : null;
                    city = callnote != null ? callnote.City : null;
                    state = callnote != null ? callnote.State : null;
                    country = callnote != null ? callnote.Country : null;
                    email = callnote != null ? callnote.EmailId : null;
                }
                else
                {
                    note = string.Empty;
                }
                var model = new CallDetailViewModel
                {
                    Id = domain.Id,
                    Dnis = domain.DialedNumber,
                    Ani = domain.CallerId,
                    CallType = domain.CallType != null ? domain.CallType.Name : null,
                    PhoneLabel = domain.PhoneLabel,
                    TransferToNumber = domain.TransferToNumber,
                    TransferType = domain.CallTransferType,
                    //DateOfCall = Convert.ToDateTime(_clock.ToLocal(domain.DateAdded.GetValueOrDefault())),
                    DateOfCall = Convert.ToDateTime(domain.DateAdded.GetValueOrDefault()),
                    CallDuration = domain.CallDuration,
                    ValidCall = domain.CallDuration >= 2 ? true : false,
                    CallTypeId = domain.CallTypeId,
                    Franchisee = domain.Franchisee != null ? domain.Franchisee.Organization.Name : null,
                    Tag = domain.Tag != null ? domain.Tag.Name : null,
                    InvoiceId = domain.Invoice != null ? (domain.InvoiceId != null ? domain.InvoiceId.Value.ToString() : "") : "",
                    DataFromNewAPI = domain.IsFromNewAPI == true ? "Yes" : "No",
                    DataFromInvoca = domain.IsFromInvoca == true ? "Yes" : "No",
                    CallFlowSetName = domain3 != null ? domain3.CallflowSetName : "",
                    FindMeList = domain2 != null ? domain2.FindMeList : "",
                    CallFlowSetId = domain3 != null ? domain3.CallflowSetId : "",
                    CallFlowDestination = domain3 != null ? domain3.CallflowDestination : "",
                    CallFlowDestinationId = domain3 != null ? domain3.CallflowDestinationId : "",
                    CallFlowSource = domain3 != null ? domain3.CallflowSource : "",
                    CallFlowSourceId = domain3 != null ? domain3.CallflowSourceId : "",
                    CallFlowSourceQualified = domain3 != null ? domain3.CallflowSourceQualified : "",
                    CallFlowRepeatSourceCaller = domain3 != null ? domain3.CallflowRepeatSourceCaller : "",
                    CallFlowSourceCap = domain3 != null ? domain3.CallflowSourceCap : "",
                    CallFlowSourceRoute = domain3 != null ? domain3.CallflowSourceRoute : "",
                    CallFlowSourceRouteId = domain3 != null ? domain3.CallflowSourceRouteId : "",
                    CallFlowSourceRouteQualified = domain3 != null ? domain3.CallflowSourceRouteQualified : "",
                    CallFlowState = domain3 != null ? domain3.CallflowState : "",
                    CallFlowEnteredZip = domain3 != null ? domain3.CallflowEnteredZip : "",
                    CallFlowReroute = domain3 != null ? domain3.CallflowReroute : "",
                    TransferToNumber_CallFlow = domain3 != null ? domain3.TransferToNumber : "",
                    TransferType_CallFlow = domain3 != null ? domain3.TransferType_CallFlow : "",
                    CallTransferStatus_CallFlow = domain3 != null ? domain3.CallTransferStatus_CallFlow : "",
                    RingSeconds_CallFlow = domain3 != null ? domain3.RingSeconds_CallFlow : default(decimal),
                    RingCount_CallFlow = domain3 != null ? domain3.RingCount_CallFlow : default(decimal),
                    KeywordGroups_CallAnalytics = domain3 != null ? domain3.KeywordGroups_CallAnalytics : "",
                    KeywordSpottingComplete_CallAnalytics = domain3 != null ? domain3.KeywordSpottingComplete_CallAnalytics : "",
                    TranscriptionStatus_CallAnalytics = domain3 != null ? domain3.TranscriptionStatus_CallAnalytics : "",
                    CallNote_CallAnalytics = domain3 != null ? domain3.CallNote_CallAnalytics : "",
                    RecordingUrl_Recording = domain3 != null ? domain3.RecordingUrl_Recording : "",
                    RecordedSeconds_Recording = domain3 != null ? domain3.RecordedSeconds_Recording : "",
                    DialogAnalytics_Recording = domain3 != null ? domain3.DialogAnalytics_Recording : "",
                    FirstName_ReverseLookUp = domain3 != null ? domain3.FirstName_ReverseLookUp : "",
                    LastName_ReverseLookUp = domain3 != null ? domain3.LastName_ReverseLookUp : "",
                    StreetLine1_ReverseLookUp = domain3 != null ? domain3.StreetLine1_ReverseLookUp : "",
                    City_ReverseLookUp = domain3 != null ? domain3.City_ReverseLookUp : "",
                    PostalArea_ReverseLookUp = domain3 != null ? domain3.PostalArea_ReverseLookUp : "",
                    StateCode_ReverseLookUp = domain3 != null ? domain3.StateCode_ReverseLookUp : "",
                    GeoLookupAttempt_ReverseLookUp = domain3 != null ? domain3.GeoLookupAttempt_ReverseLookUp : "",
                    GeoLookupResult_ReverseLookUp = domain3 != null ? domain3.GeoLookupResult_ReverseLookUp : "",
                    HomeOwner = domain3 != null ? domain3.home_owner_status_data_append : "",
                    HomeMarket = domain3 != null ? domain3.home_market_value_data_append : "",

                    MissedCall_CallMetrics = domain4 != null ? domain4.MissedCall_CallMetrics : "",
                    CallActivities = domain4 != null ? domain4.CallActivities : "",
                    Channel_Attribution = domain4 != null ? domain4.Channel_Attribution : "",
                    Status_Attribution = domain4 != null ? domain4.Status_Attribution : "",
                    Rank_Attribution = domain4 != null ? domain4.Rank_Attribution : "",
                    Pid_Attribution = domain4 != null ? domain4.Pid_Attribution : "",
                    Bid_Attribution = domain4 != null ? domain4.Bid_Attribution : "",
                    DocumentTitle_FirstTouch = domain4 != null ? domain4.DocumentTitle_FirstTouch : "",
                    DocumentUrl_FirstTouch = domain4 != null ? domain4.DocumentUrl_FirstTouch : "",
                    DocumentPath_FirstTouch = domain4 != null ? domain4.DocumentPath_FirstTouch : "",
                    DocumentTimeStamp_FirstTouch = domain4 != null ? domain4.DocumentTimeStamp_FirstTouch : "",
                    DocumentTitle_LastTouch = domain4 != null ? domain4.DocumentTitle_LastTouch : "",
                    DocumentUrl_LastTouch = domain4 != null ? domain4.DocumentUrl_LastTouch : "",
                    DocumentPath_LastTouch = domain4 != null ? domain4.DocumentPath_LastTouch : "",
                    DocumentTimeStamp_LastTouch = domain4 != null ? domain4.DocumentTimeStamp_LastTouch : "",
                    IPAddress_VisitorData = domain4 != null ? domain4.IPAddress_VisitorData : "",
                    Device_VisitorData = domain4 != null ? domain4.Device_VisitorData : "",
                    Browser_VisitorData = domain4 != null ? domain4.Browser_VisitorData : "",
                    BrowserVersion_VisitorData = domain4 != null ? domain4.BrowserVersion_VisitorData : "",
                    Os_VisitorData = domain4 != null ? domain4.Os_VisitorData : "",
                    OsVersion_VisitorData = domain4 != null ? domain4.OsVersion_VisitorData : "",
                    SearchTerm_VisitorData = domain4 != null ? domain4.SearchTerm_VisitorData : "",
                    ActivityValue_VisitorData = domain4 != null ? domain4.ActivityValue_VisitorData : "",
                    ActivityTypeId_VisitorData = domain4 != null ? domain4.ActivityTypeId_VisitorData : "",
                    ActivityKeyword_VisitorData = domain4 != null ? domain4.ActivityKeyword_VisitorData : "",
                    ActivityTag_VisitorData = domain4 != null ? domain4.ActivityTag_VisitorData : "",
                    Campaign_VisitorData = domain4 != null ? domain4.Campaign_VisitorData : "",
                    Platform_VisitorData = domain4 != null ? domain4.Platform_VisitorData : "",
                    SourceGuard_VisitorData = domain4 != null ? domain4.SourceGuard_VisitorData : "",
                    VisitorLogUrl_VisitorData = domain4 != null ? domain4.VisitorLogUrl_VisitorData : "",
                    GoogleUaClientId_VisitorData = domain4 != null ? domain4.GoogleUaClientId_VisitorData : "",
                    GClid_VisitorData = domain4 != null ? domain4.GClid_VisitorData : "",

                    UtmSource_DefaultUtmParameters = domain5 != null ? domain5.UtmSource_DefaultUtmParameters : "",
                    UtmMedium_DefaultUtmParameters = domain5 != null ? domain5.UtmMedium_DefaultUtmParameters : "",
                    UtmCampaign_DefaultUtmParameters = domain5 != null ? domain5.UtmCampaign_DefaultUtmParameters : "",
                    UtmTerm_DefaultUtmParameters = domain5 != null ? domain5.UtmTerm_DefaultUtmParameters : "",
                    UtmContent_DefaultUtmParameters = domain5 != null ? domain5.UtmContent_DefaultUtmParameters : "",
                    VtKeyword_ValueTrackParameters = domain5 != null ? domain5.VtKeyword_ValueTrackParameters : "",
                    VtMatchType_ValueTrackParameters = domain5 != null ? domain5.VtMatchType_ValueTrackParameters : "",
                    VtNetwork_ValueTrackParameters = domain5 != null ? domain5.VtNetwork_ValueTrackParameters : "",
                    VtDevice_ValueTrackParameters = domain5 != null ? domain5.VtDevice_ValueTrackParameters : "",
                    VtDeviceModel_ValueTrackParameters = domain5 != null ? domain5.VtDeviceModel_ValueTrackParameters : "",
                    VtCreative_ValueTrackParameters = domain5 != null ? domain5.VtCreative_ValueTrackParameters : "",
                    VtPlacement_ValueTrackParameters = domain5 != null ? domain5.VtPlacement_ValueTrackParameters : "",
                    VtTarget_ValueTrackParameters = domain5 != null ? domain5.VtTarget_ValueTrackParameters : "",
                    VtParam1_ValueTrackParameters = domain5 != null ? domain5.VtParam1_ValueTrackParameters : "",
                    VtParam2_ValueTrackParameters = domain5 != null ? domain5.VtParam2_ValueTrackParameters : "",
                    VtRandom_ValueTrackParameters = domain5 != null ? domain5.VtRandom_ValueTrackParameters : "",
                    VtAceid_ValueTrackParameters = domain5 != null ? domain5.VtAceid_ValueTrackParameters : "",
                    VtAdposition_ValueTrackParameters = domain5 != null ? domain5.VtAdposition_ValueTrackParameters : "",
                    VtProductTargetId_ValueTrackParameters = domain5 != null ? domain5.VtProductTargetId_ValueTrackParameters : "",
                    VtAdType_ValueTrackParameters = domain5 != null ? domain5.VtAdType_ValueTrackParameters : "",
                    DomainSetName_SourceIqData = domain5 != null ? domain5.DomainSetName_SourceIqData : "",
                    DomainSetId_SourceIqData = domain5 != null ? domain5.DomainSetId_SourceIqData : "",
                    PoolName_SourceIqData = domain5 != null ? domain5.PoolName_SourceIqData : "",
                    LocationName_SourceIqData = domain5 != null ? domain5.LocationName_SourceIqData : "",
                    CustomValue_SourceIqData = domain5 != null ? domain5.CustomValue_SourceIqData : "",
                    CustomId_SourceIqData = domain5 != null ? domain5.CustomId_SourceIqData : "",
                    CampaignId_PaidSearch = domain5 != null ? domain5.CampaignId_PaidSearch : "",
                    Adgroup_PaidSearch = domain5 != null ? domain5.Adgroup_PaidSearch : "",
                    AdgroupId_PaidSearch = domain5 != null ? domain5.AdgroupId_PaidSearch : "",
                    Ads_PaidSearch = domain5 != null ? domain5.Ads_PaidSearch : "",
                    AdId_PaidSearch = domain5 != null ? domain5.AdId_PaidSearch : "",
                    Keywords_PaidSearch = domain5 != null ? domain5.Keywords_PaidSearch : "",
                    KeywordId_PaidSearch = domain5 != null ? domain5.KeywordId_PaidSearch : "",
                    ClickId_PaidSearch = domain5 != null ? domain5.ClickId_PaidSearch : "",
                    KeyWordMatchType_PaidSearch = domain5 != null ? domain5.KeyWordMatchType_PaidSearch : "",
                    CallInlyFlag_PaidSearch = domain5 != null ? domain5.CallInlyFlag_PaidSearch : "",
                    Type_PaidSearch = domain5 != null ? domain5.Type_PaidSearch : "",
                    CalledFranchiseeName = domain != null && domain.CalledFranchiseeId != null && domain.CalledFranchisee.Organization.Name != null ? domain.CalledFranchisee.Organization.Name : "",
                    CallRoute = domain2 != null ? domain2.CallRoute : "",
                    CallNote = note,
                    PreferredContactNumber = preferredContactNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    Company = company,
                    Office = office,
                    ZipCode = zipCode,
                    ResultingAction = resultingAction,
                    HouseNumber = houseNumber,
                    Street = street,
                    City = city,
                    State = state,
                    Country = country,
                    Email = email
                };
                return model;
            }
            catch(Exception ex)
            {
                return new CallDetailViewModel();
            }
        }

        #region Invoca

        public MarketingLeadCallDetailViewModel CreateModelForInvoca(InvocaCallDetails record, long callTypeId)
        {
            var dateTimeString = DateTime.Now;
            try
            {
                var dateTimeSplitted = record.start_time_xml.Split('+');
                var dateTimeSpl = dateTimeSplitted[0].Split('T');
                var dateString = dateTimeSpl[0];
                var timeString = dateTimeSpl[1];
                dateTimeString = DateTime.Parse(dateString + " " + timeString);
                //var dateAdded2= dateAddedDate.ToString("yyyy-MM-dd HH: mm:ss");
            }
            catch (Exception e1)
            {
                dateTimeString = Convert.ToDateTime(record.start_time_xml);
            }
            var model = new MarketingLeadCallDetailViewModel
            {
                SessionId = record.SessionId,
                CallDuration = Convert.ToInt32(record.duration/60),
                CallerId = record.calling_phone_number.Replace(@"-", ""),
                CallTypeId = callTypeId,
                //ClickDescription = record.click_description,
                //DateAdded = _clock.ToUtc(dateAdded),
                DialedNumber = record.call_source_description.Replace(@"-", ""),
                PhoneLabel = record.promo_line_description,
                TransferToNumber = record.destination_phone_number.Replace(@"-", ""),
                TransferType = record.transfer_from_type,
                DateAdded = (dateTimeString),
            };
            return model;
        }

        public MarketingLeadCallDetailV2 CreateModelForInvocaAPI(InvocaCallDetails record, string sid, string callRoute)
        {
            var dateTime = Convert.ToDateTime(record.start_time_local);
            var domain = new MarketingLeadCallDetailV2
            {
                CallDuration = Convert.ToInt32(record.duration/60).ToString(),
                CallRoute = callRoute,
                CallRouteQualified = record.affiliate_call_volume_ranking,
                CallStatus = GetStatusInvoca(record.hangup_cause),
                Destination = record.advertiser_name,
                EnteredZipCode = record.caller_zip,
                PhoneLabel = record.promo_line_description,
                CallDate = dateTime,
                Reroute = record.repeat_calling_phone_number,
                SetName = record.advertiser_campaign_name,
                Sid = sid,
                Source = record.affiliate_name,
                //SourceNumber = record.advanced_routing.callflow.callflow_call_route_qualified,
                //SourceQualified = record.advanced_routing.callflow.callflow_call_route_qualified,
                TransferNumber = record.destination_phone_number,
                CallerId = record.calling_phone_number,
                FindMeList = record.find_me_list
            };
            return domain;
        }

        public MarketingLeadCallDetailV3 CreateModelForInvocaAPI3(InvocaCallDetails record, string sid)
        {
            var domain = new MarketingLeadCallDetailV3
            {
                CallflowDestination = (record.advertiser_name != null) ? record.advertiser_name : "",
                CallflowDestinationId = (record.advertiser_id_from_network != null) ? record.advertiser_id_from_network : "",
                CallflowEnteredZip = (record.caller_zip != null) ? record.caller_zip
                : "",
                CallflowRepeatSourceCaller = (record.repeat_calling_phone_number != null) ? record.repeat_calling_phone_number : "",
                CallflowReroute = (record.repeat_calling_phone_number != null) ? record.repeat_calling_phone_number : "",
                CallflowSetId = (record.advertiser_campaign_id_from_network != null) ? record.advertiser_campaign_id_from_network: "",
                CallflowSetName = (record.advertiser_campaign_name != null) ? record.advertiser_campaign_name : "",
                CallflowSource = (record.affiliate_name != null) ? record.affiliate_name : "",
                //CallflowSourceCap = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_source_cap : "",
                CallflowSourceId = (record.affiliate_id_from_network != null) ? record.affiliate_id_from_network : "",
                CallflowSourceQualified = (record.affiliate_payout_localized != null) ? record.affiliate_payout_localized : "",
                //CallflowSourceRoute = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_call_route : "",
                //CallflowSourceRouteId = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_call_route_id : "",
                //CallflowSourceRouteQualified = (record.advanced_routing != null && record.advanced_routing.callflow != null) ? record.advanced_routing.callflow.callflow_call_route_qualified : "",
                CallflowState = (record.address_state_data_append != null) ? record.address_state_data_append : "",
                CallNote_CallAnalytics = (record.notes != null) ? record.notes : "",
                CallTransferStatus_CallFlow = (record.call_result_description_detail != null) ? record.call_result_description_detail : "",
                City_ReverseLookUp = (record.address_city_data_append != null) ? record.address_city_data_append : "",
                StateCode_ReverseLookUp = (record.address_state_data_append != null) ? record.address_state_data_append : "",
                FirstName_ReverseLookUp = (record.first_name_data_append != null) ? record.first_name_data_append : "",
                LastName_ReverseLookUp = (record.last_name_data_append != null) ? record.last_name_data_append : "",
               // GeoLookupAttempt_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].geo_lookup_attempt : "",
                //GeoLookupResult_ReverseLookUp = (record.reverse_lookup.Count > 0) ? record.reverse_lookup[0].geo_lookup_result : "",
                //KeywordGroups_CallAnalytics = record.call_analytics != null ? string.Join(",", record.call_analytics.keyword_groups) : "",
                //KeywordSpottingComplete_CallAnalytics = record.call_analytics != null ? record.call_analytics.keyword_spotting_complete : "",
                PostalArea_ReverseLookUp = (record.address_zip_data_append != null) ? record.address_zip_data_append : "",
                StreetLine1_ReverseLookUp = (record.address_full_street_data_append != null) ? record.address_full_street_data_append : "",
                //RingCount_CallFlow = (record.call_transfer != null) ? record.call_transfer.ring_count : default(decimal),
                RingSeconds_CallFlow = (record.connect_duration != default(decimal)) ? record.connect_duration : default(decimal),
                Sid = sid,
                RecordingUrl_Recording = record.recording != null ? record.recording : "",
                home_owner_status_data_append = record.home_owner_status_data_append != null ? record.home_owner_status_data_append : "",
                home_market_value_data_append = record.home_market_value_data_append != null ? record.home_market_value_data_append : ""

            };
            return domain;
        }


        public MarketingLeadCallDetailV4 CreateModelForInvocaAPI4(InvocaCallDetails record, string sid)
        {
            var domain = new MarketingLeadCallDetailV4
            {
                Sid = sid,
                //ActivityKeyword_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.activity_keyword : "",
                //ActivityTag_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.activity_tag : "",
                //ActivityTypeId_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.activity_type_id : "",
                //ActivityValue_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.activity_value : "",
                Bid_Attribution = (record.invoca_id != null) ? record.invoca_id : "",
                //BrowserVersion_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.browser_version : "",
                //Browser_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.Browser : "",
                //CallActivities = (record.Attribution.visitor_data != null) ? string.Join(",", record.call_details.call_activities) : "",
                Campaign_VisitorData = (record.dynamic_number_pool_referrer_referrer_campaign != null) ? record.dynamic_number_pool_referrer_referrer_campaign : "",
                Channel_Attribution = (record.promo_line_description != null) ? record.promo_line_description : "",
                //Device_VisitorData = (record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.Browser : "",
                DocumentPath_FirstTouch = (record.calling_path != null) ? record.calling_path : "",
                //DocumentTimeStamp_FirstTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.first_touch.document_timestamp : "",
                DocumentTitle_FirstTouch = (record.landing_title != null) ? record.landing_title : "",
                DocumentUrl_FirstTouch = (record.landing_page != null) ? record.landing_page : "",
                DocumentPath_LastTouch = (record.calling_path != null) ? record.calling_path : "",
                DocumentTitle_LastTouch = (record.landing_title != null) ? record.landing_title : "",
                //DocumentTimeStamp_LastTouch = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.last_touch.document_timestamp : "",
                DocumentUrl_LastTouch = (record.calling_page != null) ? record.calling_page : "",
                GClid_VisitorData = (record.gclid != null) ? record.gclid : "",
                GoogleUaClientId_VisitorData = (record.g_cid != null) ? record.g_cid : "",
                //IPAddress_VisitorData = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.visitor_data.ip_address : "",
                MissedCall_CallMetrics = (record.signal_name != null) ? record.signal_name : "",
                //OsVersion_VisitorData = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.visitor_data.os_version : "",
                //Os_VisitorData = (record.Attribution.visitor_data != null && record.Attribution.first_touch != null) ? record.Attribution.visitor_data.Os : "",
                //Pid_Attribution = (record.Attribution != null) ? record.Attribution.Pid : "",
                Platform_VisitorData = (record.utm_source != null) ? record.utm_source : "",
                //Rank_Attribution = (record.Attribution.visitor_data != null) ? record.Attribution.Rank : "",
                //SearchTerm_VisitorData = (record.Attribution != null && record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.search_term : "",
                //Status_Attribution = record.Attribution != null ? record.Attribution.Status : "",
                SourceGuard_VisitorData = (record.media_type != null) ? record.media_type : "",
                //VisitorLogUrl_VisitorData = (record.Attribution != null && record.Attribution.visitor_data != null) ? record.Attribution.visitor_data.visitor_log_url : "",
            };
            return domain;
        }

        public MarketingLeadCallDetailV5 CreateModelForInvocaAPI5(InvocaCallDetails record, string sid)
        {

            var domain = new MarketingLeadCallDetailV5
            {
                Sid = sid,
                AdgroupId_PaidSearch = (record.dynamic_number_pool_referrer_ad_group != null) ? record.dynamic_number_pool_referrer_ad_group : "",
                Adgroup_PaidSearch = (record.dynamic_number_pool_referrer_ad_group != null) ? record.dynamic_number_pool_referrer_ad_group : "",
                AdId_PaidSearch = (record.dynamic_number_pool_referrer_ad_id != null) ? record.dynamic_number_pool_referrer_ad_id : "",
                //Ads_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? "" : "",
                //CallInlyFlag_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.call_only_flag : "",
                CampaignId_PaidSearch = (record.dynamic_number_pool_referrer_referrer_campaign_id != null) ? record.dynamic_number_pool_referrer_referrer_campaign_id : "",
                Campaign_PaidSearch = (record.dynamic_number_pool_referrer_referrer_campaign != null) ? record.dynamic_number_pool_referrer_referrer_campaign : "",
                ClickId_PaidSearch = (record.gclid != null) ? record.gclid : "",
                //CustomId_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.custom_id : "",
                //CustomValue_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.custom_value : "",
                //DomainSetId_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.domain_set_id : "",
                //DomainSetName_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.domain_set_name : "",
                KeywordId_PaidSearch = (record.dynamic_number_pool_referrer_search_keywords_id != null) ? record.dynamic_number_pool_referrer_search_keywords_id : "",
                KeyWordMatchType_PaidSearch = (record.dynamic_number_pool_referrer_keyword_match_type != null) ? record.dynamic_number_pool_referrer_keyword_match_type : "",
                //Keywords_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? "" : "",
                //LocationName_SourceIqData = (record.Attribution != null && record.Attribution.source_iq_data != null) ? record.Attribution.source_iq_data.location_name : "",
                PoolName_SourceIqData = (record.promo_line_description != null) ? record.promo_line_description : "",
                //Type_PaidSearch = (record.Attribution != null && record.Attribution.paid_search != null) ? record.Attribution.paid_search.Type : "",
                UtmCampaign_DefaultUtmParameters = (record.utm_campaign != null) ? record.utm_campaign : "",
                UtmContent_DefaultUtmParameters = (record.utm_content != null) ? record.utm_content : "",
                UtmMedium_DefaultUtmParameters = (record.utm_medium != null) ? record.utm_medium : "",
                UtmSource_DefaultUtmParameters = (record.utm_source != null) ? record.utm_source : "",
                UtmTerm_DefaultUtmParameters = (record.utm_term != null) ? record.utm_term : "",
                //VtAceid_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_aceid : "",
                //VtAdposition_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_adposition : "",
                //VtAdType_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_adtype : "",
                //VtCreative_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_creative : "",
                //VtDeviceModel_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_devicemodel : "",
                //VtDevice_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_device : "",
                //VtKeyword_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_keyword : "",
                //VtMatchType_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_matchtype : "",
                //VtNetwork_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_network : "",
                //VtParam1_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_param1 : "",
                //VtParam2_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_param2 : "",
                //VtPlacement_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_placement : "",
                //VtProductTargetId_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_producttargetid : "",
                //VtRandom_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_random : "",
                //VtTarget_ValueTrackParameters = (record.Attribution != null && record.Attribution.valuetrack_parameters != null) ? record.Attribution.valuetrack_parameters.vt_target : "",
            };
            return domain;
        }

        private string GetStatusInvoca(string hangupcause)
        {
            if (hangupcause == null)
            {
                return "Not Connected";
            }
            else if (hangupcause == "Destination: No Answer")
            {
                return "Not Connected";
            }
            else
            {
                return "Connected";
            }
        }


        #endregion
    }
}
