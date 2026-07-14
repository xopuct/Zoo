using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace Zoo
{
    [Serializable]
    public class GameService
    {
        public event Action<Unit, Unit> OnKillEvent;

        [ShowInInspector]
        public GameDefinition Definition { get; init; }

        [ShowInInspector]
        public BoxCollider WorldArea { get; init; }

        [ShowInInspector]
        public LayerMask SpawnMask { get; init; }

        public LayerMask CollisionMask { get; set; }
        public LayerMask UnitMask { get; set; }

        private Dictionary<string, GameObject> folders = new();

        public void Kill(Unit victim, Unit killer)
        {
            victim.HealthCurrent = 0;
            GameObject.Destroy(victim.gameObject);
            OnKillEvent?.Invoke(victim, killer);
            Debug.Log("Tasty");
        }

        public Transform GetTransformFolder(string name)
        {
            if (!folders.TryGetValue(name, out var go))
            {
                go = new GameObject(name + "s");
                folders.Add(name, go);
            }

            return go.transform;
        }
    }
}
