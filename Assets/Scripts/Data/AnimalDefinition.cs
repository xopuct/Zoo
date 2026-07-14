using System;
using TriInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Zoo
{
    public enum ConsumptionType
    {
        Prey,
        Predator,
    }

    public enum MovementType
    {
        Crawl,
        Jump,
    }

    public interface IMovementConfig
    {
    }

    [Serializable]
    public class MovementConfigJump : IMovementConfig
    {
        public float JumpDistance = 1;
        public float JumpInterval = 1;
        [FormerlySerializedAs("RotationSpeed")]
        [FormerlySerializedAs("RotationDeltaPerJump")]
        public float JumpRotationArc = 30;
        public float JumpHeight = 2;
    }

    [Serializable]
    public class MovementConfigCrawl : IMovementConfig
    {
        public float Speed;
        public float RotationSpeed;
    }

    [CreateAssetMenu(fileName = "NewAnimal.AnimalDefinition.asset", menuName = "Zoo/New Animal Config")]
    public class AnimalDefinition : ScriptableObject
    {
        public string Name;
        public GameObject Visuals;

        // Using to determine who is the strongest animal when they both predators and has same rank
        public int HealthMin;
        public int HealthMax;
        public ConsumptionType Consumption;

        [ShowIf(nameof(ShowIfPredator))]

        // Using to determine who is the strongest animal when they both predators
        public int Rank;

        public MovementType Movement;

        [ShowIf(nameof(ShowIfJump))]
        [InspectorName("Movement Config")]
        public MovementConfigJump ConfigMovementJump;

        [ShowIf(nameof(ShowIfCrawl))]
        [InspectorName("Movement Config")]
        public MovementConfigCrawl ConfigMovementCrawl;

        #region inspector helper

        private bool ShowIfPredator() => Consumption == ConsumptionType.Predator;
        private bool ShowIfJump() => Movement == MovementType.Jump;
        private bool ShowIfCrawl() => Movement == MovementType.Crawl;

        #endregion
    }
}
