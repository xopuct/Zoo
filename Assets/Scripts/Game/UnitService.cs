using System;
using System.Collections.Generic;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Zoo
{
    public class UnitService
    {
        public class SpawnPoolGroup
        {
            public AnimalDefinition Config { get; }
            private readonly Queue<Unit> pooledObjects = new();
            private readonly Transform rootTransform;

            public void Release(Unit unit)
            {
                Assert.AreEqual(unit.Config, Config);
                pooledObjects.Enqueue(unit);
            }

            public SpawnPoolGroup(AnimalDefinition config, Transform rootTransform)
            {
                this.rootTransform = rootTransform;
                Config = config;
            }

            public Unit GetUnit()
            {
                if (!pooledObjects.TryDequeue(out var result))
                {
                    result = Unit.ConstructEmpty(rootTransform);
                }

                return result;
            }
        }

        public event Action OnUnitSpawnedEvent;
        public event Action OnUnitDiedEvent;

        public int UnitsAlive { get; private set; }
        public int UnitsSpawned { get; private set; }
        public int UnitsPreyDeaths { get; private set; }
        public int UnitsPredatorDeaths { get; private set; }

        private Dictionary<AnimalDefinition, SpawnPoolGroup> poolGroups = new();

        private GameService gameService;

        public UnitService(GameService gameService)
        {
            this.gameService = gameService;
        }

        public PooledHandle<Unit> CreateUnit(AnimalDefinition animalDefinition)
        {
            var group = GetPoolGroup(animalDefinition);
            var unit = group.GetUnit();
            var isInitialized = unit.Initialized;
            if (!isInitialized)
            {
                unit.transform.SetParent(gameService.GetTransformFolder(animalDefinition.Name));
            }

            unit.Init(animalDefinition, OnUnitDied);
            unit.gameObject.SetActive(true);
            if (!isInitialized)
            {
                var container = unit.gameObject.scene.GetSceneContainer();
                GameObjectInjector.InjectRecursive(unit.gameObject, container);
            }

            var poolObject = unit.GetOrAddComponent<PoolObject>();
            UnitsSpawned++;
            UnitsAlive++;

            OnUnitSpawnedEvent?.Invoke();

            return new PooledHandle<Unit>(poolObject, unit);
        }

        private SpawnPoolGroup GetPoolGroup(AnimalDefinition animalDefinition)
        {
            if (!poolGroups.TryGetValue(animalDefinition, out var group))
            {
                group = new SpawnPoolGroup(animalDefinition, gameService.GetTransformFolder(animalDefinition.Name));
                poolGroups.Add(animalDefinition, group);
            }

            return group;
        }

        private void OnUnitDied(Unit unit)
        {
            var group = GetPoolGroup(unit.Config);
            group.Release(unit);

            var poolObject = unit.gameObject.GetComponent<PoolObject>();
            poolObject.Deactivate();
            unit.gameObject.SetActive(false);
            UnitsAlive--;
            if (unit.Consumption == ConsumptionType.Prey)
            {
                UnitsPreyDeaths++;
            }
            else
            {
                UnitsPredatorDeaths++;
            }

            OnUnitDiedEvent?.Invoke();
        }
    }
}
