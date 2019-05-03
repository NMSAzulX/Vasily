using System;

namespace Vasily
{
    public class ColumnAttribute : Attribute
    {
        public string Name;

        public ColumnAttribute(string mapName)
        {
            Name = mapName;
        }
    }
}
