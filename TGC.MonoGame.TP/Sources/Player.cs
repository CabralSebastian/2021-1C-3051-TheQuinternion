using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP
{
    internal class Player
    {
        private Vector2 MouseOuterBox => TGCGame.gui.ScreenSize / 7;
        private Vector2 MouseInnerBox => TGCGame.gui.ScreenSize / 10;

        private Vector2 lastMousePosition;

        internal void Update(GameTime gameTime)
        {
            ProcessMouseMovement((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            if (Input.SecondaryFire())
                World.xwing.SecondaryFire(gameTime.TotalGameTime.TotalMilliseconds, Input.MousePosition() - TGCGame.gui.ScreenCenter);
            else if (Input.Fire())
                World.xwing.Fire(gameTime.TotalGameTime.TotalMilliseconds, Input.MousePosition() - TGCGame.gui.ScreenCenter);
            if (Input.GodMode())
                World.xwing.ToggleGodMode();
        }

        //MOUSE//
        private bool MouseIsInside() =>
            (Input.MousePosition().X < TGCGame.gui.ScreenCenter.X + MouseInnerBox.X && Input.MousePosition().X > TGCGame.gui.ScreenCenter.X - MouseInnerBox.X) && 
            (Input.MousePosition().Y < TGCGame.gui.ScreenCenter.Y + MouseInnerBox.Y && Input.MousePosition().Y > TGCGame.gui.ScreenCenter.Y - MouseInnerBox.Y);
        
        private void ProcessMouseMovement(float elapsedTime)
        {
            Vector2 mouseDelta = (Input.MousePosition() - lastMousePosition);
            var fixValue = 0.001f;
            TGCGame.camera.UpdateYawNPitch(mouseDelta * elapsedTime * fixValue);

            //if (!MouseIsInside())
            //    TGCGame.world.xwing.addRotation(new Vector3(0, Math.Sign(-mouseDelta.X), 0));
            //TGCGame.world.xwing.addRotation(new Vector3(0, Math.Sign(-mouseDelta.X), Math.Sign(-mouseDelta.X)));

            //if (Input.MousePosition().Y == screenCenter.Y + mouseInnerBox.Y || Input.MousePosition().Y == screenCenter.Y - mouseInnerBox.Y)
            //    TGCGame.world.xwing.addRotation(new Vector3(Math.Sign(mouseDelta.Y), 0, 0));

            int limitedToOuterBoxSideX = (int)Math.Clamp(Input.MousePosition().X, TGCGame.gui.ScreenCenter.X - MouseOuterBox.X, TGCGame.gui.ScreenCenter.X + MouseOuterBox.X);
            int limitedToOuterBoxSideY = (int)Math.Clamp(Input.MousePosition().Y, TGCGame.gui.ScreenCenter.Y - MouseOuterBox.Y, TGCGame.gui.ScreenCenter.Y + MouseOuterBox.Y);

            Mouse.SetPosition(limitedToOuterBoxSideX, limitedToOuterBoxSideY);
            lastMousePosition = Input.MousePosition();
        }

        internal void DrawHUD(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 mousePosition = Input.MousePosition();
            Vector3 pos = World.xwing.Position();
            TGCGame.gui.DrawText("Salud: " + World.xwing.salud, new Vector2(graphicsDevice.Viewport.Width / 100, graphicsDevice.Viewport.Width / 100), 12f);
            TGCGame.gui.DrawText("(" + pos.X + ", " + pos.Y + ", " + pos.Z + ")", new Vector2(graphicsDevice.Viewport.Width / 5, 50), 12f);
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