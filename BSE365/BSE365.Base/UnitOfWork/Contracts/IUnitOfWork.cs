using BSE365.Base.Infrastructures;
using BSE365.Base.Repositories.Contracts;
using System;
using System.Data;

namespace BSE365.Base.UnitOfWork.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IObjectState;
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool Commit();
        void Rollback();
    }
}
