using System;

namespace Core.Application
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : DomainBase;
        void SaveChanges();
        void StartTransaction();
        void Setup();
        void Cleanup();
        void Rollback();
        void ResetContext();
    }
}
