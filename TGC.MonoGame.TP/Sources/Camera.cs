using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP
{
    internal class Camera
    {
        internal Vector3 position = new Vector3();
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        private const float fieldOfView = MathHelper.PiOver4;
        private const float nearPlaneDistance = 0.1f;
        private const float farPlaneDistance = 10000f;

        private float pitch, yaw = -90f;
        private Vector3 frontDirection;
        private Vector3 rightDirection = Vector3.Right;
        private Vector3 upDirection = Vector3.Up;

        internal readonly AudioListener listener = new AudioListener();

        private XWing target;

        internal void Initialize(GraphicsDevice graphicsDevice)
        {
            Projection = CreateProjectionMatrix(graphicsDevice);
            View = CreateViewMatrix();
        }

        internal void Update()
        {
            if (target != null)
                FollowTarget();
            View = CreateViewMatrix();
            listener.Position = position;
        }

        internal void SetLocation(Vector3 position, Vector3 frontDirection)
        {
            this.position = position;
            this.frontDirection = frontDirection;
        }

        internal void SetTarget(XWing newTarget) => target = newTarget;

        // Matrix
        private Matrix CreateProjectionMatrix(GraphicsDevice graphicsDevice) => Matrix.CreatePerspectiveFieldOfView(fieldOfView, graphicsDevice.Viewport.AspectRatio, nearPlaneDistance, farPlaneDistance);
        private Matrix CreateViewMatrix() => Matrix.CreateLookAt(position, position + frontDirection, upDirection);

        private void FollowTarget()
        {
            Vector3 xwingPosition = target.Position();

            float xwingVelocityScale = Math.Max(0, Vector3.Dot(target.Velocity(), target.forward) / (target.forward.Length() * target.forward.Length())) / target.maxSpeed;
            float xwingDistance = 45 + 20 * xwingVelocityScale;
            float cametaHeightDistance = 15 + 3 * xwingVelocityScale;

            if (Equals(position, Vector3.Zero))
                position = xwingPosition - xwingDistance * target.forward + cametaHeightDistance * target.upDirection;

            Vector3 newForward = Vector3.Normalize(xwingPosition - position);
            position = xwingPosition - xwingDistance * newForward;
            /*Vector3 mouseDirection = Vector3.Normalize(new Vector3(
                MathF.Cos(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch)),
                MathF.Sin(MathHelper.ToRadians(pitch)),
                MathF.Sin(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch))
            ));*/

            frontDirection = Vector3.Normalize(10 * frontDirection + target.forward); //Vector3.Normalize(xwingPosition + 10 * TGCGame.world.xwing.upDirection - position
            rightDirection = Vector3.Normalize(Vector3.Cross(frontDirection, Vector3.Up));
            upDirection = Vector3.Normalize(upDirection * 5 + Vector3.Cross(rightDirection, frontDirection));
        }

        internal void UpdateYawNPitch(Vector2 mouseDelta)
        {
            yaw += mouseDelta.X;
            pitch -= mouseDelta.Y;
            pitch = Math.Clamp(pitch, -89.9f, 89.9f);
        }
    }
}