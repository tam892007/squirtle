using BSE365.Base.Infrastructures;
using BSE365.Base.Repositories.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace BSE365.Base.UnitOfWork.Contracts
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IObjectState;
    }
}
