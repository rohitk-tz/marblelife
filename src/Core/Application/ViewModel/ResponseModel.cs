using Core.Application.ViewModel;
using System.Xml.Serialization;

namespace Core.Application.ViewModel
{
    public class ResponseModel
    {
        public FeedbackMessageModel Message { get; set; }

        public int ErrorCode { get; set; }

        public object Data { get; set; }

        [XmlIgnore]
        public bool IsFeedbackSet
        {
            get
            {
                return Message != null;
            }
        }

        public void SetErrorMessage(string message, ResponseErrorCodeType errorCode = ResponseErrorCodeType.RandomException)
        {
            Message = FeedbackMessageModel.CreateErrorMessage(message);
            ErrorCode = (int)errorCode;
        }

        public void SetSuccessMessage(string message)
        {
            Message = FeedbackMessageModel.CreateSuccessMessage(message);
        }

        public ResponseModel()
        {
            ErrorCode = 0;
        }

    }

    public enum ResponseErrorCodeType
    {
        UserNotAuthenticated =1,
        UserBlocked = 2,
        ValidationFailure = 3,
        RandomException = 4,
        TrailExpired = 5,
        InvalidData = 6
    }
}