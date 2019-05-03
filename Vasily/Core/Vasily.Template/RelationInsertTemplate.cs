using System;
using System.Collections.Generic;
using System.Text;

namespace Vasily.Core
{
    public class RelationInsertTemplate
    {
        public string InsertRule(SqlRelationModel model)
        {
            
            var temp_model = model.ModelWithoutPrimary();
            temp_model.ResetMembers(model.Tables);

            InsertTemplate insert = new InsertTemplate();
            string sql = insert.Insert(temp_model);
            model.ClearFilter();
            return sql;

        }

        public string InsertSource(SqlRelationModel parameter_model)
        {
            parameter_model.UseDefaultFilter();
            return InsertRule(parameter_model);
        }

        public string InsertTable(SqlRelationModel parameter_model)
        {
            return InsertRule(parameter_model);
        }
    }
}
