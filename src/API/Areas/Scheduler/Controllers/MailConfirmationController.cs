using Api.Areas.Application.Controller;
using Api.Impl;
using Core.Application;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Notification.Domain;
using Core.Notification.Impl;
using Core.Organizations;
using Core.Scheduler;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace API.Areas.Scheduler.Controllers
{
    [AllowAnonymous]
    public class MailConfirmationController : BaseController
    {
        private readonly IJobService _jobService;

        public MailConfirmationController(IJobService jobService)
        {
            _jobService = jobService;
        }
        
        [HttpPost]
        public ConfirmationResponseModel ConfirmSchedule([FromBody] ConfirmationModel model)
        {
            return _jobService.ConfirmSchedule(model);
        }
    }
}