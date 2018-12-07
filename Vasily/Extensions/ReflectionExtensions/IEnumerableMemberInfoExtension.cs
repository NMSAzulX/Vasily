using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vasily.Extensions
{
    public static class IEnumerableMemberInfoExtension
    {
        public static string[] GetNames(this IEnumerable<MemberInfo> members)
        {
            string[] result = new string[members.Count()];
            int index = 0;
            foreach (var item in members)
            {
                result[index] = item.Name;
                index += 1;
            }
            return result;
        }
    }
}
