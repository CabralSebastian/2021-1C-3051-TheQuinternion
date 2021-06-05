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

namespace TGC.MonoGame.TP
{
    internal class Content
    {
        private readonly ContentManager contentManager;

        private const string EffectsFolder = "Effects/";
        private const string ModelsFolder = "Models/";
        private const string TexturesFolder = "Textures/";
        private const string SoundsFolder = "Sounds/";
        //private const string MusicFolder = "Music/";
        private const string FontsFolder = "Fonts/";

        internal readonly Effect E_BasicShader, E_BlinnPhong, E_PBR, E_LaserShader, E_SkyBox;
        internal readonly Model M_SkyBox, M_XWing, M_TIE, M_Trench_Plain, M_Trench_Line, M_Trench_Corner, M_Trench_T, M_Trench_Cross, M_Trench_End, M_Trench2, M_Laser, M_Turret, M_SmallTurret;
        internal readonly TypedIndex Sh_Sphere20, SH_XWing, SH_Laser, SH_Turret, SH_SmallTurret, Sh_Trench_Plain, Sh_Trench_Line, Sh_Trench_Corner, Sh_Trench_T, Sh_Trench_End, Sh_Trench_Cross;
        internal readonly Texture2D[] T_DeathStar, T_XWing, T_TIE, T_Trench, T_Trench2, T_Turret;
        internal readonly Texture2D T_Pixel, T_TargetCursor;
        internal readonly TextureCube TC_Space;
        internal readonly SoundEffect S_Click1, S_Click2, S_Laser, S_Explotion, S_MenuMusic, S_GameMusic;
        internal readonly SpriteFont F_StarJedi;
        internal readonly Drawer D_XWing, D_TIE, D_Trench_Plain, D_Trench_Line, D_Trench_Corner, D_Trench_T, D_Trench_Cross, D_Trench_End, D_Trench2, D_Laser;
        internal readonly TurretDrawer D_Turret;
        internal readonly SmallTurretDrawer D_SmallTurret;

