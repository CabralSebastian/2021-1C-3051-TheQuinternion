using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Drawers
{
    internal class LaserDrawer : Drawer
    {
        private static Effect Effect => TGCGame.content.E_LaserShader;
        protected readonly Model model;

        internal LaserDrawer(Model model)
        {
            this.model = model;
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
                mesh.Draw();
                index++;
            }
        }
    }
}