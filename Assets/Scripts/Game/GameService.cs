using System;
using TriInspector;
using UnityEngine;

namespace Zoo
{
    [Serializable]
    public class GameService
    {
        [ShowInInspector]
        public GameDefinition Definition { get; init; }
        [ShowInInspector]
        public BoxCollider WorldArea { get; init;}
        [ShowInInspector]
        public LayerMask SpawnMask { get;init; }

        public void Kill(Unit victim, Unit killer)
        {
            GameObject.Destroy(victim.gameObject);
            Debug.Log("Tasty");
        }
    }
}
