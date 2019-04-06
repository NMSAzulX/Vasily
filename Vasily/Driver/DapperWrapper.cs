using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vasily;
using Vasily.Core;

namespace System
{
    public class DapperWrapper
    {
        public static char WR_Char;
        public string[] Tables;
        static DapperWrapper()
        {
            WR_Char = '|';
        }
        public IDbConnection Writter;
        public IDbConnection Reader;

        /// <summary>
        /// 绑定事务 
        /// </summary>
        /// <param name="other">使用other节点的事务</param>
        public void UseTransaction(DapperWrapper other)
        {
            Writter = other.Writter;
            Reader = other.Reader;
            _transcation = other._transcation;

            // DapperWrapper a = new DapperWrapper();
            // DapperWrapper b = new DapperWrapper();
            // a.Transacation((w,r)=>{
            //      int id = a.GetId();
            //      b.UseTransacation(a);
            //      b.Update(id);
            //})；
        }
        public DapperWrapper(string writter, string reader)
        {
            Writter = Connector.WriteInitor(writter)();
            Reader = Connector.ReadInitor(reader)();
            Tables = null;
        }

        public int Sum(IEnumerable<int> indexs)
        {
            if (indexs == null)
            {
                return 0;
            }
            int result = 0;
            foreach (var item in indexs)
            {
                result += item;
            }
            return result;
        }

        protected IDbTransaction _transcation;

        /// <summary>
        /// 事务重试机制
        /// </summary>
        /// <param name="action">事务操作委托</param>
        /// <param name="retry">重试次数</param>
        /// <param name="get_errors">获取指定次数的异常错误</param>
        /// <returns>错误集合</returns>
        public List<Exception> TransactionRetry(Action<IDbConnection, IDbConnection> action, int retry = 1, params int[] get_errors)
        {
            List<Exception> errors = new List<Exception>();
            HashSet<int> dict = new HashSet<int>(get_errors);
            for (int i = 1; i <= retry; i += 1)
            {
                try
                {
                    Transaction(action);
                    return errors;
                }
                catch (Exception ex)
                {
                    if (get_errors.Length == 0)
                    {
                        errors.Add(ex);
                    }
                    else if (dict.Contains(i))
                    {
                        errors.Add(ex);
                    }
                }
            }
            return errors;
        }

