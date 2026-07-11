using System;
using UnityEngine;

namespace Zoo
{
    public interface IMovementController
    {
        Vector3 MovementGoal { get; set; }
    }

    public class Unit : MonoBehaviour
    {
        public AnimalDefinition AnimalDefinition;
        public Rigidbody Rigidbody;
        public Collider Collider;

        public int HealthMax;
        public int HealthCurrent;

        public IMovementController MovementController;

        private void OnTriggerEnter(Collider other)
        {
            // if (other.gameObject.layer != LayerMask.NameToLayer("Default"))
            //     Debug.Log("Unit hit " + other.name);
        }
    }
}
