using System;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zoo
{
    // Trivial AI controller for zoo. Supports Roaming only
    public class AiController : MonoBehaviour
    {
        public Unit Unit;

        private Vector3 MovementGoal
        {
            get => Unit.MovementController.MovementGoal;
            set => Unit.MovementController.MovementGoal = value;
        }

        private float sqrProximity = 0.2f * 0.2f;

        public static void Initialize(Unit unit)
        {
            var controller = unit.AddComponent<AiController>();
            controller.InitInternal(unit);
        }

        private void InitInternal(Unit unit)
        {
            Unit = unit;
            var largestScale = Mathf.Max(Unit.Collider.bounds.size.x, Unit.Collider.bounds.size.z);
            sqrProximity = largestScale * largestScale;
        }

        public void Update()
        {
            if ((Unit.transform.position - MovementGoal).sqrMagnitude < sqrProximity)
            {
                UpdateMovementGoal();
            }
        }

        private void UpdateMovementGoal()
        {
            //TODO  Use DI for resolve level size;
            MovementGoal = new Vector3(Random.Range(-20, 20), transform.position.y, Random.Range(-20, 20));
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
            if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                return;
            }

            UpdateMovementGoal();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var nextDirection = (MovementGoal - transform.position).normalized;
            Gizmos.DrawSphere(MovementGoal, 0.1f);
        }
    }
}
