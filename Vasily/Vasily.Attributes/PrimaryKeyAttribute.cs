using System;

namespace Vasily
{
    public class PrimaryKeyAttribute : Attribute
    {
        public bool IsManually { get; set; }
        public PrimaryKeyAttribute()
        {

        }
        public PrimaryKeyAttribute(bool shut = false)
        {
            IsManually = shut;
        }
    }
}