        /// <summary>
        /// 事务重试机制
        /// </summary>
        /// <param name="action">事务操作委托</param>
        /// <param name="retry">重试次数</param>
        /// <param name="predicate">每次异常获取的逻辑</param>
        /// <returns>错误集合</returns>
        public List<Exception> TransactionRetry(Action<IDbConnection, IDbConnection> action, int retry = 1, Predicate<int> predicate = null)
        {
            List<Exception> errors = new List<Exception>();
            if (predicate != null)
            {
                for (int i = 1; i <= retry; i += 1)
                {
                    try
                    {
                        Transaction(action);
                        return errors;
                    }
                    catch (Exception ex)
                    {
                        if (predicate(i))
                        {
                            errors.Add(ex);
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i <= retry; i += 1)
                {
                    try
                    {
                        Transaction(action);
                        return errors;
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex);
                    }
                }
            }

            return errors;
        }

        public void Transaction(Action<IDbConnection, IDbConnection> action)
        {
            //开始事务
            using (_transcation = Writter.BeginTransaction())
            {
                try
                { 
                    action?.Invoke(Reader, Writter);
                    _transcation.Commit();
                }
                catch (Exception ex)
                {
                    //出现异常，事务Rollback
                    _transcation.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            _transcation = null;
        }

    }
    public class DapperWrapper<T> : DapperWrapper
    {
        public static DapperWrapper<T> UseKey(string key)
        {
            return new DapperWrapper<T>(key);
        }
        public static DapperWrapper<T> UseKey(string writter, string reader)
        {
            return new DapperWrapper<T>(writter, reader);
        }
        public DapperWrapper(string key) : base(key, key) { }
        public DapperWrapper(string writter, string reader) : base(writter, reader) { }

        public int Count { get { return GetCount(); } }


        public DapperWrapper<T> UseUnion(params string[] tables)
        {
            Tables = tables;
            return this;
        }
        public DapperWrapper<T> UseTransaction()
        {
            _transcation = Reader.BeginTransaction();
            return this;
        }

        private SqlCollectionType _collection_type;
        public DapperWrapper<T> UseCollection(SqlCollectionType type,params string[] tables)
        {
            _collection_type = type;
            Tables = tables;
            return this;
        }


        private string GetRealSqlString(SqlConditionBase condition, string query)
        {
            if (Tables != null)
            {
                string result = SqlCollection<T>.Collection(_collection_type, query + condition==null?"": condition.Full, Tables);
                Tables = null;
                _collection_type = SqlCollectionType.None;
                return result;
            }
            return query + condition.Full;
        }
        private string GetRealSqlString(string query)
        {
            if (Tables != null)
            {
                string result = SqlCollection<T>.Collection(_collection_type, query, Tables);
                Tables = null;
                _collection_type = SqlCollectionType.None;
                return result;
            }
            return query;
        }
        /// <summary>
        /// 根据条件查询单个实体类
        /// </summary>
        /// <param name="condition">条件查询</param>
        /// <param name="instance">条件参数化实例</param>
        /// <returns></returns>
        public T Get(SqlCondition<T> condition, object instance)
        {
            return Reader.QueryFirst<T>(GetRealSqlString(condition, SqlEntity<T>.SelectAllWhere), instance);

        }
        public T Get(VasilyProtocal<T> condition)
        {
            return Reader.QueryFirst<T>(GetRealSqlString(condition, SqlEntity<T>.SelectAllWhere),condition.Instance);
        }
        public T Get(Func<SqlCondition<T>, SqlCondition<T>> condition, object instance)
        {
            return Get(condition(new SqlCondition<T>()), instance);
        }
        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <param name="condition">条件查询</param>
        /// <param name="instance">更新参数化实例</param>
        /// <returns></returns>
        public int Modify(SqlCondition<T> condition, object instance)
        {
            string sql = GetRealSqlString(condition, SqlEntity<T>.UpdateAllWhere);
            return Writter.Execute(sql, instance, transaction: _transcation);
        }
        public int Modify(Func<SqlCondition<T>, SqlCondition<T>> condition, object instance)
        {
            return Modify(condition(new SqlCondition<T>()), instance);
        }
        public int Modify(VasilyProtocal<T> condition)
        {
            string sql = GetRealSqlString(condition, SqlEntity<T>.UpdateAllWhere);
            return Writter.Execute(sql, condition.Instance, transaction: _transcation);
        }
        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="condition">条件查询</param>
        /// <param name="instance">删除参数化实例</param>
        /// <returns></returns>
        public int Delete(SqlCondition<T> condition, object instance, ForceDelete flag = ForceDelete.No)
        {
            if (flag == ForceDelete.No)
            {
                return Writter.Execute(SqlEntity<T>.DeleteWhere + condition.Full, instance, transaction: _transcation);
            }
            else
            {
                string temp = GetRealSqlString(condition, SqlEntity<T>.DeleteWhere);
                return Writter.Execute(temp, instance, transaction: _transcation);
            }
        }
        public int Delete(VasilyProtocal<T> condition, ForceDelete flag = ForceDelete.No)
        {
            if (flag == ForceDelete.No)
            {
                return Writter.Execute(SqlEntity<T>.DeleteWhere + condition.Full, condition.Instance, transaction: _transcation);
            }
            else
            {
                return Writter.Execute(GetRealSqlString(condition, SqlEntity<T>.DeleteWhere), condition.Instance, transaction: _transcation);
            }
        }

        public int Delete(Func<SqlCondition<T>, SqlCondition<T>> condition, object instance, ForceDelete flag = ForceDelete.No)
        {
            return Delete(condition(new SqlCondition<T>()), instance, flag);
        }
        /// <summary>
        /// 根据条件批量查询
        /// </summary>
        /// <param name="condition">条件查询</param>
        /// <param name="instance">查询参数化实例</param>
        /// <returns></returns>
        public IEnumerable<T> Gets(SqlCondition<T> condition, object instance)
        {
            return Reader.Query<T>(GetRealSqlString(condition, SqlEntity<T>.SelectAllWhere), instance);
        }
        public IEnumerable<T> Gets(VasilyProtocal<T> condition)
        {
            return Reader.Query<T>(GetRealSqlString(condition, SqlEntity<T>.SelectAllWhere), condition.Instance);

        }
        public IEnumerable<T> Gets(Func<SqlCondition<T>, SqlCondition<T>> condition, object instance)
        {
            return Gets(condition(new SqlCondition<T>()), instance);
        }
        /// <summary>
        /// 根据条件批量查询数量
        /// </summary>
        /// <param name="condition">条件查询</param>
        /// <param name="instance">查询参数化实例</param>
        /// <returns></returns>

        public int CountWithCondition(SqlCondition<T> condition, object instance)
        {
            if (Tables==null)
            {
                return Reader.ExecuteScalar<int>(SqlEntity<T>.SelectCountWhere + condition.Full, instance);
            }
            else {
                string tempSql = GetRealSqlString(condition, SqlEntity<T>.SelectCountWhere);
                var temp = Reader.Query<int>(tempSql, instance);
                Tables = null;
                return Sum(temp);
            }
        }
        public int CountWithCondition(VasilyProtocal<T> condition)
        {
            if (Tables == null)
            {
                return Reader.ExecuteScalar<int>(SqlEntity<T>.SelectCountWhere + condition.Full, condition.Instance);
            }
            else
            {
                string tempSql = GetRealSqlString(condition, SqlEntity<T>.SelectCountWhere);
                var temp = Reader.Query<int>(tempSql, condition.Instance);
                Tables = null;
                return Sum(temp);
            }
        }

        public int CountWithCondition(Func<SqlCondition<T>, SqlCondition<T>> condition, object instance)
        {
            return CountWithCondition(condition(new SqlCondition<T>()), instance);
        }

        /// <summary>
        /// 返回当前表总数
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {

            if (Tables == null)
            {
                return Reader.ExecuteScalar<int>(SqlEntity<T>.SelectCount);
            }
            else
            {
                string tempSql = GetRealSqlString(SqlEntity<T>.SelectCount);
                var temp = Reader.Query<int>(tempSql);
                Tables = null;
                return Sum(temp);
            }
        }

        #region 把下面的Complate和Normal方法都封装一下
        /// <summary>
        /// 使用where id in (1,2,3)的方式，根据主键来获取对象集合
        /// </summary>
        /// <param name="range">主键数组</param>
        /// <returns></returns>
        public IEnumerable<T> GetsIn(params int[] range)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("keys", range);
            return Reader.Query<T>(SqlEntity<T>.SelectAllIn, dynamicParams);
        }
        /// <summary>
        /// 使用where id in (1,2,3)的方式，根据主键来获取对象集合
        /// </summary>
        /// <param name="range">主键数组</param>
        /// <returns></returns>
        public IEnumerable<T> GetsIn(IEnumerable<int> range)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("keys", range);
            return Reader.Query<T>(SqlEntity<T>.SelectAllIn, dynamicParams);
        }
        /// <summary>
        /// 使用where id in (1,2,3)的方式，根据主键来获取一个对象
        /// </summary>
        /// <param name="range">主键</param>
        /// <returns></returns>
        public T GetIn(int range)
        {
           var dynamicParams = new DynamicParameters();
            dynamicParams.Add(SqlEntity<T>.Primary, range);
            return Reader.QueryFirst<T>(SqlEntity<T>.SelectAllByPrimary, dynamicParams);
        }

        /// <summary>
        /// 获取无条件，整个对象的在数据库的所有数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            string sql = null;
            if (Tables == null)
            {
                sql = SqlEntity<T>.SelectAll;
            }
            else
            {
                sql = GetRealSqlString(SqlEntity<T>.SelectAll);
                Tables = null;
            }
            return Reader.Query<T>(sql);
        }
        /// <summary>
        /// 通过主键获取单个实体
        /// </summary>
        /// <param name="primary">主键</param>
        /// <returns></returns>
        public T GetByPrimary(object primary)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add(SqlEntity<T>.Primary, primary);
            return Reader.QueryFirst<T>(SqlEntity<T>.SelectAllByPrimary, dynamicParams);
        }
        /// <summary>
        /// 更新实体或者实体的集合
        /// </summary>
        /// <param name="instances">实体类</param>
        /// <returns></returns>
        public bool ModifyByPrimary(params T[] instances)
        {
            return Writter.Execute(

                GetRealSqlString(SqlEntity<T>.UpdateAllByPrimary), 
                instances, 
                transaction: _transcation

                ) == instances.Length;
        }
        public bool ModifyByPrimary(IEnumerable<T> instances)
        {
            return Writter.Execute(

                GetRealSqlString(SqlEntity<T>.UpdateAllByPrimary), 
                instances, 
                transaction: _transcation

                ) == instances.Count();
        }
        /// <summary>
        /// 插入实体或者实体的集合
        /// </summary>
        /// <param name="instances">实体类</param>
        /// <returns></returns>
        public int Add(params T[] instances)
        {
            return Writter.Execute(SqlEntity<T>.InsertAll, instances, transaction: _transcation);
        }
        public int Add(IEnumerable<T> instances)
        {
            return Writter.Execute(SqlEntity<T>.InsertAll, instances, transaction: _transcation);
        }


