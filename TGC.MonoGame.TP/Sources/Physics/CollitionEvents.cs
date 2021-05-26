using BepuPhysics;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TGC.MonoGame.TP.Physics
{
    class CollitionEvents
    {
        private readonly ConcurrentDictionary<StaticHandle, ICollitionHandler> collidersS = new ConcurrentDictionary<StaticHandle, ICollitionHandler>();
        private readonly ConcurrentDictionary<BodyHandle, ICollitionHandler> collidersB = new ConcurrentDictionary<BodyHandle, ICollitionHandler>();

        internal void RegisterCollider(StaticHandle handle, ICollitionHandler handler) => collidersS.TryAdd(handle, handler);
        internal void RegisterCollider(BodyHandle handle, ICollitionHandler handler) => collidersB.TryAdd(handle, handler);

        internal ICollitionHandler GetHandler(StaticHandle handle) => collidersS.GetValueOrDefault(handle);
        internal ICollitionHandler GetHandler(BodyHandle handle) => collidersB.GetValueOrDefault(handle);

        internal void UnregisterCollider(StaticHandle handle) => collidersS.TryRemove(handle, out _);
        internal void UnregisterCollider(BodyHandle handle) => collidersB.TryRemove(handle, out _);
    }
}