using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP
{
    internal class Camera
    {
        internal Vector3 Position = new Vector3();
        private Matrix View;
        private Matrix Projection;
        internal Matrix ViewProjection { get; private set; }
        internal Matrix PrevViewProjection { get; private set; }
        internal Matrix InverseViewProjection { get; private set; }

        private const float FieldOfView = MathHelper.PiOver4;
        private const float NearPlaneDistance = 0.1f;
        private const float FarPlaneDistance = 4000f;
        private const float NormalizedNearPlaneDistance = NearPlaneDistance / FarPlaneDistance;

        internal Vector3 Forward = Vector3.Forward;
        private Vector3 Up = Vector3.Up, right = Vector3.Right;

        internal readonly AudioListener Listener = new AudioListener();

        private XWing Target;

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
            if (Target != null)
                FollowTarget();
            View = CreateViewMatrix();
            PrevViewProjection = ViewProjection;
            ViewProjection = View * Projection;
            InverseViewProjection = Matrix.Invert(ViewProjection);
            Listener.Position = Position;
            Listener.Forward = Forward;
            Listener.Up = Up;
        }

        internal void SetLocation(Vector3 position, Vector3 forward, Vector3 up)
        {
            this.Position = position;
            this.Forward = forward;
            this.Up = up;
            right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
        }

        internal void SetTarget(XWing newTarget) => Target = newTarget;

        // Matrix
        private Matrix CreateProjectionMatrix(GraphicsDevice graphicsDevice) => Matrix.CreatePerspectiveFieldOfView(FieldOfView, graphicsDevice.Viewport.AspectRatio, NearPlaneDistance, FarPlaneDistance);
        private Matrix CreateViewMatrix() => Matrix.CreateLookAt(Position, Position + Forward, Up);

        private void FollowTarget()
        {
            Vector3 xwingPosition = Target.Position();

            float xwingVelocityScale = Math.Max(0, Vector3.Dot(Target.Velocity(), Target.Forward) / (Target.Forward.Length() * Target.Forward.Length())) / Target.MaxSpeed;
            float xwingDistance = 45 + 20 * xwingVelocityScale;
            float cametaHeightDistance = 15 + 3 * xwingVelocityScale;

            if (Equals(Position, Vector3.Zero))
                Position = xwingPosition - xwingDistance * Target.Forward + cametaHeightDistance * Target.UpDirection;

            Vector3 newForward = Vector3.Normalize(xwingPosition - Position);
            Position = xwingPosition - xwingDistance * newForward;

            Forward = Vector3.Normalize(10 * Forward + Target.Forward);
            right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.Up));
            Up = Vector3.Normalize(Up * 5 + Vector3.Cross(right, Forward));
        }

        internal Vector3 Unproject(float depth)
        {
            Vector3 mousePosition = new Vector3(Input.MousePosition(), depth);
            GraphicsDevice gd = TGCGame.Game.GraphicsDevice;
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

        internal Vector3 NearMouseWorldPosition() => Unproject(NormalizedNearPlaneDistance);
        internal Vector3 FarMouseWorldPosition() => Unproject(1f);
        internal Vector3 MouseDirection(Vector3 nearPoint) => Vector3.Normalize(FarMouseWorldPosition() - nearPoint);
        internal Vector3 MouseDirection() => MouseDirection(NearMouseWorldPosition());
    }
}