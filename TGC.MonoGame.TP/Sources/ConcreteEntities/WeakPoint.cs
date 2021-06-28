using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.CollitionInterfaces;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class WeakPoint : StaticPhysicEntity, ILaserDamageable
    {
        protected override Drawer Drawer() => TGCGame.content.D_WeakPoint;
        protected override Vector3 Scale => Vector3.One * DeathStar.trenchScale;
        protected override TypedIndex Shape => TGCGame.content.SH_WeakPoint;

        internal Vector3 GetPosition => Position;
        private float health = 800;

        void ILaserDamageable.ReceiveLaserDamage()
        {
            health -= 40;
            if (health < 0)
                TGCGame.game.ChangeScene(new Ending());
        }
    }
}