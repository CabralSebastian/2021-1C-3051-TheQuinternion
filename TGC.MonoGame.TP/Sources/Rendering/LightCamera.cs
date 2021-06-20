using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.Rendering.Cameras
{
    internal class LightCamera
    {
        private Matrix view;
        private readonly Matrix projection;

        internal Vector3 Direction { get; private set; } = new Vector3(0.5f, -1.3f, 0.2f);
        private readonly Quaternion orientation;
        internal Vector3 Position { get; private set; }

        private readonly Vector3 cameraOffset;
        private const float nearPlaneDistance = 1f;
        private const float farPlaneDistance = 10000f;

        internal LightCamera()
        {
            cameraOffset = new Vector3(0f, 1000f, 0f);
            orientation = Quaternion.CreateFromAxisAngle(Vector3.Left, -(float)Math.PI/2 + 0.4f)
                        * Quaternion.CreateFromAxisAngle(Vector3.Up, 0.5f);
            Direction = PhysicUtils.Forward(orientation);
            projection = BuildProjection();
        }

        private Matrix BuildView(Vector3 position, Quaternion orientation)
        {
            Vector3 forward = PhysicUtils.Forward(orientation);
            Vector3 up  = PhysicUtils.Up(orientation);
            return Matrix.CreateLookAt(position, position + forward, up);
        }

        //private Matrix BuildProjection() => Matrix.CreatePerspectiveFieldOfView(fieldOfViewDegrees, aspectRatio, nearPlaneDistance, farPlaneDistance);
        private Matrix BuildProjection() => Matrix.CreateOrthographic(TGCGame.shadowmapSize, TGCGame.shadowmapSize, nearPlaneDistance, farPlaneDistance);

        internal Matrix ViewProjection { get; private set; }

        internal void Update()
        {
            Position = cameraOffset + TGCGame.camera.position;
            view = BuildView(Position, orientation);
            ViewProjection = view * projection;
        }
    }
}