using UnityEngine;

namespace Zoo
{
    public static class MonoBehaviourHelper
    {
        public static TComponent GetOrAddComponent<TComponent>(this MonoBehaviour mono) where TComponent : Component
        {
            var result = mono.GetComponent<TComponent>();
            if (result == null)
            {
                result = mono.gameObject.AddComponent<TComponent>();
            }
            return result;
        }
    }
}
