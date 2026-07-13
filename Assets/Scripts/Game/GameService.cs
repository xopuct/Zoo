using System;
using Reflex.Attributes;
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

        public void Kill(Unit victim, Unit killer)
        {
            GameObject.Destroy(victim.gameObject);
            OnKillEvent?.Invoke(victim, killer);
            Debug.Log("Tasty");
        }
    }
}
