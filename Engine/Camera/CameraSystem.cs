using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Camera
{
    public class CameraSystem
    {
        private static CameraSystem _instance;
        public static CameraSystem Instance => _instance ??= new CameraSystem();

        public PointF Position { get; private set; } = new PointF(0, 0);
        public float Zoom { get; private set; } = 1.0f;

        public void SetPosition(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Move(float dx, float dy)
        {
            Position = new PointF(Position.X + dx, Position.Y + dy);
        }

        public void SetZoom(float zoom)
        {
            if (zoom <= 0) zoom = 0.01f;
            Zoom = zoom;
        }

        public PointF WorldToScreen(PointF world)
        {
            return new PointF(
            (world.X - Position.X) * Zoom,
            (world.Y - Position.Y) * Zoom
            );
        }

        public PointF ScreenToWorld(PointF screen)
        {
            return new PointF(
            screen.X / Zoom + Position.X,
            screen.Y / Zoom + Position.Y
            );
        }
        
        public Vector3 ScreenToWorld(float screenX, float screenY)
        {
            return new Vector3(
                screenX / Zoom + Position.X,
                screenY / Zoom + Position.Y,
                0f
            );
        }

        ///<summary>
        ///Shakes the camera with the specified intensity and duration.
        ///</summary>
        public void Shake(float intensity, float duration)
        {
            System.Diagnostics.Debug.WriteLine($"Shaking camera with intensity {intensity} for {duration} seconds.");
        }
    }
}




