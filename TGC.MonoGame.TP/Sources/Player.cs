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
        private Vector2 MouseOuterBox => TGCGame.Gui.ScreenSize / 7;
        private Vector2 MouseInnerBox => TGCGame.Gui.ScreenSize / 10;

        private Vector2 LastMousePosition;

        private readonly Bar HealthBar = new Bar(new Vector2(150f, 25f), Color.Red * 0.6f, 100f);
        private readonly Bar TurboBar = new Bar(new Vector2(150f, 25f), Color.Yellow * 0.6f, XWing.MaxTurbo);
        private readonly Compass Compass = new Compass();

        private bool ShowF1 = false;

        internal void Update(GameTime gameTime)
        {
            double totalTime = gameTime.TotalGameTime.TotalMilliseconds;
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            ProcessMouseMovement(elapsedTime);
            if (Input.SecondaryFire())
                World.XWing.SecondaryFire(totalTime, Input.MousePosition() - TGCGame.Gui.ScreenCenter);
            else if (Input.Fire())
                World.XWing.Fire(totalTime);
            if (Input.GodMode())
                World.XWing.ToggleGodMode();

            if (Input.ToggleF1())
                ShowF1 = !ShowF1;

            if (Input.BarrelRoll())
                World.XWing.ToggleBarrelRoll(gameTime);
        }

        //MOUSE//
        private bool MouseIsInside() =>
            (Input.MousePosition().X < TGCGame.Gui.ScreenCenter.X + MouseInnerBox.X && Input.MousePosition().X > TGCGame.Gui.ScreenCenter.X - MouseInnerBox.X) && 
            (Input.MousePosition().Y < TGCGame.Gui.ScreenCenter.Y + MouseInnerBox.Y && Input.MousePosition().Y > TGCGame.Gui.ScreenCenter.Y - MouseInnerBox.Y);
        
        private void ProcessMouseMovement(float elapsedTime)
        {
            int limitedToOuterBoxSideX = (int)Math.Clamp(Input.MousePosition().X, TGCGame.Gui.ScreenCenter.X - MouseOuterBox.X, TGCGame.Gui.ScreenCenter.X + MouseOuterBox.X);
            int limitedToOuterBoxSideY = (int)Math.Clamp(Input.MousePosition().Y, TGCGame.Gui.ScreenCenter.Y - MouseOuterBox.Y, TGCGame.Gui.ScreenCenter.Y + MouseOuterBox.Y);

            Mouse.SetPosition(limitedToOuterBoxSideX, limitedToOuterBoxSideY);
            LastMousePosition = Input.MousePosition();
        }

        internal void DrawHUD(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 mousePosition = Input.MousePosition();
            Vector3 pos = World.XWing.Position();
            
            spriteBatch.Draw(
                TGCGame.GameContent.T_TargetCursor, 
                new Rectangle(
                    (int)mousePosition.X - 10,
                    (int)mousePosition.Y - 10,
                    20, 20
                ), 
                Color.White);

            HealthBar.Draw(TGCGame.Gui.ScreenSize - new Vector2(150f / 2 + 5f, 25f / 2 + 5f), World.XWing.Health);
            TurboBar.Draw(TGCGame.Gui.ScreenSize - new Vector2(150f / 2 + 5f, 25f + 25f / 2 + 10f), World.XWing.Turbo);
            Compass.Draw(TGCGame.Camera.Position, World.DeathStar.WeakPoint.GetPosition, TGCGame.Camera.Forward);

            if (World.XWing.GodMode)
                TGCGame.Gui.DrawText("God mode", new Vector2(5f, TGCGame.Gui.ScreenSize.Y - 25f), 12f);

            if (ShowF1)
                TGCGame.Gui.DrawText(Math.Round(TGCGame.Game.LastFPS).ToString() + " FPS", new Vector2(5f, 5f), 12f);
        }
    }
}