using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Vasily.Core;

namespace Vasily
{
    public class DapperWrapper
    {
        public IDbConnection Writter;
        public IDbConnection Reader;
        public VasilyRequestType RequestType;
        public DapperWrapper(string writter, string reader)
        {
            Writter = Connector.ReadInitor(writter)();
            Reader = Connector.ReadInitor(reader)();
            RequestType = VasilyRequestType.Complete;
        }

        public DapperWrapper UseType(VasilyRequestType type)
        {
            RequestType = type;
            return this;
        }

    }
    public class DapperWrapper<T> : DapperWrapper
    {
        public static DapperWrapper<T> UseKey(string key)
        {
            return new DapperWrapper<T>(key);
        }
        public static DapperWrapper<T> UseKey(string writter,string reader)
        {
            return new DapperWrapper<T>(writter,reader);
        }
        public DapperWrapper(string key) : base(key, key) { }
        public DapperWrapper(string writter, string reader) : base(writter, reader) { }

        public DapperWrapper<T> Normal { get { RequestType = VasilyRequestType.Normal; return this; } }
        public DapperWrapper<T> Complete { get { RequestType = VasilyRequestType.Complete; return this; } }

        #region 把下面的Complate和Normal方法都封装一下
        public IEnumerable<T> GetEntitiesByIn(params int[] range)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complete_GetByIn(range);
            }
            else
            {
                return Normal_GetByIn(range);
            }
        }
        public IEnumerable<T> GetEntitiesByIn(IEnumerable<int> range)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complete_GetByIn(range.ToArray());
            }
            else
            {
                return Normal_GetByIn(range.ToArray());
            }
        }
        public T GetEntityByIn(params int[] range)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complete_GetByPrimary(range);
            }
            else
            {
                return Normal_GetByPrimary(range);
            }
        }
        public T GetEntityByIn(IEnumerable<int> range)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complete_GetByPrimary(range.ToArray());
            }
            else
            {
                return Normal_GetByPrimary(range.ToArray());
            }
        }
        public IEnumerable<T> GetAll()
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complete_GetAll();
            }
            else
            {
                return Normal_GetAll();
            }
        }
        public T GetByPrimary(object primary)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complete_GetByPrimary(primary);
            }
            else
            {
                return Normal_GetByPrimary(primary);
            }
        }
        public bool UpdateByPrimary(params T[] instances)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complete_UpdateByPrimary(instances);
            }
            else
            {
                return Normal_UpdateByPrimary(instances);
            }
        }
        public int Insert(params T[] instances)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complate_Insert(instances);
            }
            else
            {
                return Normal_Insert(instances);
            }
        }

        public bool NoRepeateInsert(T instance)
        {
            if (!IsRepeat(instance))
            {
                if (RequestType == VasilyRequestType.Complete)
                {
                    return Complate_Insert(instance) > 0;
                }
                else
                {
                    return Normal_Insert(instance) > 0;
                }
            }
            return false;
        }
        /// <summary>
        /// 先查重，如果没有则插入，再根据插入的实体类通过唯一约束找到主键赋值给实体类
        /// </summary>
        /// <typeparam name="S">主键类型</typeparam>
        /// <param name="instance">实体类</param>
        /// <returns></returns>
        public bool SafeInsert(T instance)
        {
            bool result = false;

            if (NoRepeateInsert(instance))
            {
                result = true;
            }

            object obj = Reader.ExecuteScalar(Sql<T>.RepeateId, instance);
            Sql<T>.SetPrimary(instance, obj);

            return result;
        }

        public bool UpdateByPrimary(IEnumerable<T> instances)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complete_UpdateByPrimary(instances);
            }
            else
            {
                return Normal_UpdateByPrimary(instances);
            }
        }
        public int Insert(IEnumerable<T> instances)
        {
            if (RequestType == VasilyRequestType.Complete)
            {
                return Complate_Insert(instances);
            }
            else
            {
                return Normal_Insert(instances);
            }
        }
        #endregion


        #region 完整实体类的SELECT函数
        /// <summary>
        /// 获取表中所有的完整实体类
        /// </summary>
        /// <returns>结果集</returns>
        public IEnumerable<T> Complete_GetAll()
        {
            return Reader.Query<T>(Sql<T>.SelectAll);
        }
        /// <summary>
        /// 根据主键来获取完整的实体类
        /// </summary>
        /// <param name="primary">主键ID</param>
        /// <returns>实体类</returns>
        public T Complete_GetByPrimary(object primary)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add(Sql<T>.Primary, primary);
            return Reader.QuerySingle<T>(Sql<T>.SelectAllByPrimary, dynamicParams);
        }
        /// <summary>
        /// 获取指定范围主键的完整实体类
        /// </summary>
        /// <param name="range">主键范围</param>
        /// <returns>结果集</returns>
        public IEnumerable<T> Complete_GetByIn<S>(params S[] range)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("keys", range);
            return Reader.Query<T>(Sql<T>.SelectAllIn, dynamicParams);
        }


        #endregion

        #region 业务相关实体类的SELECT函数
        /// <summary>
        /// 获取表中所有的业务相关的实体类(带有select_ignore标签的会被排除)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Normal_GetAll()
        {
            return Reader.Query<T>(Sql<T>.Select);
        }
        /// <summary>
        /// 根据主键来获取业务相关的实体类(带有select_ignore标签的会被排除)
        /// </summary>
        /// <param name="primary">主键ID</param>
        /// <returns>实体类</returns>
        public T Normal_GetByPrimary(object primary)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add(Sql<T>.Primary, primary);
            return Reader.QuerySingle<T>(Sql<T>.SelectByPrimary, dynamicParams);
        }
        /// <summary>
        /// 获取指定范围主键的普通实体类
        /// </summary>
        /// <param name="range">主键范围</param>
        /// <returns>结果集</returns>
        public IEnumerable<T> Normal_GetByIn<S>(params S[] range)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("keys", range);
            return Reader.Query<T>(Sql<T>.SelectIn, dynamicParams);
        }
        #endregion


        #region 完整实体类的UPDATE函数
        /// <summary>
        /// 根据主键更新
        /// </summary>
        /// <param name="instance">需要更新的实体类</param>
        /// <returns>更新结果</returns>
        public bool Complete_UpdateByPrimary(params T[] instances)
        {
            return Writter.Execute(Sql<T>.UpdateAllByPrimary, instances) == instances.Length;
        }
        public bool Complete_UpdateByPrimary(IEnumerable<T> instances)
        {
            return Writter.Execute(Sql<T>.UpdateAllByPrimary, instances) == instances.Count();
        }
        #endregion

        #region 业务相关实体类的UPDATE函数
        /// <summary>
        /// 根据主键更新
        /// </summary>
        /// <param name="instance">需要更新的实体类</param>
        /// <returns>更新结果</returns>
        public bool Normal_UpdateByPrimary(params T[] instances)
        {
            return Writter.Execute(Sql<T>.UpdateByPrimary, instances) == instances.Length;
        }
        public bool Normal_UpdateByPrimary(IEnumerable<T> instances)
        {
            return Writter.Execute(Sql<T>.UpdateByPrimary, instances) == instances.Count();
        }
        #endregion


        #region 完整实体类的INSERT函数
        /// <summary>
        /// 插入新节点
        /// </summary>
        /// <param name="instances">实体类</param>
        /// <returns>返回结果</returns>
        public int Complate_Insert(params T[] instances)
        {
            return Writter.Execute(Sql<T>.InsertAll, instances);
        }
        public int Complate_Insert(IEnumerable<T> instances)
        {
            return Writter.Execute(Sql<T>.InsertAll, instances);
        }
        #endregion

        #region 业务相关实体类的INSERT函数
        /// <summary>
        /// 插入新节点
        /// </summary>
        /// <param name="instances">实体类</param>
        /// <returns>返回结果</returns>
        public int Normal_Insert(params T[] instances)
        {
            return Writter.Execute(Sql<T>.Insert, instances);
        }
        public int Normal_Insert(IEnumerable<T> instances)
        {
            return Writter.Execute(Sql<T>.Insert, instances);
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
            return Reader.ExecuteScalar<int>(Sql<T>.RepeateCount, instance) > 0;
        }
        /// <summary>
        /// 通过实体类获取跟其相同唯一约束的集合
        /// </summary>
        /// <param name="instance">实体类</param>
        /// <returns>结果集</returns>
        public IEnumerable<T> GetRepeates(T instance)
        {
            return Reader.Query<T>(Sql<T>.RepeateEntities);
        }
        /// <summary>
        /// 通过实体类获取当前实体的主键
        /// </summary>
        /// <typeparam name="S">主键类型</typeparam>
        /// <param name="instance">实体类</param>
        /// <returns>主键</returns>
        public S GetNoRepeateId<S>(T instance)
        {
            return Reader.ExecuteScalar<S>(Sql<T>.RepeateId, instance);
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
            dynamicParams.Add(Sql<T>.Primary, primary);
            return Writter.Execute(Sql<T>.DeleteByPrimary, dynamicParams) == 1;
        }
        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="primary">主键ID</param>
        /// <returns>更新结果</returns>
        public bool EntitiesDeleteByPrimary(params T[] instances)
        {
            return Writter.Execute(Sql<T>.DeleteByPrimary, instances) == instances.Length;
        }

        public bool EntitiesDeleteByPrimary(IEnumerable<T> instances)
        {
            return Writter.Execute(Sql<T>.DeleteByPrimary, instances) == instances.Count();
        }
        #endregion

        public static implicit operator DapperWrapper<T>(string key)
        {
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
            for (int i = 0; i < _tables.Length-1; i += 1)
            {
                dynamicParams.Add(_tables[i + 1], parameters[i + 1]);
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
            for (int i = 0; i < _sources.Length-1; i += 1)
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
            for (int i = 0; i < _sources.Length-1; i += 1)
            {
                dynamicParams.Add(_sources[i + 1], _emits[i + 1](parameters[i]));
            }
            var range = Reader.Query<int>(sql, dynamicParams);
            return GetEntitiesByIn(range);
        }

        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        internal T SourceGet_Wrapper(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _sources.Length-1; i += 1)
            {
                dynamicParams.Add(_sources[i + 1], _emits[i + 1](parameters[i]));
            }
            var range = Reader.Query<int>(sql, dynamicParams);
            return GetEntityByIn(range);
        }
       
        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        internal IEnumerable<T> TableGets_Wrapper(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _tables.Length-1; i += 1)
            {
                dynamicParams.Add(_tables[i + 1], parameters[i]);
            }
            var range = Reader.Query<int>(sql, dynamicParams);
            return GetEntitiesByIn(range);
        }

        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public T TableGet_Wrapper(string sql, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < _tables.Length-1; i += 1)
            {
                dynamicParams.Add(_tables[i + 1], parameters[i]);
            }
            var range = Reader.Query<int>(sql, dynamicParams);
            return GetEntityByIn(range);
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
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public virtual int SourceUpdate(params object[] parameters)
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
        public virtual int SourceInsert(params object[] parameters)
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
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public virtual int TableUpdate(params object[] parameters)
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
        public virtual int TableInsert(params object[] parameters)
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
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceUpdate(params object[] parameters)
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
        public override int SourceInsert(params object[] parameters)
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
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableUpdate(params object[] parameters)
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
        public override int TableInsert(params object[] parameters)
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
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceUpdate(params object[] parameters)
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
        public override int SourceInsert(params object[] parameters)
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
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableUpdate(params object[] parameters)
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
        public override int TableInsert(params object[] parameters)
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
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceUpdate(params object[] parameters)
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
        public override int SourceInsert(params object[] parameters)
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
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableUpdate(params object[] parameters)
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
        public override int TableInsert(params object[] parameters)
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
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceUpdate(params object[] parameters)
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
        public override int SourceInsert(params object[] parameters)
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
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableUpdate(params object[] parameters)
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
        public override int TableInsert(params object[] parameters)
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
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceUpdate(params object[] parameters)
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
        public override int SourceInsert(params object[] parameters)
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
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableUpdate(params object[] parameters)
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
        public override int TableInsert(params object[] parameters)
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
        /// 更新操作-直接传实体类
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public override int SourceUpdate(params object[] parameters)
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
        public override int SourceInsert(params object[] parameters)
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
        /// 更新操作-直接传值
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public override int TableUpdate(params object[] parameters)
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
        public override int TableInsert(params object[] parameters)
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
    public enum VasilyRequestType
    {
        Complete = 0,
        Normal = 1
    }
}
