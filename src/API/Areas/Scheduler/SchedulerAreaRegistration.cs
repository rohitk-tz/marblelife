using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace API.Areas.Scheduler
{
    public class SchedulerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Scheduler";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.Routes.MapHttpRoute(
               "Estimate_Post",
               "Scheduler/estimate",
               new { Controller = "Estimate", Action = "SaveEstimate" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
               "scheduler_Post",
               "Scheduler/scheduler",
               new { Controller = "Scheduler", Action = "SaveJob" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
                "Schedulers_status",
                "Scheduler/scheduler/{jobId}/change/status",
                new { Controller = "Scheduler", Action = "ChangeStatus" }
            );

            context.Routes.MapHttpRoute("Schedulers_QbNumber", "Scheduler/scheduler/{qbInvoice}/verify",
              new { Controller = "Scheduler", Action = "IsValidQbNumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute(
              "Schedulers_update",
              "Scheduler/scheduler/update/qb/{jobId}/job",
              new { Controller = "Scheduler", Action = "UpdateJobInfo" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute("job_delete", "Scheduler/Scheduler/job/delete/{id}",
               new { Controller = "Scheduler", Action = "Delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) }
             );

            context.Routes.MapHttpRoute(
              "Schedulers_upload",
              "Scheduler/upload/media/{jobId}/job",
              new { Controller = "Scheduler", Action = "SaveMediaFiles" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute("Tech_verify", "Scheduler/Scheduler/job/verify",
               new { Controller = "Scheduler", Action = "CheckAvailability" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Media_upload", "Scheduler/Scheduler/media/upload",
               new { Controller = "Scheduler", Action = "UploadMedia" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Media_upload_User", "Scheduler/Scheduler/media/uploadForUser",
               new { Controller = "Scheduler", Action = "UploadMediaForUser" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Media_get", "Scheduler/Scheduler/media/get",
               new { Controller = "Scheduler", Action = "GetMediaList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("scheduler_note", "Scheduler/Scheduler/note/save",
              new { Controller = "Scheduler", Action = "SaveNotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("scheduler_List", "Scheduler/Scheduler/list",
              new { Controller = "Scheduler", Action = "GetJobList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("holiday_list", "Scheduler/Scheduler/{franchiseeId}/list/holiday",
              new { Controller = "Scheduler", Action = "GetHolidayList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute("Vacation_get", "Scheduler/estimate/get/{id}/vacation",
               new { Controller = "Estimate", Action = "GetVacationInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
             );
            context.Routes.MapHttpRoute("Meeting_get", "Scheduler/estimate/get/{id}/meeting",
               new { Controller = "Estimate", Action = "GetMeetingInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
             );
            context.Routes.MapHttpRoute("Estimate_get", "Scheduler/estimate/get/{id}/estimate",
              new { Controller = "Estimate", Action = "GetEstimate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute(
              "Vacation_Post",
              "Scheduler/estimate",
              new { Controller = "Estimate", Action = "SaveVacation" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute(
              "Vacation_repeat",
              "Scheduler/estimate/repeat/vacation",
              new { Controller = "Estimate", Action = "RepeatVacation" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute(
              "Meeting_repeat",
              "Scheduler/estimate/repeat/meeting",
              new { Controller = "Estimate", Action = "RepeatMeeting" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute(
             "Vacation_Delete",
             "Scheduler/estimate/delete/{id}/Vacation",
             new { Controller = "Estimate", Action = "DeleteVacation" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) }
         );

            context.Routes.MapHttpRoute(
            "estimate_Delete",
            "Scheduler/estimate/delete/{id}/estimate",
            new { Controller = "Estimate", Action = "Delete" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) }
        );

            context.Routes.MapHttpRoute("occurence_get", "Scheduler/estimate/get/{id}/occurence",
              new { Controller = "Estimate", Action = "GetOccurenceInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            context.Routes.MapHttpRoute("occurence_estimate_get", "Scheduler/estimate/get/{id}/Estimateoccurence",
             new { Controller = "Estimate", Action = "GetEstimateOccurenceInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute(
             "Meeting_Delete",
             "Scheduler/estimate/delete/Meeting",
             new { Controller = "Estimate", Action = "DeleteMeeting" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute(
             "scheduler_save",
             "Scheduler/estimate",
             new { Controller = "Estimate", Action = "SaveSchedule" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute(
             "estimate_repeat",
             "Scheduler/estimate/repeat",
             new { Controller = "Estimate", Action = "RepeatEstimate" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute("Customer_Address",
                "Scheduler/Scheduler/map/{jobId}",
                new { Controller = "Scheduler", Action = "GetCustomerAddress" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );

            context.Routes.MapHttpRoute(
              "upload_geoCode",
              "Scheduler/GeoCode/file/upload",
              new { Controller = "GeoCode", Action = "Upload" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute(
             "Get_GeoCode_list",
             "Scheduler/GeoCode/list",
             new { Controller = "GeoCode", Action = "GetList" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
         );
            context.Routes.MapHttpRoute(
             "GeoCode_Info",
             "Scheduler/GeoCode/info",
             new { Controller = "GeoCode", Action = "GetGeoCodeInfo" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute(
             "Get_GeoCode",
             "Scheduler/GeoCode/zip/info/{franchiseeId}",
             new { Controller = "GeoCode", Action = "GetGeoInfo" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
         );
            context.Routes.MapHttpRoute(
            "Get_ZipCode",
            "Scheduler/GeoCode/get/{zipCode}/geoCode/{state}/franchiseeId/{franchiseeId}/countryId/{countryId}",
            new { Controller = "GeoCode", Action = "GetGeoInfoByZipCode" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
        );
            context.Routes.MapHttpRoute(
            "Get_MailList",
            "Scheduler/Scheduler/mail",
            new { Controller = "Scheduler", Action = "GetMailList" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute(
            "Get_MailTemplate",
            "Scheduler/Scheduler/mail/{id}/template",
            new { Controller = "Scheduler", Action = "GetMailTemplate" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute(
            "edit_Template",
            "Scheduler/Scheduler/mail/edit",
            new { Controller = "Scheduler", Action = "EditMailTemplate" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute(
            "job_Estimate_Images",
            "Scheduler/Scheduler/job/estimate/images",
            new { Controller = "Scheduler", Action = "SaveJobEstimateImages" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute(
           "save_Rotation",
           "Scheduler/Scheduler/job/estimate/images",
           new { Controller = "Scheduler", Action = "SaveRotation" },
           new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
       );
            context.Routes.MapHttpRoute(
           "save_CroppedImage",
           "Scheduler/Scheduler/job/estimate/images",
           new { Controller = "Scheduler", Action = "SaveCroppedImage" },
           new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
       );
            context.Routes.MapHttpRoute("Media_upload_User_Before_After", "Scheduler/Scheduler/job/estimate/before/after/upload",
               new { Controller = "Scheduler", Action = "UploadMediaBeforeAfter" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Media_upload_User_Before_After_Mail", "Scheduler/Scheduler/job/estimate/before/after/send/mail/{id}",
              new { Controller = "Scheduler", Action = "BeforeAfterImageSendMail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Media_upload_User_Invoice_Mail", "Scheduler/Scheduler/job/estimate/invoice/send/mail/{id}",
              new { Controller = "Scheduler", Action = "InvoiceSendMail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Media_Deletion_Eligible", "Scheduler/Scheduler/job/estimate/isEligible",
              new { Controller = "Scheduler", Action = "IsEligibleForDeletion" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Geo_Code_Notes", "Scheduler/GeoCode/zip/code/notes",
             new { Controller = "GeoCode", Action = "SaveGeoCodeNotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("Geo_Code_Delete", "Scheduler/GeoCode/delete/{batchId}",
             new { Controller = "GeoCode", Action = "DeleteRecord" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );


            context.Routes.MapHttpRoute("holiday_listHoliday", "Scheduler/Scheduler/list/holidayMonthWise",
             new { Controller = "Scheduler", Action = "GetHolidayListMonthWise" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("franchisee_Info", "Scheduler/Scheduler/franchisee/info/{franchiseeId}",
            new { Controller = "Scheduler", Action = "GetFranchiseeInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
          );

            context.Routes.MapHttpRoute("save_dragdrop", "Scheduler/Scheduler/save/SaveDragDropEvent",
           new { Controller = "Scheduler", Action = "SaveDragDropEvent" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );

            context.Routes.MapHttpRoute("confirmation_scheduler", "Scheduler/Scheduler/confirmation",
            new { Controller = "MailConfirmation", Action = "ConfirmSchedule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute("confirmation_scheduler_from_UI", "Scheduler/Scheduler/confirmationFromUI",
           new { Controller = "Scheduler", Action = "ConfirmScheduleFromUI" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute("job_Note_Save", "Scheduler/Scheduler/saveNotes",
          new { Controller = "Scheduler", Action = "EditJobNotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute("job_Note_Delete", "Scheduler/Scheduler/delete/Notes/{id}",
         new { Controller = "Scheduler", Action = "DeleteJobNotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
       );
            context.Routes.MapHttpRoute("customer_Info", "Scheduler/Scheduler/customer/info/{customerName}",
            new { Controller = "Scheduler", Action = "GetCustomerInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
          );
            context.Routes.MapHttpRoute("Geo_Code_Reparse", "Scheduler/GeoCode/reparse/geofile/{batchId}",
            new { Controller = "GeoCode", Action = "ReparseFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute("Before_After_Images_List", "Scheduler/Job/getReviewImages",
           new { Controller = "Scheduler", Action = "GetBeforeAfterImages" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute("Best_Pair_Save", "Scheduler/Job/saveImagesBestPair",
          new { Controller = "Scheduler", Action = "SaveImagesBestPair" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute("Review_Mark_Image", "Scheduler/Job/SaveReviewMarkImage",
        new { Controller = "Scheduler", Action = "SaveReviewMarkImage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
      );

            context.Routes.MapHttpRoute("Before_After_Images_List_FA", "Scheduler/Job/getReviewImagesForFA",
                     new { Controller = "Scheduler", Action = "GetBeforeAfterImagesForFranchiseeAdmin" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("Save_After_Images_List_FA", "Scheduler/Job/saveReviewImagesForFA",
                     new { Controller = "Scheduler", Action = "SaveBeforeAfterImagesForFranchiseeAdmin" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("Save_To_Do_List", "Scheduler/ToDo/SaveToDo",
                     new { Controller = "ToDo", Action = "SaveToDoFollowUp" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("Get_To_Do_List", "Scheduler/ToDo/GetToDoList",
                     new { Controller = "ToDo", Action = "GetToDoList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("Get_To_Do_List_By_Id", "Scheduler/ToDo/GetToDoById/{id}",
                     new { Controller = "ToDo", Action = "GetToDoById" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                   );
            context.Routes.MapHttpRoute("Get_Customer_List", "Scheduler/ToDo/GetCustomerList/{text}",
                   new { Controller = "ToDo", Action = "GetCustomerList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                 );
            context.Routes.MapHttpRoute("Get_Customer_Info", "Scheduler/ToDo/GetCustomerInfo/{text}",
                   new { Controller = "ToDo", Action = "GetCustomerInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                 );
            context.Routes.MapHttpRoute("Get_Customer_Comment_Info", "Scheduler/ToDo/GetCommentToDo/{id}",
                   new { Controller = "ToDo", Action = "GetCommentToDo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                 );
            context.Routes.MapHttpRoute("Save_Customer_Comment_Info", "Scheduler/ToDo/SaveCommentToDo",
                  new { Controller = "ToDo", Action = "SaveCommentInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                );
            context.Routes.MapHttpRoute("Get_FollowUp_Info_Scheduler", "Scheduler/ToDo/GetCommentToDoForScheduler/{id}",
                   new { Controller = "ToDo", Action = "GetCommentToDoForScheduler" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                 );
            context.Routes.MapHttpRoute("Get_ToDo_List_Scheduler", "Scheduler/ToDo/GetToDoListForScheduler",
                  new { Controller = "ToDo", Action = "GetToDoListForScheduler" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                );
            context.Routes.MapHttpRoute("Save_Customer_Comment_Info_By_Status", "Scheduler/ToDo/SaveToDoByStatus",
                  new { Controller = "ToDo", Action = "SaveToDoByStatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                );
            context.Routes.MapHttpRoute("Delete_To_Do", "Scheduler/ToDo/DeleteToDo/{id}",
                  new { Controller = "ToDo", Action = "DeleteToDo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                );
            context.Routes.MapHttpRoute("Get_ToDo_Info", "Scheduler/ToDo/GetToDoInfo/{text}",
                  new { Controller = "ToDo", Action = "GetToDoInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                );
            context.Routes.MapHttpRoute("Get_ToDo_List", "Scheduler/ToDo/ToDoList/{text}",
                 new { Controller = "ToDo", Action = "GetToDoList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
               );
            context.Routes.MapHttpRoute("Estimate_Worth", "Scheduler/Scheduler/saveEstimateWorth",
                new { Controller = "Scheduler", Action = "SaveEstimateWorth" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("Save_Invoice_Info", "Scheduler/Scheduler/saveInvoiceInfo",
                new { Controller = "EstimateInvoice", Action = "SaveEstimateInvoice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("Get_Invoice_Info", "Scheduler/Scheduler/getInvoiceInfo",
               new { Controller = "EstimateInvoice", Action = "GetEstimateInvoice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Send_Invoice_Mail", "Scheduler/Scheduler/sendInvoiceToCustomer",
               new { Controller = "EstimateInvoice", Action = "MailToCustomer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Download_Invoices", "Scheduler/Scheduler/uploadInvoicesZipFile",
             new { Controller = "EstimateInvoice", Action = "UploadInvoicesZipFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("Save_SaveCustomerSignature", "Scheduler/Scheduler/SaveCustomerSignature",
             new { Controller = "EstimateInvoice", Action = "SaveCustomerSignature" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("Download_Signed_Invoices", "Scheduler/Scheduler/uploadSignedInvoicesZipFile",
            new { Controller = "EstimateInvoice", Action = "UploadSignedInvoicesZipFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute("Shift_Image_Invoice", "Scheduler/Scheduler/ShiftImagesToInvoiceBuildMaterial",
           new { Controller = "Scheduler", Action = "ShiftImagesToInvoiceBuildMaterial" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute("edit_Email_Template", "Scheduler/Scheduler/editEmailTemplate",
          new { Controller = "Scheduler", Action = "EditEmailTemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute("Download_Invoices_Customer", "Scheduler/Scheduler/UploadInvoicesCustomerZipFile",
             new { Controller = "EstimateInvoice", Action = "UploadInvoicesCustomerZipFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("Save_SaveInvoiceRequired", "Scheduler/Scheduler/SaveInvoiceRequired",
             new { Controller = "Scheduler", Action = "SaveInvoiceRequired" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("Attach_Invoices_Customer", "Scheduler/Scheduler/AddInvoiceToEstimate/{schedulerId}",
             new { Controller = "EstimateInvoice", Action = "AddInvoiceToEstimate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("InvoiceMedia_get", "Scheduler/Scheduler/invoicemedia/get",
               new { Controller = "Scheduler", Action = "GetInvoiceMediaList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Save_CustomerIsAvailable", "Scheduler/Scheduler/customerIsAvailableOrNot",
               new { Controller = "EstimateInvoice", Action = "CustomerIsAvailableOrNot" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Tech_verifyList", "Scheduler/Scheduler/job/verifyList",
               new { Controller = "Scheduler", Action = "CheckAvailabilityList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Get_ServiceType_Id", "Scheduler/Scheduler/getServiceTypeId",
               new { Controller = "EstimateInvoice", Action = "GetServiceTypeId" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Media_upload_InvoieLine", "Scheduler/Scheduler/job/estimate/invoiceLine/upload",
               new { Controller = "Scheduler", Action = "InvoiceLineUpload" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );

            context.Routes.MapHttpRoute("getLocalMarketingReview", "Scheduler/Job/getLocalMarketingReview",
                     new { Controller = "Scheduler", Action = "GetLocalMarketingReview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("getSalesRepTechnicianList", "Scheduler/Job/getSalesRepTechnicianList/{franchiseeId}",
                     new { Controller = "Scheduler", Action = "GetSalesRepTechnicianList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                   );
            context.Routes.MapHttpRoute("markImageAsReviwed", "Scheduler/Job/markImageAsReviwed",
                     new { Controller = "Scheduler", Action = "MarkImageAsReviwed" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("markImageAsBestPair", "Scheduler/Job/markImageAsBestPair",
                     new { Controller = "Scheduler", Action = "MarkImageAsBestPair" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("markImageAsAddToLocalGallery", "Scheduler/Job/markImageAsAddToLocalGallery",
                     new { Controller = "Scheduler", Action = "MarkImageAsAddToLocalGallery" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("getSchedulerListForToDo", "Scheduler/Job/getSchedulerListForToDo",
                     new { Controller = "Scheduler", Action = "GetSchedulerListForToDo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute("bestPairMarkedJobEstimateImagePair", "Scheduler/Job/bestPairMarkedJobEstimateImagePair",
                     new { Controller = "Scheduler", Action = "BestPairMarkedForJobEstimateImagePair" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
        }

    }
}