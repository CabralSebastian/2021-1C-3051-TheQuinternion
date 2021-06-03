using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal abstract class BaseTurret : StaticPhysicEntity
    {
        protected override Vector3 Scale => Vector3.One * DeathStar.trenchScale;

        protected abstract float MaxRange { get; }
        protected abstract float MinIdleTime { get; }
        protected abstract Vector3 CannonsOffset { get; }

        protected float headAngle = 0f, cannonsAngle = 0f;

        protected Vector3 CannonsPosition;
        private double idleTime = 0d;

        protected float health = 100f;

        protected bool IsInRange(float distance) => distance < MaxRange;

        protected override void OnInstantiate()
        {
            CannonsPosition = Position + CannonsOffset;
        }

        internal override void Update(double elapsedTime)
        {
            Vector3 difference = TGCGame.world.xwing.Position() - CannonsPosition;
            float distance = difference.Length();

            if (IsInRange(distance))
            {
                PhysicUtils.DirectionToEuler(difference, distance, out float objectiveYaw, out float objectivePitch);
                Aim(difference, objectiveYaw - headAngle, objectivePitch - cannonsAngle, elapsedTime);
                if (IsAimed(Math.Abs(objectiveYaw - headAngle), Math.Abs(objectivePitch - cannonsAngle)) && idleTime > MinIdleTime)
                {
                    Fire();
                    idleTime = 0;
                }
                else
                    idleTime += elapsedTime;
            }
            else
                idleTime += elapsedTime;
        }

        protected abstract void Aim(Vector3 difference, float yawDifference, float pitchDifference, double elapsedTime);
        protected abstract bool IsAimed(float yawDifference, float pitchDifference);
        protected abstract void Fire();

        public override bool HandleCollition(ICollitionHandler other)
        {
            base.HandleCollition(other);
            health -= 20;
            if (health <= 0)
                Destroy();
            return false;
        }
    }
}