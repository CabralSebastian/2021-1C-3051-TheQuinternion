using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Drawers
{
    internal class MainDrawer : Drawer
    {
        private static Effect Effect => TGCGame.content.E_MainShader;
        protected readonly Model model;
        protected readonly Texture2D[] textures;

        internal MainDrawer(Model model, Texture2D[] textures)
        {
            this.model = model;
            this.textures = textures;
        }

        internal override void Draw(Matrix generalWorldMatrix)
        {
            int index = 0;
            ModelMeshCollection meshes = model.Meshes;
            foreach (var mesh in meshes)
            {
                Matrix worldMatrix = mesh.ParentBone.Transform * generalWorldMatrix;
                Effect.Parameters["World"].SetValue(worldMatrix);
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                Effect.Parameters["baseTexture"].SetValue(textures[index]);
                mesh.Draw();
                index++;
            }
        }
    }
}