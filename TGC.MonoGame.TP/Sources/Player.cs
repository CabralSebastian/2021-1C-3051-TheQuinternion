using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Scenes;

namespace TGC.MonoGame.TP
{
    internal class Player
    {
        private Vector2 MouseOuterBox => TGCGame.gui.ScreenSize / 7;
        private Vector2 MouseInnerBox => TGCGame.gui.ScreenSize / 10;

        private Vector2 lastMousePosition;

        internal void Update(GameTime gameTime)
        {
            double totalTime = gameTime.TotalGameTime.TotalMilliseconds;
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            ProcessMouseMovement(elapsedTime);
            if (Input.SecondaryFire())
                World.xwing.SecondaryFire(totalTime, Input.MousePosition() - TGCGame.gui.ScreenCenter);
            else if (Input.Fire())
                World.xwing.Fire(totalTime, MouseDirection());
            if (Input.GodMode())
                World.xwing.ToggleGodMode();
        }

        //MOUSE//
        private bool MouseIsInside() =>
            (Input.MousePosition().X < TGCGame.gui.ScreenCenter.X + MouseInnerBox.X && Input.MousePosition().X > TGCGame.gui.ScreenCenter.X - MouseInnerBox.X) && 
            (Input.MousePosition().Y < TGCGame.gui.ScreenCenter.Y + MouseInnerBox.Y && Input.MousePosition().Y > TGCGame.gui.ScreenCenter.Y - MouseInnerBox.Y);
        
        /*private void ProcessMouseMovement(float elapsedTime)
        {
            Vector2 mouseDelta = (Input.MousePosition() - lastMousePosition);
            var fixValue = 0.001f;
            TGCGame.camera.UpdateYawNPitch(mouseDelta * elapsedTime * fixValue);

            //if (!MouseIsInside())
            //    TGCGame.world.xwing.addRotation(new Vector3(0, Math.Sign(-mouseDelta.X), 0));
            //TGCGame.world.xwing.addRotation(new Vector3(0, Math.Sign(-mouseDelta.X), Math.Sign(-mouseDelta.X)));

            //if (Input.MousePosition().Y == screenCenter.Y + mouseInnerBox.Y || Input.MousePosition().Y == screenCenter.Y - mouseInnerBox.Y)
            //    TGCGame.world.xwing.addRotation(new Vector3(Math.Sign(mouseDelta.Y), 0, 0));

            int limitedToOuterBoxSideX = (int)Math.Clamp(Input.MousePosition().X, TGCGame.gui.ScreenCenter.X - MouseOuterBox.X, TGCGame.gui.ScreenCenter.X + MouseOuterBox.X);
            int limitedToOuterBoxSideY = (int)Math.Clamp(Input.MousePosition().Y, TGCGame.gui.ScreenCenter.Y - MouseOuterBox.Y, TGCGame.gui.ScreenCenter.Y + MouseOuterBox.Y);

            Mouse.SetPosition(limitedToOuterBoxSideX, limitedToOuterBoxSideY);
            lastMousePosition = Input.MousePosition();
        }*/

        private void ProcessMouseMovement(float elapsedTime)
        {
            Vector2 mouseDelta = (Input.MousePosition() - lastMousePosition);
            
            var fixValue = 0.001f;
            mouseDelta *= fixValue * elapsedTime;

            TGCGame.camera.UpdateYawNPitch(mouseDelta);
            //World.xwing.Move(new Vector2(Math.Sign(mouseDelta.X), Math.Sign(mouseDelta.Y)));

            Mouse.SetPosition((int)TGCGame.gui.ScreenCenter.X, (int)TGCGame.gui.ScreenCenter.Y);
            Mouse.SetCursor(MouseCursor.Crosshair);

            lastMousePosition = Input.MousePosition();
        }

