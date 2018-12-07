using System;
using Vasily.Core;

namespace Vasily
{
    public class SqlPackage<T> : SqlPackage
    {
        public SqlPackage(string splites = null) : base(typeof(T), splites)
        {

        }
    }

    public class SqlPackage
    {
        public SqlPackage(Type type, string splites = null)
        {
            bool IsNormal = type.GetInterface("IVasilyNormal") != null;
            if (IsNormal)
            {
                NormalAnalysis analysis = new NormalAnalysis(type, splites);
            }
        }
    }
}
