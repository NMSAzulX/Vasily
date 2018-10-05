using System;
using Vasily.Core;

namespace Vasily
{
    public class RelationAttribute : Attribute
    {
        public Type RelationType;
        public string ColumnName;
        
        public RelationAttribute(Type type,string column_name=null)
        {
            RelationType = type;
            if (column_name==null)
            {
                var info = (new AttrOperator(type)).Member<PrimaryKeyAttribute>();
                if (info==null)
                {
                    throw new NullReferenceException($"{type.Name}类型中，主键不存在！");
                }
                else
                {
                    column_name = info.Name;
                }
            }
            ColumnName = column_name ?? throw new NullReferenceException($"不完整的映射关系，需要您制定该标签的第二个参数，如果为空则默认视为主键。");
        }
    }
}
