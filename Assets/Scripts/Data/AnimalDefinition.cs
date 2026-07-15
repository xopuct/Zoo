using System;
using TriInspector;
using UnityEngine;

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

    [Serializable]
    public class MovementConfigJump
    {
        [Min(0.1f)]
        public float JumpDistance = 1;

        [Min(0.1f)]
        public float JumpInterval = 1;

        [Min(0)]
        public float JumpRotationArc = 30;

        [Min(0.5f)]
        public float JumpHeight = 2;
    }

    [Serializable]
    public class MovementConfigCrawl
    {
        [Min(0)]
        public float Speed;

        [Min(0)]
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

        private void OnValidate()
        {
            HealthMin = Mathf.Max(0, HealthMin);
            HealthMax = Mathf.Max(HealthMin, HealthMax);
        }

        #region inspector helper

        private bool ShowIfPredator() => Consumption == ConsumptionType.Predator;
        private bool ShowIfJump() => Movement == MovementType.Jump;
        private bool ShowIfCrawl() => Movement == MovementType.Crawl;

        #endregion
    }
}
