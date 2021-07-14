using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.GraphicInterface
{
    internal class Bar
    {
        private readonly Vector2 Margin = new Vector2(3f, 3f);
        private readonly Vector2 Size;
        private readonly Color Color;
        private readonly float MaxValue;

        internal Bar(Vector2 size, Color color, float maxValue)
        {
            this.Size = size;
            this.Color = color;
            this.MaxValue = maxValue;
        }

        internal void Draw(Vector2 position, float value)
        {
            TGCGame.Gui.DrawCenteredSprite(TGCGame.GameContent.T_Pixel, position, Size, new Color(0, 0, 0, 100));
            Vector2 innerSize = Size - Margin * 2;
            innerSize.X = innerSize.X * value / MaxValue;
            TGCGame.Gui.DrawSprite(TGCGame.GameContent.T_Pixel, position - Size / 2 + Margin, innerSize, Color);
        }
    }
}