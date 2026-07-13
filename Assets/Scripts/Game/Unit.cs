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

        public static Unit Construct(AnimalDefinition animalDefinition)
        {
            var inst = new GameObject($"Animal/{animalDefinition.Name}");
            inst.layer = LayerMask.NameToLayer("Unit");
            var unit = inst.AddComponent<Unit>();
            var instVisual = GameObject.Instantiate(animalDefinition.Visuals, inst.transform);
            instVisual.name = "Visuals";
            unit.Collider = instVisual.GetComponent<Collider>();

            unit.Config = animalDefinition;
            unit.Rigidbody = inst.AddComponent<Rigidbody>();
            unit.HealthMax = UnityEngine.Random.Range(animalDefinition.HealthMin, animalDefinition.HealthMax);
            unit.HealthCurrent = unit.HealthMax;

            unit.MovementController = IMovementController.Construct(unit, animalDefinition);
            AiController.Construct(unit);

            return unit;
        }
    }
}
