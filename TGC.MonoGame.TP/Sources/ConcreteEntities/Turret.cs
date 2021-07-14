using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class Turret : BaseTurret
    {
        internal const float ScaleValue = DeathStar.TrenchScale * 1.2f;
        protected override Vector3 Scale => Vector3.One * ScaleValue;
        protected override Drawer Drawer() => TGCGame.GameContent.D_Turret;
        protected override TypedIndex Shape => TGCGame.GameContent.SH_Turret;

        protected override float MaxRange => 1200f;
        protected override float MinIdleTime => 1000f;
        protected override Vector3 CannonsOffset => new Vector3(0f, 2.8911f, 0f) * 100 * ScaleValue;

        private const float RotationSpeed = 0.2f;
        private const float Precision = (float)Math.PI / 4;

        private Quaternion HeadRotation = Quaternion.Identity, cannonsRotation = Quaternion.Identity;

        private Matrix HeadWorldMatrix()
            => Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(HeadRotation) * Matrix.CreateTranslation(Position);

        private Matrix CannonsWorldMatrix()
            => Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(cannonsRotation) * Matrix.CreateTranslation(CannonsPosition);
        
        protected override void Aim(Vector3 difference, float yawDifference, float pitchDifference, double elapsedTime)
        {
            HeadAngle += (yawDifference > 0 ? 1 : -1) * (float)Math.Min(Math.Abs(yawDifference), RotationSpeed * elapsedTime);
            CannonsAngle += (pitchDifference > 0 ? 1 : -1) * (float)Math.Min(Math.Abs(pitchDifference), RotationSpeed * elapsedTime);

            HeadRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, HeadAngle);
            cannonsRotation = HeadRotation * Quaternion.CreateFromAxisAngle(Vector3.Left, CannonsAngle);
        }

        protected override bool IsAimed(float yawDifference, float pitchDifference) => yawDifference < Precision && pitchDifference < Precision;

        protected override void Fire()
        {
            Vector3 forward = PhysicUtils.Forward(cannonsRotation);
            Vector3 left = PhysicUtils.Left(cannonsRotation);
            World.InstantiateLaser(CannonsPosition - left, forward, cannonsRotation, Emitter);
            World.InstantiateLaser(CannonsPosition + left, forward, cannonsRotation, Emitter);
        }

        internal override void Draw()
        {
            TGCGame.GameContent.D_Turret.HeadWorldMatrix = HeadWorldMatrix();
            TGCGame.GameContent.D_Turret.CannonsWorldMatrix = CannonsWorldMatrix();
            base.Draw();
        }

        internal Turret() => Health = 500f;
    }
}