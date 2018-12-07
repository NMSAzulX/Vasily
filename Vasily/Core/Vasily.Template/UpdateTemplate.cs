using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Vasily.Core
{
    public class UpdateTemplate
    {
        /// <summary>
        /// 根据model信息生成 UPDATE [TableName] SET([member1]=@member1,[member2]...=@member2...) WHERE
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>更新字符串结果</returns>
        public string UpdateWhere(SqlModel model)
        {
            var temp_model = model.ModelWithoutPrimary();

            StringBuilder update = new StringBuilder(40);
  
            foreach (var item in temp_model.Members)
            {
                update.Append(temp_model.Left);
                if (temp_model.ColFunction != null)
                {
                    update.Append(temp_model.ColFunction(item));
                }
                else
                {
                    update.Append(item);
                }
                update.Append(temp_model.Right);

                update.Append("=@");
                if (temp_model.FilterFunction!=null)
                {
                    update.Append(temp_model.FilterFunction(item));
                }
                else
                {
                    update.Append(item);
                }
                
                update.Append(',');
            }

            StringBuilder sql = new StringBuilder(60);
            if (update.Length > 0)
            {
                update.Length -= 1;
                sql.Append("UPDATE ");
                sql.Append(temp_model.Left);
                sql.Append(temp_model.TableName);
                sql.Append(temp_model.Right);
                sql.Append(" SET ");
                sql.Append(update);
                sql.Append(" WHERE ");
            }
  
            return sql.ToString();
        }

        /// <summary>
        /// 根据model信息生成 UPDATE [TableName] SET([member1]=@member1,[member2]...=@member2...) WHERE PrimaryKey=@PrimaryKe
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>更新字符串结果</returns>
        public string UpdateByPrimary(SqlModel model)
        {
            if (model.PrimaryKey != null)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(UpdateWhere(model));
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
        /// 根据model信息生成 UPDATE [TableName] SET([member1]=@member1,[member2]...=@member2...) WHERE [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition_models">需要匹配的成员集合</param>
        /// <returns>更新字符串结果</returns>
        public string UpdateWithCondition(SqlModel model, params string[] conditions)
        {
            var select = UpdateWhere(model);
            StringBuilder sql = new StringBuilder(select);
            ConditionTemplate template = new ConditionTemplate();
            sql.Append(template.Condition(model, conditions));
            return sql.ToString();
        }



    }
}
