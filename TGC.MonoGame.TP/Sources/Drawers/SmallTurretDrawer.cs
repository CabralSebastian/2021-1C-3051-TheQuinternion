using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Rendering;

namespace TGC.MonoGame.TP.Drawers
{
    internal class SmallTurretDrawer : Drawer
    {
        private static Effect Effect => TGCGame.GameContent.E_MainShader;
        protected readonly Model Model;
        protected readonly Texture2D[] Texture;
        internal Matrix CannonsWorldMatrix { private get; set; }
        protected readonly Material Material;

        internal SmallTurretDrawer(Model model, Texture2D[] texture, Material material)
        {
            this.Model = model;
            this.Texture = texture;
            this.Material = material;
        }

        internal override void Draw(Matrix generalWorldMatrix)
        {
            Effect.Parameters["baseTexture"].SetValue(Texture[0]);
            Material.Set();
            DrawMesh(Model.Meshes[0], generalWorldMatrix);
            DrawMesh(Model.Meshes[1], CannonsWorldMatrix);
        }

        private void DrawMesh(ModelMesh mesh, Matrix matrix)
        {
            Matrix worldMatrix = mesh.ParentBone.Transform * matrix;
            Effect.Parameters["World"].SetValue(worldMatrix);
            Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
            mesh.Draw();
        }
    }
}