using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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
            RequestType = VasilyRequestType.Normal;
        }

        public DapperWrapper UseType(VasilyRequestType type)
        {
            RequestType = type;
            return this;
        }
    }
    public class DapperWrapper<T> : DapperWrapper
    {
        public DapperWrapper(string key) : base(key, key) { }
        public DapperWrapper(string writter, string reader) : base(writter, reader) { }

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
            return Reader.ExecuteScalar<int>(Sql<T>.Repeate, instance) > 0;
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


    }

    public class DapperWrapper<T, R, C1> : DapperWrapper<T>
    {
        public DapperWrapper(string key) : base(key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {

        }

        private int Table_Execute(string sql, int start = 0, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = start; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1>.TableConditions[i], parameters[i]);
            }
            return Reader.Execute(sql, dynamicParams);
        }

        #region 用实体类进行查询(待开发)
        public IEnumerable<T> Srouce_GetEntities(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1>.SourceConditions[i + 1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1>.GetFromSource, dynamicParams);
            return GetEntitiesByIn(range);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public IEnumerable<T> Table_Gets(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1>.TableConditions[i+1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1>.GetFromTable, dynamicParams);
            return GetEntitiesByIn(range);
        }
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public int Table_Update(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1>.DeleteFromTable, 0, parameters);
        }
        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public int Table_Delete(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1>.DeleteFromTable, 1, parameters);
        }
        /// <summary>
        /// 增加关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public int Table_Insert(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1>.AddFromTable, 0, parameters);
        }
        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public T Table_Get(params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1>.TableConditions[i + 1], parameters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1>.GetFromTable, dynamicParams);
            return GetEntityByIn(range);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2> : DapperWrapper<T>
    {
        public DapperWrapper(string key) : base(key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {

        }

        private int Table_Execute(string sql, int start = 0, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = start; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2>.TableConditions[i], parameters[i]);
            }
            return Reader.Execute(sql, dynamicParams);
        }

        #region 用实体类进行查询(待开发)
        public IEnumerable<T> Srouce_GetEntities(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2>.SourceConditions[i + 1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2>.GetFromSource, dynamicParams);
            return GetEntitiesByIn(range);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public IEnumerable<T> Table_Gets(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2>.TableConditions[i + 1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2>.GetFromTable, dynamicParams);
            return GetEntitiesByIn(range);
        }
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public int Table_Update(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2>.DeleteFromTable, 0, parameters);
        }
        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public int Table_Delete(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2>.DeleteFromTable, 1, parameters);
        }
        /// <summary>
        /// 增加关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public int Table_Insert(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2>.AddFromTable, 0, parameters);
        }
        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public T Table_Get(params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2>.TableConditions[i], parameters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2>.GetFromTable, dynamicParams);
            return GetEntityByIn(range);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2, C3> : DapperWrapper<T>
    {
        public DapperWrapper(string key) : base(key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {

        }

        private int Table_Execute(string sql, int start = 0, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = start; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3>.TableConditions[i], parameters[i]);
            }
            return Reader.Execute(sql, dynamicParams);
        }

        #region 用实体类进行查询(待开发)
        public IEnumerable<T> Srouce_GetEntities(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3>.SourceConditions[i + 1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3>.GetFromSource, dynamicParams);
            return GetEntitiesByIn(range);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public IEnumerable<T> Table_Gets(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3>.TableConditions[i+1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3>.GetFromTable, dynamicParams);
            return GetEntitiesByIn(range);
        }
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public int Table_Update(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3>.DeleteFromTable, 0, parameters);
        }
        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public int Table_Delete(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3>.DeleteFromTable, 1, parameters);
        }
        /// <summary>
        /// 增加关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public int Table_Insert(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3>.AddFromTable, 0, parameters);
        }
        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public T Table_Get(params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3>.TableConditions[i+1], parameters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3>.GetFromTable, dynamicParams);
            return GetEntityByIn(range);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2, C3, C4> : DapperWrapper<T>
    {
        public DapperWrapper(string key) : base(key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {

        }

        private int Table_Execute(string sql, int start = 0, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = start; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4>.TableConditions[i], parameters[i]);
            }
            return Reader.Execute(sql, dynamicParams);
        }

        #region 用实体类进行查询(待开发)
        public IEnumerable<T> Srouce_GetEntities(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4>.SourceConditions[i + 1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4>.GetFromSource, dynamicParams);
            return GetEntitiesByIn(range);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public IEnumerable<T> Table_Gets(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4>.TableConditions[i+1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4>.GetFromTable, dynamicParams);
            return GetEntitiesByIn(range);
        }
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public int Table_Update(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4>.DeleteFromTable, 0, parameters);
        }
        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public int Table_Delete(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4>.DeleteFromTable, 1, parameters);
        }
        /// <summary>
        /// 增加关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public int Table_Insert(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4>.AddFromTable, 0, parameters);
        }
        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public T Table_Get(params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4>.TableConditions[i+1], parameters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4>.GetFromTable, dynamicParams);
            return GetEntityByIn(range);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2, C3, C4, C5> : DapperWrapper<T>
    {
        public DapperWrapper(string key) : base(key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {

        }

        private int Table_Execute(string sql, int start = 0, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = start; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4, C5>.TableConditions[i], parameters[i]);
            }
            return Reader.Execute(sql, dynamicParams);
        }

        #region 用实体类进行查询(待开发)
        public IEnumerable<T> Srouce_GetEntities(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4, C5>.SourceConditions[i + 1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4, C5>.GetFromSource, dynamicParams);
            return GetEntitiesByIn(range);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public IEnumerable<T> Table_Gets(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4, C5>.TableConditions[i+1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4, C5>.GetFromTable, dynamicParams);
            return GetEntitiesByIn(range);
        }
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public int Table_Update(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4, C5>.DeleteFromTable, 0, parameters);
        }
        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public int Table_Delete(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4, C5>.DeleteFromTable, 1, parameters);
        }
        /// <summary>
        /// 增加关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public int Table_Insert(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4, C5>.AddFromTable, 0, parameters);
        }
        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public T Table_Get(params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4, C5>.TableConditions[i+1], parameters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4, C5>.GetFromTable, dynamicParams);
            return GetEntityByIn(range);
        }
        #endregion


    }

    public class DapperWrapper<T, R, C1, C2, C3, C4, C5, C6> : DapperWrapper<T>
    {
        public DapperWrapper(string key) : base(key)
        {

        }
        public DapperWrapper(string writter, string reader) : base(writter, reader)
        {

        }

        private int Table_Execute(string sql, int start = 0, params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = start; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.TableConditions[i], parameters[i]);
            }
            return Reader.Execute(sql, dynamicParams);
        }

        #region 用实体类进行查询(待开发)
        public IEnumerable<T> Srouce_GetEntities(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.SourceConditions[i + 1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.GetFromSource, dynamicParams);
            return GetEntitiesByIn(range);
        }
        #endregion

        #region 不知道实体类信息
        /// <summary>
        /// 直接查询到实体类
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第三个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1 </param>
        /// <returns></returns>
        public IEnumerable<T> Table_Gets(params object[] paramters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < paramters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.TableConditions[i+1], paramters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.GetFromTable, dynamicParams);
            return GetEntitiesByIn(range);
        }
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="paramters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型），set t=@t where c1=@c1</param>
        /// <returns></returns>
        public int Table_Update(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.DeleteFromTable, 0, parameters);
        }
        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public int Table_Delete(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.DeleteFromTable, 1, parameters);
        }
        /// <summary>
        /// 增加关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第1个类型起<T,R,C1>的T,C1,详见F12泛型类型</param>
        /// <returns></returns>
        public int Table_Insert(params object[] parameters)
        {
            return Table_Execute(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.AddFromTable, 0, parameters);
        }
        /// <summary>
        /// 获取关系
        /// </summary>
        /// <param name="parameters">参数顺序（泛型类型参数从第3个类型起<T,R,C1>的C1,详见F12泛型类型），where c1=@c1</param>
        /// <returns></returns>
        public T Table_Get(params object[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            for (int i = 0; i < parameters.Length; i += 1)
            {
                dynamicParams.Add(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.TableConditions[i+1], parameters[i]);
            }
            var range = Reader.Query<int>(RelationSql<T, R, C1, C2, C3, C4, C5, C6>.GetFromTable, dynamicParams);
            return GetEntityByIn(range);
        }
        #endregion


    }

    public enum VasilyRequestType
    {
        Complete = 0,
        Normal = 1
    }
}
