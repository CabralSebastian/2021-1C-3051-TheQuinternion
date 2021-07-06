using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Rendering;

namespace TGC.MonoGame.TP.Drawers
{
    internal class VaderDrawer : Drawer
    {
        private static Effect Effect => TGCGame.content.E_MainShader;
        protected readonly Model model;
        protected readonly Material material;

        internal VaderDrawer(Model model, Material material)
        {
            this.model = model;
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
                mesh.Draw();
                index++;
            }
        }
    }
}