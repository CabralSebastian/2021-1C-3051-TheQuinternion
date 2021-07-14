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
        protected override Drawer Drawer() => TGCGame.GameContent.D_XWing;
        protected override Vector3 Scale => Vector3.One;
        protected override TypedIndex Shape => TGCGame.GameContent.SH_XWing;

        internal readonly float MinSpeed = 200f;
        internal readonly float MaxSpeed = 280f;
        private const float Acceleration = 1f;

        private readonly Vector3 RotationFixValues = new Vector3(0.0008f, 0.002f, 0.004f);
        private readonly Vector3 BaseUpDirection = Vector3.Up;
        private readonly Vector3 BaseRightDirection = Vector3.Right;
        internal Vector3 Forward, RightDirection, UpDirection;
        private Vector2 MovementAxis = Vector2.Zero;

        internal bool GodMode = false;
        internal float Health = 100;
        internal const float MaxTurbo = 3000;
        private const float TurboRegeneration = 0.3f;
        private double LastTurbo = 0;
        private const double TurboRegenerationTime = 2000f;
        internal float Turbo = MaxTurbo;

        private double LastFire;
        private const double FireCooldownTime = 400;

        private bool BarrelRollActivated = false;
        private float RotationValue = 25f;
        private float PreviousHealth;
        private float BarrelRollCooldownTime = 1250;

        private readonly AudioEmitter Emitter = new AudioEmitter();
        private const float LaserVolume = 0.2f;
        private SoundEffectInstance EngineSound;

        protected override void OnInstantiate()
        {
            base.OnInstantiate();
            UpdateOrientation(Body());

            EngineSound = TGCGame.GameContent.S_TIEEngine.CreateInstance();
            EngineSound.IsLooped = true;
            EngineSound.Volume = 0.01f;
            TGCGame.SoundManager.PlaySound(EngineSound, Emitter);
        }

        override internal void Update(double elapsedTime, GameTime gameTime)
        {
            BodyReference body = Body();
            UpdateOrientation(body);

            Brakement((float)elapsedTime, body);
            Movement((float)elapsedTime, body);
            Aligment((float)elapsedTime);
            Rotation((float)elapsedTime);

            Emitter.Position = Position();
            Emitter.Forward = Forward;
            Emitter.Up = UpDirection;
            Emitter.Velocity = body.Velocity.Linear.ToVector3();

            if (Input.Accelerate())
                LastTurbo = gameTime.TotalGameTime.TotalMilliseconds;
            if (gameTime.TotalGameTime.TotalMilliseconds > LastTurbo + TurboRegenerationTime)
                Turbo = Math.Min(Turbo + TurboRegeneration * (float)elapsedTime, MaxTurbo);

            if (BarrelRollActivated)
                BarrelRoll();
            else
            {
                BarrelRollCooldownTime += 20;
                BarrelRollCooldownTime = Math.Min(BarrelRollCooldownTime, 1250);
            }

            TGCGame.GameContent.E_MainShader.Parameters["bloomColor"].SetValue(Color.DarkRed.ToVector3() * body.Velocity.Linear.Length() / MaxSpeed * 20000);
        }

        private void UpdateOrientation(BodyReference body)
        {
            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Forward = -PhysicUtils.Forward(rotation);
            RightDirection = PhysicUtils.Left(rotation);
            UpDirection = PhysicUtils.Up(rotation);
        }

        internal Vector3 Position() => Body().Pose.Position.ToVector3();
        internal Vector3 Velocity() => Body().Velocity.Linear.ToVector3();
        private void Movement(float elapsedTime, BodyReference body)
        {
            if (Input.Accelerate())
            {
                float speed = Acceleration * elapsedTime;
                speed = Math.Min(speed, Turbo);
                AddLinearVelocity(body, Forward * speed);
                Turbo -= speed;
            }
            else if (Input.Deaccelerate())
            {
                float speed = Acceleration * elapsedTime;
                ReduceLinearVelocity(body, -(Forward * speed));
            }
            else
                AddLinearVelocity(body, Vector3.Zero);

        }

        private void Brakement(float elapsedTime, BodyReference body)
        {
            Vector3 velocity = Velocity();
            float brakmentForce = -Acceleration / 10;

            Vector3 forwardBreakment = (!Input.Deaccelerate() || !Input.Accelerate() || Turbo == 0) ? Forward : Vector3.Zero;
            float horizontalSpeed = Vector3.Dot(velocity, RightDirection) / (RightDirection.Length() * RightDirection.Length());
            Vector3 horizontalBrakment = horizontalSpeed != 0 ? Vector3.Normalize(RightDirection * horizontalSpeed) : Vector3.Zero;
            float verticalSpeed = Vector3.Dot(velocity, UpDirection) / (UpDirection.Length() * UpDirection.Length());
            Vector3 verticalBrakment = verticalSpeed != 0 ? Vector3.Normalize(UpDirection * verticalSpeed) : Vector3.Zero;

            AddLinearVelocity(body, (forwardBreakment + horizontalBrakment + verticalBrakment) * brakmentForce * elapsedTime);
        }
        internal void Aligment(float elapsedTime)
        {
            Body().Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), -Input.FrontalRotationAxis() * elapsedTime * RotationFixValues.Z).ToBEPU();
        }
        internal void Move(Vector2 directionAxis) { MovementAxis = directionAxis; }

        internal void Rotation(float elapsedTime)
        {
            Quaternion horizontalRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -Math.Sign(Input.HorizontalAxis() + MovementAxis.X) * elapsedTime * RotationFixValues.Y);
            Quaternion verticalRotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), Math.Sign(Input.VerticalAxis() - MovementAxis.Y) * elapsedTime * RotationFixValues.X);

            Body().Pose.Orientation *= (horizontalRotation * verticalRotation).ToBEPU();
        }

        private void AddLinearVelocity(BodyReference body, Vector3 velocity)
        {
            var newVelocity = body.Velocity.Linear.ToVector3() + velocity;

            float forwardSpeed = Vector3.Dot(newVelocity, Forward) / (Forward.Length() * Forward.Length());
            Vector3 forwardVelocity = Math.Clamp(forwardSpeed, MinSpeed, MaxSpeed) * Forward;
            Vector3 limitedVelocity = forwardVelocity;

            body.Velocity.Linear = limitedVelocity.ToBEPU();
        }

        private void ReduceLinearVelocity(BodyReference body, Vector3 velocity)
        {
            var newVelocity = body.Velocity.Linear.ToVector3() + velocity;

            float forwardSpeed = Vector3.Dot(newVelocity, Forward) / (Forward.Length() * Forward.Length());
            Vector3 forwardVelocity = Math.Clamp(forwardSpeed, 50, 150) * Forward;
            Vector3 limitedVelocity = forwardVelocity;

            body.Velocity.Linear = limitedVelocity.ToBEPU();
        }

        internal void Fire(double gameTime)
        {
            if (gameTime < LastFire + FireCooldownTime)
                return;

            BodyReference body = Body();
            Vector3 position = body.Pose.Position.ToVector3();
            Quaternion laserOrientation = PhysicUtils.DirectionsToQuaternion(TGCGame.Camera.MouseDirection(position), UpDirection);
            World.InstantiateLaser(position, -Forward, laserOrientation, Emitter, LaserVolume);
            LastFire = gameTime;
        }

        internal void SecondaryFire(double gameTime, Vector2 mousePosition)
        {
            if (gameTime < LastFire + FireCooldownTime)
                return;

            BodyReference body = Body();

            Vector3 position = body.Pose.Position.ToVector3();
            Quaternion laserOrientation = PhysicUtils.DirectionsToQuaternion(TGCGame.Camera.MouseDirection(position), UpDirection);
            Vector3 up = PhysicUtils.Up(laserOrientation);
            Vector3 left = PhysicUtils.Left(laserOrientation);
            World.InstantiateLaser(position + up * 1.0f + left * 4.75459f, -Forward, laserOrientation, Emitter, LaserVolume/4);
            World.InstantiateLaser(position + up * 1.0f - left * 4.75459f, -Forward, laserOrientation, Emitter, LaserVolume/4);
            World.InstantiateLaser(position - up * 1.7f + left * 4.75459f, -Forward, laserOrientation, Emitter, LaserVolume/4);
            World.InstantiateLaser(position - up * 1.7f - left * 4.75459f, -Forward, laserOrientation, Emitter, LaserVolume/4);

            LastFire = gameTime;
        }

        internal void ToggleBarrelRoll(GameTime gameTime)
        {
            if (!BarrelRollActivated)
            {
                Body().Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), RotationValue * RotationFixValues.Z).ToBEPU();
                BarrelRollActivated = true;
                PreviousHealth = Health;
            }
            else
            {
                BarrelRollActivated = !BarrelRollActivated;
            }
        }

        private void BarrelRoll()
        {
            if (BarrelRollActivated)
            {
                Body().Pose.Orientation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), RotationValue * RotationFixValues.Z).ToBEPU();
                Health = PreviousHealth;
            }

            if (CompareUp()) 
            {
                BarrelRollActivated = false;
            }
        }

        private bool CompareUp()
        {
            BarrelRollCooldownTime -= 20;
            return BarrelRollCooldownTime <= 0; 
        }

        private void Reiniciar()
        {
            TGCGame.GameContent.S_Explotion.CreateInstance().Play();
            Health = 100;
            Turbo = MaxTurbo;
            BodyReference body = Body();

            body.Pose.Position = System.Numerics.Vector3.Zero;
        }

        internal void PerderSalud(float perdida)
        {
            if (GodMode)
                return;
            Health -= perdida;
            if (Health <= 0)
                Reiniciar();
        }

        internal void ToggleGodMode() => GodMode = !GodMode;

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