using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Drawers
{
    internal abstract class Drawer
    {
        internal virtual void Draw(Matrix generalWorldMatrix) { }
    }
}