using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{
    internal class Player
    {
        private readonly Vector2 screenSize;
        private readonly Vector2 screenCenter;

        private readonly Vector2 mouseOuterBox; 
        private readonly Vector2 mouseInnerBox;

        internal Player(Vector2 screenSize)
        {
            this.screenSize = screenSize;
            this.screenCenter = screenSize / 2;
            mouseOuterBox = screenSize / 7;
            mouseInnerBox = screenSize / 10;
        }

        internal void Update(float elapsedTime)
        {
            ProcessMouseMovement(elapsedTime);
            if (Input.Fire())
                TGCGame.world.xwing.Fire();
        }

        //MOUSE//
        private bool MouseIsInside() =>
            (Input.MousePosition().X < screenCenter.X + mouseInnerBox.X && Input.MousePosition().X > screenCenter.X - mouseInnerBox.X) && 
            (Input.MousePosition().Y < screenCenter.Y + mouseInnerBox.Y && Input.MousePosition().Y > screenCenter.Y - mouseInnerBox.Y);
        
        private void ProcessMouseMovement(float elapsedTime)
        {
            Vector2 mouseDelta = (Input.MousePosition() - screenCenter);
            var fixValue = 0.001f;
            TGCGame.camera.UpdateYawNPitch(mouseDelta * elapsedTime * fixValue);

            //if (!MouseIsInside())
            //    TGCGame.world.xwing.addRotation(new Vector3(0, Math.Sign(-mouseDelta.X), 0));
            //TGCGame.world.xwing.addRotation(new Vector3(0, Math.Sign(-mouseDelta.X), Math.Sign(-mouseDelta.X)));

            //if (Input.MousePosition().Y == screenCenter.Y + mouseInnerBox.Y || Input.MousePosition().Y == screenCenter.Y - mouseInnerBox.Y)
            //    TGCGame.world.xwing.addRotation(new Vector3(Math.Sign(mouseDelta.Y), 0, 0));

            int limitedToOuterBoxSideX = (int)Math.Clamp(Input.MousePosition().X, screenCenter.X - mouseOuterBox.X, screenCenter.X + mouseOuterBox.X);
            int limitedToOuterBoxSideY = (int)Math.Clamp(Input.MousePosition().Y, screenCenter.Y - mouseOuterBox.Y, screenCenter.Y + mouseOuterBox.Y);

            Mouse.SetPosition(limitedToOuterBoxSideX, limitedToOuterBoxSideY);
            
        }

        internal void DrawHUD(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Vector2 mousePosition = Input.MousePosition();
            spriteBatch.DrawString(TGCGame.content.F_StarJedi, "Hace mucho tiempo, en una galaxia muy, muy lejana..." + TGCGame.world.xwing.angleUp,
                new Vector2(graphicsDevice.Viewport.Width/5, 50), Color.White);
            spriteBatch.Draw(
                TGCGame.content.T_TargetCursor, 
                new Rectangle(
                    (int)mousePosition.X - 10,
                    (int)mousePosition.Y - 10,
                    20, 20
                ), 
                Color.White);
        }
    }
}