using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Scenes
{
    internal class MainMenu : Scene
    {
        internal override void Initialize()
        {
            new DeathStar().Create(false);
            TGCGame.camera.SetLocation(new Vector3(50f, 0f, 0f), Vector3.Forward);
        }

        internal override void Update(GameTime gameTime)
        {
            if (Input.Submit())
                TGCGame.game.ChangeScene(new World());
            base.Update(gameTime);
        }

        internal override void Draw2D(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 center = TGCGame.gui.ScreenCenter;
            Vector2 starWarsSize = TGCGame.gui.DrawCenteredText("Star Wars", new Vector2(center.X, center.Y / 8), 20f);
            TGCGame.gui.DrawCenteredText("Trench Run", new Vector2(center.X, center.Y / 8 + starWarsSize.Y + 5), 48f);
        }
    }
}