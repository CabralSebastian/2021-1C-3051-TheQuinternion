using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.CollitionInterfaces;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.Entities
{
    internal abstract class StaticPhysicEntity : StaticEntity, ICollitionHandler
    {
        private StaticHandle handle;
        protected abstract TypedIndex Shape { get; }

        internal void Instantiate(Vector3 position) => Instantiate(position, Quaternion.Identity);
        internal override void Instantiate(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            handle = TGCGame.physicSimulation.CreateStatic(position, rotation, Shape);
            TGCGame.physicSimulation.collitionEvents.RegisterCollider(handle, this);
            base.Instantiate(position, rotation);
        }

        internal override void Destroy()
        {
            TGCGame.physicSimulation.collitionEvents.UnregisterCollider(handle);
            TGCGame.physicSimulation.DestroyStatic(handle);
            base.Destroy();
        }

        public bool HandleCollition(ICollitionHandler other)
        {
            if (other is IStaticDamageable damageable)
                damageable.ReceiveStaticDamage();
            return false;
        }
    }
}