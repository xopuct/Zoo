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
    }
}
