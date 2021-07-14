using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Entities;

namespace TGC.MonoGame.TP.Scenes
{
    internal abstract class Scene
    {
        private readonly List<Entity> PendingEntities = new List<Entity>();
        protected readonly List<Entity> Entities = new List<Entity>();
        private readonly List<Entity> RemovedEntities = new List<Entity>();

        internal void Register(Entity entity) => PendingEntities.Add(entity);

        internal void Unregister(Entity entity) => RemovedEntities.Add(entity);

        internal abstract void Initialize();

        internal virtual void Update(GameTime gameTime)
        {
            PendingEntities.ForEach(entity => Entities.Add(entity));
            PendingEntities.Clear();
            RemovedEntities.ForEach(entity => Entities.Remove(entity));
            RemovedEntities.Clear();
            Entities.ForEach(entity => entity.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds, gameTime));
        }

        internal virtual void Draw()
        {
            PreDrawSetShaderParameters();
            Entities.ForEach(entity => entity.Draw());
        }

        internal virtual void Draw2D(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }

        private void PreDrawSetShaderParameters()
        {
            // E_BlinnPhong
            TGCGame.GameContent.E_MainShader.Parameters["ambientColor"]?.SetValue(new Vector3(1f, 1f, 1f));
            TGCGame.GameContent.E_MainShader.Parameters["diffuseColor"]?.SetValue(new Vector3(1f, 1f, 1f));
            TGCGame.GameContent.E_MainShader.Parameters["specularColor"]?.SetValue(new Vector3(1f, 1f, 1f));
        }

        internal virtual void Destroy()
        {
            PendingEntities.ForEach(entity => entity.Destroy());
            RemovedEntities.ForEach(entity => Entities.Remove(entity));
            Entities.ForEach(entity => entity.Destroy());
        }
    }
}