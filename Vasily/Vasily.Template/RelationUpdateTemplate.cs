using Vasily.Model;

namespace Vasily
{
    public class RelationUpdateTemplate
    {
        public string UpdateRule(SqlRelationModel model,params string[] conditions)
        {
            UpdateTemplate template = new UpdateTemplate();
            var temp_model = model.CopyInstance();
            temp_model.FilterFunction = model.FilterFunction;
            temp_model.ResetMembers(model.PreTable);
            string sql = template.UpdateWithCondition(temp_model, conditions);
            model.ClearFilter();
            return sql;
        }
        public string UpdateTable(SqlRelationModel model)
        {
            return UpdateRule(model, model.AfterTables);
        }
        public string UpdateSource(SqlRelationModel model)
        {
            model.UseDefaultFilter();
            return UpdateRule(model, model.AfterTables);
        }
    }
}
