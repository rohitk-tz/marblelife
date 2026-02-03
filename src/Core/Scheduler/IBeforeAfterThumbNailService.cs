using Core.Scheduler.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler
{
   public interface IBeforeAfterThumbNailService
    {
        FileViewModel CreateImageThumb(Application.Domain.File file, string css);
    }
}
