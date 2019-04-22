using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using System.Diagnostics;
using System.Reflection;
using GSS.Radar.Infrastructure.Attributes;
using GSS.Radar.Infrastructure.Exceptions;
using GSS.Radar.Infrastructure.Data.Extensions;
using NHibernate.Criterion;
using System.Data;
using System.Collections;
using GSS.Radar.Infrastructure.Data.SqlHelper;
using GSS.Radar.Infrastructure.Data.Enums;

namespace GSS.Radar.Infrastructure.Data.Impl
{
    public class EntityDao<T> : DaoBase<T>, IEntityDao<T>
        where T : class
    {
        private IGenericDao DaoGeneric = GenericDao.Current;

        public static Boolean IsBatchFlush { get { return DataContextBase.Current.BatchFlush; } }

        public SortOrderBuilder<T> Sort { get { return new SortOrderBuilder<T>(); } }

        public virtual ISQLQuery SQLQuery(SqlBuilder sb)
        {
            return DaoGeneric.SQLQuery(sb);
        }

        public virtual IList<T> All()
        {
            return this.All(Sort);
        }

        public virtual IList<T> All(SortOrderBuilder<T> order)
        {
            return order.GetQueryOver(QueryOver()).List<T>();
        }

        public virtual IList<T> All(out int total, int pageIndex = 0, int size = 50)
        {
            return Query(null, Sort, out total, pageIndex, size);
        }

        public virtual IList<T> All(SortOrderBuilder<T> order, out int total, int pageIndex = 0, int size = 50)
        {
            return Query(null, order, out total, pageIndex, size);
        }

        public virtual T Single(Expression<Func<T, bool>> predicate)
        {
            IList<T> tmp = Query(predicate);
            if (tmp.Count > 1) throw new MoreThenOneException(string.Format("{0} : More then one record.", typeof(T)));
            if (tmp.Count == 0) return null;
            return tmp[0];
        }

        public virtual IList<T> Query(Expression<Func<T, bool>> predicate)
        {
            return Query(predicate, Sort);
        }

        public virtual IList<T> Query(Expression<Func<T, bool>> predicate, SortOrderBuilder<T> order)
        {
            return order.GetQueryOver(QueryOver()).Where(predicate).List<T>();
        }

        public virtual IList<T> Query(Expression<Func<T, bool>> predicate, SortOrderBuilder<T> order, out int total, int pageIndex = 0, int size = 50)
        {
            IQueryOver<T, T> query = CreateQuery(predicate);
            return PagingQuery(query, order, out total, pageIndex, size);
        }

        public virtual IList<T> PagingQuery(IQueryOver<T, T> query, SortOrderBuilder<T> order, out int total, int pageIndex = 0, int size = 50)
        {
            return query.PagingList<T>(order, out total, pageIndex, size);
        }

        public virtual IList<T> QueryByCriteria(IList<ICriterion> criterions, SortOrderBuilder<T> order, out int total, int pageIndex = 0, int size = 50)
        {
            IQueryOver<T, T> query = CreateQuery(criterions);
            return PagingQuery(query, order, out total, pageIndex, size);
        }

        protected virtual IQueryOver<T, T> CreateQuery(Expression<Func<T, bool>> predicate)
        {
            IQueryOver<T, T> query = QueryOver().Where(predicate);
            return query;
        }

        protected virtual IQueryOver<T, T> CreateQuery(IList<ICriterion> criterions)
        {
            IQueryOver<T, T> query = QueryOver();
            if (criterions != null)
            {
                AddCriterions(query, criterions);
            }
            return query;
        }

        public virtual IList<T> QueryByCriteria(IList<ICriterion> criterions)
        {
            var query = this.Criteria();
            foreach (ICriterion item in criterions)
            {
                query.Add(item);
            }
            return query.List<T>();
        }

        public virtual IList<T> QueryByCriteria(IList<ICriterion> criterions, SortOrderBuilder<T> order)
        {
            return order.GetQueryOver(CreateQuery(criterions)).List<T>();
        }

        public virtual IList<T> QueryByParameter(IQueryParameter param, SortOrderBuilder<T> order)
        {
            IList<ICriterion> cri = new List<ICriterion>().AddCriterion(param.FilterGroup);
            return QueryByCriteria(cri, order);
        }

        public virtual string GetInSQLStatement(string fieldName, string mainSql, Action<SqlBuilder> setupSql)
        {
            var sb = new SqlBuilder(mainSql);
            setupSql.Invoke(sb);
            return sb.GetConvertedSQL(true);
        }

        public virtual SQLQueryFilterGroup GetInSQL(string fieldName, string mainSql, Action<SqlBuilder> setupSql)
        {
            string inSql = GetInSQLStatement(fieldName, mainSql, setupSql);
            var result = SQLQueryFilterGroup.Factory()
                .AddFilter(fieldName, operators.InSql, inSql);
            return result;
        }

        protected virtual void AddCriterions(IQueryOver<T, T> query, IList<ICriterion> criterions)
        {
            foreach (ICriterion item in criterions)
            {
                query.UnderlyingCriteria.Add(item);
            }
        }

        public virtual void Save(T entity)
        {
            CreateOrUpdate(entity);
        }

        public virtual T CreateOrUpdate(T entity)
        {
            return DaoGeneric.CreateOrUpdate(entity) as T;
        }

        public virtual IEnumerable<T> CreateOrUpdate(IEnumerable<T> entities)
        {
            var result = DaoGeneric.CreateOrUpdate(entities);
            if (result == null) return null;
            var enumerable = result.Cast<T>();
            return enumerable;
        }

        public virtual T Create(T entity)
        {
            return DaoGeneric.Create(entity, true) as T;
        }

        public virtual T CreateWithSpecificTenant(T entity)
        {
            return DaoGeneric.Create(entity, false) as T;
        }

        public virtual IEnumerable<T> Create(IEnumerable<T> entities)
        {
            var result = DaoGeneric.Create(entities);
            if (result == null) return null;
            var enumerable = result.Cast<T>();
            return enumerable;
        }

        public virtual T Load(object id)
        {
            return Session.Load<T>(id);
        }

        public virtual T Get(object id)
        {
            return Session.Get<T>(id);
        }

        public virtual T Merge(T entity)
        {
            T result;
            if (Contains(entity))
            {
                result = Session.Merge(entity);
            }
            else
            {
                object id = typeof(T).GetProperty(GetIdPropertyName()).GetValue(entity);
                result = Session.Get<T>(id);
                if (result == null)
                {
                    var typeName = typeof(T).FullName;
                    throw new NotFoundKeyException(typeName, id);
                }
                merge(entity, result);
            }
            return result;
        }

        private void merge(T src, T target)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                object sval = typeof(T).GetProperty(prop.Name).GetValue(src);
                object tval = typeof(T).GetProperty(prop.Name).GetValue(target);
                if (sval != tval)
                {
                    //如果遇到List要再比較List裡面的每個物件
                    if (prop.PropertyType.Name == "IList`1")
                    {
                        margeList(sval, tval);
                    }
                    else
                    {
                        if (prop.Name != "CreatedDate" && prop.Name != "CreatedUser")
                        {
                            typeof(T).GetProperty(prop.Name).SetValue(target, sval);
                        }
                    }
                }
            }
        }

        private void margeList(object sval, object tval)
        {
            Type objSType = sval.GetType();
            Type objTType = tval.GetType();

            int countS = Convert.ToInt32(objSType.GetProperty("Count").GetValue(sval, null));
            int countT = Convert.ToInt32(objTType.GetProperty("Count").GetValue(tval, null));

            if (countS == 0)
            {
                tval.GetType().GetMethod("Clear").Invoke(tval, null);
                return;
            }

            compareList(sval, tval, objSType, objTType, countS, countT, "Add");

            compareList(tval, sval, objTType, objSType, countT, countS, "Remove");
        }

        private void compareList(object s, object t, Type sType, Type tType, int countS, int countT, string method)
        {

            for (int i = 0; i < countS; i++)
            {
                object itemS = sType.GetProperty("Item").GetValue(s, new object[] { i });
                Type itemSType = itemS.GetType();

                var itemSIDField = Session.SessionFactory.GetClassMetadata(itemSType).IdentifierPropertyName;

                if (!isPrimitive(itemS.GetType().GetProperty(itemSIDField).PropertyType)) return;

                var isExistsFlag = false;
                for (int j = 0; j < countT; j++)
                {
                    object itemT = tType.GetProperty("Item").GetValue(t, new object[] { j });

                    foreach (var propS in itemS.GetType().GetProperties().Where(x => x.Name == itemSIDField))
                    {
                        foreach (var propT in itemT.GetType().GetProperties().Where(x => x.Name == itemSIDField))
                        {
                            var sId = Convert.ToInt64(propS.GetValue(itemS, null));
                            var tId = Convert.ToInt64(propT.GetValue(itemT, null));
                            if (sId == tId)
                            {
                                isExistsFlag = true;
                                if (method == "Add")
                                {
                                    foreach (var prop in itemT.GetType().GetProperties())
                                    {
                                        object sval = itemT.GetType().GetProperty(prop.Name).GetValue(itemS);
                                        object tval = itemT.GetType().GetProperty(prop.Name).GetValue(itemT);
                                        if (sval != tval)
                                        {
                                            itemT.GetType().GetProperty(prop.Name).SetValue(itemT, sval);
                                        }
                                    }

                                    (itemT as Domain.Base.IAbstractModel).SetModifyInfo();
                                }
                            }
                        }
                    }
                }

                if (isExistsFlag == false)
                {
                    if (method == "Add")
                    {
                        (itemS as Domain.Base.IAbstractModel).SetCreateInfo();
                        (itemS as Domain.Base.IAbstractModel).SetModifyInfo();
                    }
                    t.GetType().GetMethod(method).Invoke(t, new object[] { itemS });
                }
            }
        }

        private bool isPrimitive(Type type)
        {
            return type == typeof(Int64);
        }

        public virtual void Update(T entity)
        {
            update(entity, true);

            flush();
        }

        public virtual void UpdateWithSpecificTenant(T entity)
        {
            update(entity, false);

            flush();
        }

        private void update(T entity, bool useCurrentTenant)
        {

            T entityToUpdate = entity;
            VerifyIdentifier(entity);

            beginTransaction();
            entityToUpdate = Merge(entity);

            (entityToUpdate as Domain.Base.IAbstractModel).SetModifyInfo();

            if (useCurrentTenant)
            {
                var b = entity as Domain.Base.IModelBase;
                if (b != null)
                {
                    b.SetTenantInfo();
                }
            }

            Session.Update(entityToUpdate);
        }

        private void flush()
        {
            if (!IsBatchFlush)
            {
                Session.Flush();
            }
        }

        private void beginTransaction()
        {
            if (!Session.Transaction.IsActive) Session.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            Session.SetBatchSize(BatchSize);
            foreach (var item in entities)
            {
                update(item, true);
            }
            flush();
        }

        public virtual bool Contains(T entity)
        {
            return Session.Contains(entity);
        }

        public virtual bool IsIdentifierValid(T entity)
        {
            if (Session.Contains(entity)) return true;
            string idPropertyName = GetIdPropertyName();
            return (typeof(T).GetProperty(idPropertyName).GetValue(entity) != null);
        }

        protected virtual string GetIdPropertyName()
        {
            return Session.SessionFactory.GetClassMetadata(typeof(T)).IdentifierPropertyName;
        }

        protected virtual void VerifyIdentifier(T entity)
        {
            if (!IsIdentifierValid(entity))
            {
                throw new InvalidException("Can't delete. Your object is not a persisten object! Please assign object id.");
            }
        }

        public virtual void Delete(T entity)
        {
            VerifyIdentifier(entity);
            beginTransaction();
            Session.Delete(entity);
            flush();
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            Session.SetBatchSize(BatchSize);
            foreach (var item in entities)
            {
                VerifyIdentifier(item);
                beginTransaction();
                Session.Delete(item);
            }
            flush();
        }

        public virtual int Delete(Expression<Func<T, bool>> predicate)
        {
            var data = Query(predicate);
            return _delete(data);
        }

        public virtual int Delete(params Action<SqlWhereBuilder>[] where)
        {
            var sb = new SqlBuilder("", SqlBuilder.ParamType.NHibernate).WhereScope(false, where);
            var t = typeof(T);
            beginTransaction();
            var result = Session.Delete(string.Format("from {0} where {1}", t.Name, sb.GetConvertedSQL(true)));
            flush();
            return result;
        }

        public virtual int Delete(IQueryOver<T, T> query)
        {
            var data = query.List();
            return _delete(data);
        }

        private int _delete(IList<T> data)
        {
            Session.SetBatchSize(BatchSize);
            int result = data.Count;
            foreach (var item in data)
            {
                beginTransaction();
                Session.Delete(item);
            }
            flush();

            return result;
        }

        public virtual IList<TResult> ExecuteSQL<TResult>(ISQLQuery query)
        {
            return query.SQLQueryToList<TResult>();
        }

        public virtual IList<TResult> ExecuteSQL<TResult>(SqlHelper.SqlBuilder sb, IQueryPagingParameter paging)
        {
            return DaoGeneric.ExecuteSQL<TResult>(sb, paging);
        }

        public virtual IList<TResult> ExecuteSQL<TResult>(SqlHelper.SqlBuilder sb, IQueryPagingParameter paging, out int totalRowCount)
        {
            return DaoGeneric.ExecuteSQL<TResult>(sb, paging, out totalRowCount);
        }

        public virtual IList<TResult> ExecuteSQL<TResult>(SqlHelper.SqlBuilder sb)
        {
            return DaoGeneric.ExecuteSQL<TResult>(sb);
        }

        public virtual DataTable ExecuteSQL(SqlHelper.SqlBuilder sb)
        {
            return DaoGeneric.ExecuteSQL(sb);
        }

        public virtual int ExecuteUpdate(SqlHelper.SqlBuilder sb)
        {
            return DaoGeneric.ExecuteUpdate(sb);
        }

        public virtual int ExecuteSpUpdate(SqlHelper.SqlBuilder sb)
        {
            return DaoGeneric.ExecuteSpUpdate(sb);
        }

        public virtual TResult ExecuteSQL2Scalar<TResult>(SqlHelper.SqlBuilder sb)
        {
            return DaoGeneric.ExecuteSQL2Scalar<TResult>(sb);
        }

        public virtual int ExecuteSQLRowCount(SqlHelper.SqlBuilder sb)
        {
            return DaoGeneric.ExecuteSQLRowCount(sb);
        }

        public virtual void Evict<T>(T entity)
        {
            DaoGeneric.Evict(entity);
        }

        public virtual void Evict(IList<T> entities)
        {
            DaoGeneric.Evict(entities);
        }

        public virtual void Evict(IList<T> entities, Action<T> others)
        {
            DaoGeneric.Evict(entities, others);
        }
        public virtual void Evict(Type type)
        {
            DaoGeneric.Evict(type);
        }
        public virtual void BatchFlushScope(Action<IEntityDao<T>> action)
        {
            action.Invoke(this);
            flush();
        }

        internal void DeleteNoFlush(T entity)
        {
            beginTransaction();
            Session.Delete(entity);
        }

        internal void CreateNoFlush(T entity)
        {
            beginTransaction();

            (entity as Domain.Base.IAbstractModel).SetModifyInfo();

            var b = entity as Domain.Base.IModelBase;
            if (b != null)
            {
                b.SetTenantInfo();
            }

            Session.Save(entity);
        }

        internal void UpdateNoFlush(T entity)
        {
            update(entity, true);
        }

        internal void DeleteNoFlush(IEnumerable<T> entities)
        {
            Session.SetBatchSize(BatchSize);
            foreach (var item in entities)
            {
                DeleteNoFlush(item);
            }
        }

        internal void CreateNoFlush(IEnumerable<T> entities)
        {
            Session.SetBatchSize(BatchSize);
            foreach (var item in entities)
            {
                CreateNoFlush(item);
            }
        }
        internal void UpdateNoFlush(IEnumerable<T> entities)
        {
            Session.SetBatchSize(BatchSize);
            foreach (var item in entities)
            {
                UpdateNoFlush(item);
            }
        }
    }

    public class BatchScopAction<T>
        where T : class
    {
        EntityDao<T> dao { get; set; }

        private BatchScopAction(IEntityDao<T> daoBase)
        {
            dao = daoBase as EntityDao<T>;
        }

        public static BatchScopAction<T> New(IEntityDao<T> daoBase)
        {
            return new BatchScopAction<T>(daoBase);
        }

        public void Update(IEnumerable<T> entites)
        {
            dao.UpdateNoFlush(entites);
        }

        public void Update(T entity)
        {
            dao.UpdateNoFlush(entity);
        }

        public void Create(IEnumerable<T> entities)
        {
            dao.CreateNoFlush(entities);
        }

        public void Create(T entity)
        {
            dao.CreateNoFlush(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            dao.DeleteNoFlush(entities);
        }

        public void Delete(T entity)
        {
            dao.DeleteNoFlush(entity);
        }
    }
}
