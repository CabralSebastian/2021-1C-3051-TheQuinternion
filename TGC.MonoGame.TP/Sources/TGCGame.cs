using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;
using TGC.MonoGame.TP.GraphicInterface;
using TGC.MonoGame.TP.Rendering;

namespace TGC.MonoGame.TP
{
    internal class TGCGame : Game
    {
        internal static TGCGame game;
        internal static Content content;
        internal static GUI gui;
        internal static readonly PhysicSimulation physicSimulation = new PhysicSimulation();
        private SpriteBatch spriteBatch;
        private SkyBox skyBox;
        private FullScreenQuad fullScreenQuad;

        internal static readonly SoundManager soundManager = new SoundManager();
        internal static Scene currentScene;
        internal static Camera camera = new Camera();

        internal Vector2 WindowSize() => new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);

        internal TGCGame()
        {
            game = this;
            new GraphicsDeviceManager(this);
            // Graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gui = new GUI(GraphicsDevice, spriteBatch);
            fullScreenQuad = new FullScreenQuad(GraphicsDevice);
            content = new Content(Content);
            skyBox = new SkyBox(content.M_SkyBox, content.TC_Space, content.E_SkyBox, 5000f);
        }

        protected override void Initialize()
        {
            base.Initialize();
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace };
            ChangeScene(new MainMenu());
            camera.Initialize(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.Exit())
                Exit();

            currentScene.Update(gameTime);
            camera.Update();
            physicSimulation.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            skyBox.Draw(camera.View, camera.Projection, camera.position);
            currentScene.Draw();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.DepthRead, RasterizerState.CullNone);
            currentScene.Draw2D(GraphicsDevice, spriteBatch);
            spriteBatch.End();
        }

        protected override void UnloadContent()
        {
            Content.Unload();
            physicSimulation.Dispose();
            fullScreenQuad.Dispose();
            base.UnloadContent();
        }

        internal void ChangeScene(Scene scene)
        {
            currentScene?.Destroy();
            currentScene = scene;
            scene.Initialize();
        }
    }
}