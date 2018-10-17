using System.Linq;
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
        public string RepeateCount(MakerModel model)
        {
            StringBuilder sql = new StringBuilder(40);
            sql.Append("SELECT COUNT(*) FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);
            sql.Append(" WHERE ");

            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, model.Members));

            return sql.ToString();
        }

        // <summary>
        /// 根据model信息生成 SELECT [primary] FROM [TableName] WHERE [Member1]=@Member1 AND [Member2]=@Member2 ....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查重字符串结果</returns>
        public string RepeateId(MakerModel model)
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
            sql.Append(" WHERE ");

            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, model.Members));

            return sql.ToString();
        }

        // <summary>
        /// 根据model信息生成 SELECT * FROM [TableName] WHERE [Member1]=@Member1 AND [Member2]=@Member2 ....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查重字符串结果</returns>
        public string RepeateEntities(MakerModel model)
        {
            StringBuilder sql = new StringBuilder(40);
            sql.Append("SELECT * FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);
            sql.Append(" WHERE ");

            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, model.Members));

            return sql.ToString();
        }
    }
}
