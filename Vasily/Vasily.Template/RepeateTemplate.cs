﻿using System.Text;
using Vasily.Model;

namespace Vasily.Core
{
    public class RepeateTemplate
    {



        /// <summary>
        /// 根据model信息生成 SELECT COUNT(*) FROM [TableName] WHERE [Member1]=@Member1 AND [Member2]=@Member2 ....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查重字符串结果</returns>
        public string RepeateCount(SqlModel model)
        {
            StringBuilder sql = new StringBuilder(40);
            sql.Append("SELECT COUNT(*) FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);

            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, model.Members));

            return sql.ToString();
        }

        // <summary>
        /// 根据model信息生成 SELECT [primary] FROM [TableName] WHERE [Member1]=@Member1 AND [Member2]=@Member2 ....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查重字符串结果</returns>
        public string RepeateId(SqlModel model)
        {
            StringBuilder sql = new StringBuilder(40);
            sql.Append("SELECT ");
            sql.Append(model.Left);
            sql.Append(model.PrimaryKey);
            sql.Append(model.Right);
            sql.Append(" FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);

            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, model.Members));

            return sql.ToString();
        }

        // <summary>
        /// 根据model信息生成 SELECT * FROM [TableName] WHERE [Member1]=@Member1 AND [Member2]=@Member2 ....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查重字符串结果</returns>
        public string RepeateEntities(SqlModel model)
        {
            StringBuilder sql = new StringBuilder(40);
            sql.Append("SELECT * FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);

            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, model.Members));

            return sql.ToString();
        }
    }
}
