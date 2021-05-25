using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Physics;

namespace TGC.MonoGame.TP
{
    internal class TGCGame : Game
    {
        
        internal static Content content;
        internal static readonly PhysicSimulation physicSimulation = new PhysicSimulation();
        private SpriteBatch spriteBatch;
        private FullScreenQuad fullScreenQuad;

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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fullScreenQuad = new FullScreenQuad(GraphicsDevice);
            content = new Content(Content);
            base.LoadContent();
        }

        protected override void Initialize()
        {
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace };
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
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            content.E_BasicShader.Parameters["View"].SetValue(camera.View);
            content.E_BasicShader.Parameters["Projection"].SetValue(camera.Projection);
            world.Draw();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.DepthRead, RasterizerState.CullNone);
            player.DrawHUD(spriteBatch, GraphicsDevice);
            spriteBatch.End();
        }

        protected override void UnloadContent()
        {
            Content.Unload();
            physicSimulation.Dispose();
            fullScreenQuad.Dispose();
            base.UnloadContent();
        }
    }
}