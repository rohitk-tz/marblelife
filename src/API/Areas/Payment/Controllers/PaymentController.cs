using Api.Areas.Application.Controller;
using Core.Application;
using Core.Billing;
using Core.Billing.ViewModel;
using Core.Users.Enum;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Payment.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IChargeCardProfileService _chargeCardProfileService;
        private readonly IECheckProfileService _eCheckProfileService;
        private readonly IPaymentService _paymentService;
        private readonly ICheckService _checkService;
        private ISessionContext _sessionContext;
        private readonly IInvoiceService _invoiceService;

        public PaymentController(IChargeCardProfileService chargeCardProfileService, IPaymentService paymentService, ICheckService checkService,
            IECheckProfileService eCheckProfileService, ISessionContext sessionContext, IInvoiceService invoiceService)
        {
            _chargeCardProfileService = chargeCardProfileService;
            _paymentService = paymentService;
            _checkService = checkService;
            _eCheckProfileService = eCheckProfileService;
            _sessionContext = sessionContext;
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public List<PaymentInstrumentViewModel> GetInstrumentList([FromUri]long franchiseeId, long paymentTypeId = 0)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            var cardDetails = _chargeCardProfileService.GetInstrumentList(franchiseeId, paymentTypeId);
            return cardDetails;
        }

        [HttpPost]
        public ProcessorResponse CreateChargeCardProfile(ChargeCardEditModel chargeCard, long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _chargeCardProfileService.CreateProfile(chargeCard, franchiseeId);
        }

        [HttpPost]
        public bool ManageInstrument(string instrumentIds, [FromBody]bool isActive)
        {
            var result = _chargeCardProfileService.ManageInstrument(instrumentIds, isActive);
            if (result)
            {
                return true;
            }
            else
            {
                ResponseModel.SetErrorMessage("Can't suspend a Primary Payment Instrument.");
                return false;
            }
        }

        [HttpGet]
        public bool DeleteInstrument(string instrumentIds)
        {
            var result = _chargeCardProfileService.DeleteInstrument(instrumentIds);
            if (result)
            {
                ResponseModel.SetSuccessMessage("Payment Instrument has been deleted successfully.");
                return true;
            }
            else
            {
                ResponseModel.SetErrorMessage("Can't delete Payment Instrument.");
                return false;
            }
        }

        [HttpGet]
        public ProcessorResponse SetPrimary(long franchiseeId, string instrumentIds)
        {
            return _chargeCardProfileService.SetPrimary(franchiseeId, instrumentIds);
        }

        [HttpPost]
        public ProcessorResponse MakePaymentByNewChargeCard([FromBody]ChargeCardPaymentEditModel model, long franchiseeId, long invoiceId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _paymentService.MakePaymentByNewChargeCard(model, franchiseeId, invoiceId);
        }

        [HttpPost]
        public ProcessorResponse MakePaymentByECheck([FromBody]ECheckEditModel model, long franchiseeId, long invoiceId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _paymentService.MakePaymentByECheck(model, franchiseeId, invoiceId);
        }

        [HttpPost]
        public ProcessorResponse MakePaymentOnFileChargeCard([FromBody]InstrumentOnFileEditModel model, long franchiseeId, long invoiceId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _paymentService.MakePaymentOnFileChargeCard(model, franchiseeId, invoiceId);
        }

        [HttpPost]
        public ProcessorResponse SaveCheck(CheckPaymentEditModel check, long franchiseeId, long invoiceId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _checkService.SaveCheck(check, franchiseeId, invoiceId);
        }

        [HttpPost]
        public ProcessorResponse CreateECheckProfile(ECheckEditModel model, long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _eCheckProfileService.CreateProfile(model, franchiseeId);
        }

        [HttpGet]
        public bool CheckExpiry(int month, int year)
        {
            var result = _chargeCardProfileService.CheckExpiryDate(month, year);
            if (result)
            {
                return true;
            }
            else
            {
                ResponseModel.SetErrorMessage("Please Enter a Valid Expiry date.");
                return false;
            }
        }

        [HttpGet]
        public List<FranchiseePaymentInstrumentViewModel> GetFranchiseeInstrumentList([FromUri]long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            var cardDetails = _chargeCardProfileService.GetFranchiseeInstrumentList(franchiseeId);
            return cardDetails;
        }

        [HttpGet]
        public ProcessorResponse AdjustAccountCredit([FromUri]long franchiseeId, [FromUri]long invoiceId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _paymentService.AdjustAccountCredit(franchiseeId, invoiceId);
        }

        [HttpGet]
        public InvoiceDetailsViewModel Get([FromUri]long invoiceId)  
        {
           return _invoiceService.InvoicePaymentDetails(invoiceId); 
        }
    }
}