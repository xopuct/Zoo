using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zoo
{
    // Trivial AI controller for zoo. Supports Roaming only
    public class AiController : MonoBehaviour
    {
        public Unit Unit;

        [Inject]
        public GameService GameService;

        private Vector3 MovementGoal
        {
            get => Unit.MovementController.MovementGoal;
            set => Unit.MovementController.MovementGoal = value;
        }

        private float sqrProximity = 0.2f * 0.2f;

        public static void Construct(Unit unit)
        {
            var controller = unit.gameObject.AddComponent<AiController>();
            controller.InitInternal(unit);
        }

        private void InitInternal(Unit unit)
        {
            Unit = unit;
            var largestScale = Mathf.Max(Unit.Collider.bounds.size.x, Unit.Collider.bounds.size.z);
            sqrProximity = largestScale * largestScale * 2;
        }

        private void Start()
        {
            UpdateMovementGoal();
        }

        private void Update()
        {
            if ((Unit.transform.position - MovementGoal).sqrMagnitude < sqrProximity)
            {
                UpdateMovementGoal();
            }
        }

        private void UpdateMovementGoal()
        {
            //TODO  Use DI for resolve level size;
            var worldSize = GameService.WorldArea.size;
            MovementGoal = new Vector3(Random.Range(-worldSize.x, worldSize.x), transform.position.y,
                Random.Range(-worldSize.y, worldSize.y));
        }

        public void OnTriggerExit(Collider other)
        {
            // Replace it with something more robust
            if (other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                return;
            }

            MovementGoal = Vector3.zero;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Unit.HealthCurrent <= 0 || collision.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                return;
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                var opponent = collision.gameObject.GetComponent<Unit>();
                if (Unit.Consumption == ConsumptionType.Predator && opponent.HealthCurrent > 0)
                {
                    var opponentConsumption = opponent.Consumption;
                    if (opponentConsumption == ConsumptionType.Prey)
                    {
                        GameService.Kill(opponent, Unit);
                    }
                    else if (opponent.Rank < Unit.Rank)
                    {
                        GameService.Kill(opponent, Unit);
                    }
                    else if (opponent.HealthCurrent <= Unit.HealthCurrent)
                    {
                        Unit.HealthCurrent -= opponent.HealthCurrent;
                        GameService.Kill(opponent, Unit);
                        if (Unit.HealthCurrent == 0)
                        {
                            GameService.Kill(Unit, opponent);
                        }
                    }
                }
            }

            UpdateMovementGoal();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(MovementGoal, 0.1f);
        }
    }
}
