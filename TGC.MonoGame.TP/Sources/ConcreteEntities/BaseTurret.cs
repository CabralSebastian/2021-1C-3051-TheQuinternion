using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal abstract class BaseTurret : StaticPhysicEntity
    {
        protected abstract float MaxRange { get; }
        //protected abstract float MinIdleTime { get; }

        private double idleTime = 0;

        protected bool IsInRange(float distance) => distance < MaxRange;

        /*internal override void Update(double elapsedTime)
        {
            Vector3 difference = TGCGame.world.xwing.Position() - (Position + cannonsOffset);
            float distance = difference.Length();

            if (IsInRange(distance))
            {
                Aim();
                if (idleTime > MinIdleTime && IsAimed())
                {
                    Fire();
                    idleTime = 0;
                }
            }
            else
                idleTime += elapsedTime;
        }

        protected abstract void Aim();
        protected abstract bool IsAimed();
        protected abstract void Fire();*/

        public override bool HandleCollition(ICollitionHandler other)
        {
            base.HandleCollition(other);
            Destroy();
            return false;
        }
    }
}