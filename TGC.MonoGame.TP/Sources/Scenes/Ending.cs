using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.GraphicInterface;

namespace TGC.MonoGame.TP.Scenes
{
    internal class Ending : Scene
    {
        private SoundEffectInstance MenuMusic;
        private readonly Button StartButton = new Button("Play again!", new Vector2(200, 40), () => TGCGame.Game.ChangeScene(new World()));
        private readonly Button ExitButton = new Button("Exit", new Vector2(200, 40), () => TGCGame.Game.Exit());

        internal override void Initialize()
        {
            TGCGame.Camera.SetTarget(null);
            TGCGame.Camera.SetLocation(new Vector3(0f, 0f, 0f), Vector3.Normalize(new Vector3(-0.05f, 0.2f, -1f)), Vector3.Up);
            PlayMusic();
            TGCGame.Game.IsMouseVisible = true;
        }

        private void PlayMusic()
        {
            MenuMusic = TGCGame.GameContent.S_MenuMusic.CreateInstance();
            MenuMusic.IsLooped = true;
            MenuMusic.Volume = 0.2f;
            MenuMusic.Play();
        }

        internal override void Update(GameTime gameTime)
        {
            if (Input.Submit())
                TGCGame.Game.ChangeScene(new World());
            StartButton.Update(TGCGame.Gui.ScreenCenter + new Vector2(0, 50));
            ExitButton.Update(TGCGame.Gui.ScreenCenter + new Vector2(0, 100));
            base.Update(gameTime);
        }

        internal override void Draw2D(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 center = TGCGame.Gui.ScreenCenter;
            Vector2 prevTextSize = TGCGame.Gui.DrawCenteredText("You destroyed the Death Star!", new Vector2(center.X, center.Y / 4), 28f);
            TGCGame.Gui.DrawCenteredText("Thanks for playing!", new Vector2(center.X, center.Y / 4 + prevTextSize.Y + 5), 20f);
            StartButton.Draw(center + new Vector2(0, 50));
            ExitButton.Draw(center + new Vector2(0, 100));
        }

        internal override void Destroy()
        {
            MenuMusic.Stop();
            base.Destroy();
        }
    }
}