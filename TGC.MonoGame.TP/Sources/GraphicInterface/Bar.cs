using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.GraphicInterface
{
    internal class Bar
    {
        private readonly Vector2 margin = new Vector2(3f, 3f);
        private readonly Vector2 size;
        private readonly Color color;
        private readonly float maxValue;

        internal Bar(Vector2 size, Color color, float maxValue)
        {
            this.size = size;
            this.color = color;
            this.maxValue = maxValue;
        }

        internal void Draw(Vector2 position, float value)
        {
            TGCGame.gui.DrawCenteredSprite(TGCGame.content.T_Pixel, position, size, new Color(0, 0, 0, 100));
            Vector2 innerSize = size - margin * 2;
            innerSize.X = innerSize.X * value / maxValue;
            TGCGame.gui.DrawSprite(TGCGame.content.T_Pixel, position - size / 2 + margin, innerSize, color);
        }
    }
}