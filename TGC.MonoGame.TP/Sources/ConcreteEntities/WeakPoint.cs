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
        protected override Drawer Drawer() => TGCGame.GameContent.D_WeakPoint;
        protected override Vector3 Scale => Vector3.One * DeathStar.TrenchScale;
        protected override TypedIndex Shape => TGCGame.GameContent.SH_WeakPoint;

        internal Vector3 GetPosition => Position;
        private float Health = 800;

        void ILaserDamageable.ReceiveLaserDamage()
        {
            Health -= 40;
            if (Health < 0)
                TGCGame.Game.ChangeScene(new Ending());
        }
    }
}