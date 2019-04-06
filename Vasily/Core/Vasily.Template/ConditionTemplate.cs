using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vasily.Core
{
    public class ConditionTemplate
    {
        /// <summary>
        /// 根据model信息生成 [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有委托处理的Model</param>
        /// <param name="conditions">需要匹配的成员集合</param>
        /// <returns>条件字符串结果</returns>
        public string Condition(SqlModel model, params string[] conditions)
        {
            if (conditions==null)
            {
                return string.Empty;
            }
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < conditions.Length; i += 1)
            {
                sql.Append(model.Left);
                if (model.ColFunction != null)
                {
                    sql.Append(model.ColFunction(conditions[i]));
                }
                else
                {
                    sql.Append(conditions[i]);
                }
                sql.Append(model.Right);

                sql.Append("=@");
                if (model.FilterFunction != null)
                {
                    sql.Append(model.FilterFunction(conditions[i]));
                }
                else
                {
                    sql.Append(conditions[i]);
                }
                sql.Append(" AND ");
            }
            sql.Length -= 5;
            return sql.ToString();
        }
        public string Condition(SqlModel model, IEnumerable<string> conditions)
        {

            StringBuilder sql = new StringBuilder();
            foreach (var item in conditions)
            {
                sql.Append(model.Left);
                if (model.ColFunction != null)
                {
                    sql.Append(model.ColFunction(item));
                }
                else
                {
                    sql.Append(item);
                }
                sql.Append(model.Right);

                sql.Append("=@");
                if (model.FilterFunction != null)
                {
                    sql.Append(model.FilterFunction(item));
                }
                else
                {
                    sql.Append(item);
                }
                sql.Append(" AND ");
            }
            if (sql.Length>0)
            {
                sql.Length -= 5;
            }
            return sql.ToString();
        }

    }
}
