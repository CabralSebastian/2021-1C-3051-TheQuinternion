using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;
using System;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class XWing : KinematicEntity
    {
        protected override Model Model() => TGCGame.content.M_XWing;
        protected override Texture2D[] Textures() => TGCGame.content.T_XWing;
        protected override Vector3 Scale => Vector3.One;
        protected override TypedIndex Shape => TGCGame.content.SH_XWing;
        //protected override float Mass => 100f;

        internal readonly float maxSpeed = 500f;
        private const float acceleration = 0.5f;

        private Vector3 baseUpDirection = Vector3.Up;
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
        }

        internal Vector3 Position() => Body().Pose.Position.ToVector3();
        internal Vector3 Velocity() => Body().Velocity.Linear.ToVector3();
        private void Movement(float elapsedTime, BodyReference body)
        {
            Vector3 directionToMove = Input.HorizontalAxis() * rightDirection + Input.ForwardAxis() * forward + Input.VerticalAxis() * upDirection;
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

        internal void RotateY(float axis, float elapsedTime)
        {
            var fixValue = 0.0001f;
            Body().Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), axis * elapsedTime * fixValue).ToBEPU();
        }
        
        private void AddLinearVelocity(BodyReference body, Vector3 velocity)
        {
            var newVelocity = body.Velocity.Linear.ToVector3() + velocity;

            float forwardSpeed = Vector3.Dot(newVelocity, forward) / (forward.Length() * forward.Length());
            float horizontalSpeed = Vector3.Dot(newVelocity, rightDirection) / (rightDirection.Length() * rightDirection.Length());
            float verticalSpeed = Vector3.Dot(newVelocity, upDirection) / (upDirection.Length() * upDirection.Length());

            Vector3 forwardVelocity = Math.Clamp(forwardSpeed, 0, maxSpeed) * forward;
            Vector3 horizontalVelocity = Math.Clamp(horizontalSpeed, -maxSpeed, maxSpeed) * rightDirection;
            Vector3 verticalVelocity = Math.Clamp(verticalSpeed, -maxSpeed, maxSpeed) * upDirection;

            /*
            double upAngle = Math.Acos(Vector3.Dot(baseUpDirection, upDirection) / (baseUpDirection.Length() * upDirection.Length()));
            if (horizontalSpeed != 0 && Math.Abs(upAngle) < Math.PI/4)
                body.Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), horizontalSpeed * 0.00001f).ToBEPU();

            if (!Vector3.Equals(baseUpDirection, upDirection))
                body.Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), (float)-upAngle * 0.00001f).ToBEPU();
            */

            Vector3 limitedVelocity = forwardVelocity + horizontalVelocity + verticalVelocity;

            body.Velocity.Linear = limitedVelocity.ToBEPU();
        }
        /*private void AddAngularVelocity(BodyReference body, Vector3 velocity)
        {
            var castedVelocity = new System.Numerics.Vector3(velocity.X, velocity.Y, velocity.Z);
            body.Velocity.Angular += castedVelocity;
        }*/


        public override bool HandleCollition(ICollitionHandler other)
        {
            //TGCGame.content.S_Explotion.CreateInstance().Play();
            return false;
        }
    }
}