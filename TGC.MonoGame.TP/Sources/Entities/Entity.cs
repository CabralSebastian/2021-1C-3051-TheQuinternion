using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Drawers;

namespace TGC.MonoGame.TP.Entities
{
    internal abstract class Entity
    {
        protected abstract Drawer Drawer();
        protected abstract Matrix GeneralWorldMatrix();

        internal virtual void Instantiate(Vector3 position, Quaternion rotation)
        {
            TGCGame.CurrentScene.Register(this);
            OnInstantiate();
        }

        internal virtual void Destroy() => TGCGame.CurrentScene.Unregister(this);

        protected virtual void OnInstantiate() { }
        internal virtual void Update(double elapsedTime, GameTime gameTime) { }

        internal virtual void Draw() => Drawer().Draw(GeneralWorldMatrix());
    }
}