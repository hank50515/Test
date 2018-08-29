using GSS.ITSM.Globalization;
using GSS.ITSM.Model;
using GSS.ITSM.ToolKit;
using KendoGridBinder;
using KendoGridBinder.Containers;
using NHibernate;
using NHibernate.Criterion;
using Spring.Data.NHibernate;
using Spring.Data.NHibernate.Generic;
using Spring.Data.NHibernate.Generic.Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
　
namespace GSS.ITSM.Dal
{
    /// <summary>
    /// 基底資料存取層，透過NHibernate進行資料存取，提供基本常用的資料存取介面
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <history>
    /// [日期]        [動作]    [作者]        [說明]
    /// 2014/01/28    Create    Carlos Liu    底層建立
    /// 2014/08/29    Modify    Joanne Chuang 新增傳入InCond條件
    /// 2015/3/3      Modify    Carlos Liu      增加多筆取號
    /// </history>
    public abstract class AbstractBaseRepository : HibernateDaoSupport, IRepository
    {
        #region IRepository 成員
　
        /// <summary>
        /// 取得單一Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Pkey">PKey</param>
        /// <returns></returns>
        public T LoadSingle<T>(string Pkey)
        {
            var entity = Session.Get<T>(Pkey);
            return entity;
            //return CheckNullObject(o) ? default(T) : o;
        }
　
        /// <summary>
        /// 取得單一Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Pkey">PKey</param>
        /// <returns></returns>
        public T LoadSingle<T>(object Pkey)
        {
            var entity = Session.Get<T>(Pkey);
            return entity;
            //return CheckNullObject(o) ? default(T) : o;
        }
        /// <summary>
        ///取得單一Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CompositeKeyObj">Entity Object,必須傳入KEY</param>
        /// <returns></returns>
        public T LoadSingle<T>(T CompositeKeyObj)
        {
            return Session.Get<T>(CompositeKeyObj);
        }
　
        /// <summary>
        /// 取得單一Entity,包含Where條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <returns></returns>
        public T LoadSingle<T>(Expression<Func<T, bool>> Cond) where T : class
        {
            return this.LoadSingle<T>(Cond, null, null, null);
        }
　
        /// <summary>
        /// 取得單一Entity,包含Where條件,like條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <returns></returns>
        public T LoadSingle<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds) where T : class
        {
            return this.LoadSingle<T>(Cond, ltLikeConds, null, null);
        }
　
        /// <summary>
        /// 取得單一Entity,包含Where條件,between條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <returns></returns>
        public T LoadSingle<T>(Expression<Func<T, bool>> Cond, List<BetweenCond<T>> ltBetweenConds) where T : class
        {
            return this.LoadSingle<T>(Cond, null, ltBetweenConds, null);
        }
　
        /// <summary>
        /// 取得單一Entity,包含Where條件,In條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltInConds">In條件</param>
        /// <returns></returns>
        public T LoadSingle<T>(Expression<Func<T, bool>> Cond, List<InCond<T>> ltInConds) where T : class
        {
            return this.LoadSingle<T>(Cond, null, null, ltInConds);
        }
　
        /// <summary>
        /// 取得單一Entity,包含Where條件,like條件,between條件, in條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <param name="ltInConds">In條件</param>
        /// <returns></returns>
        public T LoadSingle<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds, List<BetweenCond<T>> ltBetweenConds, List<InCond<T>> ltInConds) where T : class
        {
            //if (Cond == null && ltLikeConds == null && ltBetweenConds == null) return null;
　
            var q = Session.QueryOver<T>();
            //加入一般條件
            if (Cond != null)
            {
                q.Where(Cond);
            }
            //加入like條件
            if (ltLikeConds != null)
            {
                foreach (var item in ltLikeConds)
                    q.WhereRestrictionOn(item.likeField).IsLike(item.likeWord);
            }
            //加入between (...)
            if (ltBetweenConds != null)
            {
                foreach (var item in ltBetweenConds)
                    q.WhereRestrictionOn(item.betweenField).IsBetween(item.betweenFrom).And(item.betweenTo);
            }
　
            //加入in條件
            if (ltInConds != null)
            {
                foreach (var item in ltInConds)
                    q.WhereRestrictionOn(item.InField).IsIn(item.InList);
            }
　
            return q.SingleOrDefault();
        }
　
        /// <summary>
        /// 取得Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns></returns>
        public IList<T> Load<T>() where T : class
        {
            return Session.QueryOver<T>().List<T>();
        }
　
        /// <summary>
        /// 取得Entity,包含Where條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public IList<T> Load<T>(Expression<Func<T, bool>> Cond, List<OrderbyItem> ltOrders = null) where T : class
        {
            return this.Load<T>(Cond, null, null, null, ltOrders);
        }
　
        /// <summary>
        /// 取得Entity,包含Where條件,like條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public IList<T> Load<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds, List<OrderbyItem> ltOrders = null) where T : class
        {
            return this.Load<T>(Cond, ltLikeConds, null, null, ltOrders);
        }
　
        /// <summary>
        /// 取得Entity,包含Where條件,between條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public IList<T> Load<T>(Expression<Func<T, bool>> Cond, List<BetweenCond<T>> ltBetweenConds, List<OrderbyItem> ltOrders = null) where T : class
        {
            return this.Load<T>(Cond, null, ltBetweenConds, null, ltOrders);
        }
　
        /// <summary>
        /// 取得Entity,包含Where條件,in條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltInConds">In條件</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public IList<T> Load<T>(Expression<Func<T, bool>> Cond, List<InCond<T>> ltInConds, List<OrderbyItem> ltOrders = null) where T : class
        {
            return this.Load<T>(Cond, null, null, ltInConds, ltOrders);
        }
　
        /// <summary>
        /// 取得Entity,包含Where條件,like條件,between條件, in條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <param name="ltInConds">In條件</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public IList<T> Load<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds, List<BetweenCond<T>> ltBetweenConds, List<InCond<T>> ltInConds, List<OrderbyItem> ltOrders = null) where T : class
        {
            //if (Cond == null && ltLikeConds == null && ltBetweenConds == null) return null;
　
            var q = Session.QueryOver<T>();
　
            //加入一般條件
            if (Cond != null) q.Where(Cond);
　
            //加入like條件
            if (ltLikeConds != null)
            {
                foreach (var item in ltLikeConds)
                    q.WhereRestrictionOn(item.likeField).IsLike(item.likeWord);
            }
　
            //加入between (...)
            if (ltBetweenConds != null)
            {
                foreach (var item in ltBetweenConds)
                    q.WhereRestrictionOn(item.betweenField).IsBetween(item.betweenFrom).And(item.betweenTo);
            }
　
            //加入in條件
            if (ltInConds != null)
            {
                foreach (var item in ltInConds)
                    q.WhereRestrictionOn(item.InField).IsIn(item.InList);
            }
　
            //加入排序
            if (ltOrders != null)
            {
                foreach (var item in ltOrders)
                    q.UnderlyingCriteria.AddOrder(new Order(item.propertyName, item.ascending));
            }
　
            return q.List<T>();
            //return q.Future<T>().ToList<T>();
        }
　
        /// <summary>
        /// 分頁取得Entity,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public PaggerList<T> LoadPagger<T>(int PageSize, int CurrentPage, List<OrderbyItem> ltOrders) where T : class
        {
            //總數量
            var rowCount = this.Count<T>();
　
            //單頁資料
            var q = Session.QueryOver<T>();
　
            //加入排序
            foreach (var item in ltOrders)
                q.UnderlyingCriteria.AddOrder(new Order(item.propertyName, item.ascending));
　
            var result = q
               .Skip((CurrentPage - 1) * PageSize)
               .Take(PageSize)
               .Future<T>()
               .ToList<T>();
　
            Pagger Pagger = new Pagger()
            {
                PageSize = PageSize,
                CurrentPage = CurrentPage,
                ltOrders = ltOrders,
            };
　
            return CalcPagger(rowCount, Pagger, result);
        }
　
        /// <summary>
        /// 分頁取得Entity,包含Where條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public PaggerList<T> LoadPagger<T>(Expression<Func<T, bool>> Cond, int PageSize, int CurrentPage, List<OrderbyItem> ltOrders) where T : class
        {
            return this.LoadPagger<T>(Cond, null, null, null, PageSize, CurrentPage, ltOrders);
        }
　
        /// <summary>
        /// 分頁取得Entity,包含Where條件,like條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public PaggerList<T> LoadPagger<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds, int PageSize, int CurrentPage, List<OrderbyItem> ltOrders) where T : class
        {
            return this.LoadPagger<T>(Cond, ltLikeConds, null, null, PageSize, CurrentPage, ltOrders);
        }
　
        /// <summary>
        /// 分頁取得Entity,包含Where條件,between條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public PaggerList<T> LoadPagger<T>(Expression<Func<T, bool>> Cond, List<BetweenCond<T>> ltBetweenConds, int PageSize, int CurrentPage, List<OrderbyItem> ltOrders) where T : class
        {
            return this.LoadPagger<T>(Cond, null, ltBetweenConds, null, PageSize, CurrentPage, ltOrders);
        }
　
        /// <summary>
        /// 分頁取得Entity,包含Where條件,in條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltInConds">in條件</param>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public PaggerList<T> LoadPagger<T>(Expression<Func<T, bool>> Cond, List<InCond<T>> ltInConds, int PageSize, int CurrentPage, List<OrderbyItem> ltOrders) where T : class
        {
            return this.LoadPagger<T>(Cond, null, null, ltInConds, PageSize, CurrentPage, ltOrders);
        }
　
        /// <summary>
        /// 分頁取得Entity,包含Where條件,like條件,between條件,in條件,排序
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <param name="ltInConds">in條件</param>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="ltOrders">排序欄位</param>
        /// <returns></returns>
        public PaggerList<T> LoadPagger<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds, List<BetweenCond<T>> ltBetweenConds, List<InCond<T>> ltInConds, int PageSize, int CurrentPage, List<OrderbyItem> ltOrders) where T : class
        {
            //if (Cond == null && ltLikeConds == null && ltBetweenConds == null) return null;
　
            //總數量
            var rowCount = this.Count<T>(Cond, ltLikeConds, ltBetweenConds, ltInConds);
　
            //單頁資料
            var q = Session.QueryOver<T>();
　
            //加入一般條件
            if (Cond != null) q.Where(Cond);
　
            //加入like條件
            if (ltLikeConds != null)
            {
                foreach (var item in ltLikeConds)
                    q.WhereRestrictionOn(item.likeField).IsLike(item.likeWord);
            }
　
            //加入between (...)
            if (ltBetweenConds != null)
            {
                foreach (var item in ltBetweenConds)
                    q.WhereRestrictionOn(item.betweenField).IsBetween(item.betweenFrom).And(item.betweenTo);
            }
　
            //加入in條件
            if (ltInConds != null)
            {
                foreach (var item in ltInConds)
                    q.WhereRestrictionOn(item.InField).IsIn(item.InList);
            }
　
            //加入排序
            foreach (var item in ltOrders)
                q.UnderlyingCriteria.AddOrder(new Order(item.propertyName, item.ascending));
　
            if (Cond != null)
                q = q.Where(Cond);
　
            var result = q
               .Skip((CurrentPage - 1) * PageSize)
               .Take(PageSize)
               .Future<T>()
               .ToList<T>();
　
            Pagger Pagger = new Pagger()
            {
                PageSize = PageSize,
                CurrentPage = CurrentPage,
                ltOrders = ltOrders,
            };
　
            return CalcPagger(rowCount, Pagger, result);
        }
　
        /// <summary>
        /// Queryable for LinQ
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns></returns>
        public IQueryable<T> Query<T>() where T : class
        {
            return Session.QueryOver<T>().List().AsQueryable();
        }
　
        /// <summary>
        /// Queryable for LinQ,包含Where條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> Cond) where T : class
        {
            return this.Query<T>(Cond, null, null, null);
        }
　
        /// <summary>
        /// Queryable for LinQ,包含Where條件,like條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds) where T : class
        {
            return this.Query<T>(Cond, ltLikeConds, null, null);
        }
　
        /// <summary>
        /// Queryable for LinQ,包含Where條件,between條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> Cond, List<BetweenCond<T>> ltBetweenConds) where T : class
        {
            return this.Query<T>(Cond, null, ltBetweenConds, null);
        }
　
        /// <summary>
        /// Queryable for LinQ,包含Where條件,in條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltInConds">in條件</param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> Cond, List<InCond<T>> ltInConds) where T : class
        {
            return this.Query<T>(Cond, null, null, ltInConds);
        }
　
        /// <summary>
        /// Queryable for LinQ,包含Where條件,like條件,between條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <param name="ltInConds">in條件</param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds, List<BetweenCond<T>> ltBetweenConds, List<InCond<T>> ltInConds) where T : class
        {
            //if (Cond == null && ltLikeConds == null && ltBetweenConds == null) return null;
            var q = Session.QueryOver<T>();
　
            //加入一般條件
            if (Cond != null) q.Where(Cond);
　
            //加入like條件
            if (ltLikeConds != null)
            {
                foreach (var item in ltLikeConds)
                    q.WhereRestrictionOn(item.likeField).IsLike(item.likeWord);
            }
　
            //加入between (...)
            if (ltBetweenConds != null)
            {
                foreach (var item in ltBetweenConds)
                    q.WhereRestrictionOn(item.betweenField).IsBetween(item.betweenFrom).And(item.betweenTo);
            }
　
            //加入in條件
            if (ltInConds != null)
            {
                foreach (var item in ltInConds)
                    q.WhereRestrictionOn(item.InField).IsIn(item.InList);
            }
　
            return q.List<T>().AsQueryable();
        }
　
        /// <summary>
        /// 取得數量
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns></returns>
        public int Count<T>() where T : class
        {
            return Session.QueryOver<T>().RowCount();
        }
　
        /// <summary>
        /// 取得數量,包含Where條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> Cond) where T : class
        {
            return this.Count<T>(Cond, null, null, null);
        }
　
        /// <summary>
        /// 取得數量,包含Where條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds) where T : class
        {
            return this.Count<T>(Cond, ltLikeConds, null, null);
        }
　
        /// <summary>
        /// 取得數量,包含Where條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> Cond, List<BetweenCond<T>> ltBetweenConds) where T : class
        {
            return this.Count<T>(Cond, null, ltBetweenConds, null);
        }
　
        /// <summary>
        /// 取得數量,包含in條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltInConds">in條件</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> Cond, List<InCond<T>> ltInConds) where T : class
        {
            return this.Count<T>(Cond, null, null, ltInConds);
        }
　
        /// <summary>
        /// 取得數量,包含Where條件
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">Where條件</param>
        /// <param name="ltLikeConds">like條件</param>
        /// <param name="ltBetweenConds">between條件</param>
        /// <param name="ltInConds">in條件</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> Cond, List<LikeCond<T>> ltLikeConds, List<BetweenCond<T>> ltBetweenConds, List<InCond<T>> ltInConds) where T : class
        {
            //if (Cond == null && ltLikeConds == null && ltBetweenConds == null) return -1;
            var q = Session.QueryOver<T>();
            if (Cond != null) q.Where(Cond);
            //加入like條件
            if (ltLikeConds != null)
            {
                foreach (var item in ltLikeConds)
                    q.WhereRestrictionOn(item.likeField).IsLike(item.likeWord);
            }
　
            //加入between (...)
            if (ltBetweenConds != null)
            {
                foreach (var item in ltBetweenConds)
                    q.WhereRestrictionOn(item.betweenField).IsBetween(item.betweenFrom).And(item.betweenTo);
            }
　
            //加入in條件
            if (ltInConds != null)
            {
                foreach (var item in ltInConds)
                    q.WhereRestrictionOn(item.InField).IsIn(item.InList);
            }
　
            return q.RowCount();
        }
　
        /// <summary>
        /// 新增-Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Entity"></param>
        public void Add<T>(T Entity)
        {
            FlushMode flashmode = Session.FlushMode;
            if (FlushMode.Never == flashmode)
            {
                Session.FlushMode = FlushMode.Auto;
                //txObject.SessionHolder.PreviousFlushMode = flashmode;
            }
            HibernateTemplate.Save(Entity);
        }
　
        /// <summary>
        /// 新增-Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Entity"></param>
        public void Save<T>(T Entity)
        {
            Session.Save(Entity);
        }
　
        /// <summary>
        /// 更新-Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Entity"></param>
        public void Update<T>(T Entity) where T : class
        {
            Session.Update(Entity);
        }
　
        /// <summary>
        /// 更新或新增-Entity
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Entity"></param>
        public void SaveOrUpdate<T>(T Entity) where T : class
        {
            Session.SaveOrUpdate(Entity);
        }
　
        /// <summary>
        /// 更新-Entity,包含Where條件,不須先Load,可以單/多筆更新
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="kvp">key value pair</param>
        /// <param name="Cond">key/value, value可以是List物件</param>
        /// <returns></returns>
        /// <remarks>2015/05/14 Harry Chang Update的欄位若是Key 會出錯,因此在條件後面加入_cond</remarks>
        public int Update<T>(Dictionary<string, object> kvps, Dictionary<string, object> Cond)
        {
            string objName = typeof(T).Name;
            string HQL = string.Empty;
            string setStr = string.Empty;
            string conds = string.Empty;
            int idx = 0;
            int idx2 = 0;
            string prefix = string.Empty;
　
            foreach (var kv in kvps)
            {
                prefix = (idx2 == 0) ? "" : ",";
                setStr += string.Format("{0} o.{1} = :{1} ", prefix, kv.Key);
                idx2 += 1;
            }
　
            foreach (var c in Cond)
            {
                string cType = (c.Value == null) ? string.Empty : c.Value.GetType().Name;
                prefix = (idx == 0) ? "" : "and";
                if (cType == "String" || cType == "Int32" || cType == string.Empty)
                {
                    conds += string.Format("{0} o.{1} = :{1}_cond ", prefix, c.Key);
                }
                else if (cType == "List`1")
                {
                    conds += string.Format("{0} o.{1} in (:{1}_cond) ", prefix, c.Key);
                }
                idx += 1;
            }
　
            HQL = string.Format(@"update {0} o set {1} where {2}", objName, setStr, conds);
            var q = Session.CreateQuery(HQL);
　
            foreach (var kv in kvps)
            {
                if (kv.Value != null)
                {
                    switch (kv.Value.GetType().Name)
                    {
                        case "String":
                            q.SetString(kv.Key, kv.Value.ToString());
                            break;
　
                        case "DateTime":
                            q.SetDateTime(kv.Key, (DateTime)kv.Value);
                            break;
　
                        case "Int32":
                            q.SetInt32(kv.Key, (int)kv.Value);
                            break;
　
                        case "DBNull":
                            q.SetParameter(kv.Key, null, new NHibernate.Type.DateTimeType());
                            break;
　
                        default:
                            q.SetString(kv.Key, kv.Value.ToString());
                            break;
                    }
                }
                else
                {
                    q.SetString(kv.Key, null);
                }
            }
　
            foreach (var c in Cond)
            {
                string cType = (c.Value == null) ? string.Empty : c.Value.GetType().Name;
                if (cType == "String" || cType == "Int32" || cType == string.Empty)
                {
                    string cValue = (c.Value == null) ? string.Empty : c.Value.ToString();
                    q.SetString(c.Key + "_cond", cValue);
                }
                else if (cType == "List`1")
                {
                    q.SetParameterList(c.Key + "_cond", (List<string>)c.Value);
                }
            }
　
            return q.ExecuteUpdate();
        }
　
        /// <summary>
        /// 刪除- Entity必須先Load
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Entity"></param>
        public void Delete<T>(T Entity)
        {
            Session.Delete(Entity);
        }
　
        /// <summary>
        /// 刪除- Entity,包含Where條件,不須先Load,可以單/多筆刪除
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="Cond">key/value, value可以是List物件</param>
        /// <returns></returns>
        public int Delete<T>(Dictionary<string, object> Cond)
        {
            string objName = typeof(T).Name;
            string HQL = string.Empty;
            string conds = string.Empty;
            int idx = 0;
            string prefix = string.Empty;
            foreach (var c in Cond)
            {
                string cType = (c.Value == null) ? string.Empty : c.Value.GetType().Name;
                prefix = (idx == 0) ? "" : "and";
                if (cType == "String" || cType == "Int32" || cType == string.Empty)
                {
                    conds += string.Format("{0} o.{1} = :{1} ", prefix, c.Key);
                }
                else if (cType == "List`1")
                {
                    conds += string.Format("{0} o.{1} in (:{1}) ", prefix, c.Key);
                }
                idx += 1;
            }
　
            HQL = string.Format(@"delete {0} o where {1}", objName, conds);
            var q = Session.CreateQuery(HQL);
　
            foreach (var c in Cond)
            {
                string cType = (c.Value == null) ? string.Empty : c.Value.GetType().Name;
　
                if (cType == "String" || cType == "Int32" || cType == string.Empty)
                {
                    string cValue = (c.Value == null) ? string.Empty : c.Value.ToString();
                    q.SetString(c.Key, cValue);
                }
                else if (cType == "List`1")
                {
                    q.SetParameterList(c.Key, (List<string>)c.Value);
                }
            }
　
            return q.ExecuteUpdate();
        }
　
        /// <summary>
        /// 取得Seq Entity進行取號, 傳入「序號範圍」可取多筆序號
        /// </summary>
        /// <typeparam name="T">Seq Entity</typeparam>
        /// <param name="TenantId">公司Id</param>
        /// <param name="ResetData">重設週期</param>
        /// <param name="SeqObject">序號物件</param>
        /// <param name="SeqRange">序號範圍</param>
        /// <returns></returns>
        public double LoadSerilNo<T>(string TenantId, string ResetData, T SeqObject, int SeqRange = 1) where T : ISeqObject
        {
            // 取得實際的Table Name, Ex: ITSM_SEQ_SERVICE_SERIL_UNO
            string tableName = (Session.SessionFactory.GetClassMetadata(typeof(T)) as NHibernate.Persister.Entity.AbstractEntityPersister).TableName;
　
            // CALL SP_LoadSerilNo @TenantId ,@Kind ,@ResetData ,@SeqRange ,@SEQ OUTPUT
            var query = Session.CreateSQLQuery("EXEC SP_LoadSerilNo @TenantId=:TenantId ,@Kind=:Kind, @ResetData=:ResetData, @SeqRange=:SeqRange");
            query.SetString("TenantId", TenantId);
            query.SetString("Kind", tableName);
            query.SetString("ResetData", ResetData);
            query.SetInt32("SeqRange", SeqRange);
            Int64 SeqNo = (Int64)query.List()[0];
            return (double)SeqNo;
　
            //更新+1
            //string objName = typeof(T).Name;
            //StringBuilder sbHQL = new StringBuilder();
            ////            sbHQL.AppendFormat(@"
            ////                update {0}
            ////                    set Seq=Seq+1
            ////                where TenantId = :TenantId and ResetData = :ResetData
            ////                ", objName);
　
            ////            int cnts = Session.CreateQuery(sbHQL.ToString())
            ////                .SetString("TenantId", TenantId)
            ////                .SetString("ResetData", ResetData)
            ////                .ExecuteUpdate();
　
            //sbHQL.Clear();
            //sbHQL.AppendFormat(@"select s.Seq from {0} as s where TenantId = :TenantId and ResetData = :ResetData", objName);
            //double seq = Session.CreateQuery(sbHQL.ToString())
            //    .SetString("TenantId", TenantId)
            //    .SetString("ResetData", ResetData)
            //    .List<double>()
            //    .SingleOrDefault();
　
            //Dictionary<string, object> kvps = new Dictionary<string, object>();
            //Dictionary<string, object> Cond = new Dictionary<string, object>();
            //kvps.Add("Seq", seq + SeqRange);
            //Cond.Add("TenantId", TenantId);
            //Cond.Add("ResetData", ResetData);
            //int cnts = this.Update<T>(kvps, Cond);
　
            ////沒更新到，表示進入新的週期
            //if (cnts <= 0)
            //{
            //    //從第一筆序號開始
            //    SeqObject.TenantId = TenantId;
            //    SeqObject.ResetData = ResetData;
            //    SeqObject.Seq = SeqRange;
            //    this.Add<T>(SeqObject);
            //    Session.Flush();
            //}
　
            ////取出序號
            //sbHQL.Clear();
            //sbHQL.AppendFormat(@"select s.Seq from {0} as s where TenantId = :TenantId and ResetData = :ResetData", objName);
            //return Session.CreateQuery(sbHQL.ToString())
            //    .SetString("TenantId", TenantId)
            //    .SetString("ResetData", ResetData)
            //    .List<double>()
            //    .SingleOrDefault();
        }
　
        /// <summary>
        /// 強迫NHibernate執行SQL
        /// </summary>
        public void Flush()
        {
            Session.Flush();
        }
　
        /// <summary>
        /// 強迫NHibernate清除level-1 cache
        /// </summary>
        public void Clear()
        {
            Session.Clear();
        }
　
        /// <summary>
        /// 設定FlushMode
        /// </summary>
        public FlushMode FlushMode
        {
            get
            {
                return Session.FlushMode;
            }
　
            set
            {
                Session.FlushMode = value;
            }
        }
　
        public string OrderByString(List<OrderbyItem> ltOrders)
        {
            string orderby = string.Empty;
            if (ltOrders != null)
            {
                string prefix = string.Empty;
                int idx = 0;
                if (ltOrders.Count > 0)
                {
                    orderby += string.Format(" order by ");
                }
                ltOrders = (from p in ltOrders select p).Distinct().ToList();
                foreach (var item in ltOrders)
                {
                    if (!string.IsNullOrEmpty(item.propertyName))
                    {
                        prefix = (idx == 0) ? "" : ",";
　
                        orderby += string.Format(" {0} {1} {2}", prefix, item.propertyName, (item.ascending) ? "asc" : "desc");
                    }
                    else
                        return string.Empty;
                    idx++;
                }
                return orderby;
            }
            else
            {
                return string.Empty;
            }
        }
　
        public string OrderByString(IEnumerable<SortObject> ltOrders)
        {
            string orderby = string.Empty;
            if (ltOrders != null)
            {
                string prefix = string.Empty;
                int idx = 0;
                if (ltOrders.Count() > 0)
                {
                    orderby += string.Format(" order by ");
                }
                ltOrders = (from p in ltOrders select p).Distinct().ToList();
                foreach (var item in ltOrders)
                {
                    if (!string.IsNullOrEmpty(item.Field))
                    {
                        prefix = (idx == 0) ? "" : ",";
                        orderby += string.Format(" {0} {1} {2}", prefix, item.Field, item.Direction);
                    }
                    else
                        return string.Empty;
                    idx++;
                }
                return orderby;
            }
            else
            {
                return string.Empty;
            }
        }
　
        public Expression<Func<T, bool>> GetCondition<T>(T model, params string[] conditionProertyNames)
        {
            var paramExpr = System.Linq.Expressions.Expression.Parameter(typeof(T), typeof(T).Name);
            var expr = ConcactExpressions.FirstExp<T>();
　
            PropertyInfo[] propInfos = typeof(T).GetProperties();
            if (model != null)
            {
                foreach (PropertyInfo propInfo in propInfos)
                {
                    if ((conditionProertyNames.Length > 0) && !conditionProertyNames.Contains(propInfo.Name))
                    {
                        continue;
                    }
　
                    var fieldExpr = System.Linq.Expressions.Expression.Property(paramExpr, propInfo);
                    object value = propInfo.GetValue(model, null);
                    if (propInfo.PropertyType == typeof(System.String))
                    {
                        if (!string.IsNullOrEmpty((string)value))
                        {
                            var fieldValueExpr = System.Linq.Expressions.Expression.Equal(System.Linq.Expressions.Expression.Property(paramExpr, propInfo),
                                                                  System.Linq.Expressions.Expression.Constant(value, propInfo.PropertyType));
　
                            expr = expr.AndAlso(System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(fieldValueExpr, new[] { paramExpr }));
                        }
                    }
                    else if ((propInfo.PropertyType == typeof(System.DateTime)))
                    {
                        if ((DateTime)value != null && (DateTime)value != DateTime.MinValue)
                        {
                            var fieldValueExpr = System.Linq.Expressions.Expression.Equal(System.Linq.Expressions.Expression.Property(paramExpr, propInfo),
                                                                  System.Linq.Expressions.Expression.Constant(value, propInfo.PropertyType));
                            expr = expr.AndAlso(System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(fieldValueExpr, new[] { paramExpr }));
                        }
                    }
                    else if ((propInfo.PropertyType == typeof(System.Int32)))
                    {
                        //if ((Int32)value != null)
                        //{
                        var fieldValueExpr = System.Linq.Expressions.Expression.Equal(System.Linq.Expressions.Expression.Property(paramExpr, propInfo),
                                                              System.Linq.Expressions.Expression.Constant(value, propInfo.PropertyType));
                        expr = expr.AndAlso(System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(fieldValueExpr, new[] { paramExpr }));
                        //}
                    }
                }
            }
　
            return expr;
        }
　
        #endregion IRepository 成員
　
        #region Private/Protected Function
　
        /// <summary>
        /// 計算所有分頁數量
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="rowCount">總資料量</param>
        /// <param name="Pagger">分頁物件</param>
        /// <param name="result">取出結果資料</param>
        /// <returns></returns>
        protected PaggerList<T> CalcPagger<T>(int rowCount, Pagger Pagger, List<T> result)
        {
            int totalPages = 0;
            if (rowCount > 0)
            {
                if (rowCount % Pagger.PageSize > 0)
                    totalPages = rowCount / Pagger.PageSize + 1;
                else
                    totalPages = rowCount / Pagger.PageSize;
            }
　
            List<OrderbyItem> Orders = new List<OrderbyItem>();
            if (Pagger.ltOrders != null)
            {
                foreach (var item in Pagger.ltOrders)
                {
                    //把前端帶回來的參數清掉
                    //string[] splitArr = item.propertyName.Split('.');
                    Orders.Add(new OrderbyItem()
                    {
                        propertyName = item.propertyName,
                        ascending = item.ascending
                    });
                }
            }
　
            PaggerList<T> PaggerList = new PaggerList<T>()
            {
                Result = result,
                TotalCount = rowCount,
                PageSize = Pagger.PageSize,
                CurrentPage = Pagger.CurrentPage,
                TotalPage = totalPages,
                ltOrders = Orders,
            };
　
            return PaggerList;
        }
　
        /// <summary>
        /// 計算所有分頁數量
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="rowCount">總資料量</param>
        /// <param name="Pagger">分頁物件</param>
        /// <param name="result">取出結果資料</param>
        /// <returns></returns>
        protected PaggerTable CalcPagger<T>(int rowCount, Pagger Pagger, DataTable result)
        {
            int totalPages = 0;
            if (rowCount > 0)
            {
                if (rowCount % Pagger.PageSize > 0)
                    totalPages = rowCount / Pagger.PageSize + 1;
                else
                    totalPages = rowCount / Pagger.PageSize;
            }
　
            List<OrderbyItem> Orders = new List<OrderbyItem>();
            foreach (var item in Pagger.ltOrders)
                Orders.Add(new OrderbyItem() { propertyName = item.propertyName, ascending = item.ascending });
　
            PaggerTable PaggerTable = new PaggerTable()
            {
                Result = result,
                TotalCount = rowCount,
                PageSize = Pagger.PageSize,
                CurrentPage = Pagger.CurrentPage,
                TotalPage = totalPages,
                ltOrders = Orders,
            };
　
            return PaggerTable;
        }
　
        /// <summary>
        /// 計算所有分頁數量
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="rowCount">總資料量</param>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="result">取出結果資料</param>
        /// <returns></returns>
        public PaggerList<T> CombindPaggerTable<T>(int PageSize, int CurrentPage, List<List<T>> results)
        {
            List<T> UnionResult = results.SelectMany(v => v).Distinct().ToList<T>();
　
            int rowCount = UnionResult.Count;
　
            //單頁資料
            List<T> PageResult = UnionResult
                .Skip((CurrentPage - 1) * PageSize) //從第N筆開始
                .Take(PageSize).ToList<T>(); //共取回N筆
　
            int totalPages = 0;
            if (rowCount > 0)
            {
                if (rowCount % PageSize > 0)
                    totalPages = rowCount / PageSize + 1;
                else
                    totalPages = rowCount / PageSize;
            }
　
            PaggerList<T> PaggerList = new PaggerList<T>()
            {
                Result = PageResult,
                TotalCount = rowCount,
                PageSize = PageSize,
                CurrentPage = CurrentPage,
                TotalPage = totalPages
            };
　
            return PaggerList;
        }
　
        /// <summary>
        /// 計算所有分頁數量
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="rowCount">總資料量</param>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="result">取出結果資料</param>
        /// <returns></returns>
        public PaggerList<T> CombindPaggerTable<T>(Pagger pagger, List<List<T>> results)
        {
            List<T> UnionResult = results.SelectMany(v => v).Distinct().ToList<T>();
　
            int rowCount = UnionResult.Count;
　
            //單頁資料
            List<T> PageResult = UnionResult
                .Skip((pagger.CurrentPage - 1) * pagger.PageSize) //從第N筆開始
                .Take(pagger.PageSize).ToList<T>(); //共取回N筆
　
            int totalPages = 0;
            if (rowCount > 0)
            {
                if (rowCount % pagger.PageSize > 0)
                    totalPages = rowCount / pagger.PageSize + 1;
                else
                    totalPages = rowCount / pagger.PageSize;
            }
　
            PaggerList<T> PaggerList = new PaggerList<T>()
            {
                Result = PageResult,
                TotalCount = rowCount,
                PageSize = pagger.PageSize,
                CurrentPage = pagger.CurrentPage,
                TotalPage = totalPages,
                ltOrders = pagger.ltOrders
            };
　
            return PaggerList;
        }
　
　
        /// <summary>
        /// 計算所有分頁數量
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="rowCount">總資料量</param>
        /// <param name="PageSize">單頁大小</param>
        /// <param name="CurrentPage">頁碼</param>
        /// <param name="result">取出結果資料</param>
        /// <returns></returns>
        public KendoGrid<T> CombindPaggerTable<T>(KendoGridRequest request, List<T> result)
        {
            result.Distinct();
            int rowCount = result.Count();
　
            //單頁資料
            List<T> pageResult = result
                .Skip((request.Page - 1) * request.PageSize) //從第N筆開始
                .Take(request.PageSize).ToList<T>(); //共取回N筆
　
            int totalPages = 0;
            if (rowCount > 0)
            {
                if (rowCount % request.PageSize > 0)
                    totalPages = rowCount / request.PageSize + 1;
                else
                    totalPages = rowCount / request.PageSize;
            }
　
            return new KendoGrid<T>(request,pageResult,rowCount);
        }
　
        /// <summary>
        /// 轉換為Count數量專用的HQL
        /// </summary>
        /// <param name="PgrHQL">分頁專用HQL</param>
        /// <param name="Orderby">排序HQL字串</param>
        /// <returns></returns>
        protected string ConvertToCntsHQLString(string PgrHQL, string Orderby = null)
        {
            // select
            // {{{
            //  ..... column
            // }}}
            // FROM table
　
            Regex rgx = new Regex(@"{{{[\s\S]+}}}");
            Match m = rgx.Match(PgrHQL);
            if (m.Success)
            {
                PgrHQL = PgrHQL.Replace(m.Value, " count(*) ");
                var orderby = PgrHQL.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);
                if (orderby > 0)
                {
                    PgrHQL = PgrHQL.Substring(0, orderby);
                }
            }
            if (Orderby != null)
            {
                //不需要Order by
                PgrHQL = PgrHQL.Replace(Orderby, string.Empty);
            }
            return PgrHQL;
        }
　
        /// <summary>
        /// 轉換為HQL
        /// </summary>
        /// <param name="PgrHQL">分頁專用HQL</param>
        /// <returns></returns>
        protected string ConvertToHQLString(string PgrHQL)
        {
            PgrHQL = PgrHQL.Replace("{{{", "").Replace("}}}", "");
            return PgrHQL;
        }
　
        private bool CheckNullObject<T>(T t)
        {
            try
            {
                Type EntityType = t.GetType();
                //check tree prop
                int idx = 0;
                foreach (PropertyInfo prop in EntityType.GetProperties())
                {
                    var val = prop.GetValue(t);
                    idx++;
                    if (idx == 2) break;
                }
            }
            catch
            {
                return true;
            }
            return false;
        }
　
        protected string CulturePostfix
        {
            get { return Web.GetCulturePostfix(i18n.GetCulture().ToString()); }
        }
　
        #endregion Private/Protected Function
    }
}