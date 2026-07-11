using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace Zoo
{
    public static class EnumerableExtension
    {
        public delegate float GetWeightDelegate<T>(T element);

        public static T WeightSelect<T>(this IEnumerable<T> collection, GetWeightDelegate<T> weightDelegate)
        {
            if (!collection.Any())
            {
                throw new System.Exception("Empty Collection for WeightSelect");
            }

            using var _ = ListPool<T>.Get(out var list);
            list.AddRange(collection);

            var sum = list.Sum(e => weightDelegate(e));
            var rnd = UnityEngine.Random.value * sum;
            var iter = 0f;
            foreach (var e in list)
            {
                iter += weightDelegate(e);
                if (iter >= rnd)
                {
                    return e;
                }
            }

            return list.FirstOrDefault();
        }
    }
}
