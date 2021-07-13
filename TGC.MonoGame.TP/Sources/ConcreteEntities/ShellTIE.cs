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
        private readonly AudioEmitter emitter = new AudioEmitter();
        private SoundEffectInstance engineSound;

        protected override Drawer Drawer() => TGCGame.content.D_TIE;
        protected override Vector3 Scale => Vector3.One / 100 * 2;
        protected override TypedIndex Shape => TGCGame.content.SH_TIE;

        protected double LastFire = 0;
        private const double FireCooldownTime = 400;
        private const float LaserVolume = 0f;
        private readonly Random random = new Random();
        protected override float Mass => 100f;

        private Vector3 initialPos;

        protected override void OnInstantiate()
        {
            BodyReference body = Body();
            body.Pose.Orientation = Quaternion.CreateFromYawPitchRoll(-MathHelper.PiOver2, MathHelper.ToRadians(10), 0).ToBEPU();

            engineSound = TGCGame.content.S_TIEEngine.CreateInstance();
            engineSound.IsLooped = true;
            engineSound.Volume = 0.001f;

            initialPos = body.Pose.Position.ToVector3();
            emitter.Position = body.Pose.Position.ToVector3();
            TGCGame.soundManager.PlaySound(engineSound, emitter);
        }

        internal override void Update(double elapsedTime, GameTime gameTime)
        {
            Body().Pose.Position = initialPos.ToBEPU();
            if (random.NextDouble() > 0.992)
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
            World.InstantiateLaser(position, -forward, laserOrientation, emitter, LaserVolume);
            LastFire = totalTime;
        }

        internal override void Destroy()
        {
            engineSound.Stop();
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