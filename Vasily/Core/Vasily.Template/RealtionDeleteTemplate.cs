using System;
using System.Collections.Generic;
using System.Text;

namespace Vasily.Core
{
    public class RelationDeleteTemplate
    {

        /// <summary>
        /// 获取删除语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string DeletePreRule(SqlRelationModel model, string pre)
        {
            DeleteTemplate delete = new DeleteTemplate();
            string result = delete.DeleteWithCondition(model.RelationModel, pre);
            model.RelationModel.FilterFunction = null;
            return result;
        }
        public string DeletePreSource(SqlRelationModel model)
        {
            model.RelationModel.FilterFunction = (item) => { return model.SourceColumn(item); };
            return DeletePreRule(model, model.PreTable);
        }
        public string DeletePreTable(SqlRelationModel model)
        {
            return DeletePreRule(model, model.PreTable);
        }
        public string DeleteAftRule(SqlRelationModel model, string[] aft)
        {
            DeleteTemplate delete = new DeleteTemplate();
            return delete.DeleteWithCondition(model.RelationModel, aft);
        }
        public string DeleteAftSource(SqlRelationModel model)
        {
            model.RelationModel.FilterFunction = (item) => { return model.SourceColumn(item); };
            return DeleteAftRule(model, model.AfterTables);
        }
        public string DeleteAftTable(SqlRelationModel model)
        {
            return DeleteAftRule(model, model.AfterTables);
        }
    }
}
