using Microsoft.Xna.Framework;
using System;

namespace TGC.MonoGame.TP.Physics
{
    internal class PhysicUtils
    {
        internal static Vector3 Forward(Quaternion rotation) => Vector3.Normalize(new Vector3(
            2 * (rotation.X * rotation.Z + rotation.W * rotation.Y),
            2 * (rotation.Y * rotation.Z - rotation.W * rotation.X),
            1 - 2 * (rotation.X * rotation.X + rotation.Y * rotation.Y)
        ));

        internal static Vector3 Up(Quaternion rotation) => Vector3.Normalize(new Vector3(
            2 * (rotation.X * rotation.Y - rotation.W * rotation.Z),
            1 - 2 * (rotation.X * rotation.X + rotation.Z * rotation.Z),
            2 * (rotation.Y * rotation.Z + rotation.W * rotation.X)
        ));

        internal static Vector3 Left(Quaternion rotation) => Vector3.Normalize(new Vector3(
            1 - 2 * (rotation.Y * rotation.Y + rotation.Z * rotation.Z),
            2 * (rotation.X * rotation.Y + rotation.W * rotation.Z),
            2 * (rotation.X * rotation.Z - rotation.W * rotation.Y)
        ));

        internal static void DirectionToEuler(Vector3 difference, float distance, out float yaw, out float pitch)
        {
            yaw = (float)Math.Atan(difference.X / difference.Z) + (difference.Z > 0 ? MathHelper.Pi : 0f);
            pitch = -(float)Math.Asin(difference.Y / distance);
        }
    }
}