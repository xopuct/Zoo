using TriInspector;
using UnityEngine;

namespace Zoo
{
    // TODO Replace singleton with a service locator
    public class GameManager : Singleton<GameManager>
    {
        public GameDefinition Definition;
        public BoxCollider WorldArea;
        public LayerMask SpawnMask;

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

        public AnimalDefinition SelectAnimal()
        {
            return Definition.Animals.WeightSelect(animal => animal.Weight).Animal;
        }

        [Button]
        [Title("Debug")]
        public void SpawnAnimal(AnimalDefinition animalDefinition)
        {
            var unit = UnitConstructor.CreateUnit(animalDefinition);
            var attempts = 15;
            var spawnHeight = Vector3.up * unit.Collider.bounds.extents.y;
            float halfWidth = WorldArea.size.x / 2;
            float halfLength = WorldArea.size.z / 2;

            while (attempts-- > 0)
            {
                var pos = new Vector3(Random.Range(-halfWidth, halfWidth),
                    spawnHeight.y,
                    Random.Range(-halfLength, halfLength));

                if (!Physics.CheckBox(pos, unit.Collider.bounds.extents / 2, Quaternion.identity, SpawnMask,
                        QueryTriggerInteraction.Collide))
                {
                    unit.transform.position = pos;
                    break;
                }
            }

            animalSpawnCount++;
        }

        public void Kill(Unit victim, Unit killer)
        {
            GameObject.Destroy(victim.gameObject);
            Debug.Log("Tasty");
        }

        private void UpdateTime()
        {
            lastSpawnTime = Time.time;
            nextSpawnInterval = Random.Range(Definition.AnimalSpawnIntervalMin, Definition.AnimalSpawnIntervalMax);
        }
    }
}
