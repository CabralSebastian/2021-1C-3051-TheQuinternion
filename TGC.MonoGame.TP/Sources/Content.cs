using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using TGC.MonoGame.TP.Physics;
using BEPUVector3 = System.Numerics.Vector3;
using BEPUQuaternion = System.Numerics.Quaternion;
using System;
using TGC.MonoGame.TP.Drawers;
using Microsoft.Xna.Framework.Media;
using TGC.MonoGame.TP.Rendering;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP
{
    internal class Content
    {
        private readonly ContentManager ContentManager;

        private const string EffectsFolder = "Effects/";
        private const string ModelsFolder = "Models/";
        private const string TexturesFolder = "Textures/";
        private const string SoundsFolder = "Sounds/";
        private const string FontsFolder = "Fonts/";

        internal readonly Effect E_MainShader, E_LaserShader, E_SkyBox, E_PostProcessing;
        internal readonly Model M_SkyBox, M_XWing, M_TIE, M_Trench_Plain, M_Trench_Line, M_Trench_Corner, M_Trench_T, M_Trench_Cross, M_Trench_End, M_Laser, M_Turret, M_SmallTurret, M_WeakPoint;
        internal readonly TypedIndex SH_XWing, SH_TIE, SH_Laser, SH_Turret, SH_SmallTurret, Sh_Trench_Plain, Sh_Trench_Line, Sh_Trench_Corner, Sh_Trench_T, Sh_Trench_End, Sh_Trench_Cross, SH_WeakPoint;
        internal readonly Texture2D[] T_DeathStar, T_XWing, T_TIE, T_Trench, T_Turret;
        internal readonly Texture2D T_Pixel, T_TargetCursor;
        internal readonly TextureCube TC_Space;
        internal readonly SoundEffect S_Click1, S_Click2, S_Laser, S_Explotion, S_XWingEngine, S_TIEEngine, S_MenuMusic, S_GameMusic;
        internal readonly SpriteFont F_StarJedi;
        internal readonly Drawer D_XWing, D_TIE, D_Trench_Plain, D_Trench_Line, D_Trench_Corner, D_Trench_T, D_Trench_Cross, D_Trench_End, D_Laser, D_WeakPoint;
        internal readonly TurretDrawer D_Turret;
        internal readonly SmallTurretDrawer D_SmallTurret;
        internal readonly Material TrenchMaterial, TurretMaterial, XWingMaterial;

        internal Content(ContentManager contentManager)
        {
            this.ContentManager = contentManager;

            // Efects
            E_MainShader = LoadEffect("MainShader");
            E_LaserShader = LoadEffect("LaserShader");
            E_SkyBox = LoadEffect("SkyBox");
            E_PostProcessing = LoadEffect("PostProcessing");

            // Models
            M_SkyBox =  LoadModel("SkyBox/Cube", E_SkyBox);
            M_XWing = LoadModel("XWing/XWing", E_MainShader);
            M_TIE = LoadModel("TIE/TIE", E_MainShader);
            M_Trench_Plain = LoadModel("DeathStar/Trench_Plain", E_MainShader);
            M_Trench_Line = LoadModel("DeathStar/Trench_Line", E_MainShader);
            M_Trench_Corner = LoadModel("DeathStar/Trench_Corner", E_MainShader);
            M_Trench_T = LoadModel("DeathStar/Trench_T", E_MainShader);
            M_Trench_Cross = LoadModel("DeathStar/Trench_Cross", E_MainShader);
            M_Trench_End = LoadModel("DeathStar/Trench_End", E_MainShader);
            M_Laser = LoadModel("Laser", E_LaserShader);
            M_Turret = LoadModel("DeathStar/Turret", E_MainShader);
            M_SmallTurret = LoadModel("DeathStar/SmallTurret", E_MainShader);
            M_WeakPoint = LoadModel("DeathStar/WeakPoint", E_MainShader);

            // Shapes
            SH_XWing = LoadConvexHull("XWing/XWing", 1f);
            SH_TIE = LoadShape(new Sphere(8f));
            SH_Laser = LoadShape(new Cylinder(Laser.Radius / 2f, Laser.Lenght / 50f));
            SH_Turret = LoadShape(new Box(2 * 1.0f * Turret.ScaleValue * 100, 2 * 3.29f * Turret.ScaleValue * 100, 2 * 1.0f * Turret.ScaleValue * 100));
            SH_SmallTurret = LoadShape(new Box(2 * 0.45f * SmallTurret.ScaleValue * 100, 2 * 2.1f * SmallTurret.ScaleValue * 100, 2 * 0.45f * SmallTurret.ScaleValue * 100));
            SH_WeakPoint = LoadConvexHull("DeathStar/WeakPoint", DeathStar.TrenchScale * 100f);

            // DeathStar shapes
            TypedIndex trenchPlain = LoadShape(new Box(DeathStar.TrenchSize, DeathStar.TrenchHeight, DeathStar.TrenchSize));
            TypedIndex trenchLine = LoadShape(new Box(DeathStar.TrenchSize, DeathStar.TrenchHeight, DeathStar.TrenchSize / 1.6f));
            TypedIndex trenchQuarter = LoadShape(new Box(DeathStar.TrenchSize / 1.6f, DeathStar.TrenchHeight, DeathStar.TrenchSize / 1.6f));
            RigidPose plainPose = new RigidPose(new BEPUVector3(0f, -DeathStar.TrenchHeight, 0f));
            float colliderYPos = -DeathStar.TrenchHeight / 2f;
            float sideOffset = DeathStar.TrenchSize * 3.5f / 8f;
            BEPUQuaternion d90Rotation = BEPUQuaternion.CreateFromAxisAngle(BEPUVector3.UnitY, (float)Math.PI / 2f);
            Sh_Trench_Plain = LoadKinematicCompoundShape(new (TypedIndex, RigidPose)[] {
                (trenchPlain, new RigidPose(new BEPUVector3(0f, colliderYPos, 0f)))
            });
            Sh_Trench_Line = LoadKinematicCompoundShape(new (TypedIndex, RigidPose)[] {
                (trenchLine, new RigidPose(new BEPUVector3(0f, colliderYPos, -sideOffset))),
                (trenchLine, new RigidPose(new BEPUVector3(0f, colliderYPos, sideOffset))),
                (trenchPlain, plainPose)
            });
            Sh_Trench_Corner = LoadKinematicCompoundShape(new (TypedIndex, RigidPose)[] {
                (trenchLine, new RigidPose(new BEPUVector3(0f, colliderYPos, -sideOffset))),
                (trenchLine, new RigidPose(new BEPUVector3(sideOffset, colliderYPos, 0f), d90Rotation)),
                (trenchQuarter, new RigidPose(new BEPUVector3(-sideOffset, colliderYPos, sideOffset))),
                (trenchPlain, plainPose)
            });
            Sh_Trench_T = LoadKinematicCompoundShape(new (TypedIndex, RigidPose)[] {
                (trenchLine, new RigidPose(new BEPUVector3(0f, colliderYPos, -sideOffset))),
                (trenchQuarter, new RigidPose(new BEPUVector3(-sideOffset, colliderYPos, sideOffset))),
                (trenchQuarter, new RigidPose(new BEPUVector3(sideOffset, colliderYPos, sideOffset))),
                (trenchPlain, plainPose)
            });
            Sh_Trench_End = LoadKinematicCompoundShape(new (TypedIndex, RigidPose)[] {
                (trenchLine, new RigidPose(new BEPUVector3(0f, colliderYPos, -sideOffset))),
                (trenchLine, new RigidPose(new BEPUVector3(0f, colliderYPos, sideOffset))),
                (trenchLine, new RigidPose(new BEPUVector3(sideOffset, colliderYPos, 0f), d90Rotation)),
                (trenchPlain, plainPose)
            });
            Sh_Trench_Cross = LoadKinematicCompoundShape(new (TypedIndex, RigidPose)[] {
                (trenchQuarter, new RigidPose(new BEPUVector3(-sideOffset, colliderYPos, -sideOffset))),
                (trenchQuarter, new RigidPose(new BEPUVector3(-sideOffset, colliderYPos, sideOffset))),
                (trenchQuarter, new RigidPose(new BEPUVector3(sideOffset, colliderYPos, -sideOffset))),
                (trenchQuarter, new RigidPose(new BEPUVector3(sideOffset, colliderYPos, sideOffset))),
                (trenchPlain, plainPose)
            });

            // Textures
            T_DeathStar = new Texture2D[] { LoadTexture("DeathStar/DeathStar") };
            T_TIE = new Texture2D[] { LoadTexture("TIE/TIE_IN_DIFF") };
            T_XWing = new Texture2D[] {
                LoadTexture("XWing/lambert6_Base_Color"),
                LoadTexture("XWing/lambert5_Base_Color"),
                /*LoadTexture("XWing/lambert6_Normal_DirectX"),
                LoadTexture("XWing/lambert5_Normal_DirectX")*/
            };
            T_Trench = new Texture2D[] { LoadTexture("DeathStar/DeathStar") };
            T_Turret = new Texture2D[] { LoadTexture("DeathStar/Turret") };
            T_Pixel = LoadTexture("HUD/Pixel");
            T_TargetCursor = LoadTexture("HUD/TargetCursor");

            // TextureCubes
            TC_Space = LoadTextureCube("SkyBox/Space");

            // Sounds
            S_Click1 = LoadSound("Click1");
            S_Click2 = LoadSound("Click2");
            S_Laser = LoadSound("Laser");
            S_Explotion = LoadSound("Explotion");
            S_XWingEngine = LoadSound("XWingEngine");
            S_TIEEngine = LoadSound("TIEEngine");
            S_MenuMusic = LoadSound("StarWarsBattlefront2-KaminoLoadingLoopA");
            S_GameMusic = LoadSound("StarWarsBattlefront2-KaminoLoadingLoopB");

            // Fonts
            F_StarJedi = LoadFont("Starjedi");

            // Materials
            TrenchMaterial = new Material(0.8f, 0.8f, 0.1f, 8.0f);
            TurretMaterial = new Material(1.0f, 0.5f, 0.01f, 8.0f);
            XWingMaterial = new Material(0.7f, 0.9f, 0.4f, 4.0f);

            // Drawers
            D_XWing = new MainDrawer(M_XWing, T_XWing, XWingMaterial);
            D_TIE = new MainDrawer(M_TIE, T_TIE, XWingMaterial);
            D_Trench_Plain = new MainDrawer(M_Trench_Plain, T_Trench, TrenchMaterial);
            D_Trench_Line = new MainDrawer(M_Trench_Line, T_Trench, TrenchMaterial);
            D_Trench_Corner = new MainDrawer(M_Trench_Corner, T_Trench, TrenchMaterial);
            D_Trench_T = new MainDrawer(M_Trench_T, T_Trench, TrenchMaterial);
            D_Trench_Cross = new MainDrawer(M_Trench_Cross, T_Trench, TrenchMaterial);
            D_Trench_End = new MainDrawer(M_Trench_End, T_Trench, TrenchMaterial);
            D_Laser = new LaserDrawer(M_Laser);
            D_Turret = new TurretDrawer(M_Turret, T_Turret, TurretMaterial);
            D_SmallTurret = new SmallTurretDrawer(M_SmallTurret, T_Turret, TurretMaterial);
            D_WeakPoint = new MainDrawer(M_WeakPoint, T_Turret, TrenchMaterial);
        }

        private Effect LoadEffect(string name) => ContentManager.Load<Effect>(EffectsFolder + name);
        private Model LoadModel(string name, Effect effect)
        {
            Model model = ContentManager.Load<Model>(ModelsFolder + name);
            foreach (ModelMesh mesh in model.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect;
            return model;
        }
        private TypedIndex LoadShape<S>(S shape) where S : unmanaged, IShape => TGCGame.PhysicsSimulation.LoadShape(shape);
        private TypedIndex LoadKinematicCompoundShape((TypedIndex, RigidPose)[] colliders)
        {
            CompoundBuilder builder = new CompoundBuilder(TGCGame.PhysicsSimulation.BufferPool, TGCGame.PhysicsSimulation.Shapes(), colliders.Length);
            foreach ((TypedIndex, RigidPose) collider in colliders)
                builder.AddForKinematic(collider.Item1, collider.Item2, 1f);
            builder.BuildKinematicCompound(out Buffer<CompoundChild> buffer);
            return TGCGame.PhysicsSimulation.LoadShape(new Compound(buffer));
        }
        private TypedIndex LoadConvexHull(string name, float scale) => LoadShape(ConvexHullGenerator.Generate(ContentManager.Load<Model>(ModelsFolder + name), scale));
        private Texture2D LoadTexture(string name) => ContentManager.Load<Texture2D>(TexturesFolder + name);
        private TextureCube LoadTextureCube(string name) => ContentManager.Load<TextureCube>(TexturesFolder + name);
        private SoundEffect LoadSound(string name) => ContentManager.Load<SoundEffect>(SoundsFolder + name);
        private SpriteFont LoadFont(string name) => ContentManager.Load<SpriteFont>(FontsFolder + name);
    }
}