using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using TGC.MonoGame.TP.CollitionInterfaces;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP
{
    internal class ShellTIE : DynamicEntity, IStaticDamageable, ILaserDamageable
    {
        private readonly AudioEmitter Emitter = new AudioEmitter();
        private SoundEffectInstance EngineSound;

        protected override Drawer Drawer() => TGCGame.GameContent.D_TIE;
        protected override Vector3 Scale => Vector3.One / 100 * 2;
        protected override TypedIndex Shape => TGCGame.GameContent.SH_TIE;

        protected double LastFire = 0;
        private const double FireCooldownTime = 400;
        private const float LaserVolume = 0f;
        private readonly Random Random = new Random();
        protected override float Mass => 100f;

        private Vector3 InitialPos;

        protected override void OnInstantiate()
        {
            BodyReference body = Body();
            body.Pose.Orientation = Quaternion.CreateFromYawPitchRoll(-MathHelper.PiOver2, MathHelper.ToRadians(10), 0).ToBEPU();

            EngineSound = TGCGame.GameContent.S_TIEEngine.CreateInstance();
            EngineSound.IsLooped = true;
            EngineSound.Volume = 0.001f;

            InitialPos = body.Pose.Position.ToVector3();
            Emitter.Position = body.Pose.Position.ToVector3();
            TGCGame.SoundManager.PlaySound(EngineSound, Emitter);
        }

        internal override void Update(double elapsedTime, GameTime gameTime)
        {
            Body().Pose.Position = TGCGame.Camera.Position.ToBEPU() + InitialPos.ToBEPU();
            if (Random.NextDouble() > 0.994)
                Fire(gameTime);
        }

        protected void Fire(GameTime gameTime)
        {
            double totalTime = gameTime.TotalGameTime.TotalMilliseconds;
            if (totalTime < LastFire + FireCooldownTime)
                return;

            BodyReference body = Body();
            Vector3 position = body.Pose.Position.ToVector3();
            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);
            Quaternion laserOrientation = PhysicUtils.DirectionsToQuaternion(forward, Vector3.Up);
            World.InstantiateLaser(position, -forward, laserOrientation, Emitter, LaserVolume);
            LastFire = totalTime;
        }

        internal override void Destroy()
        {
            EngineSound.Stop();
            base.Destroy();
        }

        void IStaticDamageable.ReceiveStaticDamage()
        {
            Destroy();
        }

        void ILaserDamageable.ReceiveLaserDamage()
        {
            Destroy();
        }
    }
}