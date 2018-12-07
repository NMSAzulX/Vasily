using System;
using System.Collections.Generic;
using System.Text;

namespace Vasily.Core
{
    public class RelationSelectTemplate
    {
        public string SelectEntitesTable(SqlRelationModel model)
        {
            SelectTemplate template = new SelectTemplate();
            StringBuilder sql = new StringBuilder();

            sql.Append(template.SelectAll(model.EntityModel));
            sql.Append(JoinRule(model, model.Tables));
            return sql.ToString();

        }
        public string SelectEntitesSource(SqlRelationModel model)
        {
            SelectTemplate template = new SelectTemplate();
            StringBuilder sql = new StringBuilder();
            model.UseDefaultFilter();
            sql.Append(template.SelectAll(model.EntityModel));
            sql.Append(JoinRule(model, model.Sources));
            model.ClearFilter();
            return sql.ToString();

        }
        public string SelectCountTable(SqlRelationModel model)
        {
            CountTemplate template = new CountTemplate();
            StringBuilder sql = new StringBuilder();

            sql.Append(template.SelectCount(model.EntityModel));
            sql.Append(JoinRule(model, model.Tables));
            return sql.ToString();
        }
        public string SelectCountSource(SqlRelationModel model)
        {
            CountTemplate template = new CountTemplate();
            StringBuilder sql = new StringBuilder();
            model.UseDefaultFilter();
            sql.Append(template.SelectCount(model.EntityModel));
            sql.Append(JoinRule(model, model.Sources));
            model.ClearFilter();
            return sql.ToString();
        }

        /// <summary>
        /// AS `V_SRC_TA` INNER JOIN B AS `V_SRC_TB` ON `V_SRC_TA`.ID = `V_SRC_TB`.ID.....
        /// </summary>
        /// <param name="_parameters">成员集合</param>
        /// <returns>返回条件查询SQL</returns>
        public string JoinRule(SqlRelationModel model,string[] conditions)
        {

            string source_table = model.EntityModel.TableName;
            StringBuilder sql = new StringBuilder(16 + source_table.Length);
            string join_inner_table = $"{model.Left}V_{source_table}_TA{model.Right}";
            string join_outter_table = $"{model.Left}V_{model.TableName}_TB{model.Right}";
            sql.Append($" AS {join_inner_table} INNER JOIN {model.Left}{model.TableName}{model.Right} AS {join_outter_table} ON ");
            sql.Append($"{join_inner_table}.{model.Left}{model.PreSource}{model.Right}={join_outter_table}.{model.Left}{model.ColFunction(model.Tables[0])}{model.Right}");
            for (int i = 1; i < conditions.Length; i+=1)
            {
                sql.Append($" AND {join_outter_table}.");
                sql.Append(model.Left);
                    if (model.ColFunction != null)
                    {
                        sql.Append(model.ColFunction(model.Tables[i]));
                    }
                    else
                    {
                        sql.Append(model.Tables[i]);
                    }
                    sql.Append(model.Right);

                    sql.Append("=@");
                    if (model.FilterFunction != null)
                    {
                        sql.Append(model.FilterFunction(model.Tables[i]));
                    }
                    else
                    {
                        sql.Append(model.Tables[i]);
                    }
            }
            return sql.ToString();
        }


    }
}

