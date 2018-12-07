using System.Text;

namespace Vasily.Core
{
    public class InsertTemplate
    {
        /// <summary>
        /// 根据model信息生成 INSERT INTO [TableName] ([member1],[member2]...) VALUES (@member1,@member2...)
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>插入字符串的结果</returns>
        public string Insert(SqlModel parameter_model)
        {
            var model = parameter_model;
            if (!parameter_model.PrimaryManually)
            {
                model = parameter_model.ModelWithoutPrimary();
                model.FilterFunction = parameter_model.FilterFunction;
            }

            StringBuilder pre_str = new StringBuilder(20);
            StringBuilder aft_str = new StringBuilder(20);
            pre_str.Append(" (");
            aft_str.Append('(');
            foreach (var item in model.Members)
            {
                pre_str.Append(model.Left);

                if (model.ColFunction!=null)
                {
                    pre_str.Append(model.ColFunction(item));
                }
                else
                {
                    pre_str.Append(item);
                }

                pre_str.Append(model.Right);
                pre_str.Append(',');

                aft_str.Append('@');
                if (model.FilterFunction != null)
                {
                    aft_str.Append(model.FilterFunction(item));
                }
                else
                {
                    aft_str.Append(item);
                }
                aft_str.Append(',');
            }
            pre_str.Length -= 1;
            aft_str.Length -= 1;
            pre_str.Append(')');
            aft_str.Append(')');

            StringBuilder sql = new StringBuilder(40);
            sql.Append("INSERT INTO ");
            sql.Append(model.Left);
            sql.Append(model.TableName);
            sql.Append(model.Right);
            sql.Append(pre_str);
            sql.Append("VALUES");
            sql.Append(aft_str);
            return sql.ToString();
        }
    }
}
