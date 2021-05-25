using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Drawers;

namespace TGC.MonoGame.TP.Entities
{
    internal abstract class Entity
    {
        protected abstract Drawer Drawer();
        protected abstract Matrix GeneralWorldMatrix();

        internal virtual void Update(double elapsedTime) { }

        internal virtual void Draw() => Drawer().Draw(GeneralWorldMatrix());
    }
}