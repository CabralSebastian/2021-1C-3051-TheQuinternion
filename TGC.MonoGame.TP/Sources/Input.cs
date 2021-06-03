using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP
{
    internal static class Input
    {
        private static KeyboardState KeyboardState => Keyboard.GetState();

        private static bool MovingRight() => KeyboardState.IsKeyDown(Keys.D) || KeyboardState.IsKeyDown(Keys.Right);
        private static bool MovingLeft() => KeyboardState.IsKeyDown(Keys.A) || KeyboardState.IsKeyDown(Keys.Left);
        private static bool MovingUp() => KeyboardState.IsKeyDown(Keys.S);
        private static bool MovingDown() => KeyboardState.IsKeyDown(Keys.W);
        private static int BoolToInt(bool b) => b ? 1 : 0;
        private static int BoolsToAxis(bool positive, bool negative) => BoolToInt(positive) - BoolToInt(negative);
        internal static int HorizontalAxis() => BoolsToAxis(MovingRight(), MovingLeft());
        internal static int VerticalAxis() => BoolsToAxis(MovingUp(), MovingDown());
        internal static bool Accelerate() => KeyboardState.IsKeyDown(Keys.LeftShift);
        internal static bool GodMode() => KeyboardState.IsKeyDown(Keys.G);
        internal static bool Exit() => KeyboardState.IsKeyDown(Keys.Escape);

        //MOUSE//
        internal static Vector2 MousePosition() => Mouse.GetState().Position.ToVector2();
        internal static bool Fire() => Mouse.GetState().LeftButton == ButtonState.Pressed;
    }
}