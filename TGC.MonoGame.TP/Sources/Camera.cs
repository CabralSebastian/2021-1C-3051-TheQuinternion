using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TGC.MonoGame.TP
{
    internal class Camera
    {
        private Vector3 position = new Vector3();
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        private const float mouseSensitivity = 2f;

        private const float fieldOfView = MathHelper.PiOver4;
        private const float nearPlaneDistance = 0.1f;
        private const float farPlaneDistance = 10000f;

        private float pitch, yaw = -90f;
        private Vector3 frontDirection;
        private Vector3 rightDirection = Vector3.Right;
        private Vector3 upDirection = Vector3.Up;

        internal void Initialize(GraphicsDevice graphicsDevice)
        {
            Projection = CreateProjectionMatrix(graphicsDevice);
            View = CreateViewMatrix();
        }

        internal void Update(float elapsedTime)
        {
            FollowXWing();
            UpdateCameraVectors();
            View = CreateViewMatrix();
        }

        // Matrix
        private Matrix CreateProjectionMatrix(GraphicsDevice graphicsDevice) => Matrix.CreatePerspectiveFieldOfView(fieldOfView, graphicsDevice.Viewport.AspectRatio, nearPlaneDistance, farPlaneDistance);
        private Matrix CreateViewMatrix() => Matrix.CreateLookAt(position, position + frontDirection, upDirection);

        private void FollowXWing()
        {
            Vector3 xwingPosition = TGCGame.world.xwing.Position();
            float xwingVelocityScale = Math.Max(0, Vector3.Dot(TGCGame.world.xwing.Velocity(), TGCGame.world.xwing.forward) / (TGCGame.world.xwing.forward.Length() * TGCGame.world.xwing.forward.Length())) / TGCGame.world.xwing.maxSpeed;
            float xwingDistance = 30 + 15 * xwingVelocityScale;
            float cametaHeightDistance = 6 + 4 * xwingVelocityScale;

            position = xwingPosition - xwingDistance * TGCGame.world.xwing.forward + cametaHeightDistance * TGCGame.world.xwing.upDirection;
        }

        internal void UpdateYawNPitch(Vector2 mouseDelta)
        {
            yaw += mouseDelta.X;
            pitch -= mouseDelta.Y;
            pitch = Math.Clamp(pitch, -89.9f, 89.9f);
        }

        private void UpdateCameraVectors()
        {
            Vector3 mouseDirection = Vector3.Normalize(new Vector3(
                MathF.Cos(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch)),
                MathF.Sin(MathHelper.ToRadians(pitch)),
                MathF.Sin(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch))
            ));

            frontDirection = Vector3.Normalize(TGCGame.world.xwing.forward * 10 + mouseDirection);

            rightDirection = Vector3.Normalize(Vector3.Cross(frontDirection, Vector3.Up));
            upDirection = Vector3.Normalize(Vector3.Cross(rightDirection, frontDirection));
        }
    }
}