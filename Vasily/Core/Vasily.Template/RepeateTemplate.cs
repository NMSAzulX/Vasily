using System.Text;
using Vasily.Standard;

namespace Vasily.Core
{
    public class RepeateTemplate:IRepeate
    {
        /// <summary>
        /// 根据model信息生成 SELECT COUNT(*) FROM [TableName] WHERE [Member1]=@Member1 AND [Member2]=@Member2 ....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查重字符串结果</returns>
        public string Repeate(MakerModel model)
        {
            StringBuilder sql = new StringBuilder(40);
            sql.Append("SELECT COUNT(*) FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);
            sql.Append(" WHERE ");
            foreach (var item in model.Members)
            {
                sql.Append(model.Left);
                if (model.ColFunction != null)
                {
                    sql.Append(model.ColFunction(item));
                }
                else
                {
                    sql.Append(item.Name);
                }
                sql.Append(model.Right);
                sql.Append("=@");
                if (model.FilterFunction != null)
                {
                    sql.Append(model.FilterFunction(item));
                }
                else
                {
                    sql.Append(item.Name);
                }
                sql.Append(" AND ");
            }

            sql.Length -= 5;

            return sql.ToString();
        }
    }
}
