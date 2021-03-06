﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DataBase;
using System.Linq.Expressions;
using System.Collections;
using Common;
using System.Diagnostics;
using EntityFramework.Caching;
using EntityFramework.Extensions;
using System.Transactions;

namespace DataBase
{
    /// <summary>
    /// 数据操作基本实现类，公用实现方法
    /// by 小新 update 2016-12-10
    /// </summary>
    /// <typeparam name="T">具体操作的实体模型</typeparam>
    public abstract class RepositoryBase<T> where T : class
    {
        #region 固定公用帮助，含事务

        #region 上下文 DbContext,private不向外公开其属性
        //private DbContext context = null;//  EFContextFactory.GetCurrentDbContext();

        /// <summary>
        /// 数据上下文--->根据DataBase实体模型名称进行更改
        /// </summary>
        public DbContext Context
        {
            get
            {
                var context = EFContextFactory.GetHttpContextDbContext();
                context.Configuration.ValidateOnSaveEnabled = false;
                context.Configuration.AutoDetectChangesEnabled = false;
                return context;
            }
        }
        //public void Dispose()
        //{
        //    context.Dispose();
        //    context = null;
        //}
        /// <summary>
        /// 公用泛型处理属性
        /// 注:所有泛型操作的基础,总体不跟踪实体状态
        /// </summary>
        private DbQuery<T> dbSet
        {
            get { return this.Context.Set<T>().AsNoTracking(); }
        }
        #endregion

        #region 缓存
        public static CachePolicy CacheOptionsH = new CachePolicy() { SlidingExpiration = TimeSpan.FromHours(2) };
        public static CachePolicy CacheOptionsM = new CachePolicy() { SlidingExpiration = TimeSpan.FromHours(10) };
        public static CachePolicy CacheOptionsS = new CachePolicy() { SlidingExpiration = TimeSpan.FromSeconds(30) };
        private List<T> cache;
        /// <summary>
        /// 缓存List
        /// </summary>
        public List<T> Cache
        {
            get
            {
                if (cache == null)
                {
                    var name = typeof(T).Name;
                    return this.dbSet.FromCache(CacheOptionsH, tags: new string[] { name }).ToList();
                }
                return cache;
            }
        }
        /// <summary>
        /// 让缓存过期
        /// </summary>
        public void Expire()
        {
            var name = typeof(T).Name;
            CacheManager.Current.Expire(name);
        }
        #endregion

