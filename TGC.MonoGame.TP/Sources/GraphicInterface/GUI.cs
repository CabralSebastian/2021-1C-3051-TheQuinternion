using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.GraphicInterface
{
    internal class GUI
    {
        internal Vector2 ScreenSize => TGCGame.Game.WindowSize();
        internal Vector2 ScreenCenter => TGCGame.Gui.ScreenSize / 2;

        private readonly GraphicsDevice GraphicsDevice;
        private readonly SpriteBatch SpriteBatch;

        internal GUI(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.GraphicsDevice = graphicsDevice;
            this.SpriteBatch = spriteBatch;
        }

        private float FixScale(float size) => size / 96;

        internal void DrawSprite(Texture2D sprite, Vector2 position, Vector2 size, Color color) =>
            SpriteBatch.Draw(sprite, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);

        internal void DrawCenteredSprite(Texture2D sprite, Vector2 position, Vector2 size, Color color) => DrawSprite(sprite, position - size / 2, size, color);

        internal void DrawText(string text, Vector2 position, float size) =>
            SpriteBatch.DrawString(TGCGame.GameContent.F_StarJedi, text, position, Color.White, 0f, Vector2.Zero, FixScale(size), SpriteEffects.None, 0);

        internal Vector2 DrawCenteredText(string text, Vector2 position, float fontSize)
        {
            float scale = FixScale(fontSize);
            Vector2 size = TGCGame.GameContent.F_StarJedi.MeasureString(text) * scale;
            SpriteBatch.DrawString(TGCGame.GameContent.F_StarJedi, text, position - size / 2, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            return size;
        }
    }
}