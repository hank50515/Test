using GSS.Radar.Infrastructure.Data.SqlHelper;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GSS.Radar.Infrastructure.Data
{
    public interface IEntityDao<T> : IDaoBase<T>
        where T : class
    {
        SortOrderBuilder<T> Sort { get; }

        IList<T> All();

        IList<T> All(SortOrderBuilder<T> order);

        IList<T> All(out int total, int pageIndex = 0, int size = 50);

        IList<T> All(SortOrderBuilder<T> order, out int total, int pageIndex = 0, int size = 50);

        T Single(Expression<Func<T, bool>> predicate);

        IList<T> Query(Expression<Func<T, bool>> predicate);

        IList<T> Query(Expression<Func<T, bool>> predicate, SortOrderBuilder<T> order);

        IList<T> Query(Expression<Func<T, bool>> predicate, SortOrderBuilder<T> order, out int total, int pageIndex = 0, int size = 50);

        IList<T> QueryByCriteria(IList<ICriterion> criterions, SortOrderBuilder<T> order, out int total, int pageIndex = 0, int size = 50);

        IList<T> QueryByCriteria(IList<ICriterion> criterions, SortOrderBuilder<T> order);

        IList<T> QueryByCriteria(IList<ICriterion> criterions);

        IList<T> QueryByParameter(IQueryParameter param, SortOrderBuilder<T> order);

        void Save(T entity);

        T CreateOrUpdate(T entity);

        IEnumerable<T> CreateOrUpdate(IEnumerable<T> entities);

        T Create(T entity);

        T CreateWithSpecificTenant(T entity);

        IEnumerable<T> Create(IEnumerable<T> entities);

        void Update(T entity);

        void Update(IEnumerable<T> entities);

        void UpdateWithSpecificTenant(T entity);

        void Delete(T entity);

        void Delete(IEnumerable<T> entities);

        int Delete(Expression<Func<T, bool>> predicate);

        int Delete(params Action<SqlWhereBuilder>[] where);

        int Delete(IQueryOver<T, T> query);

        T Merge(T entity);

        IList<TResult> ExecuteSQL<TResult>(SqlHelper.SqlBuilder sb);

        DataTable ExecuteSQL(SqlHelper.SqlBuilder sb);

        void Evict<T>(T entity);

        void Evict(IList<T> entities);
        
        void Evict(IList<T> entities, Action<T> others);
        
        void Evict(Type type);

        void BatchFlushScope(Action<IEntityDao<T>> action);
    }
}
