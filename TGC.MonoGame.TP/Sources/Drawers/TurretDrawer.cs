using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Rendering;

namespace TGC.MonoGame.TP.Drawers
{
    internal class TurretDrawer : Drawer
    {
        private static Effect Effect => TGCGame.content.E_MainShader;
        protected readonly Model model;
        protected readonly Texture2D[] texture;
        internal Matrix HeadWorldMatrix { private get; set; }
        internal Matrix CannonsWorldMatrix { private get; set; }
        protected readonly Material material;

        internal TurretDrawer(Model model, Texture2D[] texture, Material material)
        {
            this.model = model;
            this.texture = texture;
            this.material = material;
        }

        internal override void Draw(Matrix generalWorldMatrix)
        {
            Effect.Parameters["baseTexture"].SetValue(texture[0]);
            material.Set();
            DrawMesh(model.Meshes[1], generalWorldMatrix);
            DrawMesh(model.Meshes[2], HeadWorldMatrix);
            DrawMesh(model.Meshes[0], CannonsWorldMatrix);
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