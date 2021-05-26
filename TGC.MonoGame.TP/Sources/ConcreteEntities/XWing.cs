using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;
using System;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.CollitionInterfaces;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class XWing : DynamicEntity, IStaticDamageable, ILaserDamageable
    {
        protected override Drawer Drawer() => new BasicDrawer(TGCGame.content.M_XWing, TGCGame.content.T_XWing);
        protected override Vector3 Scale => Vector3.One;
        protected override TypedIndex Shape => TGCGame.content.SH_XWing;
        protected override float Mass => 100f;

        internal readonly float maxSpeed = 200f;
        private const float acceleration = 1f;

        private readonly Vector3 baseUpDirection = Vector3.Up;
        private readonly Vector3 baseRightDirection = Vector3.Right;
        internal Vector3 forward, rightDirection, upDirection;

        internal float angleUp;

        override internal void Update(double elapsedTime)
        {
            BodyReference body = Body();
            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            forward = -PhysicUtils.Forward(rotation);
            rightDirection = PhysicUtils.Left(rotation);
            upDirection = PhysicUtils.Up(rotation);

            Brakement((float)elapsedTime, body);
            Movement((float)elapsedTime, body);
            Aligment((float)elapsedTime);
            Rotation((float)elapsedTime);
        }

        internal Vector3 Position() => Body().Pose.Position.ToVector3();
        internal Vector3 Velocity() => Body().Velocity.Linear.ToVector3();
        private void Movement(float elapsedTime, BodyReference body)
        {
            Vector3 accelerationDirection = Input.Accelerate() ? forward : Vector3.Zero; // Vector3 normalizedDirection = !Equals(accelerationDirection, Vector3.Zero) ? Vector3.Normalize(accelerationDirection) : Vector3.Zero;
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
                angleUp = (float)Math.Acos(Vector3.Dot(baseUpDirection, upDirection));

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
            //Quaternion frontalRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), -Input.HorizontalAxis() * elapsedTime * 0.0002f);

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
        /*private void AddAngularVelocity(BodyReference body, Vector3 velocity)
        {
            var castedVelocity = new System.Numerics.Vector3(velocity.X, velocity.Y, velocity.Z);
            body.Velocity.Angular += castedVelocity;
        }*/

        internal void Fire()
        {
            BodyReference body = Body();
            new Laser().Instantiate(body.Pose.Position.ToVector3() + forward * 10, body.Pose.Orientation.ToQuaternion());
        }

        public override bool HandleCollition(ICollitionHandler other)
        {
            //TGCGame.content.S_Explotion.CreateInstance().Play();
            return false;
        }


        void IStaticDamageable.ReceiveStaticDamage()
        {
            //TGCGame.content.S_Explotion.CreateInstance().Play();
        }

        void ILaserDamageable.ReceiveLaserDamage()
        {
            throw new NotImplementedException();
        }
    }
}