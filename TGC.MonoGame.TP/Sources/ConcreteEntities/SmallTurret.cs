using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class SmallTurret : BaseTurret
    {
        internal const float ScaleValue = DeathStar.TrenchScale * 0.9f;
        protected override Vector3 Scale => Vector3.One * ScaleValue;
        protected override Drawer Drawer() => TGCGame.GameContent.D_SmallTurret;
        protected override TypedIndex Shape => TGCGame.GameContent.SH_SmallTurret;

        protected override float MaxRange => 600f;
        protected override float MinIdleTime => 1000f;
        protected override Vector3 CannonsOffset => new Vector3(0f, 1.70945f, 0f) * 100 * ScaleValue;

        private const float RotationSpeed = 0.2f;
        private const float PrecisionYaw = (float)Math.PI / 30;
        private const float PrecisionPitch = (float)Math.PI / 10;

        private readonly bool Rotated;
        internal SmallTurret(bool rotated) => this.Rotated = rotated;

        private Vector3 Left;
        protected override void OnInstantiate()
        {
            if (Rotated)
                HeadAngle = MathHelper.PiOver2;
            Left = PhysicUtils.Left(Rotation);
            base.OnInstantiate();
        }

        private Quaternion cannonsRotation = Quaternion.Identity;

        private Matrix CannonsWorldMatrix()
            => Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(cannonsRotation) * Matrix.CreateTranslation(CannonsPosition);
                
        protected override void Aim(Vector3 difference, float yawDifference, float pitchDifference, double elapsedTime)
        {
            CannonsAngle += (pitchDifference > 0 ? 1 : -1) * (float)Math.Min(Math.Abs(pitchDifference), RotationSpeed * elapsedTime);
            if (!Rotated)
                HeadAngle = difference.Z < 0 ? 0 : MathHelper.Pi;
            else
                HeadAngle = difference.X < 0 ? 0 : MathHelper.Pi;
            cannonsRotation = Rotation * Quaternion.CreateFromAxisAngle(Vector3.Up, HeadAngle) * Quaternion.CreateFromAxisAngle(Vector3.Left, CannonsAngle);
        }

        protected override bool IsAimed(float yawDifference, float pitchDifference) => (yawDifference < PrecisionYaw || yawDifference - MathHelper.Pi < PrecisionYaw) && pitchDifference < PrecisionPitch;

        protected override void Fire()
        {
            Vector3 forward = PhysicUtils.Forward(cannonsRotation);
            World.InstantiateLaser(CannonsPosition - Left, forward, cannonsRotation, Emitter);
            World.InstantiateLaser(CannonsPosition + Left, forward, cannonsRotation, Emitter);
        }

        internal override void Draw()
        {
            TGCGame.GameContent.D_SmallTurret.CannonsWorldMatrix = CannonsWorldMatrix();
            base.Draw();
        }

        internal SmallTurret() => Health = 100f;
    }
}