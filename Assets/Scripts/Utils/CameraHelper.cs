using UnityEngine;

namespace Zoo
{
    public static class CameraHelper
    {
        public static bool IsPointInsideCamera(Camera camera, Vector3 worldPosition)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPosition);
            return viewportPoint is { z: > 0f, x: >= 0f and <= 1f, y: >= 0f and <= 1f };
        }

        public static bool TryGetRandomPointInViewport(
            Camera camera,
            float viewportMargin,
            LayerMask layerMask,
            float maxDistance,
            out Vector3 point)
        {
            Vector3 viewportPoint = new(
                Random.Range(viewportMargin, 1f - viewportMargin),
                Random.Range(viewportMargin, 1f - viewportMargin),
                0f);

            Ray ray = camera.ViewportPointToRay(viewportPoint);

            if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask))
            {
                point = hitInfo.point;
                return true;
            }

            point = default;
            return false;
        }
    }
}
