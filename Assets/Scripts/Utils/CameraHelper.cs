using System;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Zoo
{
    public static class CameraHelper
    {
        public static bool IsPointInsideCamera(Camera camera, Vector3 worldPosition)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPosition);
            return viewportPoint is { z: > 0f, x: >= 0f and <= 1f, y: >= 0f and <= 1f };
        }

        public static bool TryGetRect(Camera camera, float viewportMargin, float groundY, out Rect result)
        {
            var plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));

            using var _ = ListPool<Vector2>.Get(out var viewportPoints);
            viewportPoints.Add(new(viewportMargin, viewportMargin));
            viewportPoints.Add(new(viewportMargin, 1f - viewportMargin));
            viewportPoints.Add(new(1f - viewportMargin, 1f - viewportMargin));
            viewportPoints.Add(new(1f - viewportMargin, viewportMargin));

            var minX = float.PositiveInfinity;
            var maxX = float.NegativeInfinity;
            var minZ = float.PositiveInfinity;
            var maxZ = float.NegativeInfinity;


            foreach (var viewportPoint in viewportPoints)
            {
                if (!TryProjectToPlane(
                        camera,
                        plane,
                        viewportPoint,
                        out var worldPoint))
                {
                    result = default;
                    return false;
                }

                minX = Mathf.Min(minX, worldPoint.x);
                maxX = Mathf.Max(maxX, worldPoint.x);
                minZ = Mathf.Min(minZ, worldPoint.z);
                maxZ = Mathf.Max(maxZ, worldPoint.z);
            }

            result = Rect.MinMaxRect(minX, minZ, maxX, maxZ);
            return true;
        }

        private static bool TryProjectToPlane(Camera camera, Plane plane, Vector2 viewportPoint, out Vector3 worldPoint)
        {
            var ray = camera.ViewportPointToRay(new Vector3(viewportPoint.x, viewportPoint.y));
            worldPoint = default;

            if (plane.Raycast(ray, out var distance))
            {
                worldPoint = ray.GetPoint(distance);
                return true;
            }

            return false;
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
