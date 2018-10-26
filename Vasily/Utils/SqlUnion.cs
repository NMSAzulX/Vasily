using System;
using System.Collections.Generic;
using System.Text;
using Vasily;

namespace System
{
    public class SqlUnion<T>
    {
        public static string Union(string source,params string[] tables)
        {
            if (tables.Length==0)
            {
                return source;
            }
            string source_table = MakerModel<T>.TableName;
            StringBuilder result = new StringBuilder(source.Length * tables.Length);
            for (int i = 0; i < tables.Length-1; i+=1)
            {
                result.Append(source.Replace(source_table, tables[i])).Append(" UNION ");
            }
            result.Append(source.Replace(source_table, tables[tables.Length - 1]));
            return result.ToString();
        }  
    }
}
