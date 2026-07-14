using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo
{
    public interface IPoolObjectActivateHandler
    {
        public void Activate();
    }

    public interface IPoolObjectDeactivateHandler
    {
        public void Deactivate();
    }

    public class PoolObject : MonoBehaviour
    {
        private readonly List<IPoolObjectActivateHandler> handlersActivate = new();
        private readonly List<IPoolObjectDeactivateHandler> handlersDeactivate = new();


        private void Awake()
        {
            gameObject.GetComponentsInChildren(handlersActivate);
            gameObject.GetComponentsInChildren(handlersDeactivate);
        }

        public void Activate()
        {
            foreach (var handler in handlersActivate)
            {
                handler.Activate();
            }
        }

        public void Deactivate()
        {
            foreach (var handler in handlersDeactivate)
            {
                handler.Deactivate();
            }
        }
    }
}
