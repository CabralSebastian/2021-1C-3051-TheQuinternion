using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Rendering;

namespace TGC.MonoGame.TP.Drawers
{
    internal class MainDrawer : Drawer
    {
        private static Effect Effect => TGCGame.GameContent.E_MainShader;
        protected readonly Model Model;
        protected readonly Texture2D[] Textures;
        protected readonly Material Material;

        internal MainDrawer(Model model, Texture2D[] textures, Material material)
        {
            this.Model = model;
            this.Textures = textures;
            this.Material = material;
        }

        internal override void Draw(Matrix generalWorldMatrix)
        {
            int index = 0;
            ModelMeshCollection meshes = Model.Meshes;
            foreach (var mesh in meshes)
            {
                Matrix worldMatrix = mesh.ParentBone.Transform * generalWorldMatrix;
                Material.Set();
                Effect.Parameters["World"].SetValue(worldMatrix);
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                Effect.Parameters["baseTexture"].SetValue(Textures[index]);
                mesh.Draw();
                index++;
            }
        }
    }
}