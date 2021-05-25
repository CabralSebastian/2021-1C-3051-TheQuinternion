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

        private Player player;

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
            player = new Player(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
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
            camera.Update();
            base.Update(gameTime);
            player.Update(elapsedTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

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