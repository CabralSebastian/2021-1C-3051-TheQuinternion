using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.GraphicInterface;

namespace TGC.MonoGame.TP.Scenes
{
    internal class Ending : Scene
    {
        private SoundEffectInstance menuMusic;
        private readonly Button startButton = new Button("Play again!", new Vector2(200, 40), () => TGCGame.game.ChangeScene(new World()));
        private readonly Button exitButton = new Button("Exit", new Vector2(200, 40), () => TGCGame.game.Exit());

        internal override void Initialize()
        {
            TGCGame.camera.SetTarget(null);
            TGCGame.camera.SetLocation(new Vector3(0f, 0f, 0f), Vector3.Normalize(new Vector3(-0.05f, 0.2f, -1f)), Vector3.Up);
            PlayMusic();
            TGCGame.game.IsMouseVisible = true;
        }

        private void PlayMusic()
        {
            menuMusic = TGCGame.content.S_MenuMusic.CreateInstance();
            menuMusic.IsLooped = true;
            menuMusic.Volume = 0.2f;
            menuMusic.Play();
        }

        internal override void Update(GameTime gameTime)
        {
            if (Input.Submit())
                TGCGame.game.ChangeScene(new World());
            startButton.Update(TGCGame.gui.ScreenCenter + new Vector2(0, 50));
            exitButton.Update(TGCGame.gui.ScreenCenter + new Vector2(0, 100));
            base.Update(gameTime);
        }

        internal override void Draw2D(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 center = TGCGame.gui.ScreenCenter;
            Vector2 prevTextSize = TGCGame.gui.DrawCenteredText("You destroyed the Death Star!", new Vector2(center.X, center.Y / 4), 28f);
            TGCGame.gui.DrawCenteredText("Thanks for playing!", new Vector2(center.X, center.Y / 4 + prevTextSize.Y + 5), 20f);
            startButton.Draw(center + new Vector2(0, 50));
            exitButton.Draw(center + new Vector2(0, 100));
        }

        internal override void Destroy()
        {
            menuMusic.Stop();
            base.Destroy();
        }
    }
}