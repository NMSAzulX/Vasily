using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vasily.Core
{
    public class CountTemplate
    {
        /// <summary>
        /// 根据model信息生成 SELECT COUNT(*) FROM [TableName]
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectCount(SqlModel model)
        {
            StringBuilder sql = new StringBuilder(23 + model.TableName.Length);
            sql.Append("SELECT COUNT(*) FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);
            return sql.ToString();
        }


        /// <summary>
        /// 根据model信息生成 SELECT COUNT(*) FROM [TableName] WHERE
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectCountWhere(SqlModel model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(SelectCount(model));
            sql.Append(" WHERE ");
            return sql.ToString();
        }



        /// <summary>
        /// 根据model信息生成 SELECT COUNT(*) WHERE [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition_models">需要匹配的成员集合</param>
        /// <returns>查询字符串结果</returns>
        public string SelectCountWithCondition(SqlModel model, params string[] conditions)
        {
            var select = SelectCountWhere(model);
            StringBuilder sql = new StringBuilder(select);
            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, conditions));
            return sql.ToString();
        }
    }
}
