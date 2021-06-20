using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Rendering;

namespace TGC.MonoGame.TP.Drawers
{
    internal class MainDrawer : Drawer
    {
        private static Effect Effect => TGCGame.content.E_MainShader;
        protected readonly Model model;
        protected readonly Texture2D[] textures;
        protected readonly Material material;

        internal MainDrawer(Model model, Texture2D[] textures, Material material)
        {
            this.model = model;
            this.textures = textures;
            this.material = material;
        }

        internal override void Draw(Matrix generalWorldMatrix)
        {
            int index = 0;
            ModelMeshCollection meshes = model.Meshes;
            foreach (var mesh in meshes)
            {
                Matrix worldMatrix = mesh.ParentBone.Transform * generalWorldMatrix;
                material.Set();
                Effect.Parameters["World"].SetValue(worldMatrix);
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                Effect.Parameters["baseTexture"].SetValue(textures[index]);
                mesh.Draw();
                index++;
            }
        }
    }
}