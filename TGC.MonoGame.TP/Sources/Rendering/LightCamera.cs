using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.Rendering.Cameras
{
    internal class LightCamera
    {
        private Matrix View;
        private readonly Matrix Projection;

        internal Vector3 Direction { get; private set; } = new Vector3(0.5f, -1.3f, 0.2f);
        private readonly Quaternion Orientation;
        internal Vector3 Position { get; private set; }

        private readonly Vector3 CameraOffset;
        private const float NearPlaneDistance = 1f;
        private const float FarPlaneDistance = 10000f;

        internal LightCamera()
        {
            CameraOffset = new Vector3(0f, 1000f, 0f);
            Orientation = Quaternion.CreateFromAxisAngle(Vector3.Left, -(float)Math.PI/2 + 0.4f)
                        * Quaternion.CreateFromAxisAngle(Vector3.Up, 0.5f);
            Direction = PhysicUtils.Forward(Orientation);
            Projection = BuildProjection();
        }

        private Matrix BuildView(Vector3 position, Quaternion orientation)
        {
            Vector3 forward = PhysicUtils.Forward(orientation);
            Vector3 up  = PhysicUtils.Up(orientation);
            return Matrix.CreateLookAt(position, position + forward, up);
        }
        private Matrix BuildProjection() => Matrix.CreateOrthographic(TGCGame.ShadowMapSize, TGCGame.ShadowMapSize, NearPlaneDistance, FarPlaneDistance);

        internal Matrix ViewProjection { get; private set; }

        internal void Update()
        {
            Position = CameraOffset + TGCGame.Camera.Position;
            View = BuildView(Position, Orientation);
            ViewProjection = View * Projection;
        }
    }
}