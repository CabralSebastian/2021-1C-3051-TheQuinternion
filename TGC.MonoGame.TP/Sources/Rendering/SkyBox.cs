using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Rendering
{
    internal class SkyBox
    {
        public readonly Model Model;
        private readonly TextureCube Texture;
        private readonly Effect Effect;
        private readonly float Size;

        internal SkyBox(Model model, TextureCube texture, Effect effect, float size)
        {
            this.Model = model;
            this.Texture = texture;
            this.Effect = effect;
            this.Size = size;
        }

        public void Draw(Matrix viewProjection, Vector3 cameraPosition)
        {
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var mesh in Model.Meshes)
                {
                    foreach (var part in mesh.MeshParts)
                    {
                        part.Effect = Effect;
                        part.Effect.Parameters["World"].SetValue(Matrix.CreateScale(Size) * Matrix.CreateTranslation(cameraPosition));
                        part.Effect.Parameters["ViewProjection"].SetValue(viewProjection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(Texture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    }
                    mesh.Draw();
                }
            }
        }
    }
}