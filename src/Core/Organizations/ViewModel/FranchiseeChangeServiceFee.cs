﻿using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeChangeServiceFee
    {
        public bool? IsRoyality { get; set; }
        public long LoanId { get; set; }

        public long UserId { get; set; }
    }
}