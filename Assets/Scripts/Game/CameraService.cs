using UnityEngine;

namespace Zoo
{
    public class CameraService
    {
        public Camera Camera
        {
            get => camera;
            init
            {
                camera = value;
                // Right now camera is not moving, so we don't need to update it
                UpdateRect();
            }
        }

        private Rect rect;

        private readonly Camera camera;

        public void UpdateRect()
        {
            CameraHelper.TryGetRect(Camera, 0, 0, out rect);
        }

        public bool IsPointInsideCamera(Vector3 worldPosition)
        {
            return rect.Contains(worldPosition.SetY(worldPosition.z));
        }

        /// <summary>
        /// Returns random point inside camera viewport
        /// </summary>
        /// <param name="viewportMargin"> Margin in viewport units</param>
        /// <returns></returns>
        public Vector3 GetRandomPoint(float viewportMargin = 0)
        {
            var marginX = rect.width * viewportMargin;
            var marginZ = rect.height * viewportMargin;
            return new Vector3(Random.Range(rect.xMin + marginX, rect.xMax - marginX), 0,
                Random.Range(rect.yMin + marginZ, rect.yMax - marginZ));
        }
    }
}
