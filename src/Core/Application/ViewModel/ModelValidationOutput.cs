using System;
using System.Collections.Generic;

namespace Core.Application.ViewModel
{
    public class ModelValidationOutput
    {
        public bool IsValid { get; set; }

        public List<ModelValidationItem> Errors { get; set; }

        public ModelValidationOutput()
        {
            IsValid = true;
        }
    }
}