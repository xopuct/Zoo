using Reflex.Attributes;
using UnityEngine;

namespace Zoo
{
    public class MovementJump : MonoBehaviour, IMovementController, IPoolObjectActivateHandler,
        IPoolObjectDeactivateHandler
    {
        public MovementConfigJump Config;
        public Vector3 MovementGoal { get; set; }

        private Unit unit;
        private Rigidbody Rigidbody => unit.Rigidbody;
        private float timeSinceLastJump;
        private bool isGrounded;
        private float rotatePerSec = 0.5f;

        [Inject]
        private GameService gameService;

        public static MovementJump Construct(Unit unit, MovementConfigJump config)
        {
            var inst = unit.gameObject.AddComponent<MovementJump>();
            inst.InitInternal(unit, config);
            return inst;
        }

        private void InitInternal(Unit unit, MovementConfigJump config)
        {
            Config = config;
            this.unit = unit;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void FixedUpdate()
        {
            if (Time.time - timeSinceLastJump > Config.JumpInterval)
            {
                var dir = (MovementGoal - transform.position).SetY(0);
                var nextRotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(dir.normalized, Vector3.up),
                    Config.JumpRotationArc);
                var jumpVelocity = CalculateJumpVelocity(nextRotation * Vector3.forward,
                    Mathf.Min(Config.JumpDistance, dir.magnitude),
                    Config.JumpHeight);
                Rigidbody.linearVelocity = jumpVelocity;
                timeSinceLastJump = Time.time;
                var flightTime = -2f * jumpVelocity.y / Physics.gravity.y;
                rotatePerSec = 4 * Quaternion.Angle(transform.rotation, nextRotation) / flightTime;
                isGrounded = false;
            }

            if (!isGrounded)
            {
                var nextDirection = (MovementGoal - transform.position).SetY(0).normalized;
                Rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(nextDirection, Vector3.up),
                    rotatePerSec * Time.fixedDeltaTime));
            }
        }

        private void LateUpdate()
        {
            RefreshIsGrounded();
        }

        private void RefreshIsGrounded()
        {
            isGrounded = Physics.Linecast(transform.position,
                transform.position + Vector3.down * unit.Collider.bounds.size.z * 0.5f,
                gameService.GravityTestMask);
        }

        // Todo Move to helpers
        private static Vector3 CalculateJumpVelocity(Vector3 direction, float distance, float apexHeight)
        {
            float g = Mathf.Abs(Physics.gravity.y);

            float vy = Mathf.Sqrt(2f * g * apexHeight);

            float timeUp = vy / g;
            float totalTime = timeUp * 2f;

            float vx = distance / totalTime;

            return direction.normalized * vx + Vector3.up * vy;
        }

        public void Activate()
        {
            timeSinceLastJump = Time.timeSinceLevelLoad;
            RefreshIsGrounded();
        }

        public void Deactivate()
        {
            MovementGoal = Vector3.zero;
            isGrounded = false;
            rotatePerSec = 0.5f;
        }
    }
}
