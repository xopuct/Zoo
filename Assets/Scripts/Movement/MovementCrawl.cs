using UnityEngine;

namespace Zoo
{
    public class MovementCrawl : MonoBehaviour, IMovementController
    {
        public MovementConfigCrawl Config;
        public Vector3 MovementGoal { get; set; }
        private Unit unit;
        private Rigidbody Rigidbody => unit.Rigidbody;


        public static MovementCrawl Construct(Unit unit, MovementConfigCrawl config)
        {
            var inst = unit.gameObject.AddComponent<MovementCrawl>();
            inst.InitInternal(unit, config);
            return inst;
        }

        private void InitInternal(Unit unit, MovementConfigCrawl config)
        {
            Config = config;
            this.unit = unit;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void FixedUpdate()
        {
            var nextDirection = (MovementGoal - transform.position).normalized;
            Rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(nextDirection, Vector3.up), Config.RotationSpeed * Time.fixedDeltaTime));
            if ((MovementGoal - transform.position).sqrMagnitude >
                unit.Collider.bounds.size.x * unit.Collider.bounds.size.x)
            {
                Rigidbody.MovePosition(transform.position + transform.forward * Config.Speed * Time.fixedDeltaTime);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.purple;
            var nextDirection = (MovementGoal - transform.position).normalized;
            Gizmos.DrawLine(transform.position, transform.position + nextDirection * Config.Speed);
        }
    }
}
