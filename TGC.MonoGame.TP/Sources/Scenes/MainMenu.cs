using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.GraphicInterface;


namespace TGC.MonoGame.TP.Scenes
{
    internal class MainMenu : Scene
    {
        private SoundEffectInstance MenuMusic;
        private readonly Button StartButton = new Button("Start", new Vector2(200, 40), () => TGCGame.Game.ChangeScene(new World()));
        private readonly Button InfoButton = new Button("Help", new Vector2(200, 40), () => TGCGame.Game.ChangeScene(new Info()));
        private readonly Button ExitButton = new Button("Exit", new Vector2(200, 40), () => TGCGame.Game.Exit());

        internal override void Initialize()
        {
            new DeathStar().Create(true);
            TGCGame.Camera.SetLocation(new Vector3(0f, 0f, 0f), Vector3.Forward, Vector3.Up);

            Quaternion toLeft = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);
            Quaternion toRight = Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2);
            SpawnSquad(4, 100f, -800f, 0f, -100f, toLeft);
            SpawnSquad(5, 200f, -800f, 0f, -500f, toLeft);
            SpawnSquad(3, 300f, -3000f, 0f, -200f, toRight);
            SpawnSquad(3, 300f, -10000f, 0f, -1000f, toLeft);

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
            InfoButton.Update(TGCGame.Gui.ScreenCenter + new Vector2(0, 100));
            ExitButton.Update(TGCGame.Gui.ScreenCenter + new Vector2(0, 150));
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
            Vector2 center = TGCGame.Gui.ScreenCenter;
            Vector2 starWarsSize = TGCGame.Gui.DrawCenteredText("Star Wars", new Vector2(center.X, center.Y / 4), 20f);
            TGCGame.Gui.DrawCenteredText("Trench Run", new Vector2(center.X, center.Y / 4 + starWarsSize.Y + 5), 48f);
            StartButton.Draw(center + new Vector2(0, 50));
            InfoButton.Draw(center + new Vector2(0, 100));
            ExitButton.Draw(center + new Vector2(0, 150));
        }

        internal override void Destroy()
        {
            MenuMusic.Stop();
            base.Destroy();
        }
    }
}