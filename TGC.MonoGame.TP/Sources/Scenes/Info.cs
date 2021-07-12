using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.GraphicInterface;


namespace TGC.MonoGame.TP.Scenes
{
    internal class Info : Scene
    {
        private SoundEffectInstance menuMusic;
        private readonly Button exitButton = new Button("Exit", new Vector2(200, 40), () => TGCGame.game.ChangeScene(new MainMenu()));

        internal override void Initialize()
        {
            new DeathStar().Create(true);
            TGCGame.camera.SetLocation(new Vector3(0f, 0f, 0f), Vector3.Forward, Vector3.Up);

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
            exitButton.Update(TGCGame.gui.ScreenCenter + new Vector2(250, 200));
            base.Update(gameTime);
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
            float i = 1f;
            Vector2 center = TGCGame.gui.ScreenCenter;
            Vector2 title = TGCGame.gui.DrawCenteredText("objective and controls", new Vector2(center.X - 100, center.Y / 4 ), 25f);
            TGCGame.gui.DrawCenteredText("Your goal is to destroy the Death Star", new Vector2(center.X, center.Y / 4 + title.Y * i), 12f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("To accomplish that, you must shoot the objective shown by the compass", new Vector2(center.X, center.Y / 4 + title.Y * i), 12f);
            i += 1f;
            TGCGame.gui.DrawCenteredText("Dive: W", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Go up: S", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Strafe left: A", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Strafe right: D", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Roll right: E", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Roll left: Q", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Turbo: Left Shift", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Slow down: Left Ctrl", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Barrel Roll: Spacebar", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("God mode: G", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);
            i += 0.5f;
            TGCGame.gui.DrawCenteredText("Exit: Esc", new Vector2(center.X, center.Y / 4 + title.Y * i), 11f);


            exitButton.Draw(center + new Vector2(250, 200));
        }

        internal override void Destroy()
        {
            menuMusic.Stop();
            base.Destroy();
        }
    }
}