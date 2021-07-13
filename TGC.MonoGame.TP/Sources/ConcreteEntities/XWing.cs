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

        internal readonly float minSpeed = 200f;
        internal readonly float maxSpeed = 280f;
        private const float acceleration = 1f;

        private readonly Vector3 rotationFixValues = new Vector3(0.0008f, 0.002f, 0.004f);
        private readonly Vector3 baseUpDirection = Vector3.Up;
        private readonly Vector3 baseRightDirection = Vector3.Right;
        internal Vector3 forward, rightDirection, upDirection;
        private Vector2 movementAxis = Vector2.Zero;

        internal bool godMode = false;
        internal float salud = 100;
        internal const float maxTurbo = 3000;
        private const float turboRegeneration = 0.3f;
        private double lastTurbo = 0;
        private const double turboRegenerationTime = 2000f;
        internal float turbo = maxTurbo;

        private double lastFire;
        private const double fireCooldownTime = 400;

        private bool barrelRollActivated = false;
        private float rotationValue = 25f;
        private float previousHealth;
        private float barrelRollCooldownTime = 1250;

        private readonly AudioEmitter emitter = new AudioEmitter();
        private const float laserVolume = 0.2f;
        private SoundEffectInstance engineSound;

        protected override void OnInstantiate()
        {
            base.OnInstantiate();
            UpdateOrientation(Body());

            engineSound = TGCGame.content.S_TIEEngine.CreateInstance();
            engineSound.IsLooped = true;
            engineSound.Volume = 0.01f;
            TGCGame.soundManager.PlaySound(engineSound, emitter);
        }

        override internal void Update(double elapsedTime, GameTime gameTime)
        {
            BodyReference body = Body();
            UpdateOrientation(body);

            Brakement((float)elapsedTime, body);
            Movement((float)elapsedTime, body);
            Aligment((float)elapsedTime);
            Rotation((float)elapsedTime);

            emitter.Position = Position();
            emitter.Forward = forward;
            emitter.Up = upDirection;
            emitter.Velocity = body.Velocity.Linear.ToVector3();

            if (Input.Accelerate())
                lastTurbo = gameTime.TotalGameTime.TotalMilliseconds;
            if (gameTime.TotalGameTime.TotalMilliseconds > lastTurbo + turboRegenerationTime)
                turbo = Math.Min(turbo + turboRegeneration * (float)elapsedTime, maxTurbo);

            if (barrelRollActivated)
                BarrelRoll();
            else
            {
                barrelRollCooldownTime += 20;
                barrelRollCooldownTime = Math.Min(barrelRollCooldownTime, 1250);
            }

            TGCGame.content.E_MainShader.Parameters["bloomColor"].SetValue(Color.DarkRed.ToVector3() * body.Velocity.Linear.Length() / maxSpeed * 20000);
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
            if (Input.Accelerate())
            {
                float speed = acceleration * elapsedTime;
                speed = Math.Min(speed, turbo);
                AddLinearVelocity(body, forward * speed);
                turbo -= speed;
            }
            else if (Input.Deaccelerate())
            {
                float speed = acceleration * elapsedTime;
                ReduceLinearVelocity(body, -(forward * speed));
            }
            else
                AddLinearVelocity(body, Vector3.Zero);

        }

        private void Brakement(float elapsedTime, BodyReference body)
        {
            Vector3 velocity = Velocity();
            float brakmentForce = -acceleration / 10;

            Vector3 forwardBreakment = (!Input.Deaccelerate() || !Input.Accelerate() || turbo == 0) ? forward : Vector3.Zero;
            float horizontalSpeed = Vector3.Dot(velocity, rightDirection) / (rightDirection.Length() * rightDirection.Length());
            Vector3 horizontalBrakment = horizontalSpeed != 0 ? Vector3.Normalize(rightDirection * horizontalSpeed) : Vector3.Zero;
            float verticalSpeed = Vector3.Dot(velocity, upDirection) / (upDirection.Length() * upDirection.Length());
            Vector3 verticalBrakment = verticalSpeed != 0 ? Vector3.Normalize(upDirection * verticalSpeed) : Vector3.Zero;

            AddLinearVelocity(body, (forwardBreakment + horizontalBrakment + verticalBrakment) * brakmentForce * elapsedTime);
        }
        internal void Aligment(float elapsedTime)
        {
            Body().Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), -Input.FrontalRotationAxis() * elapsedTime * rotationFixValues.Z).ToBEPU();
        }
        internal void Move(Vector2 directionAxis) { movementAxis = directionAxis; }

        internal void Rotation(float elapsedTime)
        {
            Quaternion horizontalRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -Math.Sign(Input.HorizontalAxis() + movementAxis.X) * elapsedTime * rotationFixValues.Y);
            Quaternion verticalRotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), Math.Sign(Input.VerticalAxis() - movementAxis.Y) * elapsedTime * rotationFixValues.X);

            Body().Pose.Orientation *= (horizontalRotation * verticalRotation).ToBEPU();
        }

        private void AddLinearVelocity(BodyReference body, Vector3 velocity)
        {
            var newVelocity = body.Velocity.Linear.ToVector3() + velocity;

            float forwardSpeed = Vector3.Dot(newVelocity, forward) / (forward.Length() * forward.Length());
            Vector3 forwardVelocity = Math.Clamp(forwardSpeed, minSpeed, maxSpeed) * forward;
            Vector3 limitedVelocity = forwardVelocity;

            body.Velocity.Linear = limitedVelocity.ToBEPU();
        }

        private void ReduceLinearVelocity(BodyReference body, Vector3 velocity)
        {
            var newVelocity = body.Velocity.Linear.ToVector3() + velocity;

            float forwardSpeed = Vector3.Dot(newVelocity, forward) / (forward.Length() * forward.Length());
            Vector3 forwardVelocity = Math.Clamp(forwardSpeed, 50, 150) * forward;
            Vector3 limitedVelocity = forwardVelocity;

            body.Velocity.Linear = limitedVelocity.ToBEPU();
        }

        internal void Fire(double gameTime)
        {
            if (gameTime < lastFire + fireCooldownTime)
                return;

            BodyReference body = Body();
            Vector3 position = body.Pose.Position.ToVector3();
            Quaternion laserOrientation = PhysicUtils.DirectionsToQuaternion(TGCGame.camera.MouseDirection(position), upDirection);
            World.InstantiateLaser(position, -forward, laserOrientation, emitter, laserVolume);
            lastFire = gameTime;
        }

        internal void SecondaryFire(double gameTime, Vector2 mousePosition)
        {
            if (gameTime < lastFire + fireCooldownTime)
                return;

            BodyReference body = Body();

            Vector3 position = body.Pose.Position.ToVector3();
            Quaternion laserOrientation = PhysicUtils.DirectionsToQuaternion(TGCGame.camera.MouseDirection(position), upDirection);
            Vector3 up = PhysicUtils.Up(laserOrientation);
            Vector3 left = PhysicUtils.Left(laserOrientation);
            World.InstantiateLaser(position + up * 1.0f + left * 4.75459f, -forward, laserOrientation, emitter, laserVolume/4);
            World.InstantiateLaser(position + up * 1.0f - left * 4.75459f, -forward, laserOrientation, emitter, laserVolume/4);
            World.InstantiateLaser(position - up * 1.7f + left * 4.75459f, -forward, laserOrientation, emitter, laserVolume/4);
            World.InstantiateLaser(position - up * 1.7f - left * 4.75459f, -forward, laserOrientation, emitter, laserVolume/4);

            lastFire = gameTime;
        }

        internal void ToggleBarrelRoll(GameTime gameTime)
        {
            if (!barrelRollActivated)
            {
                Body().Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), rotationValue * rotationFixValues.Z).ToBEPU();
                barrelRollActivated = true;
                previousHealth = salud;
            }
            else
            {
                barrelRollActivated = !barrelRollActivated;
            }
        }

        private void BarrelRoll()
        {
            if (barrelRollActivated)
            {
                Body().Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), rotationValue * rotationFixValues.Z).ToBEPU();
                salud = previousHealth;
            }

            if (CompareUp()) 
            {
                barrelRollActivated = false;
            }
        }

        private bool CompareUp()
        {
            barrelRollCooldownTime -= 20;
            return barrelRollCooldownTime <= 0; 
        }

        private void Reiniciar()
        {
            TGCGame.content.S_Explotion.CreateInstance().Play();
            salud = 100;
            turbo = maxTurbo;
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
            PerderSalud(15);
        }

        public override bool HandleCollition(ICollitionHandler other)
        {
            if (!Destroyed)
            {
                if (other is TIE _)
                    Reiniciar();
            }
            return false;
        }
    }
}