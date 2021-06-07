using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Numerics;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP
{
    internal class DummyTIE : KinematicEntity
    {
        private readonly float linearVelocity = 1000f;
        private readonly float minX, maxX;
        private readonly float scale = 1000f;
        private readonly AudioEmitter emitter;
        private SoundEffectInstance engineSound;

        protected override Drawer Drawer() => TGCGame.content.D_TIE;
        protected override Microsoft.Xna.Framework.Vector3 Scale => Microsoft.Xna.Framework.Vector3.One * scale;
        protected override TypedIndex Shape => TGCGame.content.Sh_Sphere20;

        internal DummyTIE(float linearVelocity, float minX, float maxX, float scale = 1 / 100f)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.linearVelocity = linearVelocity;
            this.scale = scale;
            emitter = new AudioEmitter { DopplerScale = 1000 };
        }

        protected override void OnInstantiate()
        {
            BodyReference body = Body();
            System.Numerics.Vector3 velocity = PhysicUtils.Forward(body.Pose.Orientation.ToQuaternion()).ToBEPU() * linearVelocity;
            body.Velocity.Linear = velocity;

            engineSound = TGCGame.content.S_TIEEngine.CreateInstance();
            engineSound.IsLooped = true;
            engineSound.Volume = 0.001f;
            emitter.Velocity = velocity.ToVector3();
            TGCGame.soundManager.PlaySound(engineSound, emitter);
        }

        internal override void Update(double elapsedTime, GameTime gameTime)
        {
            BodyReference body = Body();
            if (body.Pose.Position.X > maxX)
                body.Pose.Position.X = minX;
            else if (body.Pose.Position.X < minX)
                body.Pose.Position.X = maxX;
            emitter.Position = body.Pose.Position.ToVector3();
        }

        internal override void Destroy()
        {
            engineSound.Stop();
            base.Destroy();
        }
    }
}