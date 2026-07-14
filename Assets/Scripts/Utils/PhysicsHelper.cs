using UnityEngine;

namespace Zoo
{
    public static class PhysicsHelper
    {
        public static Vector3 CalculateJumpVelocity(Vector3 direction, float distance, float apexHeight)
        {
            float g = Mathf.Abs(Physics.gravity.y);

            float vy = Mathf.Sqrt(2f * g * apexHeight);

            float timeUp = vy / g;
            float totalTime = timeUp * 2f;

            float vx = distance / totalTime;

            return direction.normalized * vx + Vector3.up * vy;
        }
    }
}
