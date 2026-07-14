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
        private float smallestWorldSize = 0;
        private float largestUnitdSize = 0;

        public static void Construct(Unit unit)
        {
            var controller = unit.gameObject.AddComponent<AiController>();
            controller.InitInternal(unit);
        }

        private void InitInternal(Unit unit)
        {
            Unit = unit;
        }

        private void Start()
        {
            largestUnitdSize = Mathf.Max(Unit.Collider.bounds.size.x, Unit.Collider.bounds.size.z);
            sqrProximity = largestUnitdSize * largestUnitdSize * 2;
            smallestWorldSize = Mathf.Min(GameService.WorldArea.size.x, GameService.WorldArea.size.z);
            UpdateMovementGoal();
        }

        private void OnDisable()
        {
            MovementGoal = Unit.transform.position;
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
            if (Unit.HealthCurrent <= 0)
            {
                return;
            }

            if (MaybeKill(collision))
            {
                return;
            }

            if (Test(transform.position, transform.right * smallestWorldSize, out var newGoal) ||
                Test(transform.position, -transform.right * smallestWorldSize, out newGoal))
            {
                MovementGoal = newGoal;
            }
            else
            {
                UpdateMovementGoal();
            }
        }

        private bool Test(Vector3 position, Vector3 direction, out Vector3 movementGoal)
        {
            movementGoal = Vector3.zero;
            if (!Physics.Linecast(position, direction.normalized * smallestWorldSize, out var hit,
                    GameService.CollisionMask) ||
                Vector3.Distance(hit.collider.transform.position, position) > largestUnitdSize * 2)
            {
                movementGoal = position + direction.normalized * smallestWorldSize * 1.5f;
                return true;
            }

            return false;
        }

        private bool MaybeKill(Collision collision)
        {
            if ((GameService.UnitMask.value & (1 << collision.gameObject.layer)) == 0)
            {
                return false;
            }

            var opponent = collision.gameObject.GetComponent<Unit>();
            if (Unit.Consumption == ConsumptionType.Predator && opponent.HealthCurrent > 0)
            {
                var opponentConsumption = opponent.Consumption;
                if (opponentConsumption == ConsumptionType.Prey || opponent.Rank < Unit.Rank)
                {
                    GameService.Kill(opponent, Unit);
                    return true;
                }

                if (opponent.HealthCurrent <= Unit.HealthCurrent)
                {
                    Unit.HealthCurrent -= opponent.HealthCurrent;
                    GameService.Kill(opponent, Unit);
                    if (Unit.HealthCurrent == 0)
                    {
                        GameService.Kill(Unit, opponent);
                    }

                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(MovementGoal, 0.1f);
        }
    }
}
