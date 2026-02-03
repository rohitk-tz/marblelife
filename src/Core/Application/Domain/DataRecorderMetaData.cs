using Core.Application.Impl;
using System;

namespace Core.Application.Domain
{
    public class DataRecorderMetaData : DomainBase
    {
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateModified { get; set; }
        public virtual long? CreatedBy { get; set; }
        public virtual long? ModifiedBy { get; set; }

        public virtual DataRecorderMetaData GetClone()
        {
            return new DataRecorderMetaData(ModifiedBy ?? CreatedBy ?? 0);
        }

        public virtual void SetModifiedBy(long modifiedBy)
        {
            // IsNew = true;
            SetModifiedBy(modifiedBy, ApplicationManager.DependencyInjection.Resolve<Clock>().UtcNow);
        }

        public virtual void SetModifiedBy(long modifiedBy, DateTime dateModified)
        {
            ModifiedBy = modifiedBy > 0 ? (long?)modifiedBy : null;
            DateModified = dateModified;
        }

        public DataRecorderMetaData()
            : this(ApplicationManager.DependencyInjection.Resolve<IClock>().UtcNow)
        {
            // IsNew = true;
        }

        public DataRecorderMetaData(DateTime dateCreated)
            : this(dateCreated, 0)
        {
            // IsNew = true;
        }

        public DataRecorderMetaData(long createdBy)
            : this(ApplicationManager.DependencyInjection.Resolve<Clock>().UtcNow, createdBy)
        {
            // IsNew = true;
        }

        public DataRecorderMetaData(DateTime dateCreated, long createdBy)
        {
            DateCreated = dateCreated;
            if (createdBy > 0) CreatedBy = createdBy;
        }
    }
}
