using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.CollitionInterfaces;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP
{
    internal class Laser : KinematicEntity
    {
        private const float LinearVelocity = 1000f;
        internal const float Radius = 0.2f;
        internal const float Lenght = 5f;

        protected override Drawer Drawer() => TGCGame.content.D_Laser;
        protected override Vector3 Scale => new Vector3(Radius, Radius, Lenght) / 100f;
        protected override TypedIndex Shape => TGCGame.content.SH_Laser;

        protected override void OnInstantiate() {
            BodyReference body = Body();
            body.Velocity.Linear = -PhysicUtils.Forward(body.Pose.Orientation.ToQuaternion()).ToBEPU() * LinearVelocity;
        }

        internal override void Update(double elapsedTime, GameTime gameTime)
        {
            if (Body().Pose.Position.Length() > 100000f)
                Destroy();
        }

        public override bool HandleCollition(ICollitionHandler other)
        {
            if (!Destroyed) {
                if (other is ILaserDamageable damageable)
                    damageable.ReceiveLaserDamage();
                Destroy();
            }
            return false;
        }
    }
}