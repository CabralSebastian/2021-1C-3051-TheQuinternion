using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.GraphicInterface;

namespace TGC.MonoGame.TP.Scenes
{
    internal class MainMenu : Scene
    {
        private SoundEffectInstance menuMusic;
        private readonly Button startButton = new Button("Start", new Vector2(200, 40), () => TGCGame.game.ChangeScene(new World()));
        private readonly Button exitButton = new Button("Exit", new Vector2(200, 40), () => TGCGame.game.Exit());

        internal override void Initialize()
        {
            new DeathStar().Create(true);
            TGCGame.camera.SetLocation(new Vector3(0f, 0f, 0f), Vector3.Forward);

            Quaternion toLeft = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);
            Quaternion toRight = Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2);
            SpawnSquad(4, 100f, -800f, 0f, -100f, toLeft);
            SpawnSquad(5, 200f, -800f, 0f, -500f, toLeft);
            SpawnSquad(3, 300f, -3000f, 0f, -200f, toRight);
            SpawnSquad(3, 300f, -10000f, 0f, -1000f, toLeft);

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


        private void SpawnSquad(float speed, float baseX, float baseY, float baseZ, Quaternion direction)
        {
            Random random = new Random();
            new DummyTIE(speed, baseX, -baseX).Instantiate(new Vector3(baseX + (float)random.NextDouble() * 50, baseY + 10f + (float)random.NextDouble() * 10, baseZ), direction);
            new DummyTIE(speed, baseX, -baseX).Instantiate(new Vector3(baseX + (float)random.NextDouble() * 50, baseY + -10f - (float)random.NextDouble() * 10, baseZ), direction);
            new DummyTIE(speed, baseX, -baseX).Instantiate(new Vector3(baseX + (float)random.NextDouble() * 50, baseY + 0f + (float)random.NextDouble() * 5, baseZ), direction);
        }

        private void SpawnSquad(int number, float speed, float baseX, float baseY, float baseZ, Quaternion direction)
        {
            Random random = new Random();
            for (int i = 0; i < number; i++)
                new DummyTIE(speed, baseX, -baseX)
                    .Instantiate(new Vector3(baseX + (float)random.NextDouble() * 200, baseY + 10f * i, baseZ), direction);
        }

        internal override void Draw2D(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 center = TGCGame.gui.ScreenCenter;
            Vector2 starWarsSize = TGCGame.gui.DrawCenteredText("Star Wars", new Vector2(center.X, center.Y / 4), 20f);
            TGCGame.gui.DrawCenteredText("Trench Run", new Vector2(center.X, center.Y / 4 + starWarsSize.Y + 5), 48f);
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