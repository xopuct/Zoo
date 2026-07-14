using Reflex.Attributes;
using TriInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zoo
{
    public class Spawner : MonoBehaviour
    {
        private const int SpawnAttempts = 15;
        private const float ViewportMargin = 0.05f;
        private const float FallbackRayHeight = 100f;
        private const float SurfaceOffset = 0.2f;

        public GameDefinition Definition => gameService.Definition;

        private float lastSpawnTime;
        private float nextSpawnInterval;

        [Inject]
        private GameService gameService;

        [Inject]
        private UnitService unitService;

        [Inject]
        private CameraService cameraService;


        private void Start()
        {
            UpdateTime();
        }

        public void Update()
        {
            if (unitService.UnitsSpawned == 0 && Definition.AnimalSpawnFirstTime >= 0 &&
                Time.time > Definition.AnimalSpawnFirstTime ||
                unitService.UnitsSpawned > 0 && Time.time - lastSpawnTime > nextSpawnInterval)
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
            var pooledUnit = unitService.CreateUnit(animalDefinition);
            var unit = pooledUnit.Obj;

            var spawnHeight = Vector3.up * unit.Collider.bounds.extents.y;
            float halfWidth = gameService.WorldArea.size.x / 2;
            float halfLength = gameService.WorldArea.size.z / 2;

            var attempts = SpawnAttempts + 1;
            while (attempts-- > 0)
            {
                if (!CameraHelper.TryGetRandomPointInViewport(cameraService.Camera, ViewportMargin,
                        gameService.GravityTestMask, cameraService.Camera.transform.position.y * 2, out var pos))
                {
                    pos = new Vector3(Random.Range(-halfWidth, halfWidth), 0, Random.Range(-halfLength, halfLength));
                }

                pos += spawnHeight;

                // Reserve the final attempt for guaranteed fallback spawning.
                if (attempts == 0)
                {
                    if (Physics.Raycast(pos.SetY(FallbackRayHeight), Vector3.down, out var hitInfo, FallbackRayHeight, gameService.GravityTestMask))
                    {
                        unit.transform.position = hitInfo.point + Vector3.up * (unit.Collider.bounds.extents.y + SurfaceOffset);
                    }
                    else
                    {
                        unit.transform.position = pos.SetY(gameService.Definition.FallbackHeight);
                    }
                }
                else if (!Physics.CheckBox(pos,
                             unit.Collider.bounds.extents,
                             unit.transform.rotation,
                             gameService.SpawnMask,
                             QueryTriggerInteraction.Collide))
                {
                    unit.transform.position = pos;
                    break;
                }
            }


            pooledUnit.Activate();
        }


        private void UpdateTime()
        {
            lastSpawnTime = Time.time;
            nextSpawnInterval = Random.Range(Definition.AnimalSpawnIntervalMin, Definition.AnimalSpawnIntervalMax);
        }
    }
}
