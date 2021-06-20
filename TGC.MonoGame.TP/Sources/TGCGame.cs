using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;
using TGC.MonoGame.TP.GraphicInterface;
using TGC.MonoGame.TP.Rendering;
using TGC.MonoGame.TP.Rendering.Cameras;

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
        internal const int shadowmapSize = 2048 * 8;
        internal const int cameraSize = 2048 * 2;
        private RenderTarget2D shadowMapRenderTarget;

        internal static readonly SoundManager soundManager = new SoundManager();
        internal static Scene currentScene;
        internal static Camera camera = new Camera();
        internal static readonly LightCamera lightCamera = new LightCamera();

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
            skyBox = new SkyBox(content.M_SkyBox, content.TC_Space, content.E_SkyBox, 3000f);
            shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, shadowmapSize, shadowmapSize, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
        }

        protected override void Initialize()
        {
            base.Initialize();
            Window.Title = "Star Wars: Trench Run";
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace };

            ChangeScene(new MainMenu());
            camera.Initialize(GraphicsDevice);

            content.E_MainShader.Parameters["shadowMapSize"]?.SetValue(Vector2.One * shadowmapSize);
            content.E_MainShader.Parameters["lightPosition"]?.SetValue(lightCamera.Position);
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

            FirstPass();
            SecondPass();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.DepthRead, RasterizerState.CullNone);
            currentScene.Draw2D(GraphicsDevice, spriteBatch);
            spriteBatch.End();
        }

        private void FirstPass()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SetRenderTarget(shadowMapRenderTarget); //shadowMapRenderTarget null
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            lightCamera.Update();
            content.E_MainShader.CurrentTechnique = content.E_MainShader.Techniques["DepthPass"];
            content.E_LaserShader.CurrentTechnique = content.E_LaserShader.Techniques["DepthPass"];

            content.E_MainShader.Parameters["ViewProjection"].SetValue(lightCamera.ViewProjection);
            content.E_LaserShader.Parameters["ViewProjection"].SetValue(lightCamera.ViewProjection);

            skyBox.Draw(camera.ViewProjection, camera.position);
            currentScene.Draw();
        }

        private void SecondPass()
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            content.E_MainShader.CurrentTechnique = content.E_MainShader.Techniques["DrawShadowed"];
            content.E_LaserShader.CurrentTechnique = content.E_LaserShader.Techniques["DrawShadowed"];

            content.E_MainShader.Parameters["ViewProjection"].SetValue(camera.ViewProjection);
            content.E_LaserShader.Parameters["ViewProjection"].SetValue(camera.ViewProjection);

            content.E_MainShader.Parameters["cameraPosition"]?.SetValue(camera.position);
            content.E_MainShader.Parameters["LightViewProjection"].SetValue(lightCamera.ViewProjection);
            content.E_MainShader.Parameters["lightDirection"].SetValue(-lightCamera.Direction);
            content.E_MainShader.Parameters["shadowMap"]?.SetValue(shadowMapRenderTarget);

            skyBox.Draw(camera.ViewProjection, camera.position);
            currentScene.Draw();
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