        #endregion



        #region 查重函数
        /// <summary>
        /// 节点查重
        /// </summary>
        /// <param name="instance">实体类条件</param>
        /// <returns>返回结果</returns>
        public bool IsRepeat(T instance)
        {
            if (Tables == null)
            {
                return Reader.ExecuteScalar<int>(SqlEntity<T>.RepeateCount)>0;
            }
            else
            {
                IEnumerable<int> temp = Reader.Query<int>(GetRealSqlString(SqlEntity<T>.RepeateCount), instance);
                Tables = null;
                return Sum(temp) > 0;
            }
        }
        /// <summary>
        /// 通过实体类获取跟其相同唯一约束的集合
        /// </summary>
        /// <param name="instance">实体类</param>
        /// <returns>结果集</returns>
        public IEnumerable<T> GetRepeates(T instance)
        {
            if (Tables == null)
            {
                return Reader.Query<T>(SqlEntity<T>.RepeateEntities);
            }
            else
            {
                var result = Reader.Query<T>(GetRealSqlString(SqlEntity<T>.RepeateEntities));
                Tables = null;
                return result;
            }
        }
        /// <summary>
        /// 通过实体类获取当前实体的主键，注：只有实体类有NoRepeate条件才能用
        /// </summary>
        /// <typeparam name="S">主键类型</typeparam>
        /// <param name="instance">实体类</param>
        /// <returns>主键</returns>
        public S GetNoRepeateId<S>(T instance)
        {
            if (Tables == null)
            {
                return Reader.ExecuteScalar<S>(SqlEntity<T>.RepeateId);
            }
            else
            {
                IEnumerable<S> temp = Reader.Query<S>(GetRealSqlString(SqlEntity<T>.RepeateId), instance);
                Tables = null;
                foreach (var item in temp)
                {
                    if (item != null)
                    {
                        return item;
                    }
                }
                return default(S);
            }
        }
        /// <summary>
        /// 通过查重条件进行不重复插入
        /// </summary>
        /// <param name="instance">实体类</param>
        /// <returns></returns>
        public bool NoRepeateAdd(T instance)
        {
            if (!IsRepeat(instance))
            {
                return Writter.Execute(SqlEntity<T>.InsertAll, instance, transaction: _transcation)>0;
            }
            return false;
        }
        /// <summary>
        /// 先查重，如果没有则插入，再根据插入的实体类通过唯一约束找到主键赋值给实体类
        /// </summary>
        /// <typeparam name="S">主键类型</typeparam>
        /// <param name="instance">实体类</param>
        /// <returns></returns>
        public bool SafeAdd(T instance)
        {
            bool result = false;

            if (NoRepeateAdd(instance))
            {
                result = true;
            }

            object obj = Reader.ExecuteScalar<object>(SqlEntity<T>.RepeateId, instance);
            SqlEntity<T>.SetPrimary(instance, obj);

            return result;
        }
        #endregion

