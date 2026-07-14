using System;
using UnityEngine;

namespace Zoo
{
    public interface IMovementController
    {
        Vector3 MovementGoal { get; set; }

        public static IMovementController Construct(Unit unit,
            AnimalDefinition config)
        {
            return config.Movement switch
            {
                MovementType.Jump => MovementJump.Construct(unit, config.ConfigMovementJump),
                MovementType.Crawl => MovementCrawl.Construct(unit, config.ConfigMovementCrawl),
                _ => throw new ArgumentException($"Unknown movement config type {config.Movement}")
            };
        }
    }

    public class Unit : MonoBehaviour
    {
        public AnimalDefinition Config;
        public Rigidbody Rigidbody;
        public Collider Collider;

        public int HealthMax;
        public int HealthCurrent;

        public IMovementController MovementController;
        public ConsumptionType Consumption => Config.Consumption;
        public int Rank => Config.Rank;
        public bool Inited { get; private set; }
        private Action<Unit> onDeathAction;

        public static Unit ConstructEmpty(Transform parent)
        {
            var inst = new GameObject();
            if (parent)
            {
                inst.transform.SetParent(parent);
            }

            inst.layer = LayerMask.NameToLayer("Unit");
            var unit = inst.AddComponent<Unit>();
            unit.Rigidbody = inst.AddComponent<Rigidbody>();
            unit.Inited = false;
            return unit;
        }

        public void Init(AnimalDefinition animalDefinition, Action<Unit> onDeathAction)
        {
            Config = animalDefinition;
            HealthMax = UnityEngine.Random.Range(animalDefinition.HealthMin, animalDefinition.HealthMax);
            HealthCurrent = HealthMax;
            this.onDeathAction = onDeathAction;

            if (!Inited)
            {
                gameObject.name = $"Animal_{animalDefinition.Name}";
                gameObject.layer = LayerMask.NameToLayer("Unit");
                var instVisual = Instantiate(animalDefinition.Visuals, transform);
                instVisual.name = "Visuals";
                Collider = instVisual.GetComponent<Collider>();
                MovementController = IMovementController.Construct(this, animalDefinition);
                AiController.Construct(this);
            }

            Inited = true;
        }

        public void Die()
        {
            onDeathAction?.Invoke(this);
        }
    }
}
