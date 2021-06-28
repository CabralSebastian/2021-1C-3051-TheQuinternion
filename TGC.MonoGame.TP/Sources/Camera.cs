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
        private Matrix View;
        private Matrix Projection;
        internal Matrix ViewProjection { get; private set; }
        internal Matrix PrevViewProjection { get; private set; }
        internal Matrix InverseViewProjection { get; private set; }

        private const float fieldOfView = MathHelper.PiOver4;
        private const float nearPlaneDistance = 0.1f;
        private const float farPlaneDistance = 4000f;
        private const float normalizedNearPlaneDistance = nearPlaneDistance / farPlaneDistance;

        internal Vector3 forward = Vector3.Forward;
        private Vector3 up = Vector3.Up, right = Vector3.Right;

        internal readonly AudioListener listener = new AudioListener();

        private XWing target;

        internal void Initialize(GraphicsDevice graphicsDevice)
        {
            Projection = CreateProjectionMatrix(graphicsDevice);
            View = CreateViewMatrix();
            ViewProjection = View * Projection;
            PrevViewProjection = ViewProjection;
            InverseViewProjection = Matrix.Invert(ViewProjection);
        }

        internal void Update()
        {
            if (target != null)
                FollowTarget();
            View = CreateViewMatrix();
            PrevViewProjection = ViewProjection;
            ViewProjection = View * Projection;
            InverseViewProjection = Matrix.Invert(ViewProjection);
            listener.Position = position;
            listener.Forward = forward;
            listener.Up = up;
        }

        internal void SetLocation(Vector3 position, Vector3 forward, Vector3 up)
        {
            this.position = position;
            this.forward = forward;
            this.up = up;
            right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
        }

        internal void SetTarget(XWing newTarget) => target = newTarget;

        // Matrix
        private Matrix CreateProjectionMatrix(GraphicsDevice graphicsDevice) => Matrix.CreatePerspectiveFieldOfView(fieldOfView, graphicsDevice.Viewport.AspectRatio, nearPlaneDistance, farPlaneDistance);
        private Matrix CreateViewMatrix() => Matrix.CreateLookAt(position, position + forward, up);

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

            forward = Vector3.Normalize(10 * forward + target.forward);
            right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
            up = Vector3.Normalize(up * 5 + Vector3.Cross(right, forward));
        }

        internal Vector3 Unproject(float depth)
        {
            Vector3 mousePosition = new Vector3(Input.MousePosition(), depth);
            GraphicsDevice gd = TGCGame.game.GraphicsDevice;
            if (mousePosition.Z > gd.Viewport.MaxDepth)
                throw new Exception("Source Z must be less than MaxDepth ");
            Matrix wvp = Matrix.Multiply(View, Projection);
            Matrix inv = Matrix.Invert(wvp);
            Vector3 clipSpace = mousePosition;
            clipSpace.X = (((mousePosition.X - gd.Viewport.X) / ((float)gd.Viewport.Width)) * 2f) - 1f;
            clipSpace.Y = -((((mousePosition.Y - gd.Viewport.Y) / ((float)gd.Viewport.Height)) * 2f) - 1f);
            clipSpace.Z = (mousePosition.Z - gd.Viewport.MinDepth) / (gd.Viewport.MaxDepth - gd.Viewport.MinDepth);
            Vector3 invsrc = Vector3.Transform(clipSpace, inv);
            float a = (((clipSpace.X * inv.M14) + (clipSpace.Y * inv.M24)) + (clipSpace.Z * inv.M34)) + inv.M44;
            return invsrc / a;
        }

        internal Vector3 NearMouseWorldPosition() => Unproject(normalizedNearPlaneDistance);
        internal Vector3 FarMouseWorldPosition() => Unproject(1f);
        internal Vector3 MouseDirection(Vector3 nearPoint) => Vector3.Normalize(FarMouseWorldPosition() - nearPoint);
        internal Vector3 MouseDirection() => MouseDirection(NearMouseWorldPosition());
    }
}