using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal abstract class BaseTurret : StaticPhysicEntity
    {
        protected abstract float MaxRange { get; }
        protected abstract float MinIdleTime { get; }
        protected abstract Vector3 CannonsOffset { get; }

        protected float HeadAngle = 0f, CannonsAngle = 0f;

        protected Vector3 CannonsPosition;
        private double IdleTime = 0d;
        protected readonly AudioEmitter Emitter = new AudioEmitter();

        protected float Health = 100f;

        protected bool IsInRange(float distance) => distance < MaxRange;

        protected override void OnInstantiate()
        {
            CannonsPosition = Position + CannonsOffset;
            Emitter.Position = CannonsPosition;
        }

        internal override void Update(double elapsedTime, GameTime gameTime)
        {
            if (World.XWing == null)
                return;

            Vector3 difference = World.XWing.Position() - CannonsPosition;
            float distance = difference.Length();

            if (IsInRange(distance))
            {
                PhysicUtils.DirectionToEuler(difference, distance, out float objectiveYaw, out float objectivePitch);
                Aim(difference, objectiveYaw - HeadAngle, objectivePitch - CannonsAngle, elapsedTime);
                if (IsAimed(Math.Abs(objectiveYaw - HeadAngle), Math.Abs(objectivePitch - CannonsAngle)) && IdleTime > MinIdleTime)
                {
                    Fire();
                    IdleTime = 0;
                }
                else
                    IdleTime += elapsedTime;
            }
            else
                IdleTime += elapsedTime;
        }

        protected abstract void Aim(Vector3 difference, float yawDifference, float pitchDifference, double elapsedTime);
        protected abstract bool IsAimed(float yawDifference, float pitchDifference);
        protected abstract void Fire();

        public override bool HandleCollition(ICollitionHandler other)
        {
            base.HandleCollition(other);
            Health -= 20;
            if (Health <= 0)
            {
                TGCGame.SoundManager.PlaySound(TGCGame.GameContent.S_Explotion.CreateInstance(), Emitter);
                Destroy();
            }
            return false;
        }
    }
}