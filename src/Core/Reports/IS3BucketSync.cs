using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports
{
    public interface IS3BucketSync
    {
        void S3BucketSyncInEvery2min();
    }
}
