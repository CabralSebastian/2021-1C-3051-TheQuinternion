﻿using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP.Entities
{
    internal abstract class BodyEntity : Entity, ICollitionHandler
    {
        protected virtual Vector3 Scale { get; } = Vector3.One;
        protected abstract TypedIndex Shape { get; }
        private BodyHandle handle;
        protected BodyReference Body() => TGCGame.physicSimulation.GetBody(handle);

        protected override Matrix GeneralWorldMatrix()
        {
            RigidPose pose = Body().Pose;
            return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(pose.Orientation.ToQuaternion()) * Matrix.CreateTranslation(pose.Position.ToVector3());
        }

        internal void Instantiate(Vector3 position) => Instantiate(position, Quaternion.Identity);
        internal void Instantiate(Vector3 position, Quaternion rotation)
        {
            handle = CreateBody(position, rotation);
            TGCGame.physicSimulation.collitionEvents.RegisterCollider(handle, this);
            TGCGame.world.Register(this);
        }

        protected abstract BodyHandle CreateBody(Vector3 position, Quaternion rotation);

        public virtual bool HandleCollition(ICollitionHandler other) => true;
    }
}