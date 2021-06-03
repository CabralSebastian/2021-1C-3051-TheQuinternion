using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class Trench2 : StaticPhysicEntity
    {
        protected override Drawer Drawer() => TGCGame.content.D_Trench2;
        protected override Vector3 Scale => Vector3.One / 100f;
        protected override TypedIndex Shape => TGCGame.content.Sh_Sphere20;
    }
}