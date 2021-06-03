using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class SmallTurret : BaseTurret
    {
        private readonly SmallTurretDrawer turretDrawer = new SmallTurretDrawer(TGCGame.content.M_SmallTurret, TGCGame.content.T_Turret);
        protected override Drawer Drawer() => turretDrawer;
        protected override TypedIndex Shape => TGCGame.content.SH_Turret;
        protected override Vector3 Scale => Vector3.One * DeathStar.trenchScale;

        protected override float MaxRange => 500f;
        //protected override float MinIdleTime => 1000f;

        private float headAngle = 0f, cannonsAngle = 0f;
        private double idleTime = 0;

        private readonly bool rotated;
        internal SmallTurret(bool rotated) => this.rotated = rotated;

        private const float maxRotation = 0.2f;
        private const float minIdleTime = 1000f;
        private const float precition = (float)Math.PI / 10;


        private readonly Vector3 cannonsOffset = new Vector3(0f, 1.70945f, 0f) * 10f;

        private Quaternion cannonsRotation = Quaternion.Identity;

        private Matrix rotMatrix;
        private Vector3 left;
        protected override void OnInstantiate()
        {
            rotMatrix = Matrix.CreateFromQuaternion(Rotation);
            left = PhysicUtils.Left(Rotation);
            base.OnInstantiate();
        }

        private Matrix CannonsWorldMatrix()
            => Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(cannonsRotation) * Matrix.CreateTranslation(Position + cannonsOffset);

        internal override void Update(double elapsedTime)
        {
            Vector3 difference = TGCGame.world.xwing.Position() - (Position + cannonsOffset);
            float distance = difference.Length();

            if (IsInRange(distance))
            {
                PhysicUtils.DirectionToEuler(difference, distance, out float objectiveHeadAngle, out float objectiveCannonsAngle);

                float differenceCannonsAngle = objectiveCannonsAngle - cannonsAngle;

                cannonsAngle += (differenceCannonsAngle > 0 ? 1 : -1) * (float)Math.Min(Math.Abs(differenceCannonsAngle), maxRotation * elapsedTime);
                if (!rotated)
                    headAngle = difference.Z < 0 ? 0 : MathHelper.Pi;
                else
                    headAngle = difference.X < 0 ? 0 : MathHelper.Pi;

                cannonsRotation = Rotation * Quaternion.CreateFromAxisAngle(Vector3.Up, headAngle) * Quaternion.CreateFromAxisAngle(Vector3.Left, cannonsAngle);

                float differenceHead = Math.Abs(objectiveHeadAngle - headAngle);
                differenceCannonsAngle = Math.Abs(objectiveCannonsAngle - cannonsAngle);
                if ((differenceHead < precition || differenceHead - MathHelper.Pi < precition) && differenceCannonsAngle < precition && idleTime > minIdleTime)
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

        internal override void Draw()
        {
            turretDrawer.CannonsWorldMatrix = CannonsWorldMatrix();
            base.Draw();
        }

        private void Fire()
        {
            new Laser().Instantiate(Position + cannonsOffset - left * 2f - PhysicUtils.Forward(cannonsRotation) * 25f, cannonsRotation);
            new Laser().Instantiate(Position + cannonsOffset + left * 2f - PhysicUtils.Forward(cannonsRotation) * 25f, cannonsRotation);
        }
    }
}