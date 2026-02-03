using Api.Areas.Application.Controller;
using Core.Application.ViewModel;
using Core.Scheduler;
using Core.Scheduler.ViewModel;
using System.Web.Http;

namespace API.Areas.Scheduler.Controllers
{
    public class CalendarController : BaseController
    {
        private readonly ICalendarImportService _calendarImportService;
        public CalendarController(ICalendarImportService calendarImportService)
        {
            _calendarImportService = calendarImportService;
        }

        [HttpPost]
        public bool Upload([FromBody]CalendarImportModel model)
        {
            if (model.FranchiseeId <= 0 || (model.TechId == null && model.SalesRepId == null))
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Invalid data!");
                return false;
            }
            var result = _calendarImportService.Save(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("File saved successfully! the records will appear shortly.");
                return true;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Error in saving file.");
                return false;
            }
        }
    }
}