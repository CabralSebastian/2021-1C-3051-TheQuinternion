using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;
using TGC.MonoGame.TP.GraphicInterface;
using TGC.MonoGame.TP.Rendering;
using TGC.MonoGame.TP.Rendering.Cameras;
using System;

namespace TGC.MonoGame.TP
{
    internal class TGCGame : Game
    {
        internal static TGCGame game;
        private readonly GraphicsDeviceManager graphics;
        internal static Content content;
        internal static GUI gui;
        internal static readonly PhysicSimulation physicSimulation = new PhysicSimulation();
        private SpriteBatch spriteBatch;
        private SkyBox skyBox;
        private FullScreenQuad fullScreenQuad;
        internal const int shadowmapSize = 2048 * 2;
        private RenderTarget2D shadowMapRenderTarget, mainRenderTarget, bloomRenderTarget, integratedBloomRenderTarget;

        internal static readonly SoundManager soundManager = new SoundManager();
        internal static Scene currentScene;
        internal static Camera camera = new Camera();
        internal static readonly LightCamera lightCamera = new LightCamera();

        internal Vector2 WindowSize() => new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
        internal double LastFPS { get; private set; }

        internal TGCGame()
        {
            game = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private void Fullscreen()
        {
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Fullscreen();

            spriteBatch = new SpriteBatch(GraphicsDevice);
            gui = new GUI(GraphicsDevice, spriteBatch);
            fullScreenQuad = new FullScreenQuad(GraphicsDevice);
            content = new Content(Content);
            skyBox = new SkyBox(content.M_SkyBox, content.TC_Space, content.E_SkyBox, 3000f);

            shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, shadowmapSize, shadowmapSize,
                false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
            mainRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            bloomRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            integratedBloomRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

            content.E_PostProcessing.Parameters["screenSize"]?.SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
        }

        protected override void Initialize()
        {
            base.Initialize();
            Window.Title = "Star Wars: Trench Run";
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace };

            ChangeScene(new MainMenu());
            camera.Initialize(GraphicsDevice);

            content.E_MainShader.Parameters["shadowMapSize"].SetValue(Vector2.One * shadowmapSize);
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            if (Input.Exit())
                Exit();

            currentScene.Update(gameTime);
            camera.Update();
            physicSimulation.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            LastFPS = 1 / gameTime.ElapsedGameTime.TotalSeconds;

            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            ShadowMapPass();
            MainPass();
            BloomPass();
            BloomIntegratePass();
            BlurPass();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.DepthRead, RasterizerState.CullNone);
            currentScene.Draw2D(GraphicsDevice, spriteBatch);
            spriteBatch.End();
        }

        private void ShadowMapPass()
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

        private void MainPass()
        {
            GraphicsDevice.SetRenderTarget(mainRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            content.E_MainShader.CurrentTechnique = content.E_MainShader.Techniques["DrawShadowed"];
            content.E_LaserShader.CurrentTechnique = content.E_LaserShader.Techniques["DrawShadowed"];

            content.E_MainShader.Parameters["ViewProjection"].SetValue(camera.ViewProjection);
            content.E_LaserShader.Parameters["ViewProjection"].SetValue(camera.ViewProjection);

            content.E_MainShader.Parameters["cameraPosition"].SetValue(camera.position);
            content.E_MainShader.Parameters["LightViewProjection"].SetValue(lightCamera.ViewProjection);
            content.E_MainShader.Parameters["lightDirection"].SetValue(-lightCamera.Direction);
            content.E_MainShader.Parameters["shadowMap"].SetValue(shadowMapRenderTarget);

            skyBox.Draw(camera.ViewProjection, camera.position);
            currentScene.Draw();
        }

        private void BloomPass()
        {
            GraphicsDevice.SetRenderTarget(bloomRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            content.E_MainShader.CurrentTechnique = content.E_MainShader.Techniques["BloomPass"];
            content.E_LaserShader.CurrentTechnique = content.E_LaserShader.Techniques["BloomPass"];

            currentScene.Draw();
        }

        private void BloomIntegratePass()
        {
            GraphicsDevice.SetRenderTarget(integratedBloomRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            content.E_PostProcessing.CurrentTechnique = content.E_PostProcessing.Techniques["BloomPass"];
            content.E_PostProcessing.Parameters["baseTexture"].SetValue(mainRenderTarget);
            content.E_PostProcessing.Parameters["bloomTexture"].SetValue(bloomRenderTarget);
            fullScreenQuad.Draw(content.E_PostProcessing);
        }

        private void BlurPass()
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            content.E_PostProcessing.CurrentTechnique = content.E_PostProcessing.Techniques["BlurPass"];
            content.E_PostProcessing.Parameters["InverseViewProjection"].SetValue(camera.InverseViewProjection);
            content.E_PostProcessing.Parameters["PrevViewProjection"].SetValue(camera.PrevViewProjection);
            content.E_PostProcessing.Parameters["baseTexture"].SetValue(integratedBloomRenderTarget);
            fullScreenQuad.Draw(content.E_PostProcessing);
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