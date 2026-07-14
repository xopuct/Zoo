using System;
using UnityEngine;

namespace Zoo
{
    public class Unit : MonoBehaviour, IPoolObjectDeactivateHandler
    {
        public AnimalDefinition Config;
        public Rigidbody Rigidbody;
        public Collider Collider;

        public int HealthMax;
        public int HealthCurrent;

        public IMovementController MovementController;
        public ConsumptionType Consumption => Config.Consumption;
        public int Rank => Config.Rank;
        public bool Initialized { get; private set; }
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
            unit.Initialized = false;
            return unit;
        }

        public void Init(AnimalDefinition animalDefinition, Action<Unit> onDeathAction)
        {
            Config = animalDefinition;
            HealthMax = UnityEngine.Random.Range(animalDefinition.HealthMin, animalDefinition.HealthMax);
            HealthCurrent = HealthMax;
            this.onDeathAction = onDeathAction;

            if (!Initialized)
            {
                gameObject.name = $"Animal_{animalDefinition.Name}";
                gameObject.layer = LayerMask.NameToLayer("Unit");
                var instVisual = Instantiate(animalDefinition.Visuals, transform);
                instVisual.name = "Visuals";
                Collider = instVisual.GetComponent<Collider>();
                MovementController = MovementFactory.Construct(this, animalDefinition);
                AiController.Construct(this);
            }

            Initialized = true;
        }

        public void Die()
        {
            onDeathAction?.Invoke(this);
        }

        public void Deactivate()
        {
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.rotation = Quaternion.identity;
        }
    }
}
