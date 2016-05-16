using System.Threading;
using System.Threading.Tasks;

namespace BSE365.Base.DataContext.Contracts
{
    public interface IDataContextAsync : IDataContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}
