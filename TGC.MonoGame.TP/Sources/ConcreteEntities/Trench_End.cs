using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class Trench_End : StaticPhysicEntity
    {
        protected override Drawer Drawer() => new BasicDrawer(TGCGame.content.M_Trench_End, TGCGame.content.T_Trench);
        protected override Vector3 Scale => Vector3.One * DeathStar.trenchScale;
        protected override TypedIndex Shape => TGCGame.content.Sh_Trench_End;
    }
}