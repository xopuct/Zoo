using System;
using UnityEngine;

namespace Zoo
{
    public static class UnitConstructor
    {
        public static Unit CreateUnit(AnimalDefinition animalDefinition)
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

            unit.MovementController = CreateMovement(unit, animalDefinition);
            AiController.Initialize(unit);

            return unit;
        }

        private static IMovementController CreateMovement(Unit unit, AnimalDefinition animalDefinition)
        {
            return animalDefinition.Movement switch
            {
                MovementType.Jump => MovementJump.Init(unit, animalDefinition.ConfigMovementJump),
                MovementType.Crawl => MovementCrawl.Init(unit, animalDefinition.ConfigMovementCrawl),
                _ => throw new ArgumentException($"Unknown movement config type {animalDefinition.Movement}")
            };
        }
    }
}
