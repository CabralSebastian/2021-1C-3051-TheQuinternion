using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class Turret : StaticPhysicEntity
    {
        private readonly TurretDrawer turretDrawer = new TurretDrawer(TGCGame.content.M_Turret, TGCGame.content.T_Turret);
        protected override Drawer Drawer() => turretDrawer;
        protected override TypedIndex Shape => TGCGame.content.SH_Turret;
        protected override Vector3 Scale => Vector3.One * DeathStar.trenchScale;
        private float headAngle = 0f, cannonsAngle = 0f;
        private double idleTime = 0;

        private const float maxDistance = 1000f;
        private const float maxRotation = 0.2f;
        private const float minIdleTime = 1000f;
        private const float precition = (float)Math.PI / 4;

        private Quaternion headRotation = Quaternion.Identity, cannonsRotation = Quaternion.Identity;

        private Matrix HeadWorldMatrix()
            => Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(headRotation) * Matrix.CreateTranslation(Position);

        private Matrix CannonsWorldMatrix()
            => Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(cannonsRotation) * Matrix.CreateTranslation(Position + new Vector3(0f, 2.8911f * 10f, 0f));
        
        internal override void Update(double elapsedTime)
        {
            Vector3 difference = TGCGame.world.xwing.Position() - (Position + new Vector3(0f, 2.8911f * 10f, 0f));
            float distance = difference.Length();

            if(distance < maxDistance)
            {
                Vector3 direction = Vector3.Normalize(difference);

                float objectiveHeadAngle = (float)Math.Atan(direction.X / direction.Z);
                float objectiveCannonsAngle = -(float)Math.Asin(difference.Y / distance);

                float differenceHead = objectiveHeadAngle - headAngle;
                float differenceCannonsAngle = objectiveCannonsAngle - cannonsAngle;

                headAngle += (differenceHead > 0 ? 1 : -1) * (float)Math.Min(Math.Abs(differenceHead), maxRotation * elapsedTime);
                cannonsAngle += (differenceCannonsAngle > 0 ? 1 : -1) * (float)Math.Min(Math.Abs(differenceCannonsAngle), maxRotation * elapsedTime);

                headRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, headAngle);
                cannonsRotation = headRotation * Quaternion.CreateFromAxisAngle(Vector3.Left, cannonsAngle);

                differenceHead = objectiveHeadAngle - headAngle;
                differenceCannonsAngle = objectiveCannonsAngle - cannonsAngle;
                if (differenceHead < precition && differenceCannonsAngle < precition && idleTime > minIdleTime)
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
            turretDrawer.HeadWorldMatrix = HeadWorldMatrix();
            turretDrawer.CannonsWorldMatrix = CannonsWorldMatrix();
            base.Draw();
        }

        private void Fire()
        {
            new Laser().Instantiate(Position + new Vector3(0f, 2.8911f * 10f, 0f) - PhysicUtils.Left(cannonsRotation) * 2f - PhysicUtils.Forward(cannonsRotation) * 5f, cannonsRotation);
            new Laser().Instantiate(Position + new Vector3(0f, 2.8911f * 10f, 0f) + PhysicUtils.Left(cannonsRotation) * 2f - PhysicUtils.Forward(cannonsRotation) * 5f, cannonsRotation);
        }
    }
}