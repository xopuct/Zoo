using System.Runtime.CompilerServices;
using UnityEngine;

namespace Zoo
{
    public static class Vector3Ex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SetY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }
    }
}
