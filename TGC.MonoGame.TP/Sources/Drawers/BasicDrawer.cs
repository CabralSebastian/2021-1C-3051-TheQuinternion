using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Drawers
{
    internal class BasicDrawer : Drawer
    {
        private static Effect Effect => TGCGame.content.E_BasicShader;
        protected readonly Model model;
        protected readonly Texture2D[] textures;

        internal BasicDrawer(Model model, Texture2D[] textures)
        {
            this.model = model;
            this.textures = textures;
        }

        internal static void PreDraw()
        {
            Effect.Parameters["View"].SetValue(TGCGame.camera.View);
            Effect.Parameters["Projection"].SetValue(TGCGame.camera.Projection);
        }

        internal override void Draw(Matrix generalWorldMatrix)
        {
            int index = 0;
            ModelMeshCollection meshes = model.Meshes;
            foreach (var mesh in meshes)
            {
                Matrix worldMatrix = mesh.ParentBone.Transform * generalWorldMatrix;
                Effect.Parameters["World"].SetValue(worldMatrix);
                Effect.Parameters["ModelTexture"].SetValue(textures[index]);
                mesh.Draw();
                index++;
            }
        }
    }
}