using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Rendering
{
    public class FullScreenQuad
    {
        private readonly GraphicsDevice Device;
        private IndexBuffer IndexBuffer;
        private VertexBuffer VertexBuffer;

        public FullScreenQuad(GraphicsDevice device)
        {
            this.Device = device;
            CreateVertexBuffer();
            CreateIndexBuffer();
        }

        private void CreateVertexBuffer()
        {
            var vertices = new VertexPositionTexture[4];
            vertices[0].Position = new Vector3(-1f, -1f, 0f);
            vertices[0].TextureCoordinate = new Vector2(0f, 1f);
            vertices[1].Position = new Vector3(-1f, 1f, 0f);
            vertices[1].TextureCoordinate = new Vector2(0f, 0f);
            vertices[2].Position = new Vector3(1f, -1f, 0f);
            vertices[2].TextureCoordinate = new Vector2(1f, 1f);
            vertices[3].Position = new Vector3(1f, 1f, 0f);
            vertices[3].TextureCoordinate = new Vector2(1f, 0f);

            VertexBuffer = new VertexBuffer(Device, VertexPositionTexture.VertexDeclaration, 4,
                BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices);
        }

        private void CreateIndexBuffer()
        {
            var indices = new ushort[6];

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 3;
            indices[3] = 0;
            indices[4] = 3;
            indices[5] = 2;

            IndexBuffer = new IndexBuffer(Device, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            IndexBuffer.SetData(indices);
        }


        public void Draw(Effect effect)
        {
            Device.SetVertexBuffer(VertexBuffer);
            Device.Indices = IndexBuffer;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}