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
        internal static TGCGame Game;
        private readonly GraphicsDeviceManager Graphics;
        internal static Content GameContent;
        internal static GUI Gui;
        internal static readonly PhysicSimulation PhysicsSimulation = new PhysicSimulation();
        private SpriteBatch SpriteBatch;
        private SkyBox SkyBox;
        private FullScreenQuad FullScreenQuad;
        internal const int ShadowMapSize = 2048 * 2;
        private RenderTarget2D ShadowMapRenderTarget, MainRenderTarget, BloomRenderTarget, IntegratedBloomRenderTarget;

        internal static readonly SoundManager SoundManager = new SoundManager();
        internal static Scene CurrentScene;
        internal static Camera Camera = new Camera();
        internal static readonly LightCamera LightCamera = new LightCamera();

        internal Vector2 WindowSize() => new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
        internal double LastFPS { get; private set; }

        internal TGCGame()
        {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);
            base.Content.RootDirectory = "Content";
        }

        private void Fullscreen()
        {
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Fullscreen();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Gui = new GUI(GraphicsDevice, SpriteBatch);
            FullScreenQuad = new FullScreenQuad(GraphicsDevice);
            GameContent = new Content(base.Content);
            SkyBox = new SkyBox(GameContent.M_SkyBox, GameContent.TC_Space, GameContent.E_SkyBox, 3000f);

            ShadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, ShadowMapSize, ShadowMapSize,
                false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
            MainRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            BloomRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            IntegratedBloomRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

            GameContent.E_PostProcessing.Parameters["screenSize"]?.SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
        }

        protected override void Initialize()
        {
            base.Initialize();
            Window.Title = "Star Wars: Trench Run";
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace };

            ChangeScene(new MainMenu());
            Camera.Initialize(GraphicsDevice);

            GameContent.E_MainShader.Parameters["shadowMapSize"].SetValue(Vector2.One * ShadowMapSize);
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            if (Input.Exit())
                Exit();

            CurrentScene.Update(gameTime);
            Camera.Update();
            PhysicsSimulation.Update();
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

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.DepthRead, RasterizerState.CullNone);
            CurrentScene.Draw2D(GraphicsDevice, SpriteBatch);
            SpriteBatch.End();
        }

        private void ShadowMapPass()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SetRenderTarget(ShadowMapRenderTarget); //shadowMapRenderTarget null
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            LightCamera.Update();
            GameContent.E_MainShader.CurrentTechnique = GameContent.E_MainShader.Techniques["DepthPass"];
            GameContent.E_LaserShader.CurrentTechnique = GameContent.E_LaserShader.Techniques["DepthPass"];

            GameContent.E_MainShader.Parameters["ViewProjection"].SetValue(LightCamera.ViewProjection);
            GameContent.E_LaserShader.Parameters["ViewProjection"].SetValue(LightCamera.ViewProjection);

            SkyBox.Draw(Camera.ViewProjection, Camera.Position);
            CurrentScene.Draw();
        }

        private void MainPass()
        {
            GraphicsDevice.SetRenderTarget(MainRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            GameContent.E_MainShader.CurrentTechnique = GameContent.E_MainShader.Techniques["DrawShadowed"];
            GameContent.E_LaserShader.CurrentTechnique = GameContent.E_LaserShader.Techniques["DrawShadowed"];

            GameContent.E_MainShader.Parameters["ViewProjection"].SetValue(Camera.ViewProjection);
            GameContent.E_LaserShader.Parameters["ViewProjection"].SetValue(Camera.ViewProjection);

            GameContent.E_MainShader.Parameters["cameraPosition"].SetValue(Camera.Position);
            GameContent.E_MainShader.Parameters["LightViewProjection"].SetValue(LightCamera.ViewProjection);
            GameContent.E_MainShader.Parameters["lightDirection"].SetValue(-LightCamera.Direction);
            GameContent.E_MainShader.Parameters["shadowMap"].SetValue(ShadowMapRenderTarget);

            SkyBox.Draw(Camera.ViewProjection, Camera.Position);
            CurrentScene.Draw();
        }

        private void BloomPass()
        {
            GraphicsDevice.SetRenderTarget(BloomRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            GameContent.E_MainShader.CurrentTechnique = GameContent.E_MainShader.Techniques["BloomPass"];
            GameContent.E_LaserShader.CurrentTechnique = GameContent.E_LaserShader.Techniques["BloomPass"];

            CurrentScene.Draw();
        }

        private void BloomIntegratePass()
        {
            GraphicsDevice.SetRenderTarget(IntegratedBloomRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            GameContent.E_PostProcessing.CurrentTechnique = GameContent.E_PostProcessing.Techniques["BloomPass"];
            GameContent.E_PostProcessing.Parameters["baseTexture"].SetValue(MainRenderTarget);
            GameContent.E_PostProcessing.Parameters["bloomTexture"].SetValue(BloomRenderTarget);
            FullScreenQuad.Draw(GameContent.E_PostProcessing);
        }

        private void BlurPass()
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            GameContent.E_PostProcessing.CurrentTechnique = GameContent.E_PostProcessing.Techniques["BlurPass"];
            GameContent.E_PostProcessing.Parameters["InverseViewProjection"].SetValue(Camera.InverseViewProjection);
            GameContent.E_PostProcessing.Parameters["PrevViewProjection"].SetValue(Camera.PrevViewProjection);
            GameContent.E_PostProcessing.Parameters["baseTexture"].SetValue(IntegratedBloomRenderTarget);
            FullScreenQuad.Draw(GameContent.E_PostProcessing);
        }

        protected override void UnloadContent()
        {
            base.Content.Unload();
            PhysicsSimulation.Dispose();
            FullScreenQuad.Dispose();
            base.UnloadContent();
        }

        internal void ChangeScene(Scene scene)
        {
            CurrentScene?.Destroy();
            CurrentScene = scene;
            scene.Initialize();
        }
    }
}