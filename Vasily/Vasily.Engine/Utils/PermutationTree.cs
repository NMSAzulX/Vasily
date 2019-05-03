using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vasily.Engine.Utils
{

    public class PermutationTree<T>
    {
        public PermutationTree<T>[] Next;
        public T Value;
        public PermutationTree(T[] elementes)
        {
            Next = new PermutationTree<T>[elementes.Length];
            for (int i = 0; i < Next.Length; i += 1)
            {
                HashSet<T> values = new HashSet<T>(elementes);
                values.Remove(elementes[i]);
                Next[i] = new PermutationTree<T>(values.ToArray());
                Next[i].Value = elementes[i];
            }
        }

        public List<List<T>> A(int deepth, PermutationTree<T> node = null)
        {
            if (node == null)
            {
                node = this;
            }
            List<List<T>> types = new List<List<T>>();
            if (node.Next.Length == 0 || deepth == 0)
            {
                types.Add(new List<T>());
                types[0].Add(node.Value);
            }
            else
            {
                for (int i = 0; i < node.Next.Length; i += 1)
                {
                    var collection = A(deepth - 1, node.Next[i]);

                    for (int j = 0; j < collection.Count; j += 1)
                    {
                        types.Add(new List<T>());

                        if (node.Value != null)
                        {
                            types[i * collection.Count + j].Add(node.Value);
                        }

                        for (int z = 0; z < collection[j].Count; z += 1)
                        {
                            types[i * collection.Count + j].Add(collection[j][z]);
                        }
                    }
                }
            }
            return types;
        }

        public List<List<T>> SumA(int minN, int maxN)
        {
            List<List<T>> types = new List<List<T>>();
            for (int i = minN; i <= maxN; i+=1)
            {
                foreach (var item in A(i))
                {
                    types.Add(item);
                }
            }
            return types;
        }
    }
}
