using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Numerics;
using TGC.MonoGame.TP.CollitionInterfaces;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP
{
    internal class ShellXWing : DynamicEntity, IStaticDamageable, ILaserDamageable
    {
        private readonly AudioEmitter emitter;
        private SoundEffectInstance engineSound;

        protected override Drawer Drawer() => TGCGame.content.D_XWing;
        protected override Microsoft.Xna.Framework.Vector3 Scale => Microsoft.Xna.Framework.Vector3.One;
        protected override TypedIndex Shape => TGCGame.content.SH_XWing;

        protected double LastFire = 0;
        private const double FireCooldownTime = 400;
        private const float LaserVolume = 0.2f;
        private readonly Random random = new Random();
        private BodyReference body;
        protected override float Mass => 100f;

        protected override void OnInstantiate()
        {
            body = Body();
            body.Pose.Orientation = new Microsoft.Xna.Framework.Quaternion(new Microsoft.Xna.Framework.Vector3(-3, 0, 0), 1f).ToBEPU();

            engineSound = TGCGame.content.S_TIEEngine.CreateInstance();
            engineSound.IsLooped = true;
            engineSound.Volume = 0.001f;
            TGCGame.soundManager.PlaySound(engineSound, emitter);
        }

        internal override void Update(double elapsedTime, GameTime gameTime)
        {
            if (random.NextDouble() > 0.95)
            {
                Fire(gameTime);
            }
        }

        protected void Fire(GameTime gameTime)
        {
            double totalTime = gameTime.TotalGameTime.TotalMilliseconds;
            if (totalTime < LastFire + FireCooldownTime)
                return;

            Microsoft.Xna.Framework.Vector3 position = body.Pose.Position.ToVector3();
            Microsoft.Xna.Framework.Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Microsoft.Xna.Framework.Vector3 forward = PhysicUtils.Forward(rotation);
            Microsoft.Xna.Framework.Quaternion laserOrientation = PhysicUtils.DirectionsToQuaternion(forward, Microsoft.Xna.Framework.Vector3.Up);
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