using System;
using UnityEngine;

namespace Zoo
{
    public interface IMovementController
    {
        Vector3 MovementGoal { get; set; }
    }

    public static class MovementFactory
    {
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
}
