using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Entities;

namespace TGC.MonoGame.TP.Scenes
{
    internal abstract class Scene
    {
        private readonly List<Entity> pendingEntities = new List<Entity>();
        protected readonly List<Entity> entities = new List<Entity>();
        private readonly List<Entity> removedEntities = new List<Entity>();

        internal void Register(Entity entity) => pendingEntities.Add(entity);

        internal void Unregister(Entity entity) => removedEntities.Add(entity);

        internal abstract void Initialize();

        internal virtual void Update(GameTime gameTime)
        {
            pendingEntities.ForEach(entity => entities.Add(entity));
            pendingEntities.Clear();
            removedEntities.ForEach(entity => entities.Remove(entity));
            removedEntities.Clear();
            entities.ForEach(entity => entity.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds, gameTime));
        }

        internal virtual void Draw()
        {
            PreDrawSetShaderParameters();
            entities.ForEach(entity => entity.Draw());
        }

        internal virtual void Draw2D(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }

        private void PreDrawSetShaderParameters()
        {
            // E_BasicShader -- En desuso
            /*TGCGame.content.E_BasicShader.Parameters["View"].SetValue(TGCGame.camera.View);
            TGCGame.content.E_BasicShader.Parameters["Projection"].SetValue(TGCGame.camera.Projection);*/

            // E_BlinnPhong
            TGCGame.content.E_MainShader.Parameters["ambientColor"]?.SetValue(new Vector3(1f, 1f, 1f));
            TGCGame.content.E_MainShader.Parameters["diffuseColor"]?.SetValue(new Vector3(1f, 1f, 1f));
            TGCGame.content.E_MainShader.Parameters["specularColor"]?.SetValue(new Vector3(1f, 1f, 1f));

            TGCGame.content.E_MainShader.Parameters["KAmbient"]?.SetValue(0.8f);
            TGCGame.content.E_MainShader.Parameters["KDiffuse"]?.SetValue(0.8f);
            TGCGame.content.E_MainShader.Parameters["KSpecular"]?.SetValue(0.1f);
            TGCGame.content.E_MainShader.Parameters["shininess"]?.SetValue(8.0f);

            //TGCGame.content.E_BlinnPhong.Parameters["ViewProjection"].SetValue(TGCGame.camera.View * TGCGame.camera.Projection);
            //TGCGame.content.E_BlinnPhong.Parameters["lightPosition"].SetValue(new Vector3(100000f, 80000f, 50000f));
            //TGCGame.content.E_BlinnPhong.Parameters["eyePosition"].SetValue(TGCGame.camera.position);

            // E_LaserShader
            //TGCGame.content.E_LaserShader.Parameters["View"].SetValue(TGCGame.camera.View);
            //TGCGame.content.E_LaserShader.Parameters["Projection"].SetValue(TGCGame.camera.Projection);
        }

        internal virtual void Destroy()
        {
            pendingEntities.ForEach(entity => entity.Destroy());
            removedEntities.ForEach(entity => entities.Remove(entity));
            entities.ForEach(entity => entity.Destroy());
        }
    }
}