using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.CollitionInterfaces;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.Entities
{
    internal abstract class StaticPhysicEntity : StaticEntity, ICollitionHandler
    {
        private StaticHandle Handle;
        protected abstract TypedIndex Shape { get; }

        internal void Instantiate(Vector3 position) => Instantiate(position, Quaternion.Identity);
        internal override void Instantiate(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            Handle = TGCGame.PhysicsSimulation.CreateStatic(position, rotation, Shape);
            TGCGame.PhysicsSimulation.CollitionEvents.RegisterCollider(Handle, this);
            base.Instantiate(position, rotation);
        }

        internal override void Destroy()
        {
            TGCGame.PhysicsSimulation.CollitionEvents.UnregisterCollider(Handle);
            TGCGame.PhysicsSimulation.DestroyStatic(Handle);
            base.Destroy();
        }

        public virtual bool HandleCollition(ICollitionHandler other)
        {
            if (other is IStaticDamageable damageable)
                damageable.ReceiveStaticDamage();
            return false;
        }
    }
}