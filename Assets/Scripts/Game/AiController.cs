using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zoo
{
    // Trivial AI controller for zoo. Supports Roaming only
    public class AiController : MonoBehaviour, IPoolObjectActivateHandler
    {
        public Unit Unit;

        [Inject]
        private GameService gameService;

        [Inject]
        private CameraService cameraService;

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
            smallestWorldSize = Mathf.Min(gameService.WorldArea.size.x, gameService.WorldArea.size.z);
            UpdateMovementGoal();
        }

        private void Update()
        {
            if ((Unit.transform.position - MovementGoal).sqrMagnitude < sqrProximity)
            {
                UpdateMovementGoal();
            }

            if (!CameraHelper.IsPointInsideCamera(cameraService.Camera, MovementGoal))
            {
                UpdateMovementGoal();
            }

            if (!CameraHelper.IsPointInsideCamera(cameraService.Camera, transform.position))
            {
                if (CameraHelper.TryGetRandomPointInViewport(cameraService.Camera, 0.5f, gameService.GravityTestMask,
                        cameraService.Camera.transform.position.y * 2, out var point))
                {
                    MovementGoal = point.SetY(transform.position.y);
                }
                else
                {
                    MovementGoal = Vector3.zero;
                }
            }
        }

        private void UpdateMovementGoal()
        {
            if (CameraHelper.TryGetRandomPointInViewport(cameraService.Camera, 0.05f, gameService.GravityTestMask,
                    cameraService.Camera.transform.position.y * 2, out var point))
            {
                MovementGoal = point.SetY(transform.position.y);
            }
            else
            {
                var worldSize = gameService.WorldArea.size;
                MovementGoal = new Vector3(Random.Range(-worldSize.x, worldSize.x), transform.position.y,
                    Random.Range(-worldSize.y, worldSize.y));
            }
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
            if (!Physics.Linecast(position, position + direction.normalized * smallestWorldSize, out var hit,
                    gameService.CollisionMask) ||
                hit.distance > largestUnitdSize * 2)
            {
                movementGoal = position + direction.normalized * smallestWorldSize * 1.5f;
                return true;
            }

            return false;
        }

        private bool MaybeKill(Collision collision)
        {
            if ((gameService.UnitMask.value & (1 << collision.gameObject.layer)) == 0)
            {
                return false;
            }

            var opponent = collision.gameObject.GetComponent<Unit>();
            if (Unit.Consumption == ConsumptionType.Predator && opponent.HealthCurrent > 0)
            {
                var opponentConsumption = opponent.Consumption;
                if (opponentConsumption == ConsumptionType.Prey || opponent.Rank < Unit.Rank)
                {
                    gameService.Kill(opponent, Unit);
                    return true;
                }

                if (opponent.HealthCurrent <= Unit.HealthCurrent)
                {
                    Unit.HealthCurrent -= opponent.HealthCurrent;
                    gameService.Kill(opponent, Unit);
                    if (Unit.HealthCurrent == 0)
                    {
                        gameService.Kill(Unit, opponent);
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

        public void Activate()
        {
            UpdateMovementGoal();
        }
    }
}
