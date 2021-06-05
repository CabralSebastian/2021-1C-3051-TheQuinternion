using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
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

        protected override Drawer Drawer() => TGCGame.content.D_TIE;
        protected override Vector3 Scale => Vector3.One * scale;
        protected override TypedIndex Shape => TGCGame.content.Sh_Sphere20;

        internal DummyTIE(float linearVelocity, float minX, float maxX, float scale = 1 / 100f)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.linearVelocity = linearVelocity;
            this.scale = scale;
        }

        protected override void OnInstantiate()
        {
            BodyReference body = Body();
            body.Velocity.Linear = PhysicUtils.Forward(body.Pose.Orientation.ToQuaternion()).ToBEPU() * linearVelocity;
        }

        internal override void Update(double elapsedTime)
        {
            if (Body().Pose.Position.X > maxX)
                Body().Pose.Position.X = minX;
            else if (Body().Pose.Position.X < minX)
                Body().Pose.Position.X = maxX;
        }
    }
}