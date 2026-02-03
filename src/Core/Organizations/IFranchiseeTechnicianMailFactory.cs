using Core.Organizations.ViewModel;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations
{
   public interface IFranchiseeTechnicianMailFactory
    {
        FranchiseeEmailEditModel CreateEditModel(FranchiseeTechMailService franchiseeTechMailService);
    }
}
