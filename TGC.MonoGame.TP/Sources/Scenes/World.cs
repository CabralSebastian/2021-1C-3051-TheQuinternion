using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP.Scenes
{
    internal class World : Scene
    {
        private SoundEffectInstance gameMusic;
        internal static DeathStar deathStar;
        internal static XWing xwing;
        private Player player;

        private readonly Random random = new Random();
        private double lastTieSpawn;
        private const double minTIESpawnTime = 2000;

        internal override void Initialize()
        {
            player = new Player();
            deathStar = new DeathStar();
            deathStar.Create(true);
            xwing = new XWing();
            xwing.Instantiate(new Vector3(50f, 0f, 0f));
            TGCGame.camera.SetLocation(new Vector3(80f, 0f, 0f), Vector3.Forward, Vector3.Up);
            TGCGame.camera.SetTarget(xwing);

            PlayMusic();
            TGCGame.game.IsMouseVisible = false;
        }

        private void PlayMusic()
        {
            gameMusic = TGCGame.content.S_GameMusic.CreateInstance();
            gameMusic.IsLooped = true;
            gameMusic.Volume = 0.2f;
            gameMusic.Play();
        }

        internal override void Update(GameTime gameTime)
        {
            TIESpawn(gameTime);
            player.Update(gameTime);
            base.Update(gameTime);
        }

        internal override void Draw2D(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            player.DrawHUD(graphicsDevice, spriteBatch);
        }

        private void TIESpawn(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds < lastTieSpawn + minTIESpawnTime)
                return;

            if (random.NextDouble() > 0.8f)
                new TIE().Instantiate(new Vector3(random.Next(-4000, 4000), 0f, 0f));
            lastTieSpawn = gameTime.TotalGameTime.TotalMilliseconds;
        }

        internal static void InstantiateLaser(Vector3 position, Vector3 forward, Quaternion orientation, AudioEmitter emitter, float volume = 0.01f)
        {
            new Laser().Instantiate(position - forward * 5f, orientation);
            SoundEffectInstance sound = TGCGame.content.S_Laser.CreateInstance();
            sound.Volume = volume;
            TGCGame.soundManager.PlaySound(sound, emitter);
        }

        internal override void Destroy()
        {
            gameMusic.Stop();
            base.Destroy();
        }
    }
}