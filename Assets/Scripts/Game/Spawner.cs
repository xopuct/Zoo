using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using TriInspector;
using UnityEngine;

namespace Zoo
{
    public class Spawner : MonoBehaviour
    {
        [Inject]
        public GameService GameService;

        public GameDefinition Definition => GameService.Definition;

        private float lastSpawnTime;
        private float nextSpawnInterval;

        [ShowInInspector]
        [Title("Debug")]
        private int animalSpawnCount;

        public void Start()
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
            var unit = Unit.Construct(animalDefinition);
            unit.transform.SetParent(GameService.GetTransformFolder(animalDefinition.Name));
            var attempts = 15;
            var spawnHeight = Vector3.up * unit.Collider.bounds.extents.y;
            float halfWidth = GameService.WorldArea.size.x / 2;
            float halfLength = GameService.WorldArea.size.z / 2;

            while (attempts-- > 0)
            {
                var pos = new Vector3(Random.Range(-halfWidth, halfWidth),
                    spawnHeight.y,
                    Random.Range(-halfLength, halfLength));

                if (!Physics.CheckBox(pos, unit.Collider.bounds.extents / 2, Quaternion.identity, GameService.SpawnMask,
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

        private void UpdateTime()
        {
            lastSpawnTime = Time.time;
            nextSpawnInterval = Random.Range(Definition.AnimalSpawnIntervalMin, Definition.AnimalSpawnIntervalMax);
        }
    }
}
