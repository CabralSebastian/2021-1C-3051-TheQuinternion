using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;


namespace TGC.MonoGame.TP
{
    internal class Player
    {
        private const float mouseOuterBoxSide = 6f; 
        private const float mouseInnerBoxSide = 3f;

        internal void Update(float elapsedTime)
        {
            ProcessMouseMovement(elapsedTime);
        }

        //MOUSE//
        private Vector2 CurrentMousePosition() => Mouse.GetState().Position.ToVector2();
        private bool MouseIsInside() => Math.Abs(CurrentMousePosition().X) < TGCGame.screenCenter.X + mouseInnerBoxSide && Math.Abs(CurrentMousePosition().Y) < TGCGame.screenCenter.Y + mouseInnerBoxSide;
        
        private void ProcessMouseMovement(float elapsedTime)
        {
            Vector2 mouseDelta = (CurrentMousePosition() - TGCGame.screenCenter);
            var fixValue = 0.001f;
            TGCGame.camera.UpdateYawNPitch(mouseDelta * elapsedTime * fixValue);

            //if (!MouseIsInside())
            //    TGCGame.world.xwing.RotateY(mouseDelta.Y, elapsedTime);

            int limitedToOuterBoxSideX = (int)Math.Clamp(CurrentMousePosition().X, TGCGame.screenCenter.X - mouseOuterBoxSide, TGCGame.screenCenter.X + mouseOuterBoxSide);
            int limitedToOuterBoxSideY = (int)Math.Clamp(CurrentMousePosition().Y, TGCGame.screenCenter.Y - mouseOuterBoxSide, TGCGame.screenCenter.Y + mouseOuterBoxSide);

            Mouse.SetPosition(limitedToOuterBoxSideX, limitedToOuterBoxSideY);
        }

    }
}
