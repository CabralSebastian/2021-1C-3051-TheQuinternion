using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;
using System;
using TGC.MonoGame.TP.Drawers;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class XWing : DynamicEntity
    {
        protected override Drawer Drawer() => new BasicDrawer(TGCGame.content.M_XWing, TGCGame.content.T_XWing);
        protected override Vector3 Scale => Vector3.One;
        protected override TypedIndex Shape => TGCGame.content.SH_XWing;
        protected override float Mass => 100f;

        internal readonly float maxSpeed = 200f;
        private const float acceleration = 0.5f;

        private Vector3 baseUpDirection = Vector3.Up;
        private Vector3 axisRotation = Vector3.Zero;
        internal Vector3 forward, rightDirection, upDirection;

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
            //float verticalAxis = Input.VerticalAxis();
            //float horizontalAxis = Input.HorizontalAxis();

            //Vector3 directionToMove = horizontalAxis * rightDirection + Input.ForwardAxis() * forward + verticalAxis * upDirection;
            //Vector3 directionToMove = horizontalAxis * Vector3.Cross(forward, baseUpDirection) + Input.ForwardAxis() * forward;// + verticalAxis * baseUpDirection;
            Vector3 directionToMove = Input.ForwardAxis() * forward;
            Vector3 normalizedDirection = !Equals(directionToMove, Vector3.Zero) ? Vector3.Normalize(directionToMove) : Vector3.Zero;

            Vector3 velocity = normalizedDirection * acceleration * elapsedTime;
            AddLinearVelocity(body, velocity);
        }
        private void Brakement(float elapsedTime, BodyReference body)
        {
            Vector3 velocity = Velocity();

            Vector3 forwardBreakment = Input.ForwardAxis() == 0 ? forward : Vector3.Zero; 
            float horizontalSpeed = Vector3.Dot(velocity, rightDirection) / (rightDirection.Length() * rightDirection.Length());
            Vector3 horizontalBrakment = Input.HorizontalAxis() == 0 && horizontalSpeed != 0 ? Vector3.Normalize(rightDirection * horizontalSpeed) : Vector3.Zero;
            float verticalSpeed = Vector3.Dot(velocity, upDirection) / (upDirection.Length() * upDirection.Length());
            Vector3 verticalBrakment = Input.VerticalAxis() == 0 && verticalSpeed != 0 ? Vector3.Normalize(upDirection * verticalSpeed) : Vector3.Zero;

            AddLinearVelocity(body, (forwardBreakment + horizontalBrakment + verticalBrakment) * -acceleration * elapsedTime);
        }

        private void Aligment(float elapsedTime)
        {
            if (Equals(upDirection, baseUpDirection))
                return;

            float fixValue = 0.001f;
            double angle = Math.Acos(Vector3.Dot(upDirection, baseUpDirection));
            Quaternion aligment = new Quaternion(baseUpDirection, (float)Math.Cos(angle /2));

            Body().Pose.Orientation = Quaternion.Lerp(Body().Pose.Orientation.ToQuaternion(), aligment, elapsedTime * fixValue).ToBEPU();
        }
        internal void Rotation(float elapsedTime)
        {
            var xFixValue = 0.0005f;
            var yFixValue = 0.0009f;
            var zFixValue = 0.0003f;

            Quaternion rotation =
                Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), Input.VerticalAxis() * elapsedTime * xFixValue) *
                Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -Input.HorizontalAxis() * elapsedTime * yFixValue) *
                Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), axisRotation.Z * elapsedTime * zFixValue);
            Body().Pose.Orientation *= rotation.ToBEPU(); //Quaternion.Slerp(Body().Pose.Orientation.ToQuaternion(), rotation, elapsedTime * 0.003f).ToBEPU();//
            axisRotation = Vector3.Zero;
        }

        internal void AddRotation(Vector3 rotatioPerAxis)
        {
            this.axisRotation += rotatioPerAxis;
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


        public override bool HandleCollition(ICollitionHandler other)
        {
            TGCGame.content.S_Explotion.CreateInstance().Play();
            return false;
        }

        internal void Fire()
        {
            BodyReference body = Body();
            new Laser().Instantiate(body.Pose.Position.ToVector3() + forward * 20f, body.Pose.Orientation.ToQuaternion());
        }
    }
}