using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vasily;

namespace System
{
    public class VasilyRunner
    {
        public static ConcurrentDictionary<string, Type> RelationExtentsionTyps;

        static VasilyRunner()
        {
            RelationExtentsionTyps = new ConcurrentDictionary<string, Type>();
        }
        /// <summary>
        /// 开局必须调用的函数
        /// </summary>
        /// <param name="interfaceName">如果自己有特殊接口，那么可以写自己的接口名</param>
        public static void Run(params string[] interfaceNames)
        {
            if (interfaceNames.Length==0)
            {
                interfaceNames = new string[] { "IVasilyNormal", "IVasilyRelation" };
            }
            List<Type> types = new List<Type>();
            Assembly assmbly = Assembly.GetEntryAssembly();
            if (assmbly == null) { return; }
            IEnumerator<Type> typeCollection = assmbly.ExportedTypes.GetEnumerator();
            Type temp_Type = null;
            while (typeCollection.MoveNext())
            {
                temp_Type = typeCollection.Current;
                if (temp_Type.IsClass && !temp_Type.IsAbstract)
                {
                    var temp_Name = temp_Type.Name.Split('-')[0];
                    if (!RelationExtentsionTyps.ContainsKey(temp_Name))
                    {
                        RelationExtentsionTyps[temp_Name] = temp_Type;
                    }
                    for (int i = 0; i < interfaceNames.Length; i+=1)
                    {
                        if (temp_Type.GetInterface(interfaceNames[i]) != null)
                        {
                            types.Add(temp_Type);
                        }
                    }
                    
                }
            }
            Parallel.ForEach(types, (element) =>
            {
                SqlPackage package = new SqlPackage(element);
            });
        }

    }
}
