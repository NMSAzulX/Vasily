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
                if (info!=null)
                {
                    column_name = info.Name;
                }
                else
                {
                    column_name = null;
                }
            }
            ColumnName = column_name;
        }
    }
}