        #region 实体类的DELETE函数
        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="primary">主键ID</param>
        /// <returns>更新结果</returns>
        public bool SingleDeleteByPrimary(object primary)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add(SqlEntity<T>.Primary, primary);
            return Writter.Execute(SqlEntity<T>.DeleteByPrimary, dynamicParams) == 1;
        }
        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="primary">主键ID</param>
        /// <returns>更新结果</returns>
        public bool EntitiesDeleteByPrimary(params T[] instances)
        {
            return Writter.Execute(SqlEntity<T>.DeleteByPrimary, instances, transaction: _transcation) == instances.Length;
        }

        public bool EntitiesDeleteByPrimary(IEnumerable<T> instances)
        {
            return Writter.Execute(SqlEntity<T>.DeleteByPrimary, instances, transaction: _transcation) == instances.Count();
        }
        #endregion

        public static implicit operator DapperWrapper<T>(string key)
        {
            if (key.Contains(WR_Char))
            {
                string[] result = key.Split(WR_Char);
                return new DapperWrapper<T>(result[0].Trim(), result[1].Trim());
            }
            return new DapperWrapper<T>(key);
        }
    }
    public abstract class RelationWrapper<T> : DapperWrapper<T>
    {
        internal MemberGetter[] _emits;
        internal string[] _sources;
        internal string[] _tables;
        public RelationWrapper(string key) : this(key, key)
        {

        }
        public RelationWrapper(string writter, string reader) : base(writter, reader)
        {

        }


        #region 内部函数封装
        internal int TableExecute(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _tables.Length; i += 1)
            {
                dynamicParams.Add(_tables[i], parameters[i]);
            }
            return Reader.Execute(sql, dynamicParams);
        }
        internal int TableAftExecute(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _tables.Length - 1; i += 1)
            {
                dynamicParams.Add(_tables[i + 1], parameters[i]);
            }
            return Reader.Execute(sql, dynamicParams);
        }
        internal int SourceExecute(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _sources.Length; i += 1)
            {
                dynamicParams.Add(_sources[i], _emits[i](parameters[i]));
            }
            return Reader.Execute(sql, dynamicParams);
        }
        internal int SourceAftExecute(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _sources.Length - 1; i += 1)
            {
                dynamicParams.Add(_sources[i + 1], _emits[i + 1](parameters[i]));
            }
            return Reader.Execute(sql, dynamicParams);
        }

        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        internal IEnumerable<T> SourceGets_Wrapper(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _sources.Length - 1; i += 1)
            {
                dynamicParams.Add(_sources[i + 1], _emits[i + 1](parameters[i]));
            }
            return Reader.Query<T>(sql, dynamicParams);
        }

        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        internal T SourceGet_Wrapper(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _sources.Length - 1; i += 1)
            {
                dynamicParams.Add(_sources[i + 1], _emits[i + 1](parameters[i]));
            }
            return Reader.QueryFirst<T>(sql, dynamicParams);
        }

        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        internal IEnumerable<T> TableGets_Wrapper(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _tables.Length - 1; i += 1)
            {
                dynamicParams.Add(_tables[i + 1], parameters[i]);
            }
            return Reader.Query<T>(sql, dynamicParams);
        }

        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        internal T TableGet_Wrapper(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _tables.Length - 1; i += 1)
            {
                dynamicParams.Add(_tables[i + 1], parameters[i]);
            }
            return Reader.QueryFirst<T>(sql, dynamicParams);
        }
        #endregion

        #region 用实体类进行查询
        /// <summary>
        /// 获取集合-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public virtual IEnumerable<T> SourceGets(params object[] parameters)
        {
            return null;
        }

        /// <summary>
        /// 获取集合数量-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public virtual int SourceCount(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public virtual int SourceModify(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public virtual int SourcePreDelete(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public virtual int SourceAftDelete(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 增加关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public virtual int SourceAdd(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 获取关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public virtual T SourceGet(params object[] parameters)
        {
            return default(T);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 查询到实体类-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public virtual IEnumerable<T> TableGets(params object[] parameters)
        {
            return null;
        }
        /// <summary>
        /// 获取集合数量-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public virtual int TableCount(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public virtual int TableModify(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public virtual int TablePreDelete(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public virtual int TableAftDelete(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 增加关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public virtual int TableAdd(params object[] parameters)
        {
            return 0;
        }
        /// <summary>
        /// 获取关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public virtual T TableGet(params object[] parameters)
        {
            return default(T);
        }
        #endregion
    }

    public class DapperWrapper<T, R, C1> : RelationWrapper<T>
    {
        public static implicit operator DapperWrapper<T, R, C1>(string key)
        {
            if (key.Contains(WR_Char))
            {
                string[] result = key.Split(WR_Char);
                return new DapperWrapper<T, R, C1>(result[0].Trim(), result[1].Trim());
            }
            return new DapperWrapper<T, R, C1>(key);
        }
        public new static RelationWrapper<T> UseKey(string key)
        {
            return new DapperWrapper<T, R, C1>(key);
        }
        public new static RelationWrapper<T> UseKey(string writter, string reader)
        {
            return new DapperWrapper<T, R, C1>(writter, reader);
        }
        public DapperWrapper(string key) : this(key, key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {
            _tables = RelationSql<T, R, C1>.TableConditions;
            _sources = RelationSql<T, R, C1>.SourceConditions;
            _emits = RelationSql<T, R, C1>.Getters;
        }

        #region 用实体类进行查询
        /// <summary>
        /// 获取集合-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override IEnumerable<T> SourceGets(params object[] parameters)
        {
            return SourceGets_Wrapper(RelationSql<T, R, C1>.GetFromSource, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int SourceCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1>.CountFromSource, parameters);
        }
        /// <summary>
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceModify(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1>.ModifyFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int SourcePreDelete(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1>.DeletePreFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceAftDelete(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1>.DeleteAftFromSource, parameters);
        }
        /// <summary>
        /// 增加关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int SourceAdd(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1>.AddFromSource, 0, parameters);
        }
        /// <summary>
        /// 获取关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T SourceGet(params object[] parameters)
        {
            return SourceGet_Wrapper(RelationSql<T, R, C1>.GetFromSource, parameters);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 查询到实体类-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public override IEnumerable<T> TableGets(params object[] parameters)
        {
            return TableGets_Wrapper(RelationSql<T, R, C1>.GetFromTable, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1>.CountFromTable, parameters);
        }
        /// <summary>
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableModify(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1>.ModifyFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int TablePreDelete(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1>.DeletePreFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int TableAftDelete(params object[] parameters)
        {
            return TableAftExecute(RelationSql<T, R, C1>.DeleteAftFromTable, parameters);
        }
        /// <summary>
        /// 增加关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int TableAdd(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1>.AddFromTable, parameters);
        }


        /// <summary>
        /// 获取关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T TableGet(params object[] parameters)
        {
            return TableGet_Wrapper(RelationSql<T, R, C1>.GetFromTable, parameters);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2> : RelationWrapper<T>
    {
        public static implicit operator DapperWrapper<T, R, C1, C2>(string key)
        {
            if (key.Contains(WR_Char))
            {
                string[] result = key.Split(WR_Char);
                return new DapperWrapper<T, R, C1, C2>(result[0].Trim(), result[1].Trim());
            }
            return new DapperWrapper<T, R, C1, C2>(key);
        }
        public new static RelationWrapper<T> UseKey(string key)
        {
            return new DapperWrapper<T, R, C1, C2>(key);
        }
        public new static RelationWrapper<T> UseKey(string writter, string reader)
        {
            return new DapperWrapper<T, R, C1, C2>(writter, reader);
        }
        public DapperWrapper(string key) : this(key, key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {
            _tables = RelationSql<T, R, C1, C2>.TableConditions;
            _sources = RelationSql<T, R, C1, C2>.SourceConditions;
            _emits = RelationSql<T, R, C1, C2>.Getters;
        }

        #region 用实体类进行查询
        /// <summary>
        /// 获取集合-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override IEnumerable<T> SourceGets(params object[] parameters)
        {
            return SourceGets_Wrapper(RelationSql<T, R, C1, C2>.GetFromSource, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int SourceCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2>.CountFromSource, parameters);
        }
        /// <summary>
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceModify(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2>.ModifyFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int SourcePreDelete(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2>.DeletePreFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceAftDelete(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2>.DeleteAftFromSource, parameters);
        }
        /// <summary>
        /// 增加关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int SourceAdd(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2>.AddFromSource, 0, parameters);
        }
        /// <summary>
        /// 获取关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T SourceGet(params object[] parameters)
        {
            return SourceGet_Wrapper(RelationSql<T, R, C1, C2>.GetFromSource, parameters);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 查询到实体类-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public override IEnumerable<T> TableGets(params object[] parameters)
        {
            return TableGets_Wrapper(RelationSql<T, R, C1, C2>.GetFromTable, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2>.CountFromTable, parameters);
        }
        /// <summary>
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableModify(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2>.ModifyFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int TablePreDelete(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2>.DeletePreFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int TableAftDelete(params object[] parameters)
        {
            return TableAftExecute(RelationSql<T, R, C1, C2>.DeleteAftFromTable, parameters);
        }
        /// <summary>
        /// 增加关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int TableAdd(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2>.AddFromTable, parameters);
        }
        /// <summary>
        /// 获取关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T TableGet(params object[] parameters)
        {
            return TableGet_Wrapper(RelationSql<T, R, C1, C2>.GetFromTable, parameters);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2, C3> : RelationWrapper<T>
    {
        public static implicit operator DapperWrapper<T, R, C1, C2, C3>(string key)
        {
            if (key.Contains(WR_Char))
            {
                string[] result = key.Split(WR_Char);
                return new DapperWrapper<T, R, C1, C2, C3>(result[0].Trim(), result[1].Trim());
            }
            return new DapperWrapper<T, R, C1, C2, C3>(key);
        }
        public new static RelationWrapper<T> UseKey(string key)
        {
            return new DapperWrapper<T, R, C1, C2, C3>(key);
        }
        public new static RelationWrapper<T> UseKey(string writter, string reader)
        {
            return new DapperWrapper<T, R, C1, C2, C3>(writter, reader);
        }
        public DapperWrapper(string key) : this(key, key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {
            _tables = RelationSql<T, R, C1, C2, C3>.TableConditions;
            _sources = RelationSql<T, R, C1, C2, C3>.SourceConditions;
            _emits = RelationSql<T, R, C1, C2, C3>.Getters;
        }

        #region 用实体类进行查询
        /// <summary>
        /// 获取集合-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override IEnumerable<T> SourceGets(params object[] parameters)
        {
            return SourceGets_Wrapper(RelationSql<T, R, C1, C2, C3>.GetFromSource, parameters);
        }

        /// <summary>
        /// 获取集合数量-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int SourceCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3>.CountFromSource, parameters);
        }

        /// <summary>
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceModify(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3>.ModifyFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int SourcePreDelete(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3>.DeletePreFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceAftDelete(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3>.DeleteAftFromSource, parameters);
        }
        /// <summary>
        /// 增加关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int SourceAdd(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3>.AddFromSource, 0, parameters);
        }
        /// <summary>
        /// 获取关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T SourceGet(params object[] parameters)
        {
            return SourceGet_Wrapper(RelationSql<T, R, C1, C2, C3>.GetFromSource, parameters);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 查询到实体类-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public override IEnumerable<T> TableGets(params object[] parameters)
        {
            return TableGets_Wrapper(RelationSql<T, R, C1, C2, C3>.GetFromTable, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3>.CountFromTable, parameters);
        }
        /// <summary>
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableModify(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3>.ModifyFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int TablePreDelete(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3>.DeletePreFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int TableAftDelete(params object[] parameters)
        {
            return TableAftExecute(RelationSql<T, R, C1, C2, C3>.DeleteAftFromTable, parameters);
        }
        /// <summary>
        /// 增加关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int TableAdd(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3>.AddFromTable, parameters);
        }
        /// <summary>
        /// 获取关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T TableGet(params object[] parameters)
        {
            return TableGet_Wrapper(RelationSql<T, R, C1, C2, C3>.GetFromTable, parameters);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2, C3, C4> : RelationWrapper<T>
    {
        public static implicit operator DapperWrapper<T, R, C1, C2, C3, C4>(string key)
        {
            if (key.Contains(WR_Char))
            {
                string[] result = key.Split(WR_Char);
                return new DapperWrapper<T, R, C1, C2, C3, C4>(result[0].Trim(), result[1].Trim());
            }
            return new DapperWrapper<T, R, C1, C2, C3, C4>(key);
        }
        public new static RelationWrapper<T> UseKey(string key)
        {
            return new DapperWrapper<T, R, C1, C2, C3, C4>(key);
        }
        public new static RelationWrapper<T> UseKey(string writter, string reader)
        {
            return new DapperWrapper<T, R, C1, C2, C3, C4>(writter, reader);
        }
        public DapperWrapper(string key) : this(key, key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {
            _tables = RelationSql<T, R, C1, C2, C3, C4>.TableConditions;
            _sources = RelationSql<T, R, C1, C2, C3, C4>.SourceConditions;
            _emits = RelationSql<T, R, C1, C2, C3, C4>.Getters;
        }

        #region 用实体类进行
        /// <summary>
        /// 获取集合-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override IEnumerable<T> SourceGets(params object[] parameters)
        {
            return SourceGets_Wrapper(RelationSql<T, R, C1, C2, C3, C4>.GetFromSource, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int SourceCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4>.CountFromSource, parameters);
        }
        /// <summary>
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceModify(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4>.ModifyFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int SourcePreDelete(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4>.DeletePreFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceAftDelete(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4>.DeleteAftFromSource, parameters);
        }
        /// <summary>
        /// 增加关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int SourceAdd(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4>.AddFromSource, 0, parameters);
        }
        /// <summary>
        /// 获取关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T SourceGet(params object[] parameters)
        {
            return SourceGet_Wrapper(RelationSql<T, R, C1, C2, C3, C4>.GetFromSource, parameters);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 查询到实体类-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public override IEnumerable<T> TableGets(params object[] parameters)
        {
            return TableGets_Wrapper(RelationSql<T, R, C1, C2, C3, C4>.GetFromTable, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4>.CountFromTable, parameters);
        }
        /// <summary>
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableModify(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4>.ModifyFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int TablePreDelete(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4>.DeletePreFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int TableAftDelete(params object[] parameters)
        {
            return TableAftExecute(RelationSql<T, R, C1, C2, C3, C4>.DeleteAftFromTable, parameters);
        }
        /// <summary>
        /// 增加关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int TableAdd(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4>.AddFromTable, parameters);
        }
        /// <summary>
        /// 获取关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T TableGet(params object[] parameters)
        {
            return TableGet_Wrapper(RelationSql<T, R, C1, C2, C3, C4>.GetFromTable, parameters);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2, C3, C4, C5> : RelationWrapper<T>
    {
        public static implicit operator DapperWrapper<T, R, C1, C2, C3, C4, C5>(string key)
        {
            if (key.Contains(WR_Char))
            {
                string[] result = key.Split(WR_Char);
                return new DapperWrapper<T, R, C1, C2, C3, C4, C5>(result[0].Trim(), result[1].Trim());
            }
            return new DapperWrapper<T, R, C1, C2, C3, C4, C5>(key);
        }
        public new static RelationWrapper<T> UseKey(string key)
        {
            return new DapperWrapper<T, R, C1, C2, C3, C4, C5>(key);
        }
        public new static RelationWrapper<T> UseKey(string writter, string reader)
        {
            return new DapperWrapper<T, R, C1, C2, C3, C4, C5>(writter, reader);
        }
        public DapperWrapper(string key) : this(key, key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {
            _tables = RelationSql<T, R, C1, C2, C3, C4, C5>.TableConditions;
            _sources = RelationSql<T, R, C1, C2, C3, C4, C5>.SourceConditions;
            _emits = RelationSql<T, R, C1, C2, C3, C4, C5>.Getters;
        }

        #region 用实体类进行查询
        /// <summary>
        /// 获取集合-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override IEnumerable<T> SourceGets(params object[] parameters)
        {
            return SourceGets_Wrapper(RelationSql<T, R, C1, C2, C3, C4, C5>.GetFromSource, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int SourceCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.CountFromSource, parameters);
        }
        /// <summary>
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceModify(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.ModifyFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int SourcePreDelete(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.DeletePreFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceAftDelete(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.DeleteAftFromSource, parameters);
        }
        /// <summary>
        /// 增加关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int SourceAdd(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.AddFromSource, 0, parameters);
        }
        /// <summary>
        /// 获取关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T SourceGet(params object[] parameters)
        {
            return SourceGet_Wrapper(RelationSql<T, R, C1, C2, C3, C4, C5>.GetFromSource, parameters);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 查询到实体类-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public override IEnumerable<T> TableGets(params object[] parameters)
        {
            return TableGets_Wrapper(RelationSql<T, R, C1, C2, C3, C4, C5>.GetFromTable, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.CountFromTable, parameters);
        }
        /// <summary>
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableModify(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.ModifyFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int TablePreDelete(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.DeletePreFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int TableAftDelete(params object[] parameters)
        {
            return TableAftExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.DeleteAftFromTable, parameters);
        }
        /// <summary>
        /// 增加关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int TableAdd(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4, C5>.AddFromTable, parameters);
        }
        /// <summary>
        /// 获取关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T TableGet(params object[] parameters)
        {
            return TableGet_Wrapper(RelationSql<T, R, C1, C2, C3, C4, C5>.GetFromTable, parameters);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2, C3, C4, C5, C6> : RelationWrapper<T>
    {
        public static implicit operator DapperWrapper<T, R, C1, C2, C3, C4, C5, C6>(string key)
        {
            if (key.Contains(WR_Char))
            {
                string[] result = key.Split(WR_Char);
                return new DapperWrapper<T, R, C1, C2, C3, C4, C5, C6>(result[0].Trim(), result[1].Trim());
            }
            return new DapperWrapper<T, R, C1, C2, C3, C4, C5, C6>(key);
        }
        public new static RelationWrapper<T> UseKey(string key)
        {
            return new DapperWrapper<T, R, C1, C2, C3, C4, C5, C6>(key);
        }
        public new static RelationWrapper<T> UseKey(string writter, string reader)
        {
            return new DapperWrapper<T, R, C1, C2, C3, C4, C5, C6>(writter, reader);
        }
        public DapperWrapper(string key) : this(key, key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {
            _tables = RelationSql<T, R, C1, C2, C3, C4, C5, C6>.TableConditions;
            _sources = RelationSql<T, R, C1, C2, C3, C4, C5, C6>.SourceConditions;
            _emits = RelationSql<T, R, C1, C2, C3, C4, C5, C6>.Getters;
        }

        #region 用实体类进行查询
        /// <summary>
        /// 获取集合-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override IEnumerable<T> SourceGets(params object[] parameters)
        {
            return SourceGets_Wrapper(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.GetFromSource, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int SourceCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.CountFromSource, parameters);
        }
        /// <summary>
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceModify(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.ModifyFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int SourcePreDelete(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.DeletePreFromSource, parameters);
        }
        /// <summary>
        /// 删除关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceAftDelete(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.DeleteAftFromSource, parameters);
        }
        /// <summary>
        /// 增加关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int SourceAdd(params object[] parameters)
        {
            return SourceExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.AddFromSource, 0, parameters);
        }
        /// <summary>
        /// 获取关系-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T SourceGet(params object[] parameters)
        {
            return SourceGet_Wrapper(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.GetFromSource, parameters);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 查询到实体类-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public override IEnumerable<T> TableGets(params object[] parameters)
        {
            return TableGets_Wrapper(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.GetFromTable, parameters);
        }
        /// <summary>
        /// 获取集合数量-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableCount(params object[] parameters)
        {
            return SourceAftExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.CountFromTable, parameters);
        }
        /// <summary>
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableModify(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.ModifyFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where t=@t</param>
        /// <returns></returns>
        public override int TablePreDelete(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.DeletePreFromTable, parameters);
        }
        /// <summary>
        /// 删除关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int TableAftDelete(params object[] parameters)
        {
            return TableAftExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.DeleteAftFromTable, parameters);
        }
        /// <summary>
        /// 增加关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public override int TableAdd(params object[] parameters)
        {
            return TableExecute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.AddFromTable, parameters);
        }
        /// <summary>
        /// 获取关系-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override T TableGet(params object[] parameters)
        {
            return TableGet_Wrapper(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.GetFromTable, parameters);
        }
        #endregion


    }
}
