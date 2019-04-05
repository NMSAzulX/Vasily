using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vasily
{
    public class SqlLink<T>
    {
        private DapperWrapper<T> _handler;
        private SqlModel _model;
        private string[] _conditions;
        private string _sqlCondition;

        /// <summary>
        /// 静态创建一个Link对象
        /// </summary>
        /// <param name="handler">DapperWrapper对象</param>
        /// <returns></returns>
        public static SqlLink<T> Load(DapperWrapper<T> handler)
        {
            SqlLink<T> instance = new SqlLink<T>();
            instance._handler = handler;
            instance._model = SqlModel<T>.CopyInstance();
            return instance;
        }

        /// <summary>
        /// 设置要操作的字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlLink<T> Fields(params string[] fields)
        {
            _model.ResetMembers(fields);
            return this;
        }


        /// <summary>
        /// 设置要操作的条件
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlLink<T> Conditions(params string[] conditions)
        {
            _conditions = conditions;
            return this;
        }

        public SqlLink<T> Conditions(Func<SqlCondition<T>, SqlCondition<T>> condition)
        {
            return Conditions(condition(new SqlCondition<T>())); ;
        }

        public SqlLink<T> Conditions(SqlCondition<T> condition)
        {
            _sqlCondition = condition.Full; ;
            return this;
        }

        /// <summary>
        /// 查询数据集合
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IEnumerable<S> Gets<S>(object obj)
        {
            string sql = string.Empty;
            if (_conditions!=null)
            {
                sql = SqlTemplate.CustomerSelect(_model,_sqlCondition);
            }
            else
            {
                sql = SqlTemplate.SelectWithCondition(_model,_conditions);
            }
            return _handler.Reader.Query<S>(sql, obj);
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public S Get<S>(object obj)
        {
            string sql = string.Empty;
            if (_conditions != null)
            {
                sql = SqlTemplate.CustomerSelect(_model, _sqlCondition);
            }
            else
            {
                sql = SqlTemplate.SelectWithCondition(_model, _conditions);
            }
            return _handler.Reader.QuerySingle<S>(sql, obj);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Modify(object obj)
        {
            string sql = string.Empty;
            if (_conditions != null)
            {
                sql = SqlTemplate.CustomerUpdate(_model, _sqlCondition);
            }
            else
            {
                sql = SqlTemplate.UpdateWithCondition(_model, _conditions);
            }
            return _handler.Reader.Execute(sql, obj);
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Delete(object obj)
        {
            string sql = string.Empty;
            if (_conditions != null)
            {
                sql = SqlTemplate.CustomerDelete(_model, _sqlCondition);
            }
            else
            {
                sql = SqlTemplate.DeleteWithCondition(_model, _conditions);
            }
            return _handler.Reader.Execute(sql, obj);
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Add(object obj)
        {
            string sql = string.Empty;
            if (_conditions != null)
            {
                sql = SqlTemplate.CustomerInsert(_model);
            }
            return _handler.Reader.Execute(sql, obj);
        }



        /// <summary>
        /// 查重数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Repeate(object obj)
        {
            string sql = SqlTemplate.RepeateCount(_model);
            return _handler.Reader.ExecuteScalar<int>(sql, obj);
        }
    }
}
