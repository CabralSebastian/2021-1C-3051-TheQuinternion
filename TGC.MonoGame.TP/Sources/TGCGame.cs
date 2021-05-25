using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP
{
    internal class TGCGame : Game
    {
        private SpriteBatch SpriteBatch { get; set; }

        internal static Content content;
        internal static readonly PhysicSimulation physicSimulation = new PhysicSimulation();
        internal static readonly World world = new World();
        internal static Camera camera = new Camera();
        internal static Vector2 screenCenter;
        private readonly Player player = new Player();

        internal TGCGame()
        {
            new GraphicsDeviceManager(this);
            // Graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void LoadContent()
        {
            content = new Content(Content);
            base.LoadContent();
        }

        protected override void Initialize()
        {
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
            screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            base.Initialize();
            world.Initialize();
            camera.Initialize(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.Exit())
                Exit();

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            world.Update(elapsedTime);
            physicSimulation.Update();
            camera.Update(elapsedTime);
            base.Update(gameTime);
            player.Update(elapsedTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            content.E_BasicShader.Parameters["View"].SetValue(camera.View);
            content.E_BasicShader.Parameters["Projection"].SetValue(camera.Projection);
            world.Draw();
        }

        protected override void UnloadContent()
        {
            Content.Unload();
            physicSimulation.Dispose();
            base.UnloadContent();
        }
    }
}