        internal Content(ContentManager contentManager)
        {
            this.contentManager = contentManager;

            // Efects
            E_BasicShader = LoadEffect("BasicShader");
            E_BlinnPhong = LoadEffect("BlinnPhong");
            E_PBR = LoadEffect("PBR");
            E_LaserShader = LoadEffect("LaserShader");
            E_SkyBox = LoadEffect("SkyBox");

            // Models
            M_SkyBox =  LoadModel("SkyBox/Cube", E_SkyBox);
            M_XWing = LoadModel("XWing/XWing", E_BlinnPhong);
            M_TIE = LoadModel("TIE/TIE", E_BlinnPhong);
            M_Trench_Plain = LoadModel("DeathStar/Trench_Plain", E_BlinnPhong);
            M_Trench_Line = LoadModel("DeathStar/Trench_Line", E_BlinnPhong);
            M_Trench_Corner = LoadModel("DeathStar/Trench_Corner", E_BlinnPhong);
            M_Trench_T = LoadModel("DeathStar/Trench_T", E_BlinnPhong);
            M_Trench_Cross = LoadModel("DeathStar/Trench_Cross", E_BlinnPhong);
            M_Trench_End = LoadModel("DeathStar/Trench_End", E_BlinnPhong);
            M_Trench2 = LoadModel("DeathStar/Trench2", E_BlinnPhong);
            M_Laser = LoadModel("Laser", E_LaserShader);
            M_Turret = LoadModel("DeathStar/Turret", E_BlinnPhong);
            M_SmallTurret = LoadModel("DeathStar/SmallTurret", E_BlinnPhong);

            // Convex Hulls

            // Shapes
            Sh_Sphere20 = LoadShape(new Sphere(20f));
            SH_XWing = LoadConvexHull("XWing/XWing");
            SH_Laser = LoadShape(new Cylinder(Laser.Radius / 2f, Laser.Lenght / 10f));
            SH_Turret = LoadShape(new Box(2f * 10f, 7f * 10f, 2f * 10f));
            SH_SmallTurret = LoadShape(new Box(2f * 5f, 5f * 5f, 2f * 5f));

            // DeathStar shapes
            TypedIndex trenchPlain = LoadShape(new Box(DeathStar.trenchSize, DeathStar.trenchHeight, DeathStar.trenchSize));
            TypedIndex trenchLine = LoadShape(new Box(DeathStar.trenchSize, DeathStar.trenchHeight, DeathStar.trenchSize / 1.6f));
            TypedIndex trenchQuarter = LoadShape(new Box(DeathStar.trenchSize / 1.6f, DeathStar.trenchHeight, DeathStar.trenchSize / 1.6f));
            RigidPose plainPose = new RigidPose(new BEPUVector3(0f, -DeathStar.trenchHeight * 1.5f, 0f));
            float colliderYPos = -DeathStar.trenchHeight / 2f;
            float sideOffset = DeathStar.trenchSize * 3.5f / 8f;
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
            T_Trench2 = Enumerable.Repeat(LoadTexture("DeathStar/DeathStar"), 27).ToArray();
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
            S_MenuMusic = LoadSound("StarWarsBattlefront2-KaminoLoadingLoopA");
            S_GameMusic = LoadSound("StarWarsBattlefront2-KaminoLoadingLoopB");

            // Fonts
            F_StarJedi = LoadFont("Starjedi");

            // Drawers
            D_XWing = new BlinnPhongDrawer(M_XWing, T_XWing);
            D_TIE = new BlinnPhongDrawer(M_TIE, T_TIE);
            D_Trench_Plain = new BlinnPhongDrawer(M_Trench_Plain, T_Trench);
            D_Trench_Line = new BlinnPhongDrawer(M_Trench_Line, T_Trench);
            D_Trench_Corner = new BlinnPhongDrawer(M_Trench_Corner, T_Trench);
            D_Trench_T = new BlinnPhongDrawer(M_Trench_T, T_Trench);
            D_Trench_Cross = new BlinnPhongDrawer(M_Trench_Cross, T_Trench);
            D_Trench_End = new BlinnPhongDrawer(M_Trench_End, T_Trench);
            D_Trench2 = new BlinnPhongDrawer(M_Trench2, T_Trench);
            D_Laser = new LaserDrawer(M_Laser);
            D_Turret = new TurretDrawer(M_Turret, T_Turret);
            D_SmallTurret = new SmallTurretDrawer(M_SmallTurret, T_Turret);
        }

        private Effect LoadEffect(string name) => contentManager.Load<Effect>(EffectsFolder + name);
        private Model LoadModel(string name, Effect effect)
        {
            Model model = contentManager.Load<Model>(ModelsFolder + name);
            foreach (ModelMesh mesh in model.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect;
            return model;
        }
        private TypedIndex LoadShape<S>(S shape) where S : unmanaged, IShape => TGCGame.physicSimulation.LoadShape(shape);
        private TypedIndex LoadKinematicCompoundShape((TypedIndex, RigidPose)[] colliders)
        {
            CompoundBuilder builder = new CompoundBuilder(TGCGame.physicSimulation.bufferPool, TGCGame.physicSimulation.Shapes(), colliders.Length);
            foreach ((TypedIndex, RigidPose) collider in colliders)
                builder.AddForKinematic(collider.Item1, collider.Item2, 1f);
            builder.BuildKinematicCompound(out Buffer<CompoundChild> buffer);
            return TGCGame.physicSimulation.LoadShape(new Compound(buffer));
        }
        private TypedIndex LoadConvexHull(string name) => LoadShape(ConvexHullGenerator.Generate(contentManager.Load<Model>(ModelsFolder + name)));
        private Texture2D LoadTexture(string name) => contentManager.Load<Texture2D>(TexturesFolder + name);
        private TextureCube LoadTextureCube(string name) => contentManager.Load<TextureCube>(TexturesFolder + name);
        private SoundEffect LoadSound(string name) => contentManager.Load<SoundEffect>(SoundsFolder + name);
        private SpriteFont LoadFont(string name) => contentManager.Load<SpriteFont>(FontsFolder + name);
    }
}