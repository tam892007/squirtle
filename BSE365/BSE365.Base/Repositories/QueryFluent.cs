using BSE365.Base.Infrastructures;
using BSE365.Base.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BSE365.Base.Repositories
{
    public sealed class QueryFluent<TEntity> : IQueryFluent<TEntity> where TEntity : class, IObjectState
    {
        #region Private Fields

        private readonly Expression<Func<TEntity, bool>> _expression;
        private readonly List<Expression<Func<TEntity, object>>> _includes;
        private readonly Repository<TEntity> _repository;
        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> _orderBy;
        /*Full Text Search*/
        private string _searchKey;
        private bool _exactMatch;
        private string[] _relativeFieldsPaths;

        #endregion Private Fields

        #region Constructors

        public QueryFluent(Repository<TEntity> repository)
        {
            _repository = repository;
            _includes = new List<Expression<Func<TEntity, object>>>();
        }

        public QueryFluent(Repository<TEntity> repository, IQueryObject<TEntity> queryObject)
            : this(repository)
        {
            _expression = queryObject.Query();
        }

        public QueryFluent(Repository<TEntity> repository, Expression<Func<TEntity, bool>> expression)
            : this(repository)
        {
            _expression = expression;
        }

        public QueryFluent(Repository<TEntity> repository, Expression<Func<TEntity, bool>> expression, string searchKey,
            bool exactMatch)
            : this(repository)
        {
            _expression = expression;
            _searchKey = searchKey;
            _exactMatch = exactMatch;
        }

        #endregion Constructors

        public IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _orderBy = orderBy;
            return this;
        }

        public IQueryFluent<TEntity> Include(Expression<Func<TEntity, object>> expression)
        {
            _includes.Add(expression);
            return this;
        }

        public IQueryFluent<TEntity> FullTextSearch(string searchKey, bool isExactMatch = true, params string[] relativeFieldsPaths)
        {
            _exactMatch = isExactMatch;
            _searchKey = searchKey;
            _relativeFieldsPaths = relativeFieldsPaths;
            return this;
        }


        public IQueryable<TEntity> SelectPage(int page, int pageSize, out int totalCount)
        {
            totalCount = _repository.Select(_expression, null, null, null, null, _searchKey, _exactMatch, _relativeFieldsPaths).Count();
            return _repository.Select(_expression, _orderBy, _includes, page, pageSize, _searchKey, _exactMatch, _relativeFieldsPaths);
        }

        public IEnumerable<TEntity> Select()
        {
            return _repository.Select(_expression, _orderBy, _includes, null, null, _searchKey, _exactMatch, _relativeFieldsPaths);
        }

        public IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return
                _repository.Select(_expression, _orderBy, _includes, null, null, _searchKey, _exactMatch, _relativeFieldsPaths)
                    .Select(selector);
        }

        public async Task<IEnumerable<TEntity>> SelectAsync()
        {
            return await _repository.SelectAsync(_expression, _orderBy, _includes, null, null, _searchKey, _exactMatch, _relativeFieldsPaths);
        }

        public IQueryable<TEntity> SqlQuery(string query, params object[] parameters)
        {
            return _repository.SelectQuery(query, parameters).AsQueryable();
        }


        public IQueryable<TResult> SelectQueryable<TResult>(Expression<Func<TEntity, TResult>> selector, int page,
            int pageSize, out int totalCount)
        {
            totalCount = _repository.Select(_expression, null, null, null, null, _searchKey, _exactMatch, _relativeFieldsPaths).Count();
            return
                _repository.Select(_expression, _orderBy, _includes, page, pageSize, _searchKey, _exactMatch, _relativeFieldsPaths)
                    .Select(selector);
        }
    }
}
