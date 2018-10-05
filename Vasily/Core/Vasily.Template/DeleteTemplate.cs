using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vasily.Standard;

namespace Vasily.Core
{
    public class DeleteTemplate : IDelete
    {
        /// <summary>
        /// 根据model信息生成 DELETE FROM [TableName] WHERE 
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>删除字符串结果</returns>
        public string DeleteByCondition(MakerModel model)
        {
            StringBuilder sql = new StringBuilder(21 + model.TableName.Length);
            sql.Append("DELETE FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);
            sql.Append(" WHERE ");
            return sql.ToString();
        }

        /// <summary>
        /// 根据model信息生成 DELETE FROM [TableName] WHERE [PrimaryKey] =@PrimaryKey
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>删除字符串结果</returns>
        public string DeleteByPrimary(MakerModel model)
        {
            if (model.PrimaryKey != null)
            {
                StringBuilder sql = new StringBuilder(23 + model.TableName.Length + model.PrimaryKey.Length * 2);
                sql.Append(DeleteByCondition(model));
                sql.Append(model.Left);
                sql.Append(model.PrimaryKey);
                sql.Append(model.Right);
                sql.Append("=@");
                sql.Append(model.PrimaryKey);
                return sql.ToString();
            }
            return null;
        }

        /// <summary>
        /// 生成 DELETE FROM [TableName] WHERE [condition1]=@condition1 AND [condition2]=@condition2
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="conditions">需要匹配的成员集合</param>
        /// <returns>删除字符串结果</returns>
        public string DeleteWithCondition(MakerModel model,params MemberInfo[] conditions)
        {
            var select = DeleteByCondition(model);
            StringBuilder sql = new StringBuilder(select);
            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, conditions));
            return sql.ToString();
        }
    }
}
