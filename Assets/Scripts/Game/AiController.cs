using System;
using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zoo
{
    // Trivial AI controller for zoo. Supports Roaming only
    public class AiController : MonoBehaviour, IPoolObjectActivateHandler
    {
        private const float FallbackMovementMargin = 0.4f;

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
        private float largestUnitSize = 0;

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
            largestUnitSize = Mathf.Max(Unit.Collider.bounds.size.x, Unit.Collider.bounds.size.z);
            sqrProximity = largestUnitSize * largestUnitSize * 2;
            smallestWorldSize = Mathf.Min(gameService.WorldArea.size.x, gameService.WorldArea.size.z);
            UpdateMovementGoal();
        }

        private void Update()
        {
            if (!cameraService.IsPointInsideCamera(transform.position))
            {
                UpdateMovementGoal(FallbackMovementMargin);
            }
            else if ((transform.position - MovementGoal).sqrMagnitude < sqrProximity)
            {
                UpdateMovementGoal();
            }
        }

        private void UpdateMovementGoal(float viewportMargin = 0)
        {
            MovementGoal = cameraService.GetRandomPoint(viewportMargin).SetY(transform.position.y);;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Unit.HealthCurrent <= 0)
            {
                return;
            }

            if (TryResolveAttack(collision))
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
                hit.distance > largestUnitSize * 2)
            {
                movementGoal = position + direction.normalized * smallestWorldSize * 1.5f;
                return true;
            }

            return false;
        }

        private bool TryResolveAttack(Collision collision)
        {
            if ((gameService.UnitMask.value & (1 << collision.gameObject.layer)) == 0)
            {
                return false;
            }

            var opponent = collision.gameObject.GetComponent<Unit>();

            switch (AttackResolver.Resolve(Unit, opponent))
            {
                case AttackResult.None:
                    return false;
                case AttackResult.AttackerWinsClean:
                    gameService.Kill(opponent, Unit);
                    return true;
                case AttackResult.AttackerWinsWithInjury:
                    Unit.HealthCurrent = Mathf.Max(1, Unit.HealthCurrent - opponent.HealthCurrent);
                    gameService.Kill(opponent, Unit);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