        internal void DrawHUD(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 mousePosition = Input.MousePosition();
            Vector3 pos = World.xwing.Position();

            Vector3 nearScreenPoint = new Vector3(mousePosition.X, mousePosition.Y, 0.00001f); // must be more then zero.
            Vector3 nearWorldPoint = Unproject(nearScreenPoint);

            Vector3 farScreenPoint = new Vector3(mousePosition.X, mousePosition.Y, 1f); // the projection matrice's far plane value.
            Vector3 farWorldPoint = Unproject(farScreenPoint);

            
            TGCGame.gui.DrawText("Salud: " + World.xwing.salud, new Vector2(graphicsDevice.Viewport.Width / 100, graphicsDevice.Viewport.Width / 100), 12f);
            TGCGame.gui.DrawText("XWing pos: (" + pos.X + ", " + pos.Y + ", " + pos.Z + ")", new Vector2(graphicsDevice.Viewport.Width / 5, 50), 12f);
            TGCGame.gui.DrawText("Near unprojected point: (" + nearWorldPoint.X + ", " + nearWorldPoint.Y + ", " + nearWorldPoint.Z + ")", new Vector2(graphicsDevice.Viewport.Width / 5, 30), 12f);
            TGCGame.gui.DrawText("Far unprojected point: (" + farWorldPoint.X + ", " + farWorldPoint.Y + ", " + farWorldPoint.Z + ")", new Vector2(graphicsDevice.Viewport.Width / 5, 15), 12f);

            Vector3 resta = farWorldPoint - pos;
            TGCGame.gui.DrawText("Resta: (" + resta.X + ", " + resta.Y + ", " + resta.Z + ")", new Vector2(graphicsDevice.Viewport.Width / 5, 0), 12f);

            spriteBatch.Draw(
                TGCGame.content.T_TargetCursor, 
                new Rectangle(
                    (int)mousePosition.X - 10,
                    (int)mousePosition.Y - 10,
                    20, 20
                ), 
                Color.White);
        }
        public Ray GetScreenVector2AsRayInto3dWorld(Vector2 screenPosition, Matrix projectionMatrix, Matrix viewMatrix, Matrix cameraWorld, float near, GraphicsDevice device)
        {
            //if (far > 1.0f) // this is actually a misnomer which caused me a headache this is supposed to be the max clip value not the far plane.
            //    throw new ArgumentException("Far Plane can't be more then 1f or this function will fail to work in many cases");
            Vector3 nearScreenPoint = new Vector3(screenPosition.X, screenPosition.Y, near); // must be more then zero.
            Vector3 nearWorldPoint = Unproject(nearScreenPoint);

            Vector3 farScreenPoint = new Vector3(screenPosition.X, screenPosition.Y, 1f); // the projection matrice's far plane value.
            Vector3 farWorldPoint = Unproject(farScreenPoint);

            Vector3 worldRaysNormal = Vector3.Normalize((farWorldPoint + nearWorldPoint) - nearWorldPoint);
            return new Ray(nearWorldPoint, worldRaysNormal);
        }

        internal Vector3 MouseDirection()
        {
            Vector3 xwingPosition = World.xwing.Position();
            Vector2 mousePosition = Input.MousePosition();

            Vector3 nearScreenPoint = new Vector3(mousePosition.X, mousePosition.Y, 0.00001f); // must be more then zero.
            Vector3 nearWorldPoint = Unproject(nearScreenPoint);

            Vector3 farScreenPoint = new Vector3(mousePosition.X, mousePosition.Y, 1f); // the projection matrice's far plane value.
            Vector3 farWorldPoint = Unproject(farScreenPoint);

            return Vector3.Normalize(farWorldPoint - xwingPosition);
        }


        public Vector3 Unproject(Vector3 position)
        {
            GraphicsDevice gd = TGCGame.game.GraphicsDevice;
            Camera camera = TGCGame.camera;

            if (position.Z > gd.Viewport.MaxDepth)
                throw new Exception("Source Z must be less than MaxDepth ");
            Matrix wvp = Matrix.Multiply(camera.View, camera.Projection);
            Matrix inv = Matrix.Invert(wvp);
            Vector3 clipSpace = position;
            clipSpace.X = (((position.X - gd.Viewport.X) / ((float)gd.Viewport.Width)) * 2f) - 1f;
            clipSpace.Y = -((((position.Y - gd.Viewport.Y) / ((float)gd.Viewport.Height)) * 2f) - 1f);
            clipSpace.Z = (position.Z - gd.Viewport.MinDepth) / (gd.Viewport.MaxDepth - gd.Viewport.MinDepth);
            Vector3 invsrc = Vector3.Transform(clipSpace, inv);
            float a = (((clipSpace.X * inv.M14) + (clipSpace.Y * inv.M24)) + (clipSpace.Z * inv.M34)) + inv.M44;
            return invsrc / a;
        }

    }
}