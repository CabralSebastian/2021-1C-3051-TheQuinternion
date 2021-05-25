using BepuPhysics;
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
        protected bool Destroyed { get; private set; } = false;

        protected override Matrix GeneralWorldMatrix()
        {
            RigidPose pose = Body().Pose;
            return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(pose.Orientation.ToQuaternion()) * Matrix.CreateTranslation(pose.Position.ToVector3());
        }

        internal void Instantiate(Vector3 position) => Instantiate(position, Quaternion.Identity);
        internal override void Instantiate(Vector3 position, Quaternion rotation)
        {
            handle = CreateBody(position, rotation);
            TGCGame.physicSimulation.collitionEvents.RegisterCollider(handle, this);
            base.Instantiate(position, rotation);
        }

        internal override void Destroy()
        {
            TGCGame.physicSimulation.collitionEvents.UnregisterCollider(handle);
            if (!Body().Exists && !Destroyed)
                TGCGame.physicSimulation.DestroyBody(handle);
            base.Destroy();
            Destroyed = true;
        }

        protected abstract BodyHandle CreateBody(Vector3 position, Quaternion rotation);

        public virtual bool HandleCollition(ICollitionHandler other) => !Destroyed;
    }
}