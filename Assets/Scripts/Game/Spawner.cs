using System.Collections.Generic;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using TriInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zoo
{
    public class Spawner : MonoBehaviour
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


        public GameDefinition Definition => gameService.Definition;

        private float lastSpawnTime;
        private float nextSpawnInterval;

        [ShowInInspector]
        [Title("Debug")]
        private int animalSpawnCount;

        [Inject]
        private GameService gameService;

        [Inject]
        private CameraService cameraService;

        private Dictionary<AnimalDefinition, SpawnPoolGroup> poolGroups = new();

        private void Start()
        {
            UpdateTime();
        }

        public void Update()
        {
            if (animalSpawnCount == 0 && Definition.AnimalSpawnFirstTime >= 0 &&
                Time.time > Definition.AnimalSpawnFirstTime ||
                animalSpawnCount > 0 && Time.time - lastSpawnTime > nextSpawnInterval)
            {
                SpawnAnimal(SelectAnimal());
                UpdateTime();
            }
        }

        private AnimalDefinition SelectAnimal()
        {
            return Definition.Animals.WeightSelect(animal => animal.Weight).Animal;
        }

        [Button]
        [Title("Debug")]
        public void SpawnAnimal(AnimalDefinition animalDefinition)
        {
            var unit = GetUnit(animalDefinition);
            if (!unit.Inited)
            {
                unit.transform.SetParent(gameService.GetTransformFolder(animalDefinition.Name));
            }

            unit.Init(animalDefinition, OnUnitDied);

            var attempts = 15;
            var spawnHeight = Vector3.up * unit.Collider.bounds.extents.y;
            float halfWidth = gameService.WorldArea.size.x / 2;
            float halfLength = gameService.WorldArea.size.z / 2;

            while (attempts-- > 0)
            {
                if (!CameraHelper.TryGetRandomPointInViewport(cameraService.Camera, 0.05f,
                        gameService.GravityTestMask, cameraService.Camera.transform.position.y * 2, out var pos))
                {
                    pos = new Vector3(Random.Range(-halfWidth, halfWidth),
                        0,
                        Random.Range(-halfLength, halfLength));
                }

                pos += spawnHeight;

                if (!Physics.CheckBox(pos, unit.Collider.bounds.extents / 2, Quaternion.identity, gameService.SpawnMask,
                        QueryTriggerInteraction.Collide))
                {
                    unit.transform.position = pos;
                    break;
                }
            }

            var container = gameObject.scene.GetSceneContainer();

            GameObjectInjector.InjectRecursive(
                unit.gameObject,
                container);
            animalSpawnCount++;
        }

        private void OnUnitDied(Unit unit)
        {
            var group = GetPoolGroup(unit.Config);
            group.Release(unit);
            unit.gameObject.SetActive(false);
        }

        private void UpdateTime()
        {
            lastSpawnTime = Time.time;
            nextSpawnInterval = Random.Range(Definition.AnimalSpawnIntervalMin, Definition.AnimalSpawnIntervalMax);
        }

        private Unit GetUnit(AnimalDefinition animalDefinition)
        {
            var group = GetPoolGroup(animalDefinition);
            var unit = group.GetUnit();
            unit.gameObject.SetActive(true);
            return unit;
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
    }
}
