using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Drawers
{
    internal class LaserDrawer : Drawer
    {
        private static Effect Effect => TGCGame.GameContent.E_LaserShader;
        protected readonly Model Model;

        internal LaserDrawer(Model model)
        {
            this.Model = model;
        }

        internal override void Draw(Matrix generalWorldMatrix)
        {
            int index = 0;
            ModelMeshCollection meshes = Model.Meshes;
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