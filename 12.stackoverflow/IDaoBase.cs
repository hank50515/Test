using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GSS.Radar.Infrastructure.Data
{
    public interface IDaoBase<T>
        where T : class
    {
        IQueryOver<T, T> QueryOver();

        IQueryOver<T, T> QueryOver(string excludedFilterId);

        IQueryOver<T, T> QueryOver(IList<string> excludedFilterIds);

        IQueryOver<T, T> QueryOver(Action<IQueryOver<T, T>, T> query);

        IQueryOver<T, T> QueryOver(Action<IQueryOver<T, T>, T> query, string excludedFilterId);

        IQueryOver<T, T> QueryOver(Action<IQueryOver<T, T>, T> query, IList<string> excludedFilterIds);

        IQueryOver<T, T> QueryOver(Expression<Func<T>> alias);

        IQueryOver<T, T> QueryOver(Expression<Func<T>> alias, string excludedFilterId);

        IQueryOver<T, T> QueryOver(Expression<Func<T>> alias, IList<string> excludedFilterIds);

        ICriteria Criteria();
    }
}
