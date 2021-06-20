using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Scenes;
using TGC.MonoGame.TP.GraphicInterface;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP
{
    internal class Player
    {
        private Vector2 MouseOuterBox => TGCGame.gui.ScreenSize / 7;
        private Vector2 MouseInnerBox => TGCGame.gui.ScreenSize / 10;

        private Vector2 lastMousePosition;

        private readonly Bar healthBar = new Bar(new Vector2(150f, 25f), Color.Red * 0.6f, 100f);
        private readonly Bar turboBar = new Bar(new Vector2(150f, 25f), Color.Yellow * 0.6f, XWing.maxTurbo);

        private bool showF1 = false;

        internal void Update(GameTime gameTime)
        {
            double totalTime = gameTime.TotalGameTime.TotalMilliseconds;
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            ProcessMouseMovement(elapsedTime);
            if (Input.SecondaryFire())
                World.xwing.SecondaryFire(totalTime, Input.MousePosition() - TGCGame.gui.ScreenCenter);
            else if (Input.Fire())
                World.xwing.Fire(totalTime);
            if (Input.GodMode())
                World.xwing.ToggleGodMode();

            if (Input.ToggleF1())
                showF1 = !showF1;
        }

        //MOUSE//
        private bool MouseIsInside() =>
            (Input.MousePosition().X < TGCGame.gui.ScreenCenter.X + MouseInnerBox.X && Input.MousePosition().X > TGCGame.gui.ScreenCenter.X - MouseInnerBox.X) && 
            (Input.MousePosition().Y < TGCGame.gui.ScreenCenter.Y + MouseInnerBox.Y && Input.MousePosition().Y > TGCGame.gui.ScreenCenter.Y - MouseInnerBox.Y);
        
        private void ProcessMouseMovement(float elapsedTime)
        {
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

        /*private void ProcessMouseMovement(float elapsedTime)
        {
            Vector2 mouseDelta = (Input.MousePosition() - lastMousePosition);
            
            var fixValue = 0.001f;
            mouseDelta *= fixValue * elapsedTime;

            TGCGame.camera.UpdateYawNPitch(mouseDelta);
            //World.xwing.Move(new Vector2(Math.Sign(mouseDelta.X), Math.Sign(mouseDelta.Y)));

            Mouse.SetPosition((int)TGCGame.gui.ScreenCenter.X, (int)TGCGame.gui.ScreenCenter.Y);
            Mouse.SetCursor(MouseCursor.Crosshair);

            lastMousePosition = Input.MousePosition();
        }*/

        internal void DrawHUD(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 mousePosition = Input.MousePosition();
            Vector3 pos = World.xwing.Position();
            
            spriteBatch.Draw(
                TGCGame.content.T_TargetCursor, 
                new Rectangle(
                    (int)mousePosition.X - 10,
                    (int)mousePosition.Y - 10,
                    20, 20
                ), 
                Color.White);

            healthBar.Draw(TGCGame.gui.ScreenSize - new Vector2(150f / 2 + 5f, 25f / 2 + 5f), World.xwing.salud);
            turboBar.Draw(TGCGame.gui.ScreenSize - new Vector2(150f / 2 + 5f, 25f + 25f / 2 + 10f), World.xwing.turbo);

            if (World.xwing.godMode)
                TGCGame.gui.DrawText("God mode", new Vector2(5f, TGCGame.gui.ScreenSize.Y - 25f), 12f);

            if (showF1)
                TGCGame.gui.DrawText(Math.Round(TGCGame.game.LastFPS).ToString() + " FPS", new Vector2(5f, 5f), 12f);
        }
    }
}