using Microsoft.Xna.Framework;
using System;

namespace TGC.MonoGame.TP.GraphicInterface
{
    internal class Compass
    {
        private Vector2 barSize = new Vector2(300, 2);
        private Vector2 objectiveSize = new Vector2(4, 8);
        private float horizontalFieldOfView = MathHelper.ToRadians(90);

        private Vector2 Rotate(Vector2 vector, float angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Vector2(
                (float)((cos * vector.X) - (sin * vector.Y)),
                (float)((sin * vector.X) + (cos * vector.Y))
            );
        }

        private float GetYaw(Vector2 vector) => (float)Math.Acos(vector.X / vector.Length()) * (vector.Y > 0 ? 1 : -1);

        private Vector2 RotateToVector(Vector2 vector, Vector2 newOrigin) => Rotate(vector, -GetYaw(newOrigin));

        internal void Draw(Vector3 cameraPosition, Vector3 objectivePosition, Vector3 cameraForward)
        {
            TGCGame.gui.DrawCenteredSprite(TGCGame.content.T_Pixel, new Vector2(TGCGame.gui.ScreenSize.X / 2, 15), barSize, Color.White);

            // Variables
            Vector2 proyectedCameraPosition = new Vector2(cameraPosition.X, cameraPosition.Z);
            Vector2 proyectedObjectivePosition = new Vector2(objectivePosition.X, objectivePosition.Z);
            Vector2 proyectedCameraForward = new Vector2(cameraForward.X, cameraForward.Z);
            Vector2 normalizedObjectivePosition = proyectedObjectivePosition - proyectedCameraPosition;

            // Angle
            Vector2 viewRelativeObjectivePosition = RotateToVector(normalizedObjectivePosition, proyectedCameraForward);
            float angle = GetYaw(viewRelativeObjectivePosition);

            // Offset
            float offset = angle * barSize.X / horizontalFieldOfView;
            offset = MathHelper.Clamp(offset, -barSize.X / 2, barSize.X / 2);

            TGCGame.gui.DrawCenteredSprite(TGCGame.content.T_Pixel, new Vector2(TGCGame.gui.ScreenSize.X / 2 + offset, 15), objectiveSize, Color.Yellow);
        }
    }
}