using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Base.DataContext.Contracts;
using BSE365.Base.UnitOfWork.Contracts;
using BSE365.Repository.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BSE365.Repository.Repositories
{
    public interface IMoneyRepository
    {
    }

    public class MoneyRepository : IMoneyRepository
    {
        #region Private Fields

        private readonly IDataContextAsync _context;
        private readonly IUnitOfWorkAsync _unitOfWork;

        #endregion Private Fields

        public MoneyRepository(IDataContextAsync context, IUnitOfWorkAsync unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
    }
}