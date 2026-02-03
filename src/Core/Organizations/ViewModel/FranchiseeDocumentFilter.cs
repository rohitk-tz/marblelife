using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
   public class FranchiseeDocumentFilter
    {
        public long FranchiseeId { get; set; }

        public long DocumentTypeId { get; set; }

        public string UploadedOn { get; set; }
    }
}
