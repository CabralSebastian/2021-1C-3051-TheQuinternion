using Microsoft.Xna.Framework;
using System;

namespace TGC.MonoGame.TP.GraphicInterface
{
    internal class Button
    {
        private readonly string Text;
        private readonly Vector2 Size;
        private readonly Action OnClick;
        private Color Color = Color.Transparent;
        private bool MouseOver = false;

        internal Button(string text, Vector2 size, Action onClick)
        {
            this.Text = text;
            this.Size = size;
            this.OnClick = onClick;
        }

        internal void Draw(Vector2 position)
        {
            TGCGame.Gui.DrawCenteredSprite(TGCGame.GameContent.T_Pixel, position, Size, Color);
            TGCGame.Gui.DrawCenteredText(Text, position, 16f);
        }

        private bool IsMouseOver(Vector2 position) =>
            Input.MousePosition().X > position.X - Size.X / 2 && Input.MousePosition().X < position.X + Size.X / 2 &&
            Input.MousePosition().Y > position.Y - Size.Y / 2 && Input.MousePosition().Y < position.Y + Size.Y / 2;

        internal void Update(Vector2 position)
        {
            if (IsMouseOver(position))
            {
                if (Input.Click())
                {
                    TGCGame.GameContent.S_Click1.CreateInstance().Play();
                    Color = new Color(0, 0, 0, 200);
                    OnClick.Invoke();
                }
                else
                {
                    Color = new Color(0, 0, 0, 100);
                    if (!MouseOver)
                    {
                        TGCGame.GameContent.S_Click2.CreateInstance().Play();
                        MouseOver = true;
                    }
                }
            }
            else
            {
                Color = new Color(0, 0, 0, 50);
                MouseOver = false;
            }
        }
    }
}