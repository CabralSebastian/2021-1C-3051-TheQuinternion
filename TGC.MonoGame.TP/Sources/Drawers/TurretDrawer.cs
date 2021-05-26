using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Drawers
{
    internal class TurretDrawer : Drawer
    {
        private static Effect Effect => TGCGame.content.E_BasicShader;
        protected readonly Model model;
        protected readonly Texture2D[] texture;
        internal Matrix HeadWorldMatrix { private get; set; }
        internal Matrix CannonsWorldMatrix { private get; set; }

        internal TurretDrawer(Model model, Texture2D[] texture)
        {
            this.model = model;
            this.texture = texture;
        }

        internal static void PreDraw()
        {
            Effect.Parameters["View"].SetValue(TGCGame.camera.View);
            Effect.Parameters["Projection"].SetValue(TGCGame.camera.Projection);
        }

        internal override void Draw(Matrix generalWorldMatrix)
        {
            Effect.Parameters["ModelTexture"].SetValue(texture[0]);
            DrawMesh(model.Meshes[1], generalWorldMatrix);
            DrawMesh(model.Meshes[2], HeadWorldMatrix);
            DrawMesh(model.Meshes[0], CannonsWorldMatrix);
        }

        private void DrawMesh(ModelMesh mesh, Matrix matrix)
        {
            Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * matrix);
            mesh.Draw();
        }
    }
}