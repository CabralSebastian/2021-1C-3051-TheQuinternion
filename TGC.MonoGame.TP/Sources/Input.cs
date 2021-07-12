using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP
{
    internal static class Input
    {
        private static KeyboardState KeyboardState = Keyboard.GetState();
        private static KeyboardState PrevKeyboardState = Keyboard.GetState();

        private static MouseState MouseState = Mouse.GetState();
        private static MouseState PrevMouseState = Mouse.GetState();

        internal static void Update()
        {
            PrevKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
            PrevMouseState = MouseState;
            MouseState = Mouse.GetState();
        }

        internal static bool Submit() => KeyboardState.IsKeyDown(Keys.Enter) || KeyboardState.IsKeyDown(Keys.Insert);
        private static bool MovingRight() => KeyboardState.IsKeyDown(Keys.D);
        private static bool MovingLeft() => KeyboardState.IsKeyDown(Keys.A);
        private static bool MovingUp() => KeyboardState.IsKeyDown(Keys.S);
        private static bool MovingDown() => KeyboardState.IsKeyDown(Keys.W);
        private static int BoolToInt(bool b) => b ? 1 : 0;
        private static int BoolsToAxis(bool positive, bool negative) => BoolToInt(positive) - BoolToInt(negative);
        internal static int HorizontalAxis() => BoolsToAxis(MovingRight(), MovingLeft());
        internal static int VerticalAxis() => BoolsToAxis(MovingUp(), MovingDown());
        internal static int FrontalRotationAxis() => BoolsToAxis(KeyboardState.IsKeyDown(Keys.E), KeyboardState.IsKeyDown(Keys.Q));
        internal static bool Accelerate() => KeyboardState.IsKeyDown(Keys.LeftShift);
        internal static bool Deaccelerate() => KeyboardState.IsKeyDown(Keys.LeftControl);
        internal static bool GodMode() => KeyboardState.IsKeyDown(Keys.G) && !PrevKeyboardState.IsKeyDown(Keys.G);
        internal static bool Exit() => KeyboardState.IsKeyDown(Keys.Escape);
        internal static bool ToggleF1() => KeyboardState.IsKeyDown(Keys.F1) && !PrevKeyboardState.IsKeyDown(Keys.F1);
        internal static bool BarrelRoll() => KeyboardState.IsKeyDown(Keys.Space) && !PrevKeyboardState.IsKeyDown(Keys.Space);

        //MOUSE//
        internal static Vector2 MousePosition() => MouseState.Position.ToVector2();
        internal static bool Click() => MouseState.LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton == ButtonState.Released;
        internal static bool Fire() => MouseState.LeftButton == ButtonState.Pressed;
        internal static bool SecondaryFire() => MouseState.RightButton == ButtonState.Pressed;
    }
}