using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class MinRoyaltyFeeSlabsEditModel
    {
        public long Id { get; set; }
        public decimal? StartValue { get; set; }
        public decimal? EndValue { get; set; }
        public decimal MinRoyality { get; set; }
    }
}
