using Microsoft.Xna.Framework;
using System;

namespace TGC.MonoGame.TP.GraphicInterface
{
    internal class Button
    {
        private readonly string text;
        private readonly Vector2 size;
        private readonly Action onClick;
        private Color color = Color.Transparent;
        private bool mouseOver = false;

        internal Button(string text, Vector2 size, Action onClick)
        {
            this.text = text;
            this.size = size;
            this.onClick = onClick;
        }

        internal void Draw(Vector2 position)
        {
            TGCGame.gui.DrawCenteredSprite(TGCGame.content.T_Pixel, position, size, color);
            TGCGame.gui.DrawCenteredText(text, position, 16f);
        }

        private bool IsMouseOver(Vector2 position) =>
            Input.MousePosition().X > position.X - size.X / 2 && Input.MousePosition().X < position.X + size.X / 2 &&
            Input.MousePosition().Y > position.Y - size.Y / 2 && Input.MousePosition().Y < position.Y + size.Y / 2;

        internal void Update(Vector2 position)
        {
            if (IsMouseOver(position))
            {
                if (Input.Click())
                {
                    TGCGame.content.S_Click1.CreateInstance().Play();
                    color = new Color(0, 0, 0, 200);
                    onClick.Invoke();
                }
                else
                {
                    color = new Color(0, 0, 0, 100);
                    if (!mouseOver)
                    {
                        TGCGame.content.S_Click2.CreateInstance().Play();
                        mouseOver = true;
                    }
                }
            }
            else
            {
                color = new Color(0, 0, 0, 50);
                mouseOver = false;
            }
        }
    }
}