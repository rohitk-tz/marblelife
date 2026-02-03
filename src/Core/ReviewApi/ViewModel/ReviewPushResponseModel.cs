using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
    [NoValidatorRequired]
    public class ReviewPushResponseModel
    {
        public long? ReviewId { get; set; }
        public string FeedBackResponse { get; set; }
        public bool IsSendSuccess { get; set; }
    }
}
