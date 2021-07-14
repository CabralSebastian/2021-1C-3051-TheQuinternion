using BepuPhysics;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Entities
{
    internal abstract class DynamicEntity : BodyEntity
    {
        protected abstract float Mass { get; }

        protected override BodyHandle CreateBody(Vector3 position, Quaternion rotation) =>
            TGCGame.PhysicsSimulation.CreateDynamic(position, rotation, Shape, Mass);
    }
}