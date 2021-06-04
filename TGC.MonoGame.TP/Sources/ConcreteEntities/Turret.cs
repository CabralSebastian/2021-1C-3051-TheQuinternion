using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class Turret : BaseTurret
    {
        protected override Drawer Drawer() => TGCGame.content.D_Turret;
        protected override TypedIndex Shape => TGCGame.content.SH_Turret;

        protected override float MaxRange => 1000f;
        protected override float MinIdleTime => 1000f;
        protected override Vector3 CannonsOffset => new Vector3(0f, 2.8911f, 0f) * 10f;

        private const float rotationSpeed = 0.2f;
        private const float precition = (float)Math.PI / 4;

        private Quaternion headRotation = Quaternion.Identity, cannonsRotation = Quaternion.Identity;

        private Matrix HeadWorldMatrix()
            => Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(headRotation) * Matrix.CreateTranslation(Position);

        private Matrix CannonsWorldMatrix()
            => Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(cannonsRotation) * Matrix.CreateTranslation(CannonsPosition);
        
        protected override void Aim(Vector3 difference, float yawDifference, float pitchDifference, double elapsedTime)
        {
            headAngle += (yawDifference > 0 ? 1 : -1) * (float)Math.Min(Math.Abs(yawDifference), rotationSpeed * elapsedTime);
            cannonsAngle += (pitchDifference > 0 ? 1 : -1) * (float)Math.Min(Math.Abs(pitchDifference), rotationSpeed * elapsedTime);

            headRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, headAngle);
            cannonsRotation = headRotation * Quaternion.CreateFromAxisAngle(Vector3.Left, cannonsAngle);
        }

        protected override bool IsAimed(float yawDifference, float pitchDifference) => yawDifference < precition && pitchDifference < precition;

        protected override void Fire()
        {
            Vector3 forward = PhysicUtils.Forward(cannonsRotation);
            Vector3 left = PhysicUtils.Left(cannonsRotation);
            TGCGame.world.InstantiateLaser(CannonsPosition - left, forward, cannonsRotation, emitter);
            TGCGame.world.InstantiateLaser(CannonsPosition + left, forward, cannonsRotation, emitter);
        }

        internal override void Draw()
        {
            TGCGame.content.D_Turret.HeadWorldMatrix = HeadWorldMatrix();
            TGCGame.content.D_Turret.CannonsWorldMatrix = CannonsWorldMatrix();
            base.Draw();
        }
    }
}