        #region 事务---数据库打开连接的方式,在同一线程只有一个上下文的情况不使用
        /// <summary>
        /// 事务
        /// </summary>
        private DbTransaction _transaction = null;
        /// <summary>
        /// 开始事务, 这种方式在同一个上下文时，使用不方便，后面的提交，回滚，_transaction不会置空，
        /// 下次再开启事务时就开不成功，因为_transaction一直非空，回滚时，报事务已关闭，不能操作
        /// </summary>
        private DbTransaction Transaction
        {
            get
            {
                if (this._transaction == null)
                {
                    var connection = this.Context.Database.Connection;
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    this._transaction = connection.BeginTransaction();
                }
                return this._transaction;
            }
            set { this._transaction = value; }
        }
        /// <summary>
        /// 事务状态
        /// </summary>
        private bool Committed { get; set; }
        /// <summary>
        /// 异步锁定
        /// </summary>
        private readonly object sync = new object();
        /// <summary>
        /// 提交事务
        /// </summary>
        private void Commit()
        {
            if (!Committed)
            {
                lock (sync)
                {
                    this.Context.SaveChanges();
                    if (this._transaction != null)
                        _transaction.Commit();
                }
                Committed = true;
            }
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        private void Rollback()
        {
            Committed = false;
            if (this._transaction != null)
                this._transaction.Rollback();
        }
        #endregion

        #region 在EF中使用数据库的事务DbContextTransaction,比较简单
        /// <summary>
        /// 调用时using(var tran=DB.Member_Info.BeginTransaction)  提交：tran.Commit()   回滚：tran.RollBack()
        /// </summary>
        public System.Transactions.TransactionScope BeginTransaction
        {
            get
            {
                //var tran = this.Context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                TransactionOptions transactionOption = new TransactionOptions();
                //设置事务隔离级别
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                // 设置事务超时时间为60秒
                transactionOption.Timeout = new TimeSpan(0, 0, 60);
                var tran = new System.Transactions.TransactionScope(TransactionScopeOption.Required, transactionOption);
                return tran;
            }
        }
        /// <summary>
        /// 清空跟踪的实体对象（当事务过程中抛异常时用）
        /// 因为 当事务回滚时，实体中查询出的对象已被修改，但数据库里回滚了，这样就不一致了，
        /// 暂时没找到更好的解决办法，只能清空DbContext，以后查的数据才与数据库一致
        /// </summary>
        public void ClearDbContext()
        {
            Stopwatch sw = new Stopwatch();  //计时器
            sw.Start();
            var count = this.Context.ChangeTracker.Entries().Count();
            ClearState(this.Context.ChangeTracker.Entries());
            sw.Stop();
            LogHelper.SQL("清除实体跟踪数量:" + count + " ，执行时间：" + sw.Elapsed);
        }
        /// <summary>
        /// 清除上下文的跟踪状态
        /// </summary>
        /// <param name="list"></param>
        private void ClearState(IEnumerable<DbEntityEntry> list)
        {
            try
            {
                if (list == null) return;
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        item.State = System.Data.Entity.EntityState.Detached;
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("ClearState出错：" + WebTools.getFinalException(e));
                throw;
            }
        }
        #endregion

        #region EF
        public DbQuery<T> Include(string name)
        {
            Context.Configuration.LazyLoadingEnabled = false;
            Context.Configuration.ProxyCreationEnabled = false;
            return this.dbSet.Include(name);
        }
        #endregion

        #endregion

        #region 新增
        /// <summary>
        /// 单模型新增
        /// </summary>
        /// <param name="entity">数据模型</param>
        /// <returns></returns>
        public virtual bool Insert(T entity1, T entity2 = null, T entity3 = null, T entity4 = null)
        {
            List<T> list = new List<T>();
            list.Add(entity1);
            if (entity2 != null) list.Add(entity2);
            if (entity3 != null) list.Add(entity3);
            if (entity4 != null) list.Add(entity4);
            return Insert(list) > 0;
        }
        /// <summary>
        /// 多模型新增 含事务
        /// </summary>
        /// <param name="entitys">数据模型集合</param>
        /// <returns></returns>
        public virtual int Insert(List<T> entitys, int count = 0)
        {
            return Insert<T>(entitys, count);
        }

        /// <summary>
        /// 增加多模型数据，指定独立模型集合
        /// </summary>
        public virtual int Insert<T1>(List<T1> entitys, int count = 0) where T1 : class
        {
            List<DbEntityEntry> E = new List<DbEntityEntry>();
            int r = 0;
            try
            {
                foreach (var item in entitys)
                {
                    var entry = this.Context.Entry(item);
                    entry.State = EntityState.Added;
                    E.Add(entry);
                }
                r = this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                if (count < 1)
                {
                    // 清除整体上下文的跟踪状态,然后再试一次，若再失败，就不再试了
                    ClearDbContext();
                    LogHelper.Error("添加失败：" + WebTools.getFinalException(e));
                    // 再试一次
                    return Insert(entitys, count + 1);
                }
                else
                {
                    LogHelper.Error("添加出错2：" + WebTools.getFinalException(e));
                    throw;
                }
            }
            finally
            {
                //取消跟踪状态
                ClearState(E);
            }
            return r;
            #region 原来的写法
            //try
            //{
            //    if (entitys == null || entitys.Count == 0) return 0;
            //    this.Context.Set<T1>().Local.Clear();
            //    foreach (var item in entitys)
            //    {
            //        this.Context.Set<T1>().Add(item);
            //    }
            //    return this.Context.SaveChanges();
            //}
            //catch (Exception e)
            //{
            //    throw;
            //}
            #endregion 
        }
        #endregion

        #region 更新
        /// <summary>
        /// 单模型更新
        /// </summary>
        /// <param name="entity">数据模型</param>
        /// <returns></returns>
        public virtual bool Update(T entity1, T entity2 = null, T entity3 = null, T entity4 = null)
        {
            List<T> list = new List<T>();
            list.Add(entity1);
            if (entity2 != null) list.Add(entity2);
            if (entity3 != null) list.Add(entity3);
            if (entity4 != null) list.Add(entity4);
            return Update(list) > 0;
        }
        /// <summary>
        /// 单模型更新
        /// </summary>
        /// <param name="list">数据模型</param>
        /// <returns></returns>
        public virtual int Update(List<T> list, int count = 0)
        {
            List<DbEntityEntry> E = new List<DbEntityEntry>();
            int r = 0;
            try
            {
                foreach (var item in list)
                {
                    var entry = this.Context.Entry(item);
                    entry.State = EntityState.Modified;
                    E.Add(entry);
                }
                r = this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                if (count < 1)
                {
                    // 清除整体上下文的跟踪状态,然后再试一次，若再失败，就不再试了
                    ClearDbContext();
                    LogHelper.Error("更新失败：" + WebTools.getFinalException(e));
                    // 再试一次
                    return Update(list, count + 1);
                }
                else
                {
                    LogHelper.Error("更新出错2：" + WebTools.getFinalException(e));
                    throw;
                }
            }
            finally
            {
                //取消跟踪状态
                ClearState(E);
            }
            return r;
        }
        /// <summary>
        /// 更新模型记录，如不存在进行添加操作
        /// </summary>
        public virtual bool SaveOrUpdate(T entity, bool isEdit)
        {
            try
            {
                return isEdit ? Update(entity) : Insert(entity);
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion

        #region 删除

        /// <summary>
        /// 删除一条或多条模型记录，含事务
        /// </summary>
        public virtual int Delete(Expression<Func<T, bool>> predicate = null, int count = 0)
        {
            int rows = 0;
            List<DbEntityEntry> E = new List<DbEntityEntry>();   //记录跟踪状态的对象集合
            try
            {
                IQueryable<T> list = Where(predicate);
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        var entry = this.Context.Entry(item);
                        entry.State = EntityState.Deleted;
                        E.Add(entry);
                    }
                    rows = this.Context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                if (count < 1)
                {
                    // 清除整体上下文的跟踪状态,然后再试一次，若再失败，就不再试了
                    ClearDbContext();
                    LogHelper.Error("删除出错：" + WebTools.getFinalException(e));
                    // 再试一次
                    return Delete(predicate, count + 1);
                }
                else
                {
                    LogHelper.Error("删除出错2：" + WebTools.getFinalException(e));
                    throw;
                }
            }
            finally
            {
                //取消跟踪状态
                ClearState(E);
            }
            return rows;
        }

        /// <summary>
        /// 使用原始SQL语句,含事务处理
        /// </summary>
        public virtual int Delete(string sql, params DbParameter[] para)
        {
            try
            {
                return this.Context.Database.ExecuteSqlCommand(sql, para);
            }
            catch
            {
                throw;
            }
        }
        public virtual int Delete(T t, int count = 0)
        {
            List<T> list = new List<T>();
            list.Add(t);
            return Delete<T>(list, count);
        }
        /// <summary>
        /// 批量删除数据，当前模型
        /// </summary>
        public virtual int Delete(List<T> list, int count = 0)
        {
            return Delete<T>(list, count);
        }
        /// <summary>
        /// 批量删除数据，自定义模型
        /// </summary>
        public virtual int Delete<T1>(List<T1> list, int count = 0) where T1 : class
        {
            int rows = 0;
            List<DbEntityEntry> E = new List<DbEntityEntry>();   //记录跟踪状态的对象集合
            try
            {
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        var entry = this.Context.Entry(item);
                        entry.State = EntityState.Deleted;
                        E.Add(entry);
                    }
                    rows = this.Context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                if (count < 1)
                {
                    // 清除整体上下文的跟踪状态,然后再试一次，若再失败，就不再试了
                    ClearDbContext();
                    LogHelper.Error("删除出错：" + WebTools.getFinalException(e));
                    // 再试一次
                    return Delete(list, count + 1);
                }
                else
                {
                    LogHelper.Error("删除出错2：" + WebTools.getFinalException(e));
                    throw;
                }
            }
            finally
            {
                //取消跟踪状态
                ClearState(E);
            }
            return rows;
            #region 以前的写法
            //try
            //{
            //    if (t == null || t.Count == 0) return 0;
            //    foreach (var item in t)
            //    {
            //        this.Context.Set<T1>().Remove(item);
            //    }
            //    return this.Context.SaveChanges();
            //}
            //catch (Exception e) { throw; }
            #endregion
        }

        #endregion

        #region 查询
        public DbRawSqlQuery<N> SqlQuery<N>(string sql)
        {
            return Context.Database.SqlQuery<N>(sql);
        }
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> FindList()
        {
            try
            {
                return this.dbSet.AsQueryable<T>();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 根据拉姆达表达式查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> FindList(Expression<Func<T, bool>> predicate)
        {
            try
            {
                if (predicate == null)
                {
                    return FindList();
                }
                return this.dbSet.Where(predicate);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 查询单模型对象
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T FindEntity(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return dbSet.FirstOrDefault(predicate);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 通过主键查找第一个
        /// </summary>
        /// <param name="parameters">主键值</param>
        /// <returns>查找结果</returns>
        public T FindEntity(params object[] parameters)
        {
            var model = this.Context.Set<T>().Find(parameters);
            if (model != null)
            {
                //清除跟踪状态
                var e = this.Context.Entry(model);
                if (e != null)
                {
                    e.State = EntityState.Detached;
                }
            }
            return model;
        }
        /// <summary>
        /// 根据T-SQL语句查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public virtual List<T> FindListBySql(string strSql)
        {
            return Context.Database.SqlQuery<T>(strSql).ToList<T>();
        }
        /// <summary>
        /// 根据T-SQL语句查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        public virtual List<T> FindListBySql(string strSql, DbParameter[] dbParameter)
        {
            return Context.Database.SqlQuery<T>(strSql, dbParameter).ToList<T>();
        }
        /// <summary>
        /// 根据T-SQL语句查询，返回Json数据
        /// 多用复杂的SQL查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        public virtual string FindSqlToJson(string strSql)
        {
            return this.Context.Database.SqlQueryForJson(strSql);
        }
        /// <summary>
        /// 根据T-SQL语句查询，返回Dynamic数据
        /// 多用复杂的SQL查询
        /// </summary>
        /// <param name="strsql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public virtual dynamic FindSqlToDynamic(string strsql, params DbParameter[] para)
        {
            try
            {
                return this.Context.Database.SqlQueryForDynamic(strsql, para);
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region lamada 表达式 Linq命名方法
        /// <summary>
        /// 根据拉姆达表达式查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate = null)
        {
            return FindList(predicate);
        }
        public virtual List<T> ToList(Expression<Func<T, bool>> predicate = null)
        {
            return FindList(predicate).ToList();
        }
        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Any(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return this.dbSet.Any();
            }
            return this.dbSet.Any(predicate);
        }
        /// <summary>
        /// 查询条数Count
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null) predicate = a => true;
            return this.dbSet.Count(predicate);
        }
        #endregion

        #region 分页操作
        /// <summary>
        /// 待自定义分页函数，使用必须重写，指定数据模型
        /// </summary>
        public virtual IList<T1> PageByListSql<T1>(string sql, IList<DbParameter> parameters, Common.PageCollection page)
        {
            return null;
        }
        /// <summary>
        /// 待自定义分页函数，使用必须重写，
        /// </summary>
        public virtual IList<T> PageByListSql(string sql, IList<DbParameter> parameters, Common.PageCollection page)
        {
            return null;
        }

        /// <summary>
        /// 对IQueryable对象进行分页逻辑处理，过滤、查询项、排序对IQueryable操作
        /// </summary>
        /// <param name="t">Iqueryable</param>
        /// <param name="index">当前页</param>
        /// <param name="PageSize">每页显示多少条</param>
        /// <returns>当前IQueryable to List的对象</returns>
        public virtual Common.PageInfo<T> Query(IQueryable<T> query, int index, int PageSize)
        {
            if (index < 1)
            {
                index = 1;
            }
            if (PageSize <= 0)
            {
                PageSize = 20;
            }
            int count = query.Count();

            int maxpage = count / PageSize;

            if (count % PageSize > 0)
            {
                maxpage++;
            }
            if (index > maxpage)
            {
                index = maxpage;
            }
            if (count > 0)
                query = query.Skip((index - 1) * PageSize).Take(PageSize);
            return new Common.PageInfo<T>(index, PageSize, count, query.ToList());
        }
        /// <summary>
        /// 通用EF分页，默认显示20条记录
        /// </summary>
        /// <typeparam name="TEntity">实体模型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <param name="index">当前页</param>
        /// <param name="pageSize">显示条数</param>
        /// <param name="where">过滤条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="selector">结果集合</param>
        /// <param name="isAsc">排序方向true正序 false倒序</param>
        /// <returns>自定义实体集合</returns>
        public virtual Common.PageInfo<object> Query<TEntity, TOrderBy>
            (int index, int pageSize,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>,
            List<object>> selector,
            bool isAsc)
            where TEntity : class
        {
            if (index < 1)
            {
                index = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            IQueryable<TEntity> query = this.Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }
            int count = query.Count();

            int maxpage = count / pageSize;

            if (count % pageSize > 0)
            {
                maxpage++;
            }
            if (index > maxpage)
            {
                index = maxpage;
            }

            if (orderby != null)
            {
                query = isAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (count > 0)
                query = query.Skip((index - 1) * pageSize).Take(pageSize);
            //返回结果为null，返回所有字段
            if (selector == null)
                return new Common.PageInfo<object>(index, pageSize, count, query.ToList<object>());
            return new Common.PageInfo<object>(index, pageSize, count, selector(query).ToList());
        }
        /// <summary>
        /// 普通SQL查询分页方法 SqlServer版本
        /// </summary>
        /// <param name="index">当前页</param>
        /// <param name="pageSize">显示行数</param>
        /// <param name="tableName">表名/视图</param>
        /// <param name="field">获取项</param>
        /// <param name="filter">过滤条件</param>
        /// <param name="orderby">排序字段+排序方向</param>
        /// <param name="group">分组字段</param>
        /// <returns>结果集</returns>
        public virtual Common.PageInfo Query(int index, int pageSize, string tableName, string field, string filter, string orderby, string group, params DbParameter[] para)
        {
            //执行分页算法
            if (index <= 0)
                index = 1;
            int start = (index - 1) * pageSize;
            if (start > 0)
                start -= 1;
            else
                start = 0;
            int end = index * pageSize;

            #region 查询逻辑
            string logicSql = "SELECT";
            //查询项
            if (!string.IsNullOrEmpty(field))
            {
                logicSql += " " + field;
            }
            else
            {
                logicSql += " *";
            }
            logicSql += " FROM (" + tableName + " ) where";
            //过滤条件
            if (!string.IsNullOrEmpty(filter))
            {
                logicSql += " " + filter;
            }
            else
            {
                filter = " 1=1";
                logicSql += "  1=1";
            }
            //分组
            if (!string.IsNullOrEmpty(group))
            {
                logicSql += " group by " + group;
            }
            //排序
            if (!string.IsNullOrEmpty(orderby))
            {
                logicSql += " order by " + orderby;
            }
            #endregion

            //获取当前条件下数据总条数
            int count = this.Context.Database.SqlQuery(typeof(int), "select count(*) from (" + tableName + ") where " + filter, para).Cast<int>().FirstOrDefault();
            string sql = "select c.* from (select b.* from (select a.*,Row_number() over(order by getdate()) as rn from (" + logicSql + ") a)b where b.rn>=" + start + ") c where c.rn<" + end;
            var list = ExecuteSqlQuery(sql, para) as IEnumerable;
            if (list != null)
                return new Common.PageInfo(index, pageSize, count, list.Cast<object>().ToList());
            return new Common.PageInfo(index, pageSize, count, new { });
        }

        /// <summary>
        /// 最简单的SQL分页
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="pageSize">显示行数</param>
        /// <param name="sql">纯SQL语句</param>
        /// <returns></returns>
        public virtual Common.PageInfo Query(int index, int pageSize, string sql, params DbParameter[] para)
        {
            return this.Query(index, pageSize, sql, null, null, null, null, para);
        }
        /// <summary>
        /// 多表联合分页算法
        /// </summary>
        public virtual Common.PageInfo Query(IQueryable query, int index, int PageSize)
        {
            var enumerable = (query as System.Collections.IEnumerable).Cast<object>();
            if (index < 1)
            {
                index = 1;
            }
            if (PageSize <= 0)
            {
                PageSize = 20;
            }

            int count = enumerable.Count();

            int maxpage = count / PageSize;

            if (count % PageSize > 0)
            {
                maxpage++;
            }
            if (index > maxpage)
            {
                index = maxpage;
            }
            if (count > 0)
                enumerable = enumerable.Skip((index - 1) * PageSize).Take(PageSize);
            return new Common.PageInfo(index, PageSize, count, Common.JsonConverter.JsonClass(enumerable.ToList()));
        }
        #endregion

        #region 存在验证操作
        /// <summary>
        /// 验证当前条件是否存在相同项
        /// </summary>
        public virtual bool IsExist(Expression<Func<T, bool>> predicate)
        {
            var entry = this.dbSet.Where(predicate);
            return (entry.Any());
        }

        /// <summary>
        /// 根据SQL验证实体对象是否存在
        /// </summary>
        public virtual bool IsExist(string sql, params DbParameter[] para)
        {
            IEnumerable result = this.Context.Database.SqlQuery(typeof(int), sql, para);

            if (result.GetEnumerator().Current == null || result.GetEnumerator().Current.ToString() == "0")
                return false;
            return true;
        }

        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行返回影响行数的存储过程
        /// </summary>
        /// <param name="procname">过程名称</param>
        /// <param name="parameter">参数对象</param>
        /// <returns></returns>
        public virtual object ExecuteProc(string procname, params DbParameter[] parameter)
        {
            try
            {
                return ExecuteSqlCommand(procname, parameter);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 执行返回结果集的存储过程
        /// </summary>
        /// <param name="procname">过程名称</param>
        /// <param name="parameter">参数对象</param>
        /// <returns></returns>
        public virtual object ExecuteQueryProc(string procname, params DbParameter[] parameter)
        {
            try
            {
                return this.Context.Database.SqlFunctionForDynamic(procname, parameter);
            }
            catch
            {
                throw;
            }
        }
        public virtual object ExecuteSqlCommand(string procname)
        {
            try
            {
                return this.Context.Database.ExecuteSqlCommand(procname);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region ADO.NET增删改查方法
        /// <summary>
        /// 执行增删改方法,含事务处理
        /// </summary>
        public virtual object ExecuteSqlCommand(string sql, params DbParameter[] para)
        {
            try
            {
                return this.Context.Database.ExecuteSqlCommand(sql, para);
            }
            catch
            {
                throw;
            }

        }
        /// <summary>
        /// 执行多条SQL，增删改方法,含事务处理
        /// </summary>
        public virtual object ExecuteSqlCommand(Dictionary<string, object> sqllist)
        {
            try
            {
                int rows = 0;
                IEnumerator<KeyValuePair<string, object>> enumerator = sqllist.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    rows += this.Context.Database.ExecuteSqlCommand(enumerator.Current.Key, enumerator.Current.Value);
                }
                return rows;
            }
            catch
            {
                throw;
            }

        }
        /// <summary>
        /// 执行查询方法,返回动态类，接收使用var，遍历时使用dynamic类型
        /// </summary>
        public virtual object ExecuteSqlQuery(string sql, params DbParameter[] para)
        {
            try
            {
                return this.Context.Database.SqlQueryForDynamic(sql, para);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
