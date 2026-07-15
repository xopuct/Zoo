using System.Collections.Generic;
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
        private readonly HashSet<Collision> isGroundedCollision = new();

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
            if (Time.time - timeSinceLastJump > Config.JumpInterval && isGrounded)
            {
                var dir = (MovementGoal - transform.position).SetY(0);
                var nextRotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(dir.normalized, Vector3.up),
                    Config.JumpRotationArc);
                var jumpVelocity = PhysicsHelper.CalculateJumpVelocity(nextRotation * Vector3.forward,
                    Mathf.Min(Config.JumpDistance, dir.magnitude),
                    Config.JumpHeight);
                Rigidbody.linearVelocity = jumpVelocity;
                timeSinceLastJump = Time.time;
                var flightTime = -2f * jumpVelocity.y / Physics.gravity.y;
                rotatePerSec = 4 * Quaternion.Angle(transform.rotation, nextRotation) / flightTime;
                isGrounded = false;
                isGroundedCollision.Clear();
            }

            if (!isGrounded)
            {
                var nextDirection = (MovementGoal - transform.position).SetY(0).normalized;
                Rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(nextDirection, Vector3.up),
                    rotatePerSec * Time.fixedDeltaTime));
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((gameService.GravityTestMask.value & (1 << collision.gameObject.layer)) == 0)
            {
                return;
            }

            for (var i = 0; i < collision.contactCount; i++)
            {
                if (collision.GetContact(i).normal.y > 0.5f)
                {
                    isGroundedCollision.Add(collision);
                    isGrounded = true;
                    return;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            isGroundedCollision.Remove(collision);
            isGrounded = isGroundedCollision.Count > 0;
        }

        private void RefreshIsGrounded()
        {
            isGrounded = Physics.Linecast(transform.position,
                transform.position + Vector3.down * unit.Collider.bounds.size.y * 0.5f,
                gameService.GravityTestMask);
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
