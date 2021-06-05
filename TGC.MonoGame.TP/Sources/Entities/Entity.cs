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
            TGCGame.currentScene.Register(this);
            OnInstantiate();
        }

        internal virtual void Destroy() => TGCGame.currentScene.Unregister(this);

        protected virtual void OnInstantiate() { }
        internal virtual void Update(double elapsedTime) { }

        internal virtual void Draw() => Drawer().Draw(GeneralWorldMatrix());
    }
}