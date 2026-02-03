using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IECheckProfileService
    {
        ProcessorResponse CreateProfile(ECheckEditModel model, long franchiseeId);
        ProcessorResponse CreateAuthorizeNetProfileForEcheck(long accountTypeId, ECheckEditModel model, ProcessorResponse processorResponse, long franchiseeId);
    }
}
