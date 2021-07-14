using System;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using BEPUVector3 = System.Numerics.Vector3;

namespace TGC.MonoGame.TP.Physics
{
    internal class PhysicSimulation
    {
        private readonly Simulation Simulation;
        internal readonly BufferPool BufferPool = new BufferPool();
        private readonly SimpleThreadDispatcher ThreadDispatcher;
        internal Shapes Shapes() => Simulation.Shapes;

        private readonly BEPUVector3 Gravity = new BEPUVector3();
        private const float Timestep = 1 / 60f;

        internal readonly CollitionEvents CollitionEvents = new CollitionEvents();

        internal PhysicSimulation()
        {
            ThreadDispatcher = new SimpleThreadDispatcher(ThreadCount());
            Simulation = CreateSimulation();
        }

        private int ThreadCount() => Math.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);

        private Simulation CreateSimulation() => Simulation.Create(
            BufferPool, new NarrowPhaseCallbacks(),
            new PoseIntegratorCallbacks(Gravity, 0f, 0f), new PositionFirstTimestepper()
        );

        internal void Update() => Simulation.Timestep(Timestep, ThreadDispatcher);

        internal TypedIndex LoadShape<S>(S shape) where S : unmanaged, IShape => Simulation.Shapes.Add(shape);

        internal BodyReference GetBody(BodyHandle handle) => Simulation.Bodies.GetBodyReference(handle);

        internal StaticHandle CreateStatic(Vector3 position, Quaternion rotation, TypedIndex shape) =>
            Simulation.Statics.Add(new StaticDescription(
                position.ToBEPU(),
                rotation.ToBEPU(),
                new CollidableDescription(shape, 0.1f))
            );

        internal BodyHandle CreateDynamic(Vector3 position, Quaternion rotation, TypedIndex shape, float mass)
        {
            Simulation.Shapes.GetShape<Sphere>(shape.Index).ComputeInertia(mass, out BodyInertia inertia);
            BodyDescription bodyDescription = BodyDescription.CreateDynamic(
                new RigidPose(position.ToBEPU(),  rotation.ToBEPU()),
                new BodyVelocity(new BEPUVector3(0f, 0f, 0f)),
                inertia,
                new CollidableDescription(shape, 0.1f),
                new BodyActivityDescription(-1));
            return Simulation.Bodies.Add(bodyDescription);
        }

        internal BodyHandle CreateKinematic(Vector3 position, Quaternion rotation, TypedIndex shape)
        {
            BodyDescription bodyDescription = BodyDescription.CreateKinematic(
                new RigidPose(position.ToBEPU(), rotation.ToBEPU()),
                new BodyVelocity(new BEPUVector3(0f, 0f, 0f)),
                new CollidableDescription(shape, 0.1f),
                new BodyActivityDescription(-1));
            return Simulation.Bodies.Add(bodyDescription);
        }

        internal void DestroyStatic(StaticHandle handle) => Simulation.Statics.Remove(handle);
        internal void DestroyBody(BodyHandle handle) => Simulation.Bodies.Remove(handle);

        internal void Dispose()
        {
            Simulation.Dispose();
            BufferPool.Clear();
            ThreadDispatcher.Dispose();
        }
    }
}