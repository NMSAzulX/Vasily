using System.Reflection;
using System.Text;

namespace Vasily.Core
{
    public class SelectTemplate
    {
        /// <summary>
        /// 根据model信息生成 SELECT * FROM [TableName]
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectAll(SqlModel model)
        {
            StringBuilder sql = new StringBuilder(16 + model.TableName.Length);
            sql.Append("SELECT * FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);
            return sql.ToString();
        }

       


        /// <summary>
        /// 根据model信息生成 SELECT * FROM [TableName] WHERE
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectAllWhere(SqlModel model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(SelectAll(model));
            sql.Append(" WHERE ");
            return sql.ToString();
        }


        



        /// <summary>
        /// 根据model信息生成 SELECT * FROM [TableName] WHERE [PrimaryKey] = @PrimaryKey
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectAllByPrimary(SqlModel model)
        {

            if (model.PrimaryKey != null)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(SelectAllWhere(model));
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
        /// 根据model信息生成 SELECT * FROM [TableName] WHERE [PrimaryKey] IN @keys
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectAllIn(SqlModel model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(SelectAllWhere(model));
            sql.Append(model.Left);
            sql.Append(model.PrimaryKey);
            sql.Append(model.Right);
            sql.Append(" IN @keys");
            return sql.ToString();
        }

        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName]
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string Select(SqlModel model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            foreach (var item in model.Members)
            {
               
                if (model.ColFunction!= null)
                {
                    string sourceName = model.ColFunction(item);
                    if (sourceName!=item)
                    {
                        sql.Append(model.Left);
                        sql.Append(model.ColFunction(item)).Append(" AS ");
                        sql.Append(model.Right);
                    }
                }
               
                sql.Append(model.Left);
                sql.Append(item);
                sql.Append(model.Right);
                sql.Append(",");
            }
            sql.Length -= 1;
            sql.Append(" FROM ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);
            return sql.ToString();
        }

        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE 
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectWhere(SqlModel model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(Select(model));
            sql.Append(" WHERE ");
            return sql.ToString();
        }

        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE [PrimaryKey]=@PrimaryKey
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectByPrimary(SqlModel model)
        {
            if (model.PrimaryKey != null)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(SelectWhere(model));
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
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE [PrimaryKey] IN @keys
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public string SelectIn(SqlModel model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(SelectWhere(model));
            sql.Append(model.Left);
            sql.Append(model.PrimaryKey);
            sql.Append(model.Right);
            sql.Append(" IN @keys");
            return sql.ToString();
        }

        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition_models">需要匹配的成员集合</param>
        /// <returns>查询字符串结果</returns>
        public string SelectWithCondition(SqlModel model, params string[] conditions)
        {
            var select = SelectWhere(model);
            StringBuilder sql = new StringBuilder(select);
            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, conditions));
            return sql.ToString();
        }
    }
}
