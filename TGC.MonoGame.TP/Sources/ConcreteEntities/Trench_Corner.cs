using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class Trench_Corner : StaticPhysicEntity
    {
        protected override Drawer Drawer() => TGCGame.GameContent.D_Trench_Corner;
        protected override Vector3 Scale => Vector3.One * DeathStar.TrenchScale;
        protected override TypedIndex Shape => TGCGame.GameContent.Sh_Trench_Corner;
    }
}