using BSE365.Base.Infrastructures;
using System;

namespace BSE365.Base.DataContext.Contracts
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();
        void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState;
        void SyncObjectsStatePostCommit();
    }
}
