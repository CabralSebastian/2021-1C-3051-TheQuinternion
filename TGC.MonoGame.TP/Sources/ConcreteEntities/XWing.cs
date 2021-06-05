using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;
using System;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.CollitionInterfaces;
using Microsoft.Xna.Framework.Audio;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class XWing : KinematicEntity, IStaticDamageable, ILaserDamageable
    {
        protected override Drawer Drawer() => TGCGame.content.D_XWing;
        protected override Vector3 Scale => Vector3.One;
        protected override TypedIndex Shape => TGCGame.content.SH_XWing;

        internal readonly float maxSpeed = 200f;
        private const float acceleration = 1f;

        private readonly Vector3 baseUpDirection = Vector3.Up;
        private readonly Vector3 baseRightDirection = Vector3.Right;
        internal Vector3 forward, rightDirection, upDirection;

        private bool godMode = false;
        internal float salud = 100;

        private double lastFire;
        private const double fireCooldownTime = 200;
        private readonly AudioEmitter emitter = new AudioEmitter();
        private const float laserVolume = 0.2f;

        protected override void OnInstantiate()
        {
            base.OnInstantiate();
            UpdateOrientation(Body());
        }

        override internal void Update(double elapsedTime)
        {
            BodyReference body = Body();
            UpdateOrientation(body);

            Brakement((float)elapsedTime, body);
            Movement((float)elapsedTime, body);
            //Aligment((float)elapsedTime);
            Rotation((float)elapsedTime);

            emitter.Position = Position();
        }

        private void UpdateOrientation(BodyReference body)
        {
            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            forward = -PhysicUtils.Forward(rotation);
            rightDirection = PhysicUtils.Left(rotation);
            upDirection = PhysicUtils.Up(rotation);
        }

        internal Vector3 Position() => Body().Pose.Position.ToVector3();
        internal Vector3 Velocity() => Body().Velocity.Linear.ToVector3();
        private void Movement(float elapsedTime, BodyReference body)
        {
            Vector3 accelerationDirection = Input.Accelerate() ? forward : Vector3.Zero;
            Vector3 velocity = accelerationDirection * acceleration * elapsedTime;

            AddLinearVelocity(body, velocity);
        }

        private void Brakement(float elapsedTime, BodyReference body)
        {
            Vector3 velocity = Velocity();

            Vector3 forwardBreakment = !Input.Accelerate() ? forward : Vector3.Zero;
            float horizontalSpeed = Vector3.Dot(velocity, rightDirection) / (rightDirection.Length() * rightDirection.Length());
            Vector3 horizontalBrakment = Input.HorizontalAxis() == 0 && horizontalSpeed != 0 ? Vector3.Normalize(rightDirection * horizontalSpeed) : Vector3.Zero;
            float verticalSpeed = Vector3.Dot(velocity, upDirection) / (upDirection.Length() * upDirection.Length());
            Vector3 verticalBrakment = Input.VerticalAxis() == 0 && verticalSpeed != 0 ? Vector3.Normalize(upDirection * verticalSpeed) : Vector3.Zero;

            AddLinearVelocity(body, (forwardBreakment + horizontalBrakment + verticalBrakment) * -acceleration * elapsedTime);
        }

        private void Aligment(float elapsedTime)
        {
            float fixValue = 0.005f;


            if (!Equals(upDirection, baseUpDirection) && Input.VerticalAxis() == 0)
            {
                float angleUp = (float)Math.Acos(Vector3.Dot(baseUpDirection, upDirection));

                Quaternion alignedUpQuaternion = Quaternion.CreateFromAxisAngle(Vector3.Cross(upDirection, forward), angleUp / 2 * elapsedTime * fixValue);

                Body().Pose.Orientation *= alignedUpQuaternion.ToBEPU();
            }

            if (!Equals(rightDirection, baseRightDirection) && Input.HorizontalAxis() == 0)
            {
                float angleRight = (float)Math.Acos(Vector3.Dot(baseRightDirection, rightDirection));
                Quaternion alignedRightQuaternion = Quaternion.CreateFromAxisAngle(Vector3.Cross(rightDirection, forward), angleRight / 2 * elapsedTime * fixValue);

                Body().Pose.Orientation *= alignedRightQuaternion.ToBEPU();
            }
        }

        internal void Rotation(float elapsedTime)
        {
            var xFixValue = 0.0008f;
            var yFixValue = 0.002f;

            Quaternion horizontalRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -Input.HorizontalAxis() * elapsedTime * yFixValue);
            Quaternion verticalRotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), Input.VerticalAxis() * elapsedTime * xFixValue);

            Body().Pose.Orientation *= (horizontalRotation * verticalRotation).ToBEPU();
        }

        private void AddLinearVelocity(BodyReference body, Vector3 velocity)
        {
            var newVelocity = body.Velocity.Linear.ToVector3() + velocity;

            float forwardSpeed = Vector3.Dot(newVelocity, forward) / (forward.Length() * forward.Length());
            Vector3 forwardVelocity = Math.Clamp(forwardSpeed, 0, maxSpeed) * forward;
            Vector3 limitedVelocity = forwardVelocity;

            body.Velocity.Linear = limitedVelocity.ToBEPU();
        }

        internal void Fire(double gameTime, Vector2 mousePosition)
        {
            if (gameTime < lastFire + fireCooldownTime)
                return;

            BodyReference body = Body();

            /*float yaw = 90f-mousePosition.X;
            float pitch = 90f-mousePosition.Y;

            Vector3 mouseDirection;
            mouseDirection.X = -MathF.Cos(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch));
            mouseDirection.Y = MathF.Sin(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch));
            mouseDirection.Z = MathF.Sin(MathHelper.ToRadians(pitch));

            Vector3 fireDirection = Vector3.Normalize(mouseDirection * 50 + body.Pose.Position.ToVector3());
            Quaternion laserQuaternion = new Quaternion(fireDirection, 0);*/

            Vector3 position = body.Pose.Position.ToVector3();
            Quaternion orientation = body.Pose.Orientation.ToQuaternion();
            World.InstantiateLaser(position, -forward, orientation, emitter, laserVolume);
            lastFire = gameTime;
        }

        internal void SecondaryFire(double gameTime, Vector2 mousePosition)
        {
            if (gameTime < lastFire + fireCooldownTime)
                return;

            BodyReference body = Body();

            Vector3 position = body.Pose.Position.ToVector3();
            Quaternion orientation = body.Pose.Orientation.ToQuaternion();
            Vector3 up = PhysicUtils.Up(orientation);
            Vector3 left = PhysicUtils.Left(orientation);
            World.InstantiateLaser(position + up * 1.0f + left * 4.75459f, -forward, orientation, emitter, laserVolume);
            World.InstantiateLaser(position + up * 1.0f - left * 4.75459f, -forward, orientation, emitter, laserVolume);
            World.InstantiateLaser(position - up * 1.7f + left * 4.75459f, -forward, orientation, emitter, laserVolume);
            World.InstantiateLaser(position - up * 1.7f - left * 4.75459f, -forward, orientation, emitter, laserVolume);
            lastFire = gameTime;
        }

        private void Reiniciar()
        {
            TGCGame.content.S_Explotion.CreateInstance().Play();
            salud = 100;
            BodyReference body = Body();

            /*body.Velocity.Linear = System.Numerics.Vector3.Zero;
            body.Velocity.Angular = System.Numerics.Vector3.Zero;*/
            body.Pose.Position = System.Numerics.Vector3.Zero;
            //body.Pose.Orientation = Quaternion.Normalize(new Quaternion(baseUpDirection * baseRightDirection, 0)).ToBEPU();
        }

        internal void PerderSalud(float perdida)
        {
            if (godMode)
                return;
            salud -= perdida;
            if (salud <= 0)
                Reiniciar();
        }

        internal void ToggleGodMode() => godMode = !godMode;

        void IStaticDamageable.ReceiveStaticDamage()
        {
            Reiniciar();
        }

        void ILaserDamageable.ReceiveLaserDamage()
        {
            PerderSalud(20);
        }
    }
}