using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using System.Diagnostics;
using System.Reflection;
using GSS.Radar.Infrastructure.Attributes;
using GSS.Radar.Infrastructure.Exceptions;
using GSS.Radar.Infrastructure.NH;

namespace GSS.Radar.Infrastructure.Data.Impl
{
    public abstract class DaoBase<T> :IDaoBase<T>, IDisposable
        where T : class
    {
        protected static int BatchSize
        {
            get
            {
                int result = 0;
                int.TryParse(DataContextBase.Current.GetHibernateProperty("adonet.batch_size"), out result);
                return result;
            }
        }

        protected virtual IDaoInjector Injector { get { return SpringContext.GetObject<IDaoInjector>("IDaoInjector"); } }

        protected virtual ISession Session { get { return DataContextBase.Current.Session; } }

        protected virtual bool EnableCache { get { return false; } }

        public virtual IQueryOver<T, T> QueryOver()
        {
            return this.QueryOver((queryOver, alias) => { });
        }
        public virtual IQueryOver<T, T> QueryOver(string excludedFilterId)
        {
            return QueryOver(new List<string> { excludedFilterId });
        }
        public virtual IQueryOver<T, T> QueryOver(IList<string> excludedFilterIds)
        {
            return QueryOver((queryOver, alias) => { }, excludedFilterIds);
        }

        public virtual IQueryOver<T, T> QueryOver(Action<IQueryOver<T,T>, T> query)
        {
            return QueryOver(query, null as IList<string>);
        }

        public virtual IQueryOver<T, T> QueryOver(Action<IQueryOver<T,T>, T> query, string excludedFilterId)
        {
            return QueryOver(query, new List<string> { excludedFilterId });
        }

        public virtual IQueryOver<T, T> QueryOver(Action<IQueryOver<T,T>, T> query, IList<string> excludedFilterIds)
        {
            T alias = null;
            IQueryOver<T, T> result;
            Expression<Func<T>> aliasExp = () => alias;

            if (EnableCache)
            {
                result = Session.QueryOver<T>(aliasExp).Cacheable().CacheMode(CacheMode.Get) as IQueryOver<T, T>;
            }
            else
            { result = Session.QueryOver<T>(aliasExp); }

            Injector.InjectFilter<T>(result.UnderlyingCriteria, excludedFilterIds);
            query.Invoke(result, alias);
            return result;
        }

        public virtual IQueryOver<T, T> QueryOver(Expression<Func<T>> alias)
        {
            return QueryOver(alias, null as IList<string>);
        }

        public virtual IQueryOver<T, T> QueryOver(Expression<Func<T>> alias, string excludedFilterId)
        {
            return QueryOver(alias, new List<string> { excludedFilterId });
        }

        public virtual IQueryOver<T, T> QueryOver(Expression<Func<T>> alias, IList<string> excludedFilterIds)
        {
            IQueryOver<T, T> result;
            if (EnableCache)
            {
                result = Session.QueryOver<T>(alias).Cacheable().CacheMode(CacheMode.Get) as IQueryOver<T, T>;
            }
            else
            { result = Session.QueryOver<T>(alias); }

            // Injector.InjectFilter<T>(result.UnderlyingCriteria, excludedFilterIds);
            return result;
        }

        public virtual ICriteria Criteria()
        {
            ICriteria result;
            if (EnableCache)
            {
                result = Session.CreateCriteria<T>().SetCacheable(true).SetCacheMode(CacheMode.Get);
            }
            else
            { result = Session.CreateCriteria<T>(); }

            Injector.InjectFilter<T>(result, null as IList<string>);

            return result;
        }

        ~DaoBase()
        {
            Dispose(false);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
