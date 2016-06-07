using BSE365.Base.DataContext.Contracts;
using BSE365.Base.UnitOfWork.Contracts;

namespace BSE365.Repository.Repositories
{
    public interface IMoneyRepository
    {
    }

    public class MoneyRepository : IMoneyRepository
    {
        public MoneyRepository(IDataContextAsync context, IUnitOfWorkAsync unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        #region Private Fields

        private readonly IDataContextAsync _context;
        private readonly IUnitOfWorkAsync _unitOfWork;

        #endregion Private Fields
    }
}