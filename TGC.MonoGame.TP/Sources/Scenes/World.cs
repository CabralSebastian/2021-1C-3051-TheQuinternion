using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP.Scenes
{
    internal class World : Scene
    {
        private SoundEffectInstance GameMusic;
        internal static DeathStar DeathStar;
        internal static XWing XWing;
        internal static DistantFight DistantFight;
        private Player Player;

        private readonly Random Random = new Random();
        private double LastTieSpawn;
        private const double MinTIESpawnTime = 2000;

        internal override void Initialize()
        {
            Player = new Player();
            DeathStar = new DeathStar();
            DeathStar.Create(true);
            XWing = new XWing();
            XWing.Instantiate(new Vector3(50f, 0f, 0f));
            DistantFight = new DistantFight();
            DistantFight.Create();
            TGCGame.Camera.SetLocation(new Vector3(80f, 0f, 0f), Vector3.Forward, Vector3.Up);
            TGCGame.Camera.SetTarget(XWing);

            PlayMusic();
            TGCGame.Game.IsMouseVisible = false;
        }

        private void PlayMusic()
        {
            GameMusic = TGCGame.GameContent.S_GameMusic.CreateInstance();
            GameMusic.IsLooped = true;
            GameMusic.Volume = 0.2f;
            GameMusic.Play();
        }

        internal override void Update(GameTime gameTime)
        {
            TIESpawn(gameTime);
            Player.Update(gameTime);
            base.Update(gameTime);
        }

        internal override void Draw2D(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Player.DrawHUD(graphicsDevice, spriteBatch);
        }

        private void TIESpawn(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds < LastTieSpawn + MinTIESpawnTime)
                return;

            if (Random.NextDouble() > 0.8f)
                new TIE().Instantiate(new Vector3(Random.Next(-4000, 4000), 0f, Random.Next(-4000, 4000)));
            LastTieSpawn = gameTime.TotalGameTime.TotalMilliseconds;
        }

        internal static void InstantiateLaser(Vector3 position, Vector3 forward, Quaternion orientation, AudioEmitter emitter, float volume = 0.01f)
        {
            new Laser().Instantiate(position - forward * 5f, orientation);
            SoundEffectInstance sound = TGCGame.GameContent.S_Laser.CreateInstance();
            sound.Volume = volume;
            TGCGame.SoundManager.PlaySound(sound, emitter);
        }

        internal override void Destroy()
        {
            GameMusic.Stop();
            base.Destroy();
        }
    }
}