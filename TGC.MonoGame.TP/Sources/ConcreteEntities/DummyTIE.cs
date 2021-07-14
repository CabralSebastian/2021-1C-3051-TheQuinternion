using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP
{
    internal class DummyTIE : KinematicEntity
    {
        private readonly float LinearVelocity = 1000f;
        private readonly float MinX, maxX;
        private readonly float ScaleValue = 1000f;
        private readonly AudioEmitter Emitter;
        private SoundEffectInstance EngineSound;

        protected override Drawer Drawer() => TGCGame.GameContent.D_TIE;
        protected override Vector3 Scale => Vector3.One * ScaleValue;
        protected override TypedIndex Shape => TGCGame.GameContent.SH_TIE;

        internal DummyTIE(float linearVelocity, float minX, float maxX, float scale = 1 / 100f)
        {
            this.MinX = minX;
            this.maxX = maxX;
            this.LinearVelocity = linearVelocity;
            this.ScaleValue = scale;
            Emitter = new AudioEmitter { DopplerScale = 1000 };
        }

        protected override void OnInstantiate()
        {
            BodyReference body = Body();
            System.Numerics.Vector3 velocity = PhysicUtils.Forward(body.Pose.Orientation.ToQuaternion()).ToBEPU() * LinearVelocity;
            body.Velocity.Linear = velocity;

            EngineSound = TGCGame.GameContent.S_TIEEngine.CreateInstance();
            EngineSound.IsLooped = true;
            EngineSound.Volume = 0.001f;
            Emitter.Velocity = velocity.ToVector3();
            TGCGame.SoundManager.PlaySound(EngineSound, Emitter);
        }

        internal override void Update(double elapsedTime, GameTime gameTime)
        {
            BodyReference body = Body();
            if (body.Pose.Position.X > maxX)
                body.Pose.Position.X = MinX;
            else if (body.Pose.Position.X < MinX)
                body.Pose.Position.X = maxX;
            Emitter.Position = body.Pose.Position.ToVector3();
        }

        internal override void Destroy()
        {
            EngineSound.Stop();
            base.Destroy();
        }
    